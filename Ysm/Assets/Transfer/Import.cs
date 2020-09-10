using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;
using Ysm.Windows;

namespace Ysm.Assets.Transfer
{
    public class Import : TransferBase
    {
        private string backup_file;

        private TransferWindow _window;

        protected override void BeforeStart()
        {
            // Import will overwrite all of your current data. Do you want to proceed?
            DialogWindow window = new DialogWindow(Properties.Resources.Question_Import);
            window.Owner = Application.Current.MainWindow;
            window.UserAnswer += answer =>
            {
                if (answer == Answer.Yes)
                {
                    Allowed = true;
                }
                else
                {
                    Allowed = false;
                }
            };
            window.ShowDialog();

            if (Allowed)
            {
                backup_file = Dialogs.ImportData();

                if (File.Exists(backup_file) == false)
                    return;

                Kernel.Default.Import = true;

                SubscriptionServiceWrapper.Default.Cancel();
                VideoServiceWrapper.Default.Cancel();

                _window = new TransferWindow("Importing YSM Data");
                _window.Owner = Application.Current.MainWindow;
                _window.Show();
            }
        }

        protected override void DoWork()
        {
            if (File.Exists(backup_file) == false)
                return;

            try
            {
                using (ZipFile zip = ZipFile.Read(backup_file))
                {
                    zip.ExtractAll(FileSystem.Backup, ExtractExistingFileAction.OverwriteSilently);
                }

                ImportCategories(Path.Combine(FileSystem.Backup, "categories"));
                ImportChannels(Path.Combine(FileSystem.Backup, "channels"));
                MoveFiles(FileSystem.Playlists, "Playlists");
                MoveFiles(FileSystem.Markers, "Bookmarks");

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

        private void MoveFiles(string targetDir, string sourceDir)
        {
            string playlists = Path.Combine(FileSystem.Backup, sourceDir);

            DirectoryInfo info = new DirectoryInfo(playlists);
            
            if (info.Exists)
            {
                foreach (FileInfo file in info.GetFiles())
                {
                    file.CopyTo(Path.Combine(targetDir, file.Name), true);
                }
            }
            
        }


        private void ImportCategories(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = path.ReadText();

                    if (json.NotNull())
                    {
                        List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(json);

                        Repository.Default.Categories.Remove();

                        Repository.Default.Categories.Insert(categories);
                    }
                }
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

        private void ImportChannels(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = path.ReadText();

                    if (json.NotNull())
                    {
                        Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                        List<Channel> channels = Repository.Default.Channels.Get();

                        foreach (Channel channel in channels)
                        {
                            if (dictionary.TryGetValue(channel.Id, out var parent) == false)
                            {
                                parent = Identifier.Empty;
                            }

                            channel.Parent = parent;
                        }

                        Repository.Default.Channels.Update(channels);
                    }
                }
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
            ViewRepository.Get<ChannelView>()?.Reset();
            ViewRepository.Get<MarkerView>()?.Reset();
            ViewRepository.Get<PlaylistView>()?.Reset();
            ViewRepository.Get<FavoritesView>()?.Reset();
            ViewRepository.Get<WatchLaterView>()?.Reset();

            Kernel.Default.Import = false;

            _window?.Close();
        }
    }
}
