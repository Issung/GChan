using GChan.Exceptions;
using GChan.Helpers.Extensions;
using GChan.Models;
using GChan.Models.Trackers;
using GChan.Properties;
using Nito.AsyncEx;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace GChan.Services
{
    /// <summary>
    /// A queue for <see cref="IProcessable"/>s. Controls how often they are started.<br/>
    /// Items are only taken off the queue when <see cref="ProcessPool"/> has capacity, so priority order, request spacing and
    /// "Too Many Requests" backoff apply to every request.
    /// </summary>
    public class ProcessQueue
    {
        /// <summary>
        /// Assets that fail this many times in a row are dropped until the thread is next loaded.
        /// Trackers are retried indefinitely (with backoff), as they are expected to recover.
        /// </summary>
        private const int MAX_ASSET_ATTEMPTS = 5;
        private static readonly TimeSpan INITIAL_RETRY_DELAY = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan MAX_RETRY_DELAY = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan IDLE_WAIT = TimeSpan.FromSeconds(15);

        private record RetryState(int Failures, DateTimeOffset RetryAt);

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDistinctPriorityQueue<IProcessable, ProcessPriority> queue = new(LockRecursionPolicy.SupportsRecursion);
        private readonly ConcurrentDictionary<IProcessable, RetryState> retries = new(ReferenceEqualityComparer.Instance);  // By instance, like the queue.
        private readonly AsyncAutoResetEvent itemEnqueuedSignal = new();
        private readonly AsyncManualResetEvent resumeSignal = new(set: true);
        private readonly ProcessPool pool;
        private readonly Task task;

        private readonly Action<IReadOnlyList<Tracker>> addTrackersCallback;
        private readonly CancellationToken shutdownCancellationToken;
        private readonly ProcessableParams processableParams;

        private readonly object tooManyRequestsLock = new();
        private TimeSpan? tooManyRequestsDelay;
        private bool pause;

        public event Action? PausedPropertyChanged;

        public bool Pause
        {
            get => pause;
            set
            {
                var raiseEvent = pause != value;    // Only raise event if new value is different.

                pause = value;

                if (pause)
                {
                    resumeSignal.Reset();
                }
                else
                {
                    resumeSignal.Set();
                }

                if (raiseEvent)
                {
                    PausedPropertyChanged?.Invoke();
                }
            }
        }

        public ProcessQueue(
            IProcessableHooks hooks,
            Action<IReadOnlyList<Tracker>> addTrackersCallback,
            CancellationToken shutdownCancellationToken
        )
        {
            this.addTrackersCallback = addTrackersCallback;
            this.shutdownCancellationToken = shutdownCancellationToken;
            this.processableParams = new(hooks);

            this.pool = new(HandleResult);
            this.task = Task.Run(WorkAsync);
        }

        public void Enqueue(IProcessable processable, bool requeue = false)
        {
            var wasQueued = queue.Enqueue(processable, processable.Priority);

            if (wasQueued && logger.IsTraceEnabled)
            {
                var operation = requeue ? "Requeued" : "Enqueued";
                logger.Trace($"{operation} processable {{processable}} with priority {{priority}}.", processable, processable.Priority);
            }

            itemEnqueuedSignal.Set();
        }

        private async Task WorkAsync()
        {
            while (!shutdownCancellationToken.IsCancellationRequested)
            {
                try
                {
                    await StepAsync();
                }
                catch (OperationCanceledException) when (shutdownCancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception e)
                {
                    logger.Error(e, "Unexpected error in the process queue loop.");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            logger.Debug("Process queue stopped.");
        }

        private async Task StepAsync()
        {
            // Pause is checked again after every wait, as it may have changed while waiting.
            await resumeSignal.WaitAsync(shutdownCancellationToken);

            await pool.WaitForCapacityAsync(shutdownCancellationToken);

            if (HasTooManyRequestsDelay)
            {
                // Let requests already in flight finish, then wait out the delay before sending anything else.
                // Taken after they finish, so any 429s they also get are merged into this one wait rather than waited out in turn.
                await pool.WaitForIdleAsync(shutdownCancellationToken);
                var backoff = TakeTooManyRequestsDelay();
                await Task.Delay(backoff + TimeSpan.FromSeconds(1), shutdownCancellationToken);
                return;
            }

            if (Pause)
            {
                return;
            }

            var (item, nextReadyAt) = MaybeDequeue();

            if (item == null)
            {
                var wait = IDLE_WAIT;

                if (nextReadyAt.HasValue)
                {
                    var untilReady = nextReadyAt.Value - DateTimeOffset.Now;
                    wait = untilReady < wait ? untilReady : wait;
                }

                if (wait > TimeSpan.Zero)
                {
                    // Cancel whichever wait loses, so an abandoned signal wait can't swallow a later enqueue's signal.
                    using var waitCancellation = CancellationTokenSource.CreateLinkedTokenSource(shutdownCancellationToken);

                    await Task.WhenAny(
                        Task.Delay(wait, waitCancellation.Token),
                        itemEnqueuedSignal.WaitAsync(waitCancellation.Token)
                    );

                    waitCancellation.Cancel();
                }

                return;
            }

            logger.Debug("Starting processable {processable}.", item);

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(shutdownCancellationToken, item.CancellationToken);
            pool.Start(new(item, processableParams, cancellationTokenSource));

            if (Settings.Default.Max1RequestPerSecond)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), shutdownCancellationToken);
            }
        }

        private void HandleResult(ProcessPool.Item item, Task<ProcessResult> completedTask)
        {
            var processable = item.Processable;

            try
            {
                // The task is already completed. GetResult rethrows its exception (unwrapped) for the handling below.
                var result = completedTask.GetAwaiter().GetResult();

                retries.TryRemove(processable, out _);

                // Board may return new threads as processables. Added in one batch; the callback will enqueue them.
                var newTrackers = result.NewProcessables.OfType<Tracker>().ToArray();

                if (newTrackers.Length > 0)
                {
                    addTrackersCallback(newTrackers);
                }

                foreach (var newProcessable in result.NewProcessables)
                {
                    if (newProcessable is not Tracker)
                    {
                        Enqueue(newProcessable);
                    }
                }

                // Only once the new processables are all added, so e.g. a board doesn't record threads as seen that were never added.
                result.OnNewProcessablesAdded?.Invoke();

                if (!result.RemoveFromQueue)
                {
                    Enqueue(processable, requeue: true);
                }
            }
            catch (OperationCanceledException) when (item.CancellationToken.IsCancellationRequested)
            {
                // The processable was removed or the program is shutting down. It is already out of the queue, do nothing here to discard it.
                logger.Debug("Cancelled processing of {processable}.", processable);
                retries.TryRemove(processable, out _);
            }
            catch (OperationCanceledException e)
            {
                // Nothing we own was cancelled, so this was a timeout (HttpClient throws TaskCanceledException on timeout).
                RetryLater(processable, e);
            }
            catch (HttpRequestException e) when (e.IsGone())
            {
                // The item is already out of the queue, do nothing here to discard it.
                logger.Debug("Processing {processable} resulted in gone status code {status_code}.", processable, e.StatusCode);
                retries.TryRemove(processable, out _);
            }
            catch (TooManyRequestsException e)
            {
                var retryAfterIsSet = e.RetryAfter.HasValue && e.RetryAfter.Value.Ticks > 0;
                var delay = retryAfterIsSet ? e.RetryAfter!.Value : TimeSpan.FromMinutes(1);
                logger.Warn("Got \"Too Many Requests\" (429) response while processing {processable}. Backing off for approx {delay}.", processable, delay);

                lock (tooManyRequestsLock)
                {
                    tooManyRequestsDelay = tooManyRequestsDelay.HasValue && tooManyRequestsDelay.Value > delay ? tooManyRequestsDelay : delay;
                }

                // We got rate limited, the processable may still be able to succeed, requeue it.
                Enqueue(processable, requeue: true);
            }
            catch (Exception e)
            {
                RetryLater(processable, e);
            }
        }

        /// <summary>
        /// Requeue a failed processable with exponential backoff, or give up on an asset that keeps failing.
        /// </summary>
        private void RetryLater(IProcessable processable, Exception e)
        {
            var failures = (retries.TryGetValue(processable, out var previous) ? previous.Failures : 0) + 1;

            if (processable is IAsset && failures >= MAX_ASSET_ATTEMPTS)
            {
                logger.Error(e, "Processable {processable} failed {failures} times, giving up on it.", processable, failures);
                retries.TryRemove(processable, out _);
                return;
            }

            var delayTicks = Math.Min(INITIAL_RETRY_DELAY.Ticks * (1L << Math.Min(failures - 1, 20)), MAX_RETRY_DELAY.Ticks);
            var delay = TimeSpan.FromTicks(delayTicks);
            retries[processable] = new(failures, DateTimeOffset.Now + delay);

            logger.Error(e, "Processable {processable} failed (attempt {failures}), retrying in {delay}.", processable, failures, delay);
            Enqueue(processable, requeue: true);
        }

        private bool HasTooManyRequestsDelay
        {
            get
            {
                lock (tooManyRequestsLock)
                {
                    return tooManyRequestsDelay.HasValue;
                }
            }
        }

        private TimeSpan TakeTooManyRequestsDelay()
        {
            lock (tooManyRequestsLock)
            {
                var delay = tooManyRequestsDelay ?? TimeSpan.Zero;
                tooManyRequestsDelay = null;
                return delay;
            }
        }

        /// <summary>
        /// Dequeues processables until one is found that is ready to be processed, or the queue is depleted.
        /// </summary>
        /// <returns>The processable to process, or null with the earliest time a deferred processable will be ready.</returns>
        private (IProcessable? Item, DateTimeOffset? NextReadyAt) MaybeDequeue()
        {
            var deferredProcessables = new List<IProcessable>();
            var nextReadyAt = (DateTimeOffset?)null;
            var now = DateTimeOffset.Now;

            try
            {
                while (queue.TryDequeue(out var processable, out _))
                {
                    if (!processable.ShouldProcess)
                    {
                        logger.Trace("Discarding dequeued processable {0}.", processable);
                        retries.TryRemove(processable, out _);
                        continue;
                    }

                    var readyAt = ReadyAt(processable);

                    if (readyAt == null || readyAt <= now)
                    {
                        logger.Trace("Dequeued processable {0}.", processable);
                        return (processable, null);
                    }

                    logger.Trace("Deferring processable {0}. Ready to process at {1}.", processable, readyAt);
                    deferredProcessables.Add(processable);

                    if (nextReadyAt == null || readyAt < nextReadyAt)
                    {
                        nextReadyAt = readyAt;
                    }
                }

                logger.Trace("Processable queue contained none or no ready-to-process items.");
                return (null, nextReadyAt);
            }
            finally
            {
                foreach (var deferredProcessable in deferredProcessables)
                {
                    queue.Enqueue(deferredProcessable, deferredProcessable.Priority);
                }
            }
        }

        /// <summary>
        /// The later of when the processable says it is ready and when its retry backoff ends.
        /// </summary>
        private DateTimeOffset? ReadyAt(IProcessable processable)
        {
            var readyAt = processable.ReadyToProcessAt;

            if (retries.TryGetValue(processable, out var retry) && (readyAt == null || retry.RetryAt > readyAt))
            {
                readyAt = retry.RetryAt;
            }

            return readyAt;
        }
    }
}
