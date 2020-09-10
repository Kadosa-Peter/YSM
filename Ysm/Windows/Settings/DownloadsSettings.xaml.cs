using System.IO;
using System.Text;
using System.Windows;
using Microsoft.WindowsAPICodePack.Shell;
using Ysm.Assets;
using Ysm.Core;

namespace Ysm.Windows
{
    public partial class DownloadsSettings
    {
        public DownloadsSettings()
        {
            InitializeComponent();

            Loaded += DownloadsSettings_Loaded;
        }

        private void DownloadsSettings_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FileSystem.Downloads))
            {
                string dir;

                using (StreamReader reader = new StreamReader(FileSystem.Downloads, Encoding.UTF8))
                {
                    dir = reader.ReadToEnd();
                }

                if (Directory.Exists(dir))
                {
                    DownloadDir.Text = dir;
                }
            }
            else
            {
                DownloadDir.Text = KnownFolders.Downloads.Path;
            }
        }

        private void SelectDownloadFolder_OnClick(object sender, RoutedEventArgs e)
        {
            string downloadFolder = Dialogs.GetDownloadFolder(Window.GetWindow(this));

            if (Directory.Exists(downloadFolder))
            {
                using (StreamWriter writer = new StreamWriter(FileSystem.Downloads, false, Encoding.UTF8))
                {
                    writer.Write(downloadFolder);
                }

                DownloadDir.Text = downloadFolder;
            }
        }
    }
}
