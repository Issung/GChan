using GChan.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GChan.Helpers.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static async Task<IEnumerable<TSource>> ParallelWhereAsync<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, Task<bool>> predicate,
            int maxDegreeOfParallelism = 4
        )
        {
            var results = new ConcurrentBag<TSource>();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            await Parallel.ForEachAsync(source, options, async (item, cancellationToken) =>
            {
                if (await predicate(item))
                {
                    results.Add(item);
                }
            });

            return results;
        }

    }
}
