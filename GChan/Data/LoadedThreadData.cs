using GChan.Models;
using System.Data.SQLite;

namespace GChan.Data
{
    public class LoadedThreadData : LoadedData
    {
        public string Subject;

        public SavedIdsCollection SavedIds;

        public LoadedThreadData(SQLiteDataReader dataReader)
        {
            var index = 0;
            URL = dataReader.GetString(index++) ?? "";
            Subject = dataReader.GetString(index++) ?? "";
            SavedIds = new SavedIdsCollection(dataReader.GetString(index++) ?? "");
        }
    }
}
