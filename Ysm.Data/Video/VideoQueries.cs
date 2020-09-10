using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Data
{
    internal static class VideoQueries
    {
        internal static void Insert(List<Video> videos)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Video video in videos)
                        {
                            if (video == null)
                            {
                                continue;
                            }

                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = VideoSql.Insert();

                                command.Parameters.Add(new SQLiteParameter("@VideoId", video.VideoId));
                                command.Parameters.Add(new SQLiteParameter("@ChannelId", video.ChannelId));
                                command.Parameters.Add(new SQLiteParameter("@Title", video.Title));
                                command.Parameters.Add(new SQLiteParameter("@Link", video.Link));
                                command.Parameters.Add(new SQLiteParameter("@Published", video.Published));
                                command.Parameters.Add(new SQLiteParameter("@Duration", video.Duration.Ticks));
                                command.Parameters.Add(new SQLiteParameter("@Added", video.Added));
                                command.Parameters.Add(new SQLiteParameter("@ThumbnailUrl", video.ThumbnailUrl));

                                command.ExecuteScalar();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
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
                        command.CommandText = VideoSql.Get();

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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static List<Video> Get(List<string> chnnales)
        {
            List<Video> videos = new List<Video>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = VideoSql.Get(chnnales);

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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static void Delete(List<string> ids)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (string id in ids)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = VideoSql.Delete(id);

                                command.ExecuteScalar();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                }
            }
        }

        internal static void Delete()
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {

                        command.CommandText = VideoSql.Delete();

                        command.ExecuteScalar();
                    }

                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static IEnumerable<Video> Search(string query, List<string> channels, int state)
        {
            List<Video> Videos = new List<Video>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = VideoSql.Search(query, channels, state);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Video video = Create(reader);

                                if (video != null)
                                    Videos.Add(video);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

            return Videos;
        }

        internal static void MarkWatched(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = VideoSql.MarkVideoWatched(id);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static void MarkWatched(List<string> channelIds)
        {
            // ids = channels ids

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (string id in channelIds)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = VideoSql.MarkWatched(id);

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                }
            }
        }

        internal static void MarkWatched(DateTime dateTime)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = VideoSql.MarkWatched(dateTime);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static void MarkAllWatched()
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = VideoSql.MarkAllWatched();
                        command.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static void MarkAllUnwatched()
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = VideoSql.MarkAllUnwatched();

                        command.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static void MarkUnwatchedDate(IEnumerable<DateTimeOffset> dateList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (DateTimeOffset dateTimeOffset in dateList)
                        {
                            // mark unwatched 
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = VideoSql.MarkUnwatchedDate(dateTimeOffset.Date);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                }
            }
        }

        internal static void MarkUnwatched(IEnumerable<DateTimeOffset> dateList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (DateTimeOffset dateTimeOffset in dateList)
                        {
                            // mark unwatched 
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = VideoSql.MarkUnwatched(dateTimeOffset.DateTime);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                }
            }
        }

        internal static void MarkUnwatchedAfter(DateTime dateTime)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = VideoSql.MarkUnwatchedAfter(dateTime);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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
                        AssemblyName = typeof(VideoQueries).Assembly.FullName,
                        ClassName = typeof(VideoQueries).FullName,
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

        internal static void MarkUnwatchedByChannelId(List<string> channels)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (string channel in channels)
                        {
                            // mark unwatched 
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = VideoSql.MarkUnwatchedByChannel(channel);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(VideoQueries).Assembly.FullName,
                            ClassName = typeof(VideoQueries).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            InnerException = ex.InnerException?.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                        transaction.Rollback();
                    }
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
                    video.Duration = new TimeSpan(reader.GetInt64(index));

                index++;

                // Added
                if (!reader.IsDBNull(index))
                    video.Added = reader.GetDateTime(index);

                index++;


                // https://stackoverflow.com/questions/2068344/how-do-i-get-a-youtube-video-thumbnail-from-the-youtube-api
                // https://boingboing.net/features/getthumbs.html

                // ThumbnailUrl
                //if (!reader.IsDBNull(index))
                //    video.ThumbnailUrl = reader.GetString(index);
                video.ThumbnailUrl = $"http://img.youtube.com/vi/{video.VideoId}/mqdefault.jpg";

                index++;

                // State
                if (!reader.IsDBNull(index))
                    video.State = reader.GetInt32(index);

                return video;
            }
            catch (SQLiteException ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(VideoQueries).Assembly.FullName,
                    ClassName = typeof(VideoQueries).FullName,
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
                    AssemblyName = typeof(VideoQueries).Assembly.FullName,
                    ClassName = typeof(VideoQueries).FullName,
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
