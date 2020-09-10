using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Shell;
using Ysm.Core;

namespace Ysm.Update
{
    public partial class ProgressWindow
    {
        private readonly bool _install;

        private readonly string _fileName;

        private string _path;

        private WebClient _client;

        public ProgressWindow(bool install, string fileName)
        {
            InitializeComponent();

            _install = install;
            _fileName = fileName;
        }

        private void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string url = $"http://yosuma.com/Resources/Installers/{_fileName}";

            _path = KnownFolders.Downloads.Path;
            _path = Path.Combine(_path, _fileName);
            _path = Files.VerifyFilePath(_path);

            _client = new WebClient();
            _client.DownloadProgressChanged += Client_DownloadProgressChanged;
            _client.DownloadFileCompleted += Client_DownloadFileCompleted;
            _client.DownloadFileAsync(new Uri(url), _path);
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            long percent = e.BytesReceived/(e.TotalBytesToReceive/100);

            DownloadProgress.Value = percent;

            DownloadPercent.Text = $"{percent}%";
        }
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _client.Dispose();

            if (_install)
            {
                Process.Start(_path);
            }
            else
            {
                string directory = Path.GetDirectoryName(_path);

                if (Directory.Exists(directory))
                {
                    Process.Start(directory);
                }
            }

            Task.Delay(TimeSpan.FromSeconds(3)).GetAwaiter().OnCompleted(Close);
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            _client.Dispose();

            Close();
        }
    }
}
