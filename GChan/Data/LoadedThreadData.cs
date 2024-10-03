using GChan.Models;
using System.Data.SQLite;

namespace GChan.Data
{
    public class LoadedThreadData : LoadedData
    {
        public string Subject;

        public SavedAssetIdsCollection SavedIds;

        public LoadedThreadData(SQLiteDataReader dataReader)
        {
            var index = 0;
            Url = dataReader.GetString(index++) ?? "";
            Subject = dataReader.GetString(index++) ?? "";
            SavedIds = new SavedAssetIdsCollection(dataReader.GetString(index++) ?? "");
        }
    }
}
