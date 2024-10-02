using GChan.Models;
using System;
using System.Collections.Generic;

namespace GChan.Helpers
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

        /// <summary>
        /// If <paramref name="includeAlreadySaved"/> is false then remove items from <paramref name="assets"/> if they are already in <paramref name="savedIds"/>.
        /// </summary>
        public static IEnumerable<Asset> MaybeRemoveAlreadySavedLinks(
            this IEnumerable<Asset> assets,
            bool includeAlreadySaved,
            SavedIdsCollection savedIds
            )
        {
            foreach (var link in assets)
            {
                var alreadySaved = savedIds.Contains(link.Tim);

                if (includeAlreadySaved || !alreadySaved)
                {
                    yield return link;
                }
            }
        }
    }
}
