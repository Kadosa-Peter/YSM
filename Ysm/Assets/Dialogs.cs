using System.Windows;
using System.Windows.Documents;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ysm.Data;
using Ysm.Windows;

namespace Ysm.Assets
{
    public static class Dialogs
    {

        public static void OpenLoginWindow()
        {
            LoginWindow window = new LoginWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public static string GetDownloadFolder(Window owner)
        {
            Window window = OpenDummyWindow();
            window.Owner = owner;

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = Properties.Resources.Title_SettingsDownloads,
                EnsureReadOnly = true,
                DefaultDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
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

        public static bool OpenDialog(string message, TextAlignment textAlignment = TextAlignment.Center)
        {
            DialogWindow window = new DialogWindow(message, textAlignment);
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();

            return window.DialogResult;
        }

        public static bool OpenDialog(FlowDocument document)
        {
            DialogWindow window = new DialogWindow(document);
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();

            return window.DialogResult;
        }

        public static void OpenInfo(string message)
        {
            InfoWindow window = new InfoWindow(message);
            window.Owner = Application.Current.MainWindow;
            window.Show();
        }

        public static void OpenInfo(FlowDocument document)
        {
            InfoWindow window = new InfoWindow(document);
            window.Owner = Application.Current.MainWindow;
            window.Show();
        }

        public static void OpenUnwatchedWindow()
        {
            UnwatchedWindow window = new UnwatchedWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public static void OpenWatchedWindow()
        {
            WatchedWindow window = new WatchedWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public static void OpenPlaylistWindow(Video video)
        {
            PlaylistWindow playlistWindow = new PlaylistWindow(video);
            playlistWindow.Owner = Application.Current.MainWindow;
            playlistWindow.ShowDialog();
        }

        public static void OpenIterationsWindow()
        {
            IterationWindow iterationWindow = new IterationWindow();
            iterationWindow.Owner = Application.Current.MainWindow;
            iterationWindow.Show();
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
