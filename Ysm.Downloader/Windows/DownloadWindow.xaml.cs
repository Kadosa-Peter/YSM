using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Downloader.Assets;
using Ysm.Downloader.Download;

namespace Ysm.Downloader.Windows
{
    public partial class DownloadWindow
    {
        public event EventHandler<DownloadWindowEventArgs> Download;

        private Video _video;
        private MuxedStreamInfo _muxedStream;
        private VideoStreamInfo _videoStream;
        private AudioStreamInfo _audioStream;
        private MediaStreamInfoSet _streamSet;

        private bool _windowLoaded;

        public bool CancelAll { get; set; }

        public bool OnlyAudio { get; set; }

        public DownloadWindow(string id, bool onlyAudio)
        {
            InitializeComponent();

            OnlyAudioButton.IsChecked = onlyAudio;

            Output.Text = DownloadPath.Get();

            HideWindow();

            LoadVideo(id);
        }

        public DownloadWindow(string playlistTitle, string id, int index, int count, bool onlyAudio)
        {
            InitializeComponent();

            CancelAllButton.Visibility = Visibility.Visible;
            CancelButton.Content = Properties.Resources.Button_Skip;
            OnlyAudioButton.IsChecked = onlyAudio;

            if (playlistTitle.IsNull())
            {
                VideoIndex.Visibility = Visibility.Visible;
                VideoIndex.Text = $"({index}/{count})";
            }
            else
            {
                PlaylistTitle.Visibility = Visibility.Visible;
                PlaylistTitle.Text = $"{playlistTitle}: {index}/{count}";
            }

            Output.Text = DownloadPath.Get();

            HideWindow();

            LoadVideo(id);

        }

        private void DownloadWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_windowLoaded) Download_OnClick(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                Cancel_OnClick(null, null);
            }
        }

        private async void LoadVideo(string id)
        {
            YoutubeClient client = new YoutubeClient();

            _video = await TryGetVideo(client, id);
            if (_video == null)
            {
                // error occured: close the window and return from the method
                Close();
                return;
            }

            _streamSet = await TryGetMediaStreamInfoSet(client, id);
            if (_streamSet == null)
            {
                // error occured: close the window and return from the method
                Close(); 
                return;
            }

            VideoTitle.Text = _video.Title;
            VideoDuration.Text = _video.Duration.Format();

            _audioStream = _streamSet.Audio.OrderByDescending(a => a.Bitrate).First();

            // sometimes muxed does not contains 720p stream
            int qualityLevel = _streamSet.Muxed.Any(x => x.VideoQuality == VideoQuality.High720) == false ? 3 : 4;

            foreach (VideoStreamInfo info in _streamSet.Video.Where(x => (int)x.VideoQuality > qualityLevel))
            {
                VideoRadioButton button = new VideoRadioButton();
                button.GroupName = "download";
                button.Video = info;
                button.Container = info.Container.Format();
                button.VideoQualityLevel = info.VideoQualityLabel;
                button.Size = (info.Size + _audioStream.Size).ToMb();
                button.Margin = new Thickness(0, 10, 0, 0);

                button.Checked += Button_Checked;

                ButtonPanel.Children.Add(button);
            }

            foreach (MuxedStreamInfo info in _streamSet.Muxed.Where(x => (int)x.VideoQuality > 1))
            {
                VideoRadioButton button = new VideoRadioButton();
                button.GroupName = "download";
                button.Muxed = info;
                button.Container = info.Container.Format();
                button.VideoQualityLevel = info.VideoQualityLabel;
                button.Size = info.Size.ToMb();
                button.Margin = new Thickness(0, 10, 0, 0);

                button.Checked += Button_Checked;

                ButtonPanel.Children.Add(button);
            }

            if (ButtonPanel.Children.Count > 0)
            {
                if (ButtonPanel.Children[0] is VideoRadioButton radioButton) radioButton.IsChecked = true;
            }

            ShowWindow();

            _windowLoaded = true;
        }

        private async Task<Video> TryGetVideo(YoutubeClient client, string id)
        {
            try
            {
                return await client.GetVideoAsync(id);
            }
            catch (YoutubeExplode.Exceptions.VideoRequiresPurchaseException ex)
            {
                DialogHelper.ShowInfoWindow(ex.Message);
            }
            catch (YoutubeExplode.Exceptions.VideoUnavailableException ex)
            {
                DialogHelper.ShowInfoWindow(ex.Message);
            }
            catch (YoutubeExplode.Exceptions.VideoUnplayableException ex)
            {
                DialogHelper.ShowInfoWindow(ex.Message);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowInfoWindow(ex.Message);
            }

            return null;
        }

        private async Task<MediaStreamInfoSet> TryGetMediaStreamInfoSet(YoutubeClient client, string id)
        {
            try
            {
                return await client.GetVideoMediaStreamInfosAsync(id);
            }
            catch (YoutubeExplode.Exceptions.VideoUnavailableException ex)
            {
                DialogHelper.ShowInfoWindow(ex.Message);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowInfoWindow(ex.Message);
            }

            return null;
        }

        private void Button_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is VideoRadioButton radioButton)
            {
                if (radioButton.Muxed != null)
                {
                    _muxedStream = radioButton.Muxed as MuxedStreamInfo;
                    _videoStream = null;

                }
                else
                {
                    _videoStream = radioButton.Video as VideoStreamInfo;
                    _muxedStream = null;
                }
            }
        }

        private void ShowWindow()
        {
            if (Application.Current.MainWindow is MainWindow window)
            {
                double left = 0;
                double top = 0;

                if (window.WindowState != WindowState.Maximized)
                {
                    left = window.Left;
                    top = window.Top;
                }

                Left = left + (window.ActualWidth - ActualWidth) / 2;
                Top = top + ((window.ActualHeight - ActualHeight) / 2) - 60;
            }
        }

        private void HideWindow()
        {
            Top = 5000;
        }

        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            if (_muxedStream != null)
            {
                DownloadWindowEventArgs args = new DownloadWindowEventArgs
                {
                    Video = _video,
                    MuxedStream = _muxedStream,
                    AudioStream = _audioStream,
                    OnlyAudio = OnlyAudioButton.GetValue(),
                    Convert = ConvertButton.GetValue(),
                    Format = Format.GetValue(),
                    Output = Output.Text
                };

                Download?.Invoke(this, args);
            }
            else
            {
                DownloadWindowEventArgs args = new DownloadWindowEventArgs
                {
                    Video = _video,
                    AudioStream = _audioStream,
                    VideoStream = _videoStream,
                    OnlyAudio = OnlyAudioButton.GetValue(),
                    Convert = ConvertButton.GetValue(),
                    Format = Format.GetValue(),
                    Output = Output.Text
                };

                Download?.Invoke(this, args);
            }

            Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelAll_OnClick(object sender, RoutedEventArgs e)
        {
            CancelAll = true;

            Close();
        }

        private void Output_Click(object sender, RoutedEventArgs e)
        {
            string output = DialogHelper.GetOutputFolder(Application.Current.MainWindow);

            if (Directory.Exists(output))
            {
                Output.Text = output;

                using (StreamWriter writer = new StreamWriter(FileSystem.Downloads, false, Encoding.UTF8))
                {
                    writer.Write(output);
                }
            }
        }

     
    }
}
