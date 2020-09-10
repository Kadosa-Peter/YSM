using System;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using YoutubeExplode.Models;
using Ysm.Downloader.Windows;

namespace Ysm.Downloader.Assets
{
    public static class DialogHelper
    {
        public static void ShowInfoWindow(string info)
        {
            InfoWindow window = new InfoWindow(info);
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public static int ShowPlaylist()
        {
            PlaylistWindow window = new PlaylistWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
            return window.result;
        }

        public static string GetOutputFolder(Window owner)
        {
            Window window = OpenDummyWindow();
            window.Owner = owner;

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = Properties.Resources.Title_OutputFolder,
                EnsureReadOnly = true,
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                IsFolderPicker = true,
                AllowNonFileSystemItems = false
            };

            if (dialog.ShowDialog(window) == CommonFileDialogResult.Ok)
            {
                window.Close();
                return dialog.FileName;
            }

            window.Close();
            return null;
        }

        public static DownloadModeWindowResult OpenDownloadModeWindow(Playlist playlist)
        {
            DownloadModeWindow window = new DownloadModeWindow(playlist)
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();

            DownloadModeWindowResult result = new DownloadModeWindowResult
            {
                IsCancelled = window.IsCancelled,
                PreferredQuality = window.PreferredQuality,
                PlaylistDownloadMode = window.PlaylistDownloadMode
            };

            return result;
        }

        public static DownloadModeWindowResult OpenDownloadModeWindow(int count)
        {
            DownloadModeWindow window = new DownloadModeWindow(count)
            {
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();

            DownloadModeWindowResult result = new DownloadModeWindowResult
            {
                IsCancelled = window.IsCancelled,
                PreferredQuality = window.PreferredQuality,
                PlaylistDownloadMode = window.PlaylistDownloadMode
            };

            return result;
        }

        private static Window OpenDummyWindow()
        {
            double left = SystemParameters.PrimaryScreenWidth / 2 - 480;
            double top = SystemParameters.PrimaryScreenHeight / 2 - 300;

            Window window = new Window
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 960,
                Height = 600,
                Top = top,
                Left = left,
                AllowsTransparency = true,
                Opacity = 0,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.None
            };

            window.Show();

            return window;
        }
    }
}
