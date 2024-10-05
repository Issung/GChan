using GChan.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Return <paramref name="assets"/> filtered to those not present in <paramref name="savedIds"/>.
        /// </summary>
        public static IEnumerable<IAsset> FilterAssets(
            this IEnumerable<IAsset> assets,
            AssetIdsCollection savedIds
        )
        {
            return assets.Where(a => savedIds.Contains(a.Id));
        }
    }
}
