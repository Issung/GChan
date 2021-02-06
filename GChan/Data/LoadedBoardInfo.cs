using System.Data.SQLite;

namespace GChan.Data
{
    public class LoadedBoardInfo : LoadedInfo
    {
        public LoadedBoardInfo(SQLiteDataReader dataReader)
        {
            URL = dataReader.GetString(0) ?? "";
            GreatestSaved = dataReader.GetString(1) ?? "";
        }
    }
}
