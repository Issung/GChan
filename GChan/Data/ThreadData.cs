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

        public ThreadData(Thread thread)
        {
            this.Site = thread.Site;
            this.BoardCode = thread.BoardCode.Trim('/');
            this.Id = thread.Id;
            this.Subject = thread.Subject;
            this.FileCount = thread.FileCount;
            this.LastScrape = thread.LastScrape;
            this.SavedAssetIds = thread.SavedAssetIds;
        }
    }
}
