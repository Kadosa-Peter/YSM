using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using Ysm.Core;
using Ysm.Data;
using Ysm.Windows;

namespace Ysm.Assets.Transfer
{
    public class Export : TransferBase
    {
        private TransferWindow _window;

        private string _path;

        protected override void BeforeStart()
        {
            base.BeforeStart();

            _path = Dialogs.ExportData();

            _path = Files.VerifyFilePath(_path);

            if (_path.NotNull())
            {
                _window = new TransferWindow("Exporting YSM Data");
                _window.Owner = Application.Current.MainWindow;
                _window.Show();
            }
        }

        protected override void DoWork()
        {
            if(_path.IsNull()) return;

            CreateBackupFiles();

            try
            {
                using (ZipFile zip = new ZipFile(_path, Encoding.UTF8))
                {
                    zip.CompressionLevel = CompressionLevel.BestCompression;
                    zip.Comment = $"YSM Data 1.1 -- {DateTime.Now.ToString("yy-MM-dd", CultureInfo.CurrentCulture)}";

                    foreach (FileInfo file in new DirectoryInfo(FileSystem.Backup).GetFiles())
                    {
                        zip.AddFile(file.FullName, "");
                    }

                    foreach (FileInfo file in new DirectoryInfo(FileSystem.Playlists).GetFiles())
                    {
                        zip.AddFile(file.FullName, "Playlists");
                    }

                    foreach (FileInfo file in new DirectoryInfo(FileSystem.Markers).GetFiles())
                    {
                        zip.AddFile(file.FullName, "Bookmarks");
                    }

                    zip.Save();
                }

                Cleanup();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }

        private void CreateBackupFiles()
        {
            try
            {
                List<Category> categories = Repository.Default.Categories.Get();
                string categoriesJson = JsonConvert.SerializeObject(categories, Formatting.Indented);
                Path.Combine(FileSystem.Backup, "categories").WriteText(categoriesJson);

                Dictionary<string, string> channels = Repository.Default.Channels.Get().ToDictionary(x => x.Id, x => x.Parent);
                string channelsJson = JsonConvert.SerializeObject(channels, Formatting.Indented);
                Path.Combine(FileSystem.Backup, "channels").WriteText(channelsJson);
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        protected override void AfterFinish()
        {
            _window?.Close();
        }

    }
}
