using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GChan.Models
{
    /// <summary>
    /// Thread safe collection of <see cref="long"/>s. For saving downloaded image ids.
    /// </summary>
    public class AssetIdsCollection : ConcurrentHashSet<AssetId>, IEquatable<AssetIdsCollection>
    {
        private static readonly JsonSerializerOptions jsonOptions = new() { Converters = { new JsonStringEnumConverter() } };

        public AssetIdsCollection()
        { 
        
        }

        public AssetIdsCollection(string json)
        {
            LoadJson(json);
        }

        public AssetIdsCollection(IEnumerable<AssetId> ids)
        {
            AddRange(ids);
        }

        public void AddRange(IEnumerable<IAsset> assets)
        {
            var ids = assets.Select(a => a.Id);
            AddRange(ids);
        }

        private void LoadJson(string json)
        {
            var assetIds = JsonSerializer.Deserialize<AssetId[]>(json, jsonOptions);

            locker.EnterWriteLock();

            try
            {
                foreach (var assetId in assetIds)
                {
                    set.Add(assetId);
                }
            }
            finally
            {
                if (locker.IsWriteLockHeld)
                {
                    locker.ExitWriteLock();
                }
            }
        }

        public string ToJson()
        {
            var array = ToArray();
            return JsonSerializer.Serialize(array, jsonOptions);
        }

        public static AssetIdsCollection FromJson(string json)
        {
            var collection = new AssetIdsCollection();
            collection.LoadJson(json);
            return collection;
        }

        public AssetIdsCollection Clone()
        {
            var ids = this.Select(assetId => assetId.Clone());
            var newCollection = new AssetIdsCollection(ids);
            return newCollection;
        }

        public override int GetHashCode()
        {
            if (Count == 0)
            {
                return 0;
            }

            // Initialize with a prime number
            int hash = 17;

            foreach (var assetId in this)
            {
                // Multiply by a prime number and add each item's hash code
                hash = hash * 31 + assetId.GetHashCode();
            }

            return hash;
        }

        public bool Equals(AssetIdsCollection other)
        {
            return GetHashCode() == other.GetHashCode();
        }
    }
}
