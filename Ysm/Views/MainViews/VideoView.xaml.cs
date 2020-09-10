using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Ysm.Assets;
using Ysm.Assets.Caches;
using Ysm.Assets.Menu;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;

namespace Ysm.Views
{
    public partial class VideoView
    {
        public PlayEngine PlayEngine { get; set; }

        public ObservableCollection<VideoItem> Videos { get; set; }

        public SearchEngine<Video> Search { get; set; }

        private ScrollViewer _scrollViewer;

        private Action _action;

        public VideoView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(VideoView));

            Videos = new VideoList();
            PlayEngine = new PlayEngine(Videos);

            SetContextMenu();

            Search = new SearchEngine<Video>(SearchFunc);
            Search.Search += Search_OnSearch;

            FavoritesCache.Default.Removed += Favorites_Removed;
            FavoritesCache.Default.Added += Favorites_Added;
            WatchLaterCache.Default.Removed += WatchLater_Removed;
            WatchLaterCache.Default.Added += WatchLater_Added;

            Kernel.Default.PropertyChanged += Kernel_PropertyChanged;
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void VideoView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = VideoList.GetChildOfType<ScrollViewer>();
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VideoDisplayMode")
            {
                VideoStateChanged();
            }
        }

        private void Kernel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "View")
            {
                Search.Clear();

                SetContextMenu();
            }
        }


        // Favorites & WatchLater
        private void Favorites_Removed(string id)
        {
            foreach (VideoItem item in Videos)
            {
                if (item.Id == id)
                {
                    item.FIconVisibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void Favorites_Added(string id)
        {
            foreach (VideoItem item in Videos)
            {
                if (item.Id == id)
                {
                    item.FIconVisibility = Visibility.Collapsed;
                    break;
                }
            }
        }

        private void WatchLater_Removed(string id)
        {
            foreach (VideoItem item in Videos)
            {
                if (item.Id == id)
                {
                    item.WIconVisibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void WatchLater_Added(string id)
        {
            foreach (VideoItem item in Videos)
            {
                if (item.Id == id)
                {
                    item.WIconVisibility = Visibility.Collapsed;
                    break;
                }
            }
        }

        public void AddToFavorites()
        {
            if (VideoList.SelectedItems.Count > 0)
            {
                VideoItem item = VideoList.SelectedItems[0] as VideoItem;
                if (item == null) return;

                Video video = item.Video;

                if (video != null && FavoritesCache.Default.Contains(video.VideoId) == false)
                {
                    Repository.Default.Playlists.Insert(video, "Favorites");
                    FavoritesCache.Default.Add(video.VideoId);
                }
            }
        }

        public void AddToWatchLater()
        {
            if (VideoList.SelectedItems.Count > 0)
            {
                VideoItem item = VideoList.SelectedItems[0] as VideoItem;
                if (item == null) return;

                Video video = item.Video;

                if (video != null && FavoritesCache.Default.Contains(video.VideoId) == false)
                {
                    Repository.Default.Playlists.Insert(video, "WatchLater");
                    FavoritesCache.Default.Add(video.VideoId);
                }
            }
        }

        // *************************

        private void ContextMenu_OnMenuClosed(object sender, EventArgs e)
        {
            foreach (VideoItem item in Videos)
            {
                item.IsFaded = false;
            }
        }

        private void ContextMenu_OnMenuOpened(object sender, EventArgs e)
        {
            if (Kernel.Default.SelectedVideoItem != null)
            {
                foreach (VideoItem item in Videos)
                {
                    item.IsFaded = !item.Equals(Kernel.Default.SelectedVideoItem);
                }
            }


        }

        private IEnumerable<Video> SearchFunc(string query)
        {
            //  0 => show only unwatched videos, 1 => show all videos
            return Repository.Default.Videos.Search(query, Kernel.Default.SelectedChannels, (int)Settings.Default.VideoDisplayMode);
        }

        private void Search_OnSearch(object sender, SearchEventArgs<Video> e)
        {
            if (e.Reset)
            {
                if (Search.Catch != null)
                {
                    Videos.Clear();

                    List<VideoItem> items = (List<VideoItem>)Search.Catch;

                    foreach (VideoItem item in items)
                    {
                        Videos.Add(item);
                    }

                    Search.Catch = null;
                }
            }
            else
            {
                if (Search.Catch == null)
                {
                    Search.Catch = Videos.ToList();
                }

                Videos.Clear();

                foreach (Video video in e.Result)
                {
                    VideoItem item = CreateVideoItem(video);
                    Videos.Add(item);
                }
            }
        }

        private void VideoList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (Videos.Count == 0 || (e.OriginalSource as DependencyObject).GetParentOfType<ListViewItem>() == null)
            {
                e.Handled = true;
            }
        }

        private void VideoList_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (e.OriginalSource as DependencyObject).GetParentOfType<ListViewItem>();

            if (item != null)
            {
                Kernel.Default.SelectedVideoItem = item.Content as VideoItem;
            }
        }

        private void VideoList_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (SearchBox.IsSearch)
                {
                    SearchBox.Clear();
                }

                e.Handled = true;
            }
        }

        private void SetContextMenu()
        {
            if (AuthenticationService.Default.IsLoggedIn)
            {
                if (Kernel.Default.View == View.Subscriptions)
                {
                    VideoMenu videoMenu = new VideoMenu();
                    videoMenu.OnMenuOpened += ContextMenu_OnMenuOpened;
                    videoMenu.OnMenuClosed += ContextMenu_OnMenuClosed;
                    VideoList.ContextMenu = videoMenu.Get();
                }
                else if (Kernel.Default.View == View.Favorites)
                {
                    FavoritesMenu favoritesMenu = new FavoritesMenu();
                    favoritesMenu.OnMenuOpened += ContextMenu_OnMenuOpened;
                    favoritesMenu.OnMenuClosed += ContextMenu_OnMenuClosed;
                    VideoList.ContextMenu = favoritesMenu.Get();
                }
                else if (Kernel.Default.View == View.WatchLater)
                {
                    WatchLaterMenu favoritesMenu = new WatchLaterMenu();
                    favoritesMenu.OnMenuOpened += ContextMenu_OnMenuOpened;
                    favoritesMenu.OnMenuClosed += ContextMenu_OnMenuClosed;
                    VideoList.ContextMenu = favoritesMenu.Get();
                }
                else if (Kernel.Default.View == View.Playlists)
                {
                    PlaylistVideoMenu playlistVideoMenu = new PlaylistVideoMenu();
                    playlistVideoMenu.OnMenuOpened += ContextMenu_OnMenuOpened;
                    playlistVideoMenu.OnMenuClosed += ContextMenu_OnMenuClosed;
                    VideoList.ContextMenu = playlistVideoMenu.Get();
                }
            }
        }

        private void VideoStateChanged()
        {
            Search.Clear();
            Videos.Clear();

            List<string> channels = Kernel.Default.SelectedChannels;

            if (channels != null && channels.Count > 0)
            {
                ListVideos(channels);
            }
        }

        public void RestoreVideos(List<string> channels)
        {
            if (Kernel.Default.SelectedChannels.NotEmpty())
            {
                _action = () =>
                {
                    _scrollViewer.ScrollToVerticalOffset(Kernel.Default.VerticalOffset);
                };

                PlayEngine.Reset();
                Search.Clear();
                Videos.Clear();

                ListVideos(channels);
            }
        }

        public void SaveScrollPosition()
        {
            if (_scrollViewer != null)
                Kernel.Default.VerticalOffset = _scrollViewer.VerticalOffset;
        }

        public void SelectedChannelChanged(List<string> channels)
        {
            PlayEngine.Reset();
            Search.Clear();
            Videos.Clear();

            if (channels != null && channels.Count > 0)
            {
                ListVideos(channels);
            }
        }

        public void ListVideos(List<string> channels)
        {
            Task<IEnumerable<Video>> task = Task<IEnumerable<Video>>.Factory.StartNew(() =>
            {
                List<Video> videos;

                if (Settings.Default.VideoDisplayMode == VideoDisplayMode.UnwatchedVideos)
                {
                    videos = Repository.Default.Videos.Get(channels).Where(x => x.State == 0).ToList();
                }
                else
                {
                    videos = Repository.Default.Videos.Get(channels);
                }

                if (videos.Count > 5000)
                {
                    return videos.OrderByDescending(x => x.Published).Take(5000);
                }
                else
                {
                    return videos.OrderByDescending(x => x.Published);
                }

            });

            task.ContinueWith(t =>
            {
                _scrollViewer.ScrollToTop();

                IEnumerable<Video> videos = t.Result;

                foreach (Video video in videos)
                {
                    VideoItem item = CreateVideoItem(video);
                    Videos.Add(item);
                }

                _action?.Invoke();
                _action = null;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void ListVideos(IEnumerable<Video> videos)
        {
            //PlayEngine.Reset();

            Videos.Clear();

            foreach (Video video in videos)
            {
                VideoItem item = CreateVideoItem(video);
                Videos.Add(item);
            }

            _scrollViewer.ScrollToTop();
        }

        private VideoItem CreateVideoItem(Video video)
        {
            string channel = ChannelMapper.Get(video.ChannelId).Title;

            VideoItem item = new VideoItem
            {
                Video = video,
                Channel = channel,
                WIconVisibility = WatchLaterCache.Default.Contains(video.VideoId).ToReveseVisibility(),
                FIconVisibility = FavoritesCache.Default.Contains(video.VideoId).ToReveseVisibility()
            };

            return item;
        }

        public void AddVideo(Video video)
        {
            VideoItem item = CreateVideoItem(video);

            Videos.Add(item);
        }

        public void AddVideos(List<Video> videos)
        {
            if (Kernel.Default.SelectedChannels != null)
            {
                Task.Factory
                    .StartNew(() =>
                    {
                        List<VideoItem> items = new List<VideoItem>(Videos);

                        foreach (Video video in videos)
                        {
                            if (Kernel.Default.SelectedChannels.Contains(video.ChannelId))
                            {
                                VideoItem item = CreateVideoItem(video);
                                items.Add(item);
                            }
                        }

                        return items.OrderByDescending(x => x.Video.Published);

                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current)
                    .ContinueWith(t =>
                    {
                        Videos.Clear();
                        Videos.AddRange(t.Result);

                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void Remove(List<string> ids)
        {
            // Playlist

            foreach (string id in ids)
            {
                VideoItem item = Videos.FirstOrDefault(x => x.Id == id);

                if (item != null)
                    Videos.Remove(item);
            }
        }

        public void Remove(string id)
        {
            // Favorites & WatchLater View

            foreach (VideoItem item in Videos)
            {
                if (item.Id == id)
                {
                    Videos.Remove(item);
                    break;
                }
            }
        }

        public void RemoveByChannelId(string id)
        {
            // Favorites & WatchLater View

            IReadOnlyList<VideoItem> videos = Videos.Where(x => x.Video.ChannelId == id).ToList();

            foreach (VideoItem item in videos)
            {
                Videos.Remove(item);
            }
        }

        public void RemoveAll()
        {
            Videos.Clear();
        }

        public void MarkWatched()
        {
            // show all
            if (Settings.Default.VideoDisplayMode == VideoDisplayMode.AllVideos)
            {
                foreach (VideoItem item in Videos)
                {
                    item.Video.State = 1;
                }
            }
            else
            {
                Videos.Clear();
            }
        }

        public void MarkUnwatched()
        {
            foreach (VideoItem item in Videos)
            {
                item.Video.State = 0;
            }
        }

        public void VideoEnded(string id)
        {
            if (PlayEngine.AutoPlay)
            {
                VideoItem item;

                if (PlayEngine.Shuffle)
                {
                    item = PlayEngine.RandomNext();
                }
                else
                {
                    item = PlayEngine.Next();
                }

                if (item != null)
                {
                    OpenVideo(item);
                }
            }

        }

        public void VideoStarted(string id)
        {
            VideoItem item = Videos.FirstOrDefault(x => x.Id == id);

            if (item != null)
            {
                PlayEngine.Set(item.Id);
            }
        }

        public void NextVideo(string id)
        {
            VideoItem item;

            if (PlayEngine.Shuffle)
            {
                item = PlayEngine.RandomNext();
            }
            else
            {
                item = PlayEngine.Next();
            }

            if (item != null)
            {
                OpenVideo(item);
            }
        }

        public void PreviousVideo(string id)
        {
            VideoItem item = PlayEngine.Previous();
            if (item != null)
            {
                OpenVideo(item);
            }
        }

        public void OpenVideo(VideoItem item)
        {
            VideoList.ScrollIntoView(item);

            ViewRepository.Get<PlayerTabView>()?.Open(item.Video);

            Repository.Default.History.Insert(item.Id);

            if (item.Video.State == 0)
            {
                item.Video.State = 1;

                Repository.Default.Videos.MarkWatched(item.Id, item.Video.ChannelId);

                ViewRepository.Get<FooterView>().DecreaseVideoCount(1);
                ViewRepository.Get<ChannelView>().DecreaseVideoCount(item.Video.ChannelId);
            }
        }

        public void Locate(string videoId)
        {
            if (Videos.Any(x => x.Id == videoId) == false)
            {
                if (Dialogs.OpenDialog(Messages.DoYouWantChangeVideoDisplayMode()))
                {
                    Settings.Default.VideoDisplayMode = VideoDisplayMode.AllVideos;
                    VideoStateChanged();

                    _action = () =>
                    {
                        if (videoId.NotNull())
                        {
                            VideoItem item = Videos.FirstOrDefault(x => x.Id == videoId);

                            if (item != null)
                            {
                                VideoList.ScrollIntoView(item);
                                //StartLocateAnimation(item);
                            }
                        }
                    };
                }
            }
            else
            {
                VideoItem item = Videos.FirstOrDefault(x => x.Id == videoId);

                if (item != null)
                {
                    VideoList.ScrollIntoView(item);
                    //StartLocateAnimation(item);

                }
            }
        }

        //private void StartLocateAnimation(VideoItem item)
        //{
        //    foreach (VideoItem video in Videos)
        //    {
        //        if (video.Id != item.Id)
        //        {
        //            video.IsLocated = true;
        //            video.IsLocated = false;
        //        }
        //    }
        //}

        public void Reset()
        {
            Videos.Clear();
            Kernel.Default.SelectedVideoItem = null;
        }

        public void Logout()
        {
            Videos.Clear();
            SearchBox.IsEnabled = false;
            VideoList.ContextMenu = null;
        }

        public void Login()
        {
            Videos.Clear();
            SearchBox.IsEnabled = true;
            SetContextMenu();
        }
    }
}
