using GChan.Trackers;
using System;
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
        public const int DATABASE_VERSION = 2;

        private const string DATABASE_FILENAME = "trackers.db";
        private static readonly string DATABASE_FOLDERNAME = "data";
        private static readonly string DATABASE_PATH = Path.Combine(AppContext.BaseDirectory, DATABASE_FOLDERNAME, DATABASE_FILENAME);
        private static readonly string DATABASE_CONNECTION_STRING = "DataSource=" + DATABASE_PATH;

        /// <summary>
        /// Tables
        /// </summary>
        private const string TB_VERSION = "version";
        private const string TB_THREAD = "thread";
        private const string TB_BOARD = "board";

        // Columns
        private const string COL_VERSION = "version";
        private const string COL_URL = "url";

        // Thread columns
        private const string COL_SUBJECT = "subject";
        private const string COL_SAVED_IDS = "saved_ids";

        // Board columns
        private const string COL_GREATEST_THREAD_ID = "greatest_thread_id";

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
                        Directory.CreateDirectory(DATABASE_FOLDERNAME);
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

                cmd.CommandText = $"CREATE TABLE {TB_THREAD} ({COL_URL} TEXT PRIMARY KEY NOT NULL, {COL_SUBJECT} TEXT, {COL_SAVED_IDS} TEXT)";
                cmd.ExecuteNonQuery();

                // Board Table
                cmd.CommandText = $"DROP TABLE IF EXISTS {TB_BOARD}";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE TABLE {TB_BOARD} ({COL_URL} TEXT PRIMARY KEY NOT NULL, {COL_GREATEST_THREAD_ID} INTEGER)";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Drop the current database and build a new one.
        /// </summary>
        private static void UpgradeDB()
        {
            int version = -1;

            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"SELECT * FROM {TB_VERSION}";

                using SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    version = reader.GetInt32(0);
                }
            }

            if (version == -1 || version != DATABASE_VERSION)
            {
                // TODO: Figure out migration strategies.
                CreateDB();
            }
        }

        /// <summary>
        /// Replaces the stored thread and board lists with <paramref name="threads"/> and <paramref name="boards"/>.
        /// </summary>
        public static void SaveAll(IList<Thread> threads, IList<Board> boards)
        {
            InTransaction(() =>
            {
                ReplaceThreads(threads);
                ReplaceBoards(boards);
            });
        }

        public static void SaveThreads(IList<Thread> threads) => InTransaction(() => ReplaceThreads(threads));

        public static void SaveBoards(IList<Board> boards) => InTransaction(() => ReplaceBoards(boards));

        /// <summary>
        /// Inserts or updates each of <paramref name="threads"/>, leaving any other stored threads untouched.
        /// </summary>
        public static void UpsertThreads(IEnumerable<Thread> threads)
        {
            InTransaction(() =>
            {
                foreach (var thread in threads)
                {
                    UpsertThread(thread);
                }
            });
        }

        /// <summary>
        /// Persists a single thread to the database (insert or replace).
        /// </summary>
        public static void SaveThread(Thread thread)
        {
            lock (_connectionLock)
            {
                UpsertThread(thread);
            }
        }

        /// <summary>
        /// Removes a single thread from the database by URL.
        /// </summary>
        public static void DeleteThread(string url)
        {
            lock (_connectionLock)
            {
                DeleteByUrl(TB_THREAD, url);
            }
        }

        /// <summary>
        /// Persists a single board to the database (insert or replace).
        /// </summary>
        public static void SaveBoard(Board board)
        {
            lock (_connectionLock)
            {
                UpsertBoard(board);
            }
        }

        /// <summary>
        /// Removes a single board from the database by URL.
        /// </summary>
        public static void DeleteBoard(string url)
        {
            lock (_connectionLock)
            {
                DeleteByUrl(TB_BOARD, url);
            }
        }

        /// <summary>
        /// Runs <paramref name="action"/> inside a single transaction while holding the connection lock.
        /// <see cref="SQLiteConnection"/> is not thread-safe, so all database access must hold <see cref="_connectionLock"/>.
        /// </summary>
        private static void InTransaction(Action action)
        {
            lock (_connectionLock)
            {
                using var transaction = Connection.BeginTransaction();
                action();
                transaction.Commit();
            }
        }

        private static void ReplaceThreads(IList<Thread> threads)
        {
            DeleteAll(TB_THREAD);

            foreach (var thread in threads)
            {
                UpsertThread(thread);
            }
        }

        private static void ReplaceBoards(IList<Board> boards)
        {
            DeleteAll(TB_BOARD);

            foreach (var board in boards)
            {
                UpsertBoard(board);
            }
        }

        private static void UpsertThread(Thread thread)
        {
            using var cmd = new SQLiteCommand(Connection);
            cmd.CommandText = $"INSERT OR REPLACE INTO {TB_THREAD} ({COL_URL}, {COL_SUBJECT}, {COL_SAVED_IDS}) VALUES (@{COL_URL}, @{COL_SUBJECT}, @{COL_SAVED_IDS})";
            cmd.Parameters.AddWithValue(COL_URL, thread.Url);
            cmd.Parameters.AddWithValue(COL_SUBJECT, thread.Subject);
            cmd.Parameters.AddWithValue(COL_SAVED_IDS, thread.SavedIds.ToStringList());
            cmd.ExecuteNonQuery();
        }

        private static void UpsertBoard(Board board)
        {
            using var cmd = new SQLiteCommand(Connection);
            cmd.CommandText = $"INSERT OR REPLACE INTO {TB_BOARD} ({COL_URL}, {COL_GREATEST_THREAD_ID}) VALUES (@{COL_URL}, @{COL_GREATEST_THREAD_ID})";
            cmd.Parameters.AddWithValue(COL_URL, board.Url);
            cmd.Parameters.AddWithValue(COL_GREATEST_THREAD_ID, board.GreatestThreadId);
            cmd.ExecuteNonQuery();
        }

        private static void DeleteByUrl(string table, string url)
        {
            using var cmd = new SQLiteCommand(Connection);
            cmd.CommandText = $"DELETE FROM {table} WHERE {COL_URL} = @{COL_URL}";
            cmd.Parameters.AddWithValue(COL_URL, url);
            cmd.ExecuteNonQuery();
        }

        private static void DeleteAll(string table)
        {
            using var cmd = new SQLiteCommand(Connection);
            cmd.CommandText = $"DELETE FROM {table}";
            cmd.ExecuteNonQuery();
        }

        public static IList<LoadedThreadData> LoadThreads()
        {
            var loadedThreads = new List<LoadedThreadData>();

            lock (_connectionLock)
            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"SELECT * FROM {TB_THREAD}";

                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    loadedThreads.Add(new LoadedThreadData(reader));
                }
            }

            return loadedThreads;
        }

        public static IList<LoadedBoardData> LoadBoards()
        {
            var loadedBoards = new List<LoadedBoardData>();

            lock (_connectionLock)
            using (var cmd = new SQLiteCommand(Connection))
            {
                cmd.CommandText = $"SELECT * FROM {TB_BOARD}";

                using SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    loadedBoards.Add(new LoadedBoardData(reader));
                }
            }

            return loadedBoards;
        }
    }
}
