using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Data
{
    internal static class ChannelQueries
    {
        internal static void Insert(List<Channel> channels)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Channel channel in channels)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = ChannelSql.Insert();

                                command.Parameters.Add(new SQLiteParameter("@Id", channel.Id));
                                command.Parameters.Add(new SQLiteParameter("@SubscriptionId", channel.SubscriptionId));
                                command.Parameters.Add(new SQLiteParameter("@Parent", channel.Parent));
                                command.Parameters.Add(new SQLiteParameter("@Title", channel.Title));
                                command.Parameters.Add(new SQLiteParameter("@Date", channel.Date));
                                command.Parameters.Add(new SQLiteParameter("@Thumbnail", channel.Thumbnail));

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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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

        internal static List<Channel> Get()
        {
            List<Channel> channels = new List<Channel>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ChannelSql.Get();

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Channel channel = Create(reader);

                                if (channel != null)
                                    channels.Add(channel);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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

            return channels;
        }

        internal static Channel Get_By_Id(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ChannelSql.Get_By_Id(id);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return Create(reader);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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

            return null;
        }

        internal static List<Channel> Get_By_Parent(string parent)
        {
            List<Channel> channels = new List<Channel>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ChannelSql.Get_By_Parent(parent);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Channel channel = Create(reader);

                                if (channel != null)
                                    channels.Add(channel);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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

            return channels;
        }

        internal static void Update(List<Channel> channels)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Channel channel in channels)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = ChannelSql.Update();

                                command.Parameters.Add(new SQLiteParameter("@Id", channel.Id));
                                command.Parameters.Add(new SQLiteParameter("@Parent", channel.Parent));
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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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

        internal static void Move(Dictionary<string, string> dictionary)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (KeyValuePair<string, string> kvp in dictionary)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = ChannelSql.Move(kvp.Key, kvp.Value);

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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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

                                command.CommandText = ChannelSql.Delete(id);
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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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
                            AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                            ClassName = typeof(ChannelQueries).FullName,
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

        internal static void DeleteAll()
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ChannelSql.DeleteAll();
                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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

        internal static List<Channel> Search(string text)
        {
            List<Channel> channels = new List<Channel>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ChannelSql.Search(text);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Channel channel = Create(reader);

                                if (channel != null)
                                    channels.Add(channel);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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
                        AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                        ClassName = typeof(ChannelQueries).FullName,
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

            return channels;
        }

        private static Channel Create(SQLiteDataReader reader)
        {
            try
            {
                int index = 0;

                Channel channel = new Channel();

                // Id
                if (!reader.IsDBNull(index))
                    channel.Id = reader.GetString(index);

                index++;


                // SubscriptionId
                if (!reader.IsDBNull(index))
                    channel.SubscriptionId = reader.GetString(index);

                index++;

                // Parent
                if (!reader.IsDBNull(index))
                    channel.Parent = reader.GetString(index);

                index++;

                // Title
                if (!reader.IsDBNull(index))
                    channel.Title = reader.GetString(index);

                index++;

                // Date
                if (!reader.IsDBNull(index))
                    channel.Date = reader.GetDateTime(index);

                index++;

                // Thumbnail
                if (!reader.IsDBNull(index))
                    channel.Thumbnail = reader.GetString(index);

                index++;

                // State
                if (!reader.IsDBNull(index))
                    channel.State = reader.GetInt32(index);

                return channel;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ChannelQueries).Assembly.FullName,
                    ClassName = typeof(ChannelQueries).FullName,
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
