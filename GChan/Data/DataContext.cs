using Microsoft.EntityFrameworkCore;
using System.IO;

namespace GChan.Data
{
    public class DataContext : DbContext
    {
        public DbSet<ThreadData> ThreadData { get; set; }
        public DbSet<BoardData> BoardData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var path = Path.Combine(Program.USER_DATA_PATH, "gchan.db");
            options.UseSqlite($"Data Source={path}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ThreadData>()
                .Property(d => d.SavedAssetIds)
                .HasConversion(new AssetIdsCollectionConverter())
                .Metadata
                .SetValueComparer(new AssetIdsCollectionComparer())
            ;
        }
    }
}
