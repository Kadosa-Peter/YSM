using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeExplode;
using YoutubeExplode.Models;
using Ysm.Core;
using Ysm.Downloader.Assets;
using Ysm.Downloader.Download;
using Ysm.Downloader.Views;
using Ysm.Downloader.Windows;

// test: https://www.youtube.com/watch?v=NgTdNd5lkvY unplayable

namespace Ysm.Downloader
{
    public partial class MainWindow
    {
        private bool _isLoaded;

        private bool _audioOnly;

        private string _id;

        public MainWindow()
        {
            InitializeComponent();

            Task.Run(() => StartCleanup());

        }

        private /*async*/ void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;

            SetPosition();

            if (_id.NotNull())
            {
                CreateDownload(_id);
            }

            if (DebugMode.IsDebugMode)
            {
                //CreateDownload("https://www.youtube.com/watch?v=rih_xFUWyQQ");
                //await CreateDownloadFromPlaylist("https://www.youtube.com/watch?v=kE6_B5G14AE&list=PLbb_Ofa_DkjFodipOWYoqPYl-f7rgQ3Te"); // 75
                //await CreateDownloadFromPlaylist("https://www.youtube.com/playlist?list=PLeoTwkKsw0mnEuot8owRgYDCwSQjtGic1"); // 9
                //await CreateDownloadFromPlaylist("https://www.youtube.com/playlist?list=PL72e6dBabYNVRfpvEUt-WNfV7tZuKDAPB"); //8
            }
        }

