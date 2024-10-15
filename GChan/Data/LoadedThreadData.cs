using GChan.Models;
using System;
using System.Data.SQLite;

namespace GChan.Data
{
    public class LoadedThreadData : LoadedData
    {
        public string Subject;

        public DateTimeOffset? LastScrape;

        public AssetIdsCollection SavedIds;

        public LoadedThreadData(SQLiteDataReader dataReader)
        {
            var index = 0;
            Url = dataReader.GetString(index++) ?? "";
            Subject = dataReader.GetString(index++) ?? "";

            var lastScrape = dataReader.GetString(index++) ?? string.Empty;
            LastScrape = string.IsNullOrWhiteSpace(lastScrape) ? null : DateTimeOffset.Parse(lastScrape);

            SavedIds = new AssetIdsCollection(dataReader.GetString(index++) ?? "[]");
        }
    }
}
