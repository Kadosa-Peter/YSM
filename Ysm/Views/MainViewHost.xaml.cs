using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Core;

namespace Ysm.Views
{
    public partial class MainViewHost
    {
        private ChannelView _channelView;
        private FavoritesView _favoritesView;
        private WatchLaterView _watchLaterView;
        private HistoryView _historyView;
        private PlaylistView _playlistView;
        private MarkerView _markerView;
        private View _currentView;

        public MainViewHost()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(MainViewHost));

            Kernel.Default.PropertyChanged += Kernel_PropertyChanged;

            SetView();
        }

        private void Kernel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "View")
            {
                SetView();
            }
        }

        private void SetView()
        {
            if (Kernel.Default.View == View.Subscriptions)
            {
                if (_channelView == null)
                    _channelView = new ChannelView();

                RemoveCurrentView();

                Grid.SetColumn(_channelView, 1);
                MainLayout.Children.Add(_channelView);

                _currentView = View.Subscriptions;

                if (Kernel.Default.SelectedChannels != null)
                {
                    ViewRepository.Get<VideoView>().RestoreVideos(Kernel.Default.SelectedChannels);
                }
            }
            else if (Kernel.Default.View == View.Favorites)
            {
                RemoveCurrentView();

                if (_favoritesView == null)
                    _favoritesView = new FavoritesView();

                Grid.SetColumn(_favoritesView, 1);

                MainLayout.Children.Add(_favoritesView);

                _currentView = View.Favorites;
            }
            else if (Kernel.Default.View == View.WatchLater)
            {
                RemoveCurrentView();

                if (_watchLaterView == null)
                    _watchLaterView = new WatchLaterView();

                Grid.SetColumn(_watchLaterView, 1);

                MainLayout.Children.Add(_watchLaterView);

                _currentView = View.WatchLater;
            }
            else if (Kernel.Default.View == View.Playlists)
            {
                RemoveCurrentView();

                if (_playlistView == null)
                    _playlistView = new PlaylistView();

                Grid.SetColumn(_playlistView, 1);

                MainLayout.Children.Add(_playlistView);

                _currentView = View.Playlists;
            }
            else if (Kernel.Default.View == View.History)
            {
                RemoveCurrentView();

                if (_historyView == null)
                    _historyView = new HistoryView();

                Grid.SetColumn(_historyView, 1);

                MainLayout.Children.Add(_historyView);

                Splitter2.Visibility = Visibility.Collapsed;
                Splitter1.IsEnabled = false;
                MainLayout.ColumnDefinitions[3].Width = new GridLength(0);

                _currentView = View.History;
            }
            else if (Kernel.Default.View == View.Markers)
            {
                RemoveCurrentView();

                if (_markerView == null)
                    _markerView = new MarkerView();

                Grid.SetColumn(_markerView, 1);

                MainLayout.Children.Add(_markerView);
                MainLayout.ColumnDefinitions[1].Width = new GridLength(332);

                Splitter2.Visibility = Visibility.Collapsed;
                Splitter1.IsEnabled = false;
                MainLayout.ColumnDefinitions[3].Width = new GridLength(0);

                _currentView = View.Markers;
            }
        }

        private void RemoveCurrentView()
        {
            if (_currentView == View.Subscriptions)
            {
                ViewRepository.Get<VideoView>().SaveScrollPosition();
                MainLayout.Children.Remove(_channelView);
            }
            else if (_currentView == View.Favorites)
            {
                _favoritesView.Cleanup();
                MainLayout.Children.Remove(_favoritesView);
            }
            else if (_currentView == View.WatchLater)
            {
                _watchLaterView.Cleanup();
                MainLayout.Children.Remove(_watchLaterView);
            }
            else if (_currentView == View.Playlists)
            {
                _playlistView.Cleanup();
                MainLayout.Children.Remove(_playlistView);
            }
            else if (_currentView == View.History)
            {
                Splitter2.Visibility = Visibility.Visible;
                Splitter1.IsEnabled = true;
                MainLayout.ColumnDefinitions[3].Width = new GridLength(270);
                MainLayout.Children.Remove(_historyView);
            }
            else if (_currentView == View.Markers)
            {
                Splitter2.Visibility = Visibility.Visible;
                Splitter1.IsEnabled = true;
                MainLayout.ColumnDefinitions[3].Width = new GridLength(270);
                MainLayout.ColumnDefinitions[1].Width = new GridLength(275);
                MainLayout.Children.Remove(_markerView);
            }
        }

        public void ShowNotifyLayer(string text)
        {
            NotifyText.Text = text;
            NotifyLayer.Visibility = Visibility.Visible;

            Task.Delay(4000).GetAwaiter().OnCompleted(()=>NotifyLayer.Visibility = Visibility.Collapsed);
        }

        private void Splitter1_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainLayout.ColumnDefinitions[1].Width = new GridLength(275);
        }
    }
}
