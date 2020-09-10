using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using YoutubeExplode;
using Ysm.Actions;
using Ysm.Assets;
using Ysm.Assets.Caches;
using Ysm.Core;
using Ysm.Data;
using Ysm.Data.Comments;
using Ysm.Windows;

namespace Ysm.Views
{
    public partial class PlayerControlView
    {
        private Video _video;
        private string _patreonUrl;

        public PlayerControlView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(PlayerControlView));

            FavoritesCache.Default.AllRemoved += Favorites_AllRemoved;
            FavoritesCache.Default.Removed += Favorites_Removed;
            FavoritesCache.Default.Added += Favorites_Added;
            WatchLaterCache.Default.AllRemoved += WatchLater_AllRemoved;
            WatchLaterCache.Default.Removed += WatchLater_Removed;
            WatchLaterCache.Default.Added += WatchLater_Added;

            //VisibleNodes.Default.Changed += Locate_Changed;

            //if (DebugMode.IsDebugMode)
            //{
            //    btnGetSource.Visibility = Visibility.Visible;
            //}
        }

        public void SetVideo(Video video)
        {
            _video = video;

            VideoInfo.Visibility = Visibility.Collapsed;
            Description.Visibility = Visibility.Collapsed;

            VideoTitle.Text = video.Title;

            DownloadVideoInfo(video);

            SetButtons(video.VideoId);
        }

        private void SetButtons(string id)
        {
            //btnLocate.IsEnabled = true;

            if (FavoritesCache.Default.Contains(id) == false)
                btnFavorites.IsEnabled = true;
            else
                btnFavorites.IsEnabled = false;

            if (WatchLaterCache.Default.Contains(id) == false)
                btnWatchLater.IsEnabled = true;
            else
                btnWatchLater.IsEnabled = false;

            btnPatreon.Visibility = Visibility.Collapsed;
        }

        private async void DownloadVideoInfo(Video video)
        {
            try
            {
                YoutubeClient client = new YoutubeClient();
                YoutubeExplode.Models.Video v = await client.GetVideoAsync(video.VideoId);

                MapObj mapObj = ChannelMapper.Get(video.ChannelId);

                if (mapObj != null)
                {
                    Author.Text = mapObj.Title;
                    AuthorLogo.Source = mapObj.Thumbnail.ToImage();
                }
                else
                {
                    Author.Text = "";
                    AuthorLogo.Source = null;
                }

                Views.Text = v.Statistics.ViewCount.ToString("N0");
                Description.Text = v.Description;

                if (v.Duration.Hours > 0)
                {
                    Duration.Text = v.Duration.ToString(@"h\:mm\:ss");
                }
                else
                {
                    Duration.Text = v.Duration.ToString(@"mm\:ss");
                }

                Published.Text = v.UploadDate.ToString("d");

                Description.Visibility = Visibility.Visible;
                VideoInfo.Visibility = Visibility.Visible;
                
                if (Settings.Default.ShowPatreonLink && v.Description.NotNull())
                {
                    _patreonUrl = FindPatreonUrl(v.Description);

                    if (_patreonUrl.NotNull())
                    {
                        btnPatreon.Visibility = Visibility.Visible;
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
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion

                VideoInfo.Visibility = Visibility.Collapsed;
                Description.Visibility = Visibility.Collapsed;
            }
        }

        //private void GetComments()
        //{
        //    var comments = CommentService.GetComments(_video.VideoId);

        //    CommentsList.Items.Clear();

        //    foreach (string comment in comments)
        //    {
        //        CommentsList.Items.Add(comment);
        //    }
        //}

        private string FindPatreonUrl(string text)
        {
            try
            {
                // https://stackoverflow.com/a/10576770
                Regex regex = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (Match item in regex.Matches(text))
                {
                    if (item.Value.Contains("patreon"))
                    {
                        return item.Value;
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

            return null;
        }

        //private void Locate_Changed(object sender, EventArgs e)
        //{
        //    if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.UnwatchedVideos)
        //    {
        //        if (VisibleNodes.Default.Contains(_video?.VideoId))
        //        {
        //            btnLocate.IsEnabled = true;
        //        }
        //        else
        //        {
        //            btnLocate.IsEnabled = false;
        //        }
        //    }
        //    else
        //    {
        //        btnLocate.IsEnabled = true;
        //    }

        //}

        private void Favorites_AllRemoved(string id)
        {
            btnFavorites.IsEnabled = true;
        }

        private void Favorites_Added(string id)
        {
            if (_video != null && id == _video.VideoId)
                btnFavorites.IsEnabled = false;
        }

        private void Favorites_Removed(string id)
        {
            if (_video != null && id == _video.VideoId)
                btnFavorites.IsEnabled = true;
        }

        private void WatchLater_Added(string id)
        {
            if (_video != null && id == _video.VideoId)
                btnWatchLater.IsEnabled = false;
        }

        private void WatchLater_Removed(string id)
        {
            if (_video != null && id == _video.VideoId)
                btnWatchLater.IsEnabled = true;
        }

        private void WatchLater_AllRemoved(string id)
        {
            btnWatchLater.IsEnabled = true;
        }

        private void AuthorLogo_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string url = UrlHelper.GetChannelUrl(_video.ChannelId);
            Process.Start(url);
        }

        private void VideoTitle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewRepository.Get<PlayerTabView>().OpenDefault();
        }

        private void Author_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_video != null)
            {
                if (ModifierKeyHelper.IsShiftDown)
                {
                    ViewRepository.Get<ChannelView>().Locate(_video.ChannelId, _video.VideoId);                    
                }
                else
                {
                    string url = UrlHelper.GetChannelUrl(_video.ChannelId);
                    Process.Start(url);
                }
            }
        }

        private void Favorites_OnClick(object sender, RoutedEventArgs e)
        {
            FavoritesCache.Default.Add(_video.VideoId);
            Repository.Default.Playlists.Insert(_video, "Favorites");
            btnFavorites.IsEnabled = false;
        }

        private void WatchLater_OnClick(object sender, RoutedEventArgs e)
        {
            WatchLaterCache.Default.Add(_video.VideoId);
            Repository.Default.Playlists.Insert(_video, "WatchLater");
            btnWatchLater.IsEnabled = false;
        }

        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            IAction action = ActionRepository.Find("DownloadVideo");
            action.Execute(_video.VideoId);
        }

        private void AddToPlaylist_OnClick(object sender, RoutedEventArgs e)
        {
            Dialogs.OpenPlaylistWindow(_video);
        }

        private void Locate_OnClick(object sender, RoutedEventArgs e)
        {
            if (_video != null)
            {
                ViewRepository.Get<ChannelView>().Locate(_video.ChannelId, _video.VideoId);                
            }
        }

        private void Marker_OnClick(object sender, RoutedEventArgs e)
        {
            ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.CreateMarker();
        }

        private void Comments_OnClick(object sender, RoutedEventArgs e)
        {
            CommentWindow window = new CommentWindow(_video);
            window.Show();
        }

        public void Logout()
        {
            VideoTitle.Text = string.Empty;
            VideoInfo.Visibility = Visibility.Collapsed;
            Description.Visibility = Visibility.Collapsed;
        }

        private void GetSource_OnClick(object sender, RoutedEventArgs e)
        {
            ViewRepository.Get<PlayerTabView>().GetSource();
        }

        private void Patreon_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(_patreonUrl);
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

        public void SearchGoogle()
        {
            Process.Start("http://www.google.com/search?q=" + Uri.EscapeDataString(Description.SelectedText));
        }

        public void CopyText()
        {
            Clipboard.SetText(Description.SelectedText);
        }

    }
}
