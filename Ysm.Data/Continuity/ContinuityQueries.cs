using System;
using System.Data.SQLite;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Data
{
    internal class ContinuityQueries
    {
        internal static void Save(string id, int second)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ContinuitySql.Save();

                        command.Parameters.Add(new SQLiteParameter("@Id", id));
                        command.Parameters.Add(new SQLiteParameter("@Date", DateTime.Now.ToSqlDateTime()));
                        command.Parameters.Add(new SQLiteParameter("@Second", second));

                        command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ContinuityQueries).Assembly.FullName,
                    ClassName = typeof(ContinuityQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static int Get(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(FileSystem.ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = ContinuitySql.Get(id);

                        object obj = command.ExecuteScalar();

                        if (obj != null)
                            return Convert.ToInt32(obj);
                    }
                }
                catch (SQLiteException ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(ContinuityQueries).Assembly.FullName,
                        ClassName = typeof(ContinuityQueries).FullName,
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
                        AssemblyName = typeof(ContinuityQueries).Assembly.FullName,
                        ClassName = typeof(ContinuityQueries).FullName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ExceptionType = ex.GetType().ToString(),
                        Message = ex.Message,
                        InnerException = ex.InnerException?.Message,
                        Trace = ex.StackTrace
                    };

                    Logger.Log(error);

                    #endregion
                }

                return 0;
            }
        }
    }
}
