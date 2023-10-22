using System;
using System.Threading;

namespace GChan.Controllers
{
    internal class Download : IDisposable
    {
        private readonly Thread thread;
        private readonly CancellationTokenSource cancellationTokenSource;

        public Download(Thread thread, CancellationTokenSource cancellationTokenSource)
        {
            this.thread = thread;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        /// <summary>
        /// Signal cancellation to the thread.
        /// </summary>
        public void Cancel()
        { 
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Cancel and dispose of download.
        /// </summary>
        public void Dispose()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }
    }
}
