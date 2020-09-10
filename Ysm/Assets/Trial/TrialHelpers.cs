using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Assets.Trial
{
    public static class TrialHelpers
    {
        public static string GetFileVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion.Remove(fvi.FileVersion.Length - 4);
        }

        public static string GetIdentifier()
        {
            string id = GetMotherBoardID();

            if (id.IsNull())
            {
                id = GetProcessorId();
            }

            if (id.IsNull())
            {
                return null;
            }

            return id;
        }

        private static string GetMotherBoardID()
        {
            try
            {
                string mbInfo = String.Empty;
                ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
                scope.Connect();
                ManagementObject wmiClass = new ManagementObject(scope, new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new ObjectGetOptions());

                foreach (PropertyData propData in wmiClass.Properties)
                {
                    if (propData.Name == "SerialNumber")
                        mbInfo = Convert.ToString(propData.Value);
                }

                if (mbInfo.Contains(" ") || mbInfo.Any(char.IsDigit) == false)
                {
                    return null;
                }

                return mbInfo;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetProcessorId()
        {
            try
            {
                string cpuInfo = null;

                ManagementClass managClass = new ManagementClass("win32_processor");
                ManagementObjectCollection managCollec = managClass.GetInstances();

                foreach (ManagementBaseObject o in managCollec)
                {
                    ManagementObject managObj = (ManagementObject)o;
                    if (managObj != null)
                    {
                        cpuInfo = managObj.Properties["processorID"].Value.ToString();
                        break;
                    }
                }

                if (cpuInfo == null || cpuInfo.Contains(" ") || cpuInfo.Any(char.IsDigit) == false)
                {
                    return null;
                }

                return cpuInfo;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void DBSaveTrial(TrialEntry entry)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Kernel.Default.ConnectionString.ConvertToString()))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO [Trial]([Id], [Version], [DateTime], [Country]) VALUES(@Id, @Vesrion, @DateTime, @Country)";
                        command.Parameters.Add("@Id", SqlDbType.NVarChar).Value = entry.Id;
                        command.Parameters.Add("@Vesrion", SqlDbType.NVarChar).Value = entry.Version;
                        command.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = entry.DateTime;
                        command.Parameters.Add("@Country", SqlDbType.NVarChar).Value = entry.Country;
                        command.ExecuteScalar();
                    }
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public static TrialEntry DBReadTrial(string id, string version)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Kernel.Default.ConnectionString.ConvertToString()))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT * FROM [dbo].[Trial] WHERE Id = '{id}' AND Version = '{version}';";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TrialEntry entry = new TrialEntry();
                                entry.DateTime = reader.GetDateTime(2);
                                entry.Id = id;
                                entry.Version = version;

                                return entry;
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        public static void SettingsSaveTrial(DateTime dateTime)
        {
            try
            {
                // Mikor lett regisztrálva a trial. Innen számítva 48 óra van vissza trialból.
                string dt = StringEncryption.Encrypt(dateTime.ToString(CultureInfo.InvariantCulture), Kernel.Default.EncryptionKey.ConvertToString());
                Settings.Default.Trial = dt;
                Settings.Default.Save();
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
        }

        public static double SettingsReadTrial()
        {
            try
            {
                if (Settings.Default.Trial.IsNull())
                {
                    return -1;
                }
                else
                {
                    string dt = StringEncryption.Decrypt(Settings.Default.Trial, Kernel.Default.EncryptionKey.ConvertToString());
                    DateTime dateTime = DateTime.Parse(dt);

                    TimeSpan ts = DateTime.Now - dateTime;

                    return Math.Round(ts.TotalHours);
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }

            return -1;
        }

        public static string GetCountry()
        {
            try
            {
                return RegionInfo.CurrentRegion.EnglishName;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(TrialHelpers).Assembly.FullName,
                    ClassName = typeof(TrialHelpers).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }

            return "unknown";
        }
    }
}