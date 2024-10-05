using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GChan.Models
{
    /// <summary>
    /// Thread safe collection of <see cref="long"/>s. For saving downloaded image ids.
    /// </summary>
    public class AssetIdsCollection : ConcurrentHashSet<AssetId>
    {
        private static readonly JsonSerializerOptions jsonOptions = new() { Converters = { new JsonStringEnumConverter() } };

        public AssetIdsCollection()
        { 
        
        }

        public AssetIdsCollection(string json)
        {
            LoadJson(json);
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
    }
}
