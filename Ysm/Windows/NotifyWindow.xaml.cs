using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;
using Ysm.Views;

namespace Ysm.Windows
{
    public partial class NotifyWindow
    {
        public ObservableCollection<NotifyItem> Items { get; set; }
        public ObservableCollection<object> SelectedItems { get; set; }

        private readonly List<Video> _videos;
        private readonly List<Channel> _channels;

        private Timer _timer;

        public SearchQuery SearchQuery { get; set; }

        public NotifyWindow(List<Video> videos)
        {
            InitializeComponent();

            Items = new ObservableCollection<NotifyItem>();
            SelectedItems = new ObservableCollection<object>();

            _videos = videos;
        }

        public NotifyWindow(List<Channel> channels)
        {
            InitializeComponent();

            Items = new ObservableCollection<NotifyItem>();
            SelectedItems = new ObservableCollection<object>();

            _channels = channels;
        }

        private void NotifyWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_videos != null)
            {
                if (_videos.Count == 1)
                {
                    WindowTitle.Text = Properties.Resources.Title_NewVideo;
                }
                else
                {
                    string str = Properties.Resources.Title_NewVideos;
                    str = str.Replace("{xy}", _videos.Count.ToString());

                    WindowTitle.Text = str;
                }
            }
            else
            {
                if (_channels.Count == 1)
                {
                    WindowTitle.Text = Properties.Resources.Title_NewSubscription;
                }
                else
                {
                    string str = Properties.Resources.Title_NewSubscriptions;
                    str = str.Replace("{xy}", _channels.Count.ToString());

                    WindowTitle.Text = str;
                }
            }

            SetLocation();

            if (_videos != null)
                SetVideoItems();
            else
                SetChannelItems();

            SetTimer();

            PlayNotifySound();
        }

        private void NotifyWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Save settings after change
            Settings.Default.NotifyWindowHeight = ActualHeight;
        }

        private void NotifyWindow_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            border.Visibility = Visibility.Collapsed;
        }

        private void PlayNotifySound()
        {
            if (Settings.Default.PlayNotifySound)
            {
                SoundPlayer simpleSound = new SoundPlayer(@"Resources\desktopalert.wav");
                simpleSound.Play();
            }
        }

        private void SetTimer()
        {
            _timer = new Timer(1000, 15);
            _timer.Done += (s, e) => Dispatcher.BeginInvoke(new Action(Close));
            _timer.Start();
        }

        private void SetVideoItems()
        {
            List<Channel> channels = Repository.Default.Channels.Get();

            ILookup<string, Video> lookup = _videos.ToLookup(x => x.ChannelId);

            List<NotifyItem> items = new List<NotifyItem>();

            foreach (IGrouping<string, Video> group in lookup)
            {
                Channel channel = channels.FirstOrDefault(x => x.Id == group.Key);

                if (channel != null)
                {
                    NotifyItem item = new NotifyItem
                    {
                        Id = channel.Id,
                        Title = channel.Title,
                        ShowCount = true,
                        Count = group.Count(),
                        Icon = UrlHelper.GetIcon(channel.Id, 60, 60)
                    };

                    items.Add(item);
                }
            }

            items = items.OrderBy(x => x.Title).ToList();
            Items.AddRange(items);
        }

        private void SetChannelItems()
        {
            foreach (Channel channel in _channels)
            {
                NotifyItem item = new NotifyItem
                {
                    Id = channel.Id,
                    Title = channel.Title,
                    Icon = UrlHelper.GetIcon(channel.Id, 60, 60)
                };

                Items.Add(item);
            }
        }

        private void SetLocation()
        {
            double w = SystemParameters.WorkArea.Width;
            double h = SystemParameters.WorkArea.Height;

            if (_channels != null)
            {
                Left = w - (300 + 15);
                Top = h - (Settings.Default.NotifyWindowHeight + 38);
            }
            else
            {
                Left = w - (300 + 15);
                Top = h - (Settings.Default.NotifyWindowHeight + 15);
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();

            Settings.Default.Save();
        }

        private void Header_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ItemList_OnSelectedItemsChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                if (Kernel.Default.View != View.Subscriptions)
                {
                    Kernel.Default.View = View.Subscriptions;
                }

                if (SelectedItems.FirstOrDefault() is NotifyItem item)
                {
                    Window window = Application.Current.MainWindow;

                    if (window.Visibility == Visibility.Hidden)
                    {
                        window.Show();
                    }

                    if (window.WindowState == WindowState.Minimized)
                    {
                        window.WindowState = WindowState.Normal;
                    }

                    window.Activate();

                    Activate();

                    ViewRepository.Get<ChannelView>().Locate(item.Id, null);
                }
            }
        }
    }
}
