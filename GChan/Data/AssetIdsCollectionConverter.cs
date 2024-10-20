using GChan.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GChan.Data
{
    public class AssetIdsCollectionConverter : ValueConverter<AssetIdsCollection, string>
    {
        public AssetIdsCollectionConverter() : base(
            value => value.ToJson(),
            value => AssetIdsCollection.FromJson(value)
        )
        {
        }
    }
}
