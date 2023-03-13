using GChan.Trackers;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace GChan.Data
{
    class DataController
    {
        /// <summary>
        /// Current database version, change this when database structure is changed.
        /// </summary>
        public const int DATABASE_VERSION = 1;

        private const string DATABASE_FILENAME = "trackers.db";
        private static readonly string DATABASE_PATH = Path.Combine(Program.PROGRAM_DATA_PATH, DATABASE_FILENAME);
        private static readonly string DATABASE_CONNECTION_STRING = "DataSource=" + DATABASE_PATH;

        /// <summary>
        /// Tables
        /// </summary>
        private const string TB_VERSION = "version";
        private const string TB_THREAD = "thread";
        private const string TB_BOARD = "board";

        /// <summary>
        /// Columns
        /// </summary>
        private const string COL_VERSION = "version";
        private const string COL_URL = "url";
        private const string COL_GREATEST_SAVED = "greatest_saved";
        private const string COL_SUBJECT = "subject";

        static SQLiteConnection _connection = null;

        static object _connectionLock = new object();

        static SQLiteConnection Connection 
        { 
            get 
            {
                lock (_connectionLock)
                { 
                    bool dbExisted = File.Exists(DATABASE_PATH);

                    if (!dbExisted)
                    {
                        SQLiteConnection.CreateFile(DATABASE_PATH);
                    }

                    if (_connection == null)
                    {
                        _connection = new SQLiteConnection(DATABASE_CONNECTION_STRING);
                        _connection.Open();

                        if (!dbExisted)
                        {
                            CreateDB();
                        }
                        else
                        {
                            UpgradeDB();
                        }
                    }

                    return _connection;
                }
            } 
        }

        private static void CreateDB()
        {
            using (var cmd = new SQLiteCommand(Connection))
            {
                // Version Table
                cmd.CommandText = $"DROP TABLE IF EXISTS {TB_VERSION}";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE {TB_VERSION} ({COL_VERSION} INTEGER PRIMARY KEY NOT NULL)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"INSERT INTO {TB_VERSION} ({COL_VERSION}) VALUES (@{COL_VERSION})";
                cmd.Parameters.AddWithValue(COL_VERSION, DATABASE_VERSION);
                cmd.ExecuteNonQuery();

                // Thread Table
                cmd.CommandText = $"DROP TABLE IF EXISTS {TB_THREAD}";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE {TB_THREAD} ({COL_URL} TEXT PRIMARY KEY NOT NULL, {COL_GREATEST_SAVED} TEXT, {COL_SUBJECT} TEXT)";
                cmd.ExecuteNonQuery();

                // Board Table
                cmd.CommandText = $"DROP TABLE IF EXISTS {TB_BOARD}";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE {TB_BOARD} ({COL_URL} TEXT PRIMARY KEY NOT NULL, {COL_GREATEST_SAVED} TEXT)";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Drop the current database and build a new one.
        /// </summary>
        private static void UpgradeDB()
        {
            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"SELECT * FROM {TB_VERSION}";

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader.GetInt32(0) != DATABASE_VERSION)
                        {
                            /*var tet = (LoadThreads(), LoadBoards());
                            CreateDB();
                            SaveAll(tet.Item1, tet.Item2);*/
                            CreateDB();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the thread and board lists to disk.
        /// </summary>
        public static void SaveAll(IList<Thread> threads, IList<Board> boards)
        {
            SaveThreads(threads);
            SaveBoards(boards);
        }

        public static void SaveThreads(IList<Thread> threads)
        {
            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"DELETE FROM {TB_THREAD}";

                cmd.ExecuteNonQuery();

                for (int i = 0; i < threads.Count; i++)
                {
                    cmd.CommandText = $@"INSERT INTO
                                            {TB_THREAD} ({COL_URL}, {COL_GREATEST_SAVED}, {COL_SUBJECT}) 
                                        VALUES 
                                            (@{COL_URL}, @{COL_GREATEST_SAVED}, @{COL_SUBJECT})";

                    cmd.Parameters.AddWithValue(COL_URL, threads[i].Url);
                    cmd.Parameters.AddWithValue(COL_GREATEST_SAVED, threads[i].GreatestSavedFileTim);
                    cmd.Parameters.AddWithValue(COL_SUBJECT, threads[i].Subject);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static IList<LoadedThreadInfo> LoadThreads()
        {
            List<LoadedThreadInfo> loadedThreads = new List<LoadedThreadInfo>();

            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"SELECT * FROM {TB_THREAD}";

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loadedThreads.Add(new LoadedThreadInfo(reader));
                    }
                }
            }

            return loadedThreads;
        }

        public static void SaveBoards(IList<Board> boards)
        {
            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"DELETE FROM {TB_BOARD}";

                cmd.ExecuteNonQuery();

                for (int i = 0; i < boards.Count; i++)
                {

                    cmd.CommandText = $@"INSERT INTO {TB_BOARD} ({COL_URL}, {COL_GREATEST_SAVED}) VALUES (@{COL_URL}, @{COL_GREATEST_SAVED})";
                    cmd.Parameters.AddWithValue(COL_URL, boards[i].Url);
                    cmd.Parameters.AddWithValue(COL_GREATEST_SAVED, boards[i].LargestAddedThreadNo);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static IList<LoadedBoardInfo> LoadBoards()
        {
            List<LoadedBoardInfo> loadedBoards = new List<LoadedBoardInfo>();

            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"SELECT * FROM {TB_BOARD}";

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loadedBoards.Add(new LoadedBoardInfo(reader));
                    }
                }
            }

            return loadedBoards;
        }
    }
}
