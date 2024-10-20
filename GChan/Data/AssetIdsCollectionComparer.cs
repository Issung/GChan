using GChan.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GChan.Data
{
    public class AssetIdsCollectionComparer : ValueComparer<AssetIdsCollection>
    {
        public AssetIdsCollectionComparer() : base(
            (c1, c2) => c1.Equals(c2),
            c => c.GetHashCode(),
            c => c.Clone()
        )
        {
        }
    }
}
