using System;
using System.Data.SQLite;
using System.IO;

namespace Ysm.Setup
{
   public static class Database
    {
        public static void AddColorColumn()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = Path.Combine(appData, "Ysm", "v1");

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            foreach (FileInfo file in directoryInfo.GetFiles("*.sqlite3", SearchOption.AllDirectories))
            {
                string connectionString = $"Data Source = {file.FullName}; Version=3; PRAGMA automatic_index = 0;";

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = connection.CreateCommand())
                        {
                            // Tables
                            command.CommandText = "ALTER TABLE `Categories` ADD COLUMN `Color` Text";
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    // duplicate column name
                    // just ignor
                }
            }
        }

        public static void AddContinueTable()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = Path.Combine(appData, "Ysm", "v1");

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            foreach (FileInfo file in directoryInfo.GetFiles("*.sqlite3", SearchOption.AllDirectories))
            {
                string connectionString = $"Data Source = {file.FullName}; Version=3; PRAGMA automatic_index = 0;";

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = connection.CreateCommand())
                        {
                            // Tables
                            command.CommandText = "CREATE TABLE Continue('Id' TEXT UNIQUE,'End' INTEGER);";
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    // table already xists
                    // just ignor
                }
            }
        }
    }
}
