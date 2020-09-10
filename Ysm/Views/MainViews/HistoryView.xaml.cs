using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Assets.Menu;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;

namespace Ysm.Views
{
    public partial class HistoryView
    {
        #region Properties & Variables

        public SearchQuery SearchQuery { get; set; }

        public ObservableCollection<VideoItem> Items { get; set; }

        private VideoItem _selectedItem;

        #endregion

        public HistoryView()
        {
            InitializeComponent();

            SearchQuery = new SearchQuery();
            SearchQuery.PropertyChanged += SearchQuery_OnPropertyChanged;

            ViewRepository.Add(this, nameof(HistoryView));

            Items = new ObservableCollection<VideoItem>();

            // default last 7 days
            Settings.Default.HistoryFilter = 3;

            VideoList.ContextMenu = new HistoryMenu().Get();

            Settings.Default.PropertyChanged += ApplicationSettings_OnPropertyChanged;
        }

        private void ApplicationSettings_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HistoryFilter")
            {
                LoadItems();
            }
        }

        private void SearchQuery_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!AuthenticationService.Default.IsLoggedIn) return;

            if (SearchQuery.Text.IsNull())
            {
                foreach (VideoItem item in Items)
                {
                    item.IsVisible = true;
                }
            }
            else
            {
                string text = SearchQuery.Text.ToLower();

                foreach (VideoItem item in Items)
                {
                    if (item.Video.Title.Contains(text, true))
                    {
                        item.IsVisible = true;
                    }
                    else
                    {
                        item.IsVisible = false;
                    }
                }
            }
        }

        private void VideoList_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (e.OriginalSource as DependencyObject).GetParentOfType<ListViewItem>();

            if (item != null)
            {
                _selectedItem = item.Content as VideoItem;
                Kernel.Default.SelectedVideoItem = _selectedItem;
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

        private void HistoryView_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadItems();
        }

        private void LoadItems()
        {
            if (!AuthenticationService.Default.IsLoggedIn) return;

            Task<IOrderedEnumerable<Video>> task = Task.Run(() =>
            {
                switch (Settings.Default.HistoryFilter)
                {
                    case 0:
                        {
                            DateTime date = DateTime.Now.Date;
                            return Repository.Default.History.Get().Where(x => x.Added.Date == date).OrderByDescending(x => x.Added);
                        }
                    case 1:
                        {
                            DateTime date = DateTime.Now.Date.AddDays(-1);
                            return Repository.Default.History.Get().Where(x => x.Added.Date == date).OrderByDescending(x => x.Added);
                        }
                    case 2:
                        {
                            DateTime date = DateTime.Now.Date.AddDays(-2);
                            return Repository.Default.History.Get().Where(x => x.Added.Date == date).OrderByDescending(x => x.Added);
                        }
                    case 3:
                        {
                            DateTime date = DateTime.Now.AddDays(-7).Date;
                            return Repository.Default.History.Get().Where(x => x.Added.Date >= date).OrderByDescending(x => x.Added);
                        }
                    case 4:
                        {
                            DateTime date = DateTime.Now.AddDays(-30).Date;
                            return Repository.Default.History.Get().Where(x => x.Added.Date >= date).OrderByDescending(x => x.Added);
                        }
                    default:
                        return null;
                }
            });

            task.ContinueWith(t =>
            {
                Items.Clear();

                if(t.Result != null)
                {
                    IEnumerable<Video> videos = t.Result;

                    foreach (Video favorite in videos)
                    {
                        VideoItem item = CreateVideoItem(favorite);
                        Items.Add(item);
                    }
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());


        }

        private VideoItem CreateVideoItem(Video video)
        {
            string channel = ChannelMapper.Get(video.ChannelId).Title;

            VideoItem item = new VideoItem
            {
                Video = video,
                Channel = channel
            };

            return item;

        }

        public void RemoveAll()
        {
            Items.Clear();

            Repository.Default.History.RemoveAll();
        }

        public void Remove()
        {
            if (_selectedItem != null)
            {
                Items.Remove(_selectedItem);

                Repository.Default.History.Remove(_selectedItem.Video.VideoId);
            }
        }

        public void Login()
        {
            VideoList.ContextMenu = new HistoryMenu().Get();
            SearchBox.IsEnabled = true;
        }

        public void Logout()
        {
            Items.Clear();
            SearchBox.Clear();
            SearchBox.IsEnabled = false;
            VideoList.ContextMenu = null;
        }
    }
}
