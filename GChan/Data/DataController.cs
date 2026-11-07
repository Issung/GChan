using EFCore.BulkExtensions;
using GChan.Data.Models;
using GChan.Models.Trackers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Thread = GChan.Models.Trackers.Thread;

namespace GChan.Data
{
    public static class DataController
    {
        /// <summary>
        /// Serialises all database access. Writes come from the UI thread and from processing threads at once, and running them
        /// one at a time keeps them in order (e.g. a removal's delete can't be overtaken by an earlier save) and avoids SQLite lock errors.
        /// </summary>
        private static readonly SemaphoreSlim dbLock = new(1, 1);

        /// <summary>
        /// Make sure this is called before any other data access.
        /// </summary>
        public static async Task EnsureCreatedAndMigrate()
        {
            await Locked(async context => await context.Database.MigrateAsync());
        }

        /// <summary>
        /// Inserts or updates the given threads and boards. Removed (cancelled) trackers are skipped, so a save that was
        /// started before a tracker's removal can't write it back after its delete.
        /// </summary>
        public static async Task Save(IEnumerable<Thread> threads, IEnumerable<Board> boards)
        {
            await Locked(async context =>
            {
                // Built inside the lock: removal cancels the tracker before deleting it, so this sees the latest state.
                var threadData = threads.Where(t => t.Scraping).Select(t => new ThreadData(t)).ToArray();
                var boardData = boards.Where(b => b.Scraping).Select(b => new BoardData(b)).ToArray();

                await context.BulkInsertOrUpdateAsync(boardData);
                await context.BulkInsertOrUpdateAsync(threadData);

                await context.SaveChangesAsync();
            });
        }

        public static async Task<(IReadOnlyList<ThreadData> ThreadData, IReadOnlyList<BoardData> BoardData)> Load()
        {
            IReadOnlyList<ThreadData> threadData = null;
            IReadOnlyList<BoardData> boardData = null;

            await Locked(async context =>
            {
                threadData = await context.ThreadData.ToArrayAsync();
                boardData = await context.BoardData.ToArrayAsync();
            });

            return (threadData, boardData);
        }

        public static async Task RemoveBoard(IEnumerable<Board> boards)
        {
            var boardDatas = boards.Select(b => new BoardData(b)).ToArray();

            await Locked(async context =>
            {
                try
                {
                    context.BoardData.RemoveRange(boardDatas);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex) when (ex.Message.Contains("affected") && ex.Message.Contains("row(s)")) // E.g. "Affected 0 row(s)".
                {
                    // Swallow exceptions when we delete but there was nothing in the database to be deleted.
                    // This will happen if a board is added and removed before being saved to database. We don't care.
                }
            });
        }

        public static async Task RemoveThreads(IEnumerable<Thread> threads)
        {
            var threadDatas = threads.Select(t => new ThreadData(t)).ToArray();

            await Locked(async context =>
            {
                try
                {
                    context.ThreadData.RemoveRange(threadDatas);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex) when (ex.Message.Contains("affected") && ex.Message.Contains("row(s)")) // E.g. "Affected 0 row(s)".
                {
                    // Swallow exceptions when we delete but there was nothing in the database to be deleted.
                    // This will happen if a thread is added and removed before being saved to database. We don't care.
                }
            });
        }

        public static async Task RemoveAllBoards()
        {
            await Locked(async context => await context.BoardData.ExecuteDeleteAsync());
        }

        public static async Task RemoveAllThreads()
        {
            await Locked(async context => await context.ThreadData.ExecuteDeleteAsync());
        }

        private static async Task Locked(Func<DataContext, Task> action)
        {
            await dbLock.WaitAsync();

            try
            {
                await using var context = new DataContext();
                await action(context);
            }
            finally
            {
                dbLock.Release();
            }
        }
    }
}
