using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MoreLinq;
using Ysm.Assets;
using Ysm.Controls;
using Ysm.Core;

namespace Ysm.Views
{
    public partial class SidebarView
    {
        public SidebarView()
        {
            InitializeComponent();

            Width = 0;

            ViewRepository.Add(this, nameof(SidebarView));

            Kernel.Default.PropertyChanged += Kernel_PropertyChanged;
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowSidebar")
            {
                Window window = Application.Current.MainWindow;
                if (window == null) return;

                if (Settings.Default.ShowSidebar)
                {
                    Width = 25;
                    if (window.WindowState == WindowState.Normal)
                    {
                        window.Width = window.ActualWidth + 25;
                        window.Left = window.Left - 12.5;
                    }
                }
                else
                {
                    Width = 0;
                    if (window.WindowState == WindowState.Normal)
                    {
                        window.Width = window.ActualWidth - 25;
                        window.Left = window.Left + 12.5;
                    }
                }
            }
        }

        private void Kernel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "View")
            {
                switch (Kernel.Default.View)
                {
                    case View.Subscriptions:
                        SubscriptionToggle.IsChecked = true;
                        break;
                    case View.Favorites:
                        FavoritesToggle.IsChecked = true;
                        break;
                    case View.WatchLater:
                        WatchLaterToggle.IsChecked = true;
                        break;
                    case View.History:
                        HistoryToggle.IsChecked = true;
                        break;
                    case View.Playlists:
                        PlaylistsToggle.IsChecked = true;
                        break;
                    case View.Markers:
                        NotesToggle.IsChecked = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ViewToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is SideRadioButton button)
            {
                switch (button.Name)
                {
                    case "SubscriptionToggle":
                    {
                        Kernel.Default.View = View.Subscriptions;
                    }
                        break;
                    case "FavoritesToggle":
                    {
                        Kernel.Default.View = View.Favorites;
                    }
                        break;
                    case "WatchLaterToggle":
                    {
                        Kernel.Default.View = View.WatchLater;
                    }
                        break;
                    case "PlaylistsToggle":
                    {
                        Kernel.Default.View = View.Playlists;
                    }
                        break;
                    case "NotesToggle":
                    {
                        Kernel.Default.View = View.Markers;
                    }
                        break;
                    case "HistoryToggle":
                    {
                        Kernel.Default.View = View.History;
                    }
                        break;
                }
            }
        }

        public void SetSidebar()
        {
            if (!Kernel.Default.LoggedIn)
            {
                Sidebar.Children.Cast<SideRadioButton>().ForEach(x => x.IsEnabled = false);
            }

            if (Settings.Default.ShowSidebar)
            {
                Window window = Application.Current.MainWindow;
                if(window == null) return;

                Width = 25;
                if (window.WindowState == WindowState.Normal)
                {
                    window.Width = window.ActualWidth + 25;
                    window.Left = window.Left - 12.5;
                }
            }
        }

        public void Logout()
        {
            Sidebar.Children.Cast<SideRadioButton>().ForEach(x => x.IsEnabled = false);
        }

        public void Login()
        {
            Sidebar.Children.Cast<SideRadioButton>().ForEach(x => x.IsEnabled = true);
        }
    }
}
