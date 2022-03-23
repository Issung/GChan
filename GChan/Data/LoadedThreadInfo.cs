using System.Data.SQLite;

namespace GChan.Data
{
    public class LoadedThreadInfo : LoadedInfo
    {
        public string Subject;

        public LoadedThreadInfo(SQLiteDataReader dataReader)
        {
            URL = dataReader.GetString(0) ?? "";
            GreatestSaved = dataReader.GetString(1) ?? "";
            Subject = dataReader.GetString(2) ?? "";
        }
    }
}
