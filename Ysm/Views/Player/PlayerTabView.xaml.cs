using System.Windows;
using System.Windows.Controls;
using Ysm.Assets;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Views
{
    public partial class PlayerTabView
    {
        public PlayerView SelectedPlayerView { get; set; }

        public bool IsMuted { get; set; }

        public PlayerTabView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(PlayerTabView));
        }

        public void Open(Video video)
        {
            if (TabView.Count == 0)
            {
                OpenTab(video);
            }
            else
            {
                if (TabView.SelectedItem is ExtendedTabItem tab && tab.Content is PlayerView browser)
                {
                    tab.Header = video.Title;

                    Kernel.Default.PlayerVideo = video;

                    browser.Open(video);
                }
            }
        }

        public void OpenTab(Video video)
        {
            ExtendedTabItem tab = new ExtendedTabItem();
            tab.Header = video.Title;
            tab.Close += Tab_Close;
            tab.Tag = video;
            tab.IsSelected = TabView.Count == 0;
            TabView.Add(tab);


            UpdateTabLayout();
        }

        private void UpdateTabLayout()
        {
            if (TabView.Count < 2)
            {
                TabView.Margin = new Thickness(0, -26, 0, 0);
            }
            else
            {
                TabView.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void Tab_Close(object sender, RoutedEventArgs e)
        {
            if (sender is ExtendedTabItem tab)
            {
                if (tab.Content is PlayerView browser)
                {
                    browser.Close();

                    TabView.Remove(tab);

                    UpdateTabLayout();
                }
                // ha a tab még nem volt aktíválva, akkor a content null
                else 
                {
                    TabView.Remove(tab);

                    UpdateTabLayout();
                }

                ClosedTabs.Default.Insert(tab.Tag as Video);
            }
        }

        private void TabView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ExtendedTabItem tab in TabView.Items)
            {
                if (tab.IsSelected)
                {
                    if (tab.Content is PlayerView browser)
                    {
                        browser.Show();
                    }
                    else
                    {
                        Video video = tab.Tag as Video;
                        Kernel.Default.PlayerVideo = video;
                        PlayerView playerView = new PlayerView(video, IsMuted);
                        tab.Content = playerView;
                    }
                }
                else
                {
                    if (tab.Content is PlayerView browser)
                    {
                        browser.Hide();
                    }
                }
            }

            if (TabView.SelectedItem != null)
            {
                if (TabView.SelectedItem is ExtendedTabItem tab)
                {
                    SelectedPlayerView = tab.Content as PlayerView;

                    if (SelectedPlayerView != null)
                    {
                        Kernel.Default.PlayerVideo = SelectedPlayerView.Video;
                    }
                }
            }
            else
            {
                Kernel.Default.PlayerVideo = null;
                SelectedPlayerView = null;
            }
        }

        public void GoForward()
        {
            if (SelectedPlayerView != null)
            {
                Video video = SelectedPlayerView.History.GoForward();
                Kernel.Default.PlayerVideo = video;
                SelectedPlayerView.Open(video);
                CommandsHelper.Commands.PlayerNextCommand.RaiseCanExecuteChanged();
            }
        }

        public void GoBack()
        {
            if (SelectedPlayerView != null)
            {
                Video video = SelectedPlayerView.History.GoBack();
                Kernel.Default.PlayerVideo = video;
                SelectedPlayerView.Open(video);
                CommandsHelper.Commands.PlayerPreviousCommand.RaiseCanExecuteChanged();
            }
        }

        public void Refresh()
        {
            SelectedPlayerView?.Refresh();
        }

        public void Logout()
        {
            foreach (ExtendedTabItem tab in TabView.Items)
            {
                if (tab.Content is PlayerView browser)
                {
                    browser.Close();
                }
            }

            TabView.Clear();

            UpdateTabLayout();

            SelectedPlayerView = null;
        }

        public void GetSource()
        {
            SelectedPlayerView?.GetSource();
        }

        public void Close()
        {
            if (TabView.SelectedItem is ExtendedTabItem tab && tab.Content is PlayerView browser)
            {
                TabView.Remove(tab);

                browser.Close();

                UpdateTabLayout();
            }
        }

        public void CloseAll()
        {
            foreach (ExtendedTabItem tab in TabView.Items)
            {
                if (tab.Content is PlayerView browser)
                {
                    browser.Close();
                }
            }

            TabView.Clear();

            UpdateTabLayout();

            SelectedPlayerView = null;
        }

        public void OpenDefault()
        {
            SelectedPlayerView?.OpenInBrowser();
        }

        public void SetVolume(string volume, string videoId)
        {
            foreach (ExtendedTabItem tab in TabView.Items)
            {
                if (tab.Content is PlayerView browser)
                {
                    if (browser.Video.VideoId != videoId)
                    {
                        browser.SetVolume(volume);
                    }
                }
            }
        }

        public void Mute()
        {
            foreach (ExtendedTabItem tab in TabView.Items)
            {
                if (tab.Content is PlayerView browser)
                {
                    browser.ToggleMute();
                }
            }
        }

        
    }
}
