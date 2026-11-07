using GChan.Data.Models;
using GChan.Models;
using GChan.Models.Trackers;
using Microsoft.EntityFrameworkCore;
using System;

namespace GChan.Data
{
    [PrimaryKey(nameof(Site), nameof(BoardCode), nameof(Id))]
    public class ThreadData
    {
        /// <summary>
        /// Site of the thread.
        /// </summary>
        public Site Site { get; set; }

        /// <summary>
        /// The code of the board with no slashes (e.g. "wsg").
        /// </summary>
        public string BoardCode { get; set; }

        /// <summary>
        /// Identifier of the thread.
        /// </summary>
        public long Id { get; set; }

        public string Subject { get; set; }
        public int? FileCount { get; set; }
        public DateTimeOffset? LastScrape { get; set; }
        public AssetIdsCollection SavedAssetIds { get; set; }

        /// <summary>
        /// Parameterless constructor for Entity Framework.
        /// </summary>
        public ThreadData() { }

        /// <summary>
        /// Only the key, for deleting.
        /// </summary>
        public static ThreadData KeyOf(Thread thread)
        {
            return new()
            {
                Site = thread.Site,
                BoardCode = thread.BoardCode.Trim('/'),
                Id = thread.Id,
            };
        }

        public ThreadData(Thread thread)
        {
            this.Site = thread.Site;
            this.BoardCode = thread.BoardCode.Trim('/');
            this.Id = thread.Id;
            this.Subject = thread.subject;
            this.FileCount = thread.FileCount;
            this.LastScrape = thread.LastModified;
            this.SavedAssetIds = thread.SavedAssetIds.Clone();  // A copy, so a save stores this moment rather than whatever it is when serialised.
        }
    }
}
