using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Data
{
    internal static class CategoryQueries
    {
        internal static void Insert(Category category)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = CategorySql.Insert();

                        command.Parameters.Add(new SQLiteParameter("@Id", category.Id));
                        command.Parameters.Add(new SQLiteParameter("@Parent", category.Parent));
                        command.Parameters.Add(new SQLiteParameter("@Title", category.Title));
                        command.Parameters.Add(new SQLiteParameter("@Color", category.Color));

                        command.ExecuteScalar();
                    }

                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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

        internal static void Insert(List<Category> categories)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Category category in categories)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = CategorySql.Insert();

                                command.Parameters.Add(new SQLiteParameter("@Id", category.Id));
                                command.Parameters.Add(new SQLiteParameter("@Parent", category.Parent));
                                command.Parameters.Add(new SQLiteParameter("@Title", category.Title));
                                command.Parameters.Add(new SQLiteParameter("@Color", category.Color));

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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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

        internal static List<Category> Get()
        {
            List<Category> categories = new List<Category>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = CategorySql.Get();

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Category category = Create(reader);

                                if (category != null)
                                    categories.Add(category);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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

            return categories;
        }

        internal static List<Category> Get(string parent)
        {
            List<Category> categories = new List<Category>();

            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = CategorySql.Get(parent);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Category category = Create(reader);

                                if (category != null)
                                    categories.Add(category);
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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

            return categories;
        }

        internal static Category Get_By_Id(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = CategorySql.Get_By_Id(id);

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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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

        internal static void Update(Category category)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = CategorySql.Update();

                        command.Parameters.Add(new SQLiteParameter("@Id", category.Id));
                        command.Parameters.Add(new SQLiteParameter("@Parent", category.Parent));
                        command.Parameters.Add(new SQLiteParameter("@Title", category.Title));
                        command.Parameters.Add(new SQLiteParameter("@Color", category.Color));

                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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

        internal static void Update(List<Category> categories)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Category category in categories)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = CategorySql.Update();

                                command.Parameters.Add(new SQLiteParameter("@Id", category.Id));
                                command.Parameters.Add(new SQLiteParameter("@Parent", category.Parent));
                                command.Parameters.Add(new SQLiteParameter("@Title", category.Title));
                                command.Parameters.Add(new SQLiteParameter("@Title", category.Color));

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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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
                        command.CommandText = CategorySql.Delete();
                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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

        internal static void Delete(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = CategorySql.Delete(id);
                        command.ExecuteScalar();
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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
                        AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                        ClassName = typeof(CategoryQueries).FullName,
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

                                command.CommandText = CategorySql.Delete(id);
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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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

                                command.CommandText = CategorySql.Move(kvp.Key, kvp.Value);

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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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
                            AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                            ClassName = typeof(CategoryQueries).FullName,
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

        private static Category Create(SQLiteDataReader reader)
        {
            try
            {
                int index = 0;

                Category category = new Category();

                // Id
                if (!reader.IsDBNull(index))
                    category.Id = reader.GetString(index);

                index++;

                // Parent
                if (!reader.IsDBNull(index))
                    category.Parent = reader.GetString(index);

                index++;

                // Title
                if (!reader.IsDBNull(index))
                    category.Title = reader.GetString(index);

                index++;

                // Title
                if (!reader.IsDBNull(index))
                    category.Color = reader.GetString(index);

                return category;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(CategoryQueries).Assembly.FullName,
                    ClassName = typeof(CategoryQueries).FullName,
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