        private void SetPosition()
        {
            double m_witdh = SystemParameters.PrimaryScreenWidth;
            double m_height = SystemParameters.PrimaryScreenHeight;

            Top = (m_height - 600) / 2 + 5;
            Left = (m_witdh - 800) / 2;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            CloseCleanup();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Add_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();

                    if (text.StartsWith("http"))
                    {
                        if (text.Contains("watch?v=") && text.Contains("list="))
                        {
                            // 0 = playlist, 1 = video
                            int result = DialogHelper.ShowPlaylist();

                            if (result == 0)
                            {
                                string playlistId = Helpers.GetPlaylistIdFromMixedUrl(text);

                                await CreateDownloadFromPlaylist(playlistId);
                            }
                            else
                            {
                                string videoId = Helpers.GetVideoIdFromMixedUrl(text);

                                CreateDownload(videoId);
                            }

                        }
                        else if (text.Contains("list="))
                        {
                            await CreateDownloadFromPlaylist(text);
                        }
                        else
                        {
                            CreateDownload(text);
                        }
                    }
                    else
                    {
                        if (text.Length == 11)
                        {
                            CreateDownload(text);
                        }
                        else
                        {
                            // BUG: Ha a vágólapon általános szöveg azaz nem YT url vagy id van, akkor ez a metódus hívódik meg és dob kivételt.
                            await CreateDownloadFromPlaylist(text);
                        }
                    }

                }
                else
                {
                    DialogHelper.ShowInfoWindow(Properties.Resources.Warning_no_id);
                }
            }
            catch
            {
                DialogHelper.ShowInfoWindow(Properties.Resources.Warning_invalid_id);
                AddButton.IsEnabled = true;
            }
        }

        public void CreateDownload(string arg)
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            if (IsActive == false)
                Activate();

            if (arg.StartsWith("@"))
            {
                CreateDownloadFromCommandLine(arg);
            }
            else
            {
                string id = Validator.ValidateVideoId(arg);

                // Invalid Id
                if (id.IsNull())
                {
                    if (_isLoaded)
                    {
                        DialogHelper.ShowInfoWindow(Properties.Resources.Warning_invalid_id);
                    }
                    return;
                }

                if (_isLoaded)
                {
                    AddButton.IsEnabled = false;

                    try
                    {
                        OpenDownloadWindow(id);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(MethodBase.GetCurrentMethod(), e);
                    }

                    AddButton.IsEnabled = true;
                }
                else // MainWindow is not loaded yet
                {
                    _id = id;
                }
            }
        }

        private void CreateDownloadFromCommandLine(string arg)
        {
            List<string> ids = arg.Substring(1).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (ids.Count == 0) return;

            if (ids.Count > 1)
            {
                AddButton.IsEnabled = false;

                Task.Run(() => GetVideoFromId(ids)).ContinueWith(t =>
                {
                    List<Video> videos = t.Result;

                    if (videos.Count != 0)
                    {
                        DownloadModeWindowResult result = DialogHelper.OpenDownloadModeWindow(videos.Count);

                        if (!result.IsCancelled)
                        {
                            if (result.PlaylistDownloadMode == PlaylistDownloadMode.DownloadOneByOne)
                            {
                                DownloadOneByOne(ids, null);
                            }
                            else
                            {
                                int preferredQuality = result.PreferredQuality;
                                bool audioOnly = result.PreferredQuality == 0;

                                DownloadAllAtOnce(videos, audioOnly, preferredQuality);
                            }
                        }
                        else
                        {
                            AddButton.IsEnabled = true;
                        }
                    }
                    else
                    {
                        AddButton.IsEnabled = true;
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                AddButton.IsEnabled = false;

                OpenDownloadWindow(ids[0]);

                AddButton.IsEnabled = true;
            }
        }

        private List<Video> GetVideoFromId(List<string> ids)
        {
            YoutubeClient client = new YoutubeClient();

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 };

            ConcurrentBag<Video> videos = new ConcurrentBag<Video>();

            Parallel.ForEach(ids, options, id =>
            {
                try
                {
                    Video video = client.GetVideoAsync(id).Result;

                    if (video != null)
                    {
                        videos.Add(video);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), e);
                }

            });

            return videos.ToList();
        }

        private async Task CreateDownloadFromPlaylist(string arg)
        {
            YoutubeClient client = new YoutubeClient();
            string playlistId;

            if (arg.Contains("list="))
            {
                playlistId = Helpers.GetPlaylistId(arg);
            }
            else
            {
                playlistId = arg;
            }

            if (playlistId.IsNull())
            {
                DialogHelper.ShowInfoWindow(Properties.Resources.Warning_invalid_id);
                return;
            }

            AddButton.IsEnabled = false;

            Playlist playlist = await client.GetPlaylistAsync(playlistId);

            DownloadModeWindowResult result = DialogHelper.OpenDownloadModeWindow(playlist);

            if (!result.IsCancelled)
            {
                if (result.PlaylistDownloadMode == PlaylistDownloadMode.DownloadOneByOne)
                {
                    List<string> ids = playlist.Videos.Select(x => x.Id).ToList();
                    DownloadOneByOne(ids, playlist.Title);
                }
                else
                {
                    int preferredQuality = result.PreferredQuality;
                    bool audioOnly = result.PreferredQuality == 0;
                    DownloadAllAtOnce(playlist.Videos, audioOnly, preferredQuality);
                }
            }
            else
            {
                AddButton.IsEnabled = true;
            }
        }

        private async void DownloadAllAtOnce(IEnumerable<Video> videos, bool audioOnly, int preferredQuality)
        {
            string output = DownloadPath.Get();

            foreach (Video video in videos)
            {
                StreamObj streamObj = await PreferredStream.GetAsync(video.Id, audioOnly, preferredQuality);
                if (streamObj == null)
                {
                    continue;
                }

                if (audioOnly)
                {
                    DownloadView view = new DownloadView(streamObj.AudioStream, video, output);
                    view.Added += (k, l) => UpdateView();
                    view.Removed += (k, l) => UpdateView();
                    DownloadViewHost.Children.Add(view);
                }
                else if (streamObj.MuxedStream != null)
                {
                    DownloadView view = new DownloadView(streamObj.MuxedStream, video, false, "", output);
                    view.Added += (k, l) => UpdateView();
                    view.Removed += (k, l) => UpdateView();
                    DownloadViewHost.Children.Add(view);
                }
                else
                {
                    DownloadView view = new DownloadView(streamObj.VideoStream, streamObj.AudioStream, video, false, "", output);
                    view.Added += (k, l) => UpdateView();
                    view.Removed += (k, l) => UpdateView();
                    DownloadViewHost.Children.Add(view);
                }
            }

            AddButton.IsEnabled = true;
        }

        private void DownloadOneByOne(List<string> ids, string playlistTitle)
        {
            int i = 1;
            int count = ids.Count;

            bool audioOnly = false;

            foreach (string id in ids)
            {
                var tupple = OpenDownloadWindow(playlistTitle, id, i, count, audioOnly);
                if (tupple.Item1) break;
                audioOnly = tupple.Item2;
                i++;
            }

            AddButton.IsEnabled = true;
        }

        private (bool cancelAll, bool onlyAudio) OpenDownloadWindow(string playlistTitle, string id, int index, int count, bool audioOnly)
        {
            DownloadWindow window = new DownloadWindow(playlistTitle, id, index, count, audioOnly);
            window.Owner = this;
            window.Download += DownloadWindow_Download;
            window.ShowDialog();

            return (cancelAll: window.CancelAll, onlyAudio: window.OnlyAudio);
        }

        private void OpenDownloadWindow(string id)
        {
            DownloadWindow window = new DownloadWindow(id, _audioOnly);
            window.Owner = this;
            window.Download += DownloadWindow_Download;
            window.ShowDialog();

            _audioOnly = window.OnlyAudio;
        }

        private void DownloadWindow_Download(object sender, DownloadWindowEventArgs e)
        {
            if (e.OnlyAudio)
            {
                DownloadView view = new DownloadView(e.AudioStream, e.Video, e.Output);
                view.Added += (k, l) => UpdateView();
                view.Removed += (k, l) => UpdateView();
                DownloadViewHost.Children.Add(view);
            }
            else if (e.MuxedStream != null)
            {
                DownloadView view = new DownloadView(e.MuxedStream, e.Video, e.Convert, e.Format, e.Output);
                view.Added += (k, l) => UpdateView();
                view.Removed += (k, l) => UpdateView();
                DownloadViewHost.Children.Add(view);
            }
            else
            {
                DownloadView view = new DownloadView(e.VideoStream, e.AudioStream, e.Video, e.Convert, e.Format, e.Output);
                view.Added += (k, l) => UpdateView();
                view.Removed += (k, l) => UpdateView();
                DownloadViewHost.Children.Add(view);
            }

            UpdateView();
        }

        private void UpdateView()
        {
            ViewHostScrollViewer.ScrollToBottom();

            int videoCount = DownloadViewHost.Children.Count;

            if (videoCount == 0)
            {
                HeaderView.SetTitle("Y.S.M. Downloader");
            }
            else if (videoCount == 1)
            {
                HeaderView.SetTitle($"Y.S.M. Downloader (1 {Properties.Resources.Text_Video})");
            }
            else
            {
                HeaderView.SetTitle($"Y.S.M. Downloader ({videoCount} {Properties.Resources.Text_Videos})");
            }
        }

        private void CloseCleanup()
        {
            try
            {
                Process[] ffmpeg = Process.GetProcessesByName("ffmpeg");

                foreach (Process process in ffmpeg)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
                    }
                }

                DirectoryInfo info = new DirectoryInfo(FileSystem.Temp);

                foreach (FileInfo file in info.GetFiles())
                {
                    File.Delete(file.FullName);
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }
        }

        private void StartCleanup()
        {
            DirectoryInfo info = new DirectoryInfo(FileSystem.Temp);

            foreach (FileInfo file in info.GetFiles())
            {
                string ext = Path.GetExtension(file.Name);

                if (ext == "mp4" || ext == "mkv" || ext == "flv" || ext == "webm")
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
                    }
                }
            }
        }

        private void Help_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.yosuma.com/userguide/v1/#download");
        }
    }
}
