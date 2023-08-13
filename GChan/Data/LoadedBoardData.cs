using System.Data.SQLite;

namespace GChan.Data
{
    public class LoadedBoardData : LoadedData
    {
        public long GreatestThreadId;

        public LoadedBoardData(SQLiteDataReader dataReader)
        {
            var index = 0;
            Url = dataReader.GetString(index++) ?? "";
            GreatestThreadId = dataReader.GetInt64(index++);
        }
    }
}
