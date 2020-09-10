using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Data
{
    public static class Database
    {
        public static void Setup()
        {
            try
            {
                string database = FileSystem.Database;

                if (File.Exists(database)) return;

                using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
                {
                    connection.Open();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        using (SQLiteCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;

                            // Tables
                            command.CommandText = Table_Categories;
                            command.ExecuteNonQuery();

                            command.CommandText = Table_Channels;
                            command.ExecuteNonQuery();

                            command.CommandText = Table_Videos;
                            command.ExecuteNonQuery();

                            command.CommandText = Table_History;
                            command.ExecuteNonQuery();

                            command.CommandText = Table_Continuity;
                            command.ExecuteNonQuery();

                            command.CommandText = Table_Meta;
                            command.ExecuteNonQuery();

                            //Index
                            command.CommandText = Channels_Title_Index;
                            command.ExecuteNonQuery();

                            command.CommandText = Videos_Title_Index;
                            command.ExecuteNonQuery();

                            command.CommandText = Videos_ChannelId_Index;
                            command.ExecuteNonQuery();

                            // Metadata
                            command.CommandText = Table_Info_Description;
                            command.ExecuteNonQuery();

                            command.CommandText = Table_Info_Version;
                            command.ExecuteNonQuery();

                            command.CommandText = Table_Info_Created;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Database).Assembly.FullName,
                    ClassName = typeof(Database).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Database).Assembly.FullName,
                    ClassName = typeof(Database).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }

        // Tables
        private static string Table_Categories => @"CREATE TABLE Categories(
	                                    `Id`	        TEXT,
	                                    `Parent`	    TEXT,
	                                    `Title`	        TEXT,
	                                    `Color`	        TEXT);";

        private static string Table_Channels => @"CREATE TABLE Channels(
	                                    `Id`	        TEXT,
	                                    `SubscriptionId`TEXT,
	                                    `Parent`	    TEXT,
	                                    `Title`	        TEXT,
	                                    `Date`	        TEXT,
	                                    `Thumbnail`	    TEXT);";

        private static string Table_Videos => @"CREATE TABLE Videos(
	                                    `VideoId`       TEXT,
	                                    `ChannelId`     TEXT,
	                                    `Title`         TEXT,
	                                    `Link`	        TEXT,
	                                    `Published`	    INTEGER,
	                                    `Duration`	    INTEGER,
	                                    `Added`	        INTEGER,
	                                    `ThumbnailUrl`	TEXT,
                                        `State`         INTEGER DEFAULT 0);";

        private static string Table_History => @"CREATE TABLE History(
	                                    `Id`	        TEXT,
	                                    `Added`	        TEXT);";

        private static string Table_Continuity => @"CREATE TABLE Continuity(
	                                    `Id`	        TEXT UNIQUE,
	                                    `Date`	        TEXT,
	                                    `Second`	    INTEGER);";

        private static string Table_Meta => @"CREATE TABLE Meta (
	                                    `Key`	    TEXT,
	                                    `Value`	    TEXT
                                        );";

        // Index
        private static string Videos_ChannelId_Index => "CREATE INDEX idx1 ON Videos(ChannelId);";

        private static string Videos_Title_Index => "CREATE INDEX idx2 ON Videos(Title);";

        private static string Channels_Title_Index => "CREATE INDEX idx3 ON Channels(Title);";

        // Metadata
        private static string Table_Info_Description => @"INSERT INTO Meta (Key, Value) VALUES ('Description', 'YouTube Subscription Manager');";

        private static string Table_Info_Version => @"INSERT INTO Meta (Key, Value) VALUES ('Version', '2.0');";

        private static string Table_Info_Created => @"INSERT INTO Meta (Key, Value) VALUES ('Created', CURRENT_TIMESTAMP);";

    
    }
}
