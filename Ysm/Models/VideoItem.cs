using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YoutubeExplode;
using Ysm.Actions;
using Ysm.Assets;
using Ysm.Assets.Caches;
using Ysm.Core;
using Ysm.Core.RelayCommand;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Models
{
    public class VideoItem : INotifyPropertyChanged, IComparable
    {
        public RelayCommand LoadedCommand { get; }
        public RelayCommand<MouseButtonEventArgs> ChannelClickCommand { get; }
        public RelayCommand<MouseButtonEventArgs> OpenTabCommand { get; }
        public RelayCommand<MouseButtonEventArgs> OpenVideoCommand { get; }
        public RelayCommand<MouseButtonEventArgs> LocateCommand { get; }
        public RelayCommand<MouseButtonEventArgs> WatchLaterCommand { get; }
        public RelayCommand<MouseButtonEventArgs> AddFavoritesCommand { get; }
        public RelayCommand<MouseButtonEventArgs> PlaylistCommand { get; }
        public RelayCommand<MouseButtonEventArgs> DownloadCommand { get; }
        public RelayCommand<MouseButtonEventArgs> NewtabCommand { get; }

        public bool IsVisible
        {
            get => _isVisible;

            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isVisible = true;

        //public bool IsLocated
        //{
        //    get { return _isLocated; }

        //    set
        //    {
        //        if (_isLocated != value)
        //        {
        //            _isLocated = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //private bool _isLocated;

        public string Duration
        {
            get => _duration;

            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _duration;

        public bool IsFaded
        {
            get => _isFaded;

            set
            {
                if (_isFaded != value)
                {
                    _isFaded = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isFaded;

        public Visibility FIconVisibility
        {
            get => _fIconVisibility;

            set
            {
                if (_fIconVisibility != value)
                {
                    _fIconVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _fIconVisibility = Visibility.Visible;

        public Visibility WIconVisibility
        {
            get => _wIconVisibility;

            set
            {
                if (_wIconVisibility != value)
                {
                    _wIconVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _wIconVisibility = Visibility.Visible;

        public Video Video { get; set; }

        public string Channel { get; set; }

        public string Id => Video.VideoId;

        private bool _loaded;

        // Ctro
        public VideoItem()
        {
            LoadedCommand = new RelayCommand(DoLoaded);
            ChannelClickCommand = new RelayCommand<MouseButtonEventArgs>(DoChannelClick);
            LocateCommand = new RelayCommand<MouseButtonEventArgs>(DoLocate);
            OpenVideoCommand = new RelayCommand<MouseButtonEventArgs>(DoOpenVideo);
            OpenTabCommand = new RelayCommand<MouseButtonEventArgs>(DoOpenTab);
            WatchLaterCommand = new RelayCommand<MouseButtonEventArgs>(DoWatchLater);
            AddFavoritesCommand = new RelayCommand<MouseButtonEventArgs>(DoAddFavorites);
            NewtabCommand = new RelayCommand<MouseButtonEventArgs>(DoNewtab);
            DownloadCommand = new RelayCommand<MouseButtonEventArgs>(DoDownload);
            PlaylistCommand = new RelayCommand<MouseButtonEventArgs>(DoPlaylist);
        }

        private async void DoLoaded()
        {
            if (!_loaded)
            {
                _loaded = true;

                try
                {
                    YoutubeClient client = new YoutubeClient();
                    TimeSpan duration = await client.GetDurationAsync(Id);

                    if (duration.TotalSeconds < 1)
                    {
                        Duration = "Live"; // todo: translate
                    }
                    else if (duration.Hours > 0)
                    {
                        Duration = duration.ToString(@"h\:mm\:ss");
                    }
                    else
                    {
                        Duration = duration.ToString(@"mm\:ss");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), e);
                }
            }
        }

        private void DoOpenVideo(MouseButtonEventArgs e)
        {
            if (e.OriginalSource.IsExactly<Image>()) return;

            if (ModifierKeyHelper.IsCtrlDown)
            {
                ViewRepository.Get<PlayerTabView>()?.OpenTab(Video);
            }
            else
            {
                if (ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.Video.VideoId == Video.VideoId)
                {
                    return;
                }

                ViewRepository.Get<PlayerTabView>()?.Open(Video);
            }

            if (Settings.Default.History && Kernel.Default.View != View.History)
            {
                Repository.Default.History.Insert(Video.VideoId);
            }

            if (Video.State == 0)
            {
                Video.State = 1;

                Repository.Default.Videos.MarkWatched(Video.VideoId, Video.ChannelId);

                ViewRepository.Get<FooterView>().DecreaseVideoCount(1);
                ViewRepository.Get<ChannelView>().DecreaseVideoCount(Video.ChannelId);
            }

            e.Handled = true;
        }

        private void DoOpenTab(MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ViewRepository.Get<PlayerTabView>()?.OpenTab(Video);

                Repository.Default.History.Insert(Video.VideoId);

                if (Video.State == 0)
                {
                    Video.State = 1;

                    Repository.Default.Videos.MarkWatched(Video.VideoId, Video.ChannelId);

                    ViewRepository.Get<FooterView>().DecreaseVideoCount(1);
                    ViewRepository.Get<ChannelView>().DecreaseVideoCount(Video.ChannelId);
                }
            }

            e.Handled = true;
        }

        private void DoChannelClick(MouseButtonEventArgs e)
        {

            if (ModifierKeyHelper.IsShiftDown)
            {
                ViewRepository.Get<ChannelView>().Locate(Video.ChannelId, Video.VideoId);
            }
            else
            {
                string url = UrlHelper.GetChannelUrl(Video.ChannelId);
                Process.Start(url);
            }

            e.Handled = true;
        }

        private void DoLocate(MouseButtonEventArgs e)
        {
            ViewRepository.Get<ChannelView>().Locate(Video.ChannelId, Video.VideoId);
            e.Handled = true;
        }

        private void DoNewtab(MouseButtonEventArgs e)
        {
            ViewRepository.Get<PlayerTabView>()?.OpenTab(Video);

            Repository.Default.History.Insert(Video.VideoId);

            if (Video.State == 0)
            {
                Video.State = 1;

                Repository.Default.Videos.MarkWatched(Video.VideoId, Video.ChannelId);

                ViewRepository.Get<FooterView>().DecreaseVideoCount(1);
                ViewRepository.Get<ChannelView>().DecreaseVideoCount(Video.ChannelId);
            }

            e.Handled = true;
        }

        private void DoDownload(MouseButtonEventArgs e)
        {
            IAction action = ActionRepository.Find("DownloadVideo");
            action?.Execute(Video.VideoId);

            e.Handled = true;
        }

        private void DoPlaylist(MouseButtonEventArgs e)
        {
            Dialogs.OpenPlaylistWindow(Video);
            e.Handled = true;
        }

        private void DoWatchLater(MouseButtonEventArgs e)
        {
            if (WatchLaterCache.Default.Contains(Video.VideoId) == false)
            {
                WatchLaterCache.Default.Add(Video.VideoId);
                WIconVisibility = Visibility.Collapsed;
                Repository.Default.Playlists.Insert(Video, "WatchLater");
            }

            e.Handled = true;
        }

        private void DoAddFavorites(MouseButtonEventArgs e)
        {
            if (FavoritesCache.Default.Contains(Video.VideoId) == false)
            {
                FavoritesCache.Default.Add(Video.VideoId);
                FIconVisibility = Visibility.Collapsed;
                Repository.Default.Playlists.Insert(Video, "Favorites");
            }

            e.Handled = true;
        }

        public int CompareTo(object obj)
        {
            VideoItem item = obj as VideoItem;

            if (item == null)
                return 0;

            // Descending
            if (DateTime.Compare(Video.Published, item.Video.Published) < 0)
            {
                return 1;
            }

            return -1;
        }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
