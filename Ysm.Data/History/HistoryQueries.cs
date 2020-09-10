using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Data
{
    internal class HistoryQueries
    {
        internal static void Insert(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = HistorySql.Insert();

                        command.Parameters.Add(new SQLiteParameter("@Id", id));
                        command.Parameters.Add(new SQLiteParameter("@Added", DateTime.Now));

                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
        }

        internal static List<Video> Get()
        {
            List<Video> videos = new List<Video>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = HistorySql.Get();

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Video video = Create(reader);

                                if (video != null)
                                    videos.Add(video);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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

            return videos;
        }

        internal static void Remove(IEnumerable<string> ids)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = HistorySql.Remove(ids);

                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
        }

        internal static void Remove(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = HistorySql.Remove(id);

                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
        }

        internal static void RemoveAll()
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = HistorySql.RemoveAll();

                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
                        AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                        ClassName = typeof(HistoryQueries).FullName,
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
        }

        private static Video Create(SQLiteDataReader reader)
        {
            try
            {
                int index = 0;

                Video video = new Video();

                // Id
                if (!reader.IsDBNull(index))
                    video.VideoId = reader.GetString(index);

                index++;

                // ChannelId
                if (!reader.IsDBNull(index))
                    video.ChannelId = reader.GetString(index);

                index++;

                // Title
                if (!reader.IsDBNull(index))
                    video.Title = reader.GetString(index);

                index++;

                // Link
                if (!reader.IsDBNull(index))
                    video.Link = reader.GetString(index);

                index++;

                // Published
                if (!reader.IsDBNull(index))
                    video.Published = reader.GetDateTime(index);

                index++;

                // Duration
                if (!reader.IsDBNull(index))
                    video.Duration = new TimeSpan(reader.GetInt32(index));

                // Added
                index++;

                index++;

                // ThumbnailUrl
                //if (!reader.IsDBNull(index))
                //    video.ThumbnailUrl = reader.GetString(index);
                video.ThumbnailUrl = $"http://img.youtube.com/vi/{video.VideoId}/mqdefault.jpg";

                index++;

                // State
                if (!reader.IsDBNull(index))
                    video.State = reader.GetInt32(index);

                index++;

                // Added
                if (!reader.IsDBNull(index))
                    video.Added = reader.GetDateTime(index);

                return video;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(HistoryQueries).Assembly.FullName,
                    ClassName = typeof(HistoryQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }

            return null;
        }
    }
}
