using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ysm.Assets.Transfer
{
    public static class Dialogs
    {
        public static string ExportData()
        {
            Window window = OpenDummyWindow();

            using (CommonSaveFileDialog dialog = new CommonSaveFileDialog())
            {
                dialog.DefaultFileName = "YSM_Backup";
                dialog.Title = "Export YSM Data";
                dialog.DefaultDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
                dialog.DefaultExtension = "zip";
                dialog.OverwritePrompt = false;
                dialog.Filters.Add(new CommonFileDialogFilter("Zip", ".zip"));

                if (dialog.ShowDialog(window) == CommonFileDialogResult.Ok)
                {
                    window.Close();
                    return dialog.FileName;
                }
            }

            window.Close();
            return null;
        }

        public static string ImportData()
        {
            Window window = OpenDummyWindow();

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = "Import YSM Data",
                EnsureReadOnly = true,
                DefaultDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
                IsFolderPicker = false,
                AllowNonFileSystemItems = false,
                Filters = { new CommonFileDialogFilter("Zip", ".zip") }
            };

            if (dialog.ShowDialog(window) == CommonFileDialogResult.Ok)
            {
                window.Close();
                return dialog.FileName;
            }

            window.Close();
            return null;
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
