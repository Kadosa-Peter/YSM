using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;
using Ysm.Core;
using Ysm.Downloader.Assets;
using Ysm.Downloader.Download;

namespace Ysm.Downloader.Views
{
    public partial class DownloadView : INotifyPropertyChanged
    {
        #region UI Data
        public string ThumbnailUrl
        {
            get => _thumbnailUrl;

            set
            {
                if (_thumbnailUrl != value)
                {
                    _thumbnailUrl = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _thumbnailUrl;

        public string Title
        {
            get => _title;

            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _title;

        public string Duration
        {
            get => _duration;

            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _duration;

        public string Size
        {
            get => _size;

            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _size;

        public string Extension
        {
            get => _extension;

            set
            {
                if (_extension != value)
                {
                    _extension = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _extension;

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (Math.Abs(_progressValue - value) > 0.1)
                {
                    _progressValue = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _progressValue;

        public string ProgressState
        {
            get => _progressState;

            set
            {
                if (_progressState != value && value != null)
                {
                    _progressState = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _progressState;

        public string ProgressPercent
        {
            get => _progressPercent;

            set
            {
                if (_progressPercent != value)
                {
                    _progressPercent = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _progressPercent;

        public SolidColorBrush ProgressbarBrush
        {
            get => _progressbarBrush;
            set
            {
                if (!Equals(_progressbarBrush, value))
                {
                    _progressbarBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        private SolidColorBrush _progressbarBrush;

        #endregion

        public bool IsComplete { get; set; }

        public bool IsDownloading { get; set; }

        public event EventHandler DownloadComplete;
        public event EventHandler DownloadCanceled;

        public event EventHandler Removed;
        public event EventHandler Added;

        private IDownloader _downloader;

        private readonly MuxedStreamInfo _stream;
        private readonly VideoStreamInfo _videoStream;
        private readonly AudioStreamInfo _audioStream;
        private readonly Video _video;
        private readonly bool _audioOnly;
        private readonly bool _convert;
        private readonly string _format;
        private string _output;

        public DownloadView()
        {
            ProgressbarBrush = new SolidColorBrush(Color.FromArgb(255, 119, 211, 146));
        }

        // Muxed
        public DownloadView(MuxedStreamInfo stream, Video video, bool convert, string format, string output) : this()
        {
            _stream = stream;
            _video = video;
            _convert = convert;
            _format = format;
            _output = output;

            Title = _video.Title;
            Duration = _video.Duration.Format();
            ThumbnailUrl = video.GetThumbnail();
            Size = stream.Size.ToMb();
            Extension = convert ? format.ToUpper() : stream.Container.GetFileExtension().ToUpper();

            InitializeComponent();

            DownloadMonitor.Default.Add(this);

            Loaded += (k, l) => Added?.Invoke(this, EventArgs.Empty);
        }

        // Regular - Video/Audio stream
        public DownloadView(VideoStreamInfo videoStream, AudioStreamInfo audioStream, Video video, bool convert, string format, string output) : this()
        {
            _videoStream = videoStream;
            _audioStream = audioStream;
            _video = video;
            _convert = convert;
            _format = format;
            _output = output;

            Title = _video.Title;
            Duration = _video.Duration.Format();
            ThumbnailUrl = video.GetThumbnail();
            Size = (videoStream.Size + audioStream.Size).ToMb();
            Extension = convert ? format.ToUpper() : videoStream.Container.GetFileExtension().ToUpper();

            InitializeComponent();

            DownloadMonitor.Default.Add(this);

            Loaded += (k, l) => Added?.Invoke(this, EventArgs.Empty);
        }

        // Audio Only
        public DownloadView(AudioStreamInfo audioStream, Video video, string output) : this()
        {
            _audioStream = audioStream;
            _video = video;
            _output = output;

            _audioOnly = true;

            Title = _video.Title;
            Duration = _video.Duration.Format();
            ThumbnailUrl = video.GetThumbnail();
            Size = audioStream.Size.ToMb();
            Extension = "MP3";

            InitializeComponent();

            DownloadMonitor.Default.Add(this);

            Loaded+=(k,l) => Added?.Invoke(this, EventArgs.Empty);
        }

        public void StartDownload()
        {
            if (_audioOnly) // audio stream
            {
                _downloader = new AudioDownloader(_audioStream);
                _downloader.DownloadStarted += Downloader_DownloadStarted;
                _downloader.DownloadProgessChanged += Downloader_DownloadProgessChanged;
                _downloader.DownloadFinished += Downloader_DownloadFinished;
                _downloader.DownloadAudio(_video.Title, _output);
            }
            else if (_stream != null) // muxed stream
            {
                _downloader = new MuxedDownloader(_stream);
                _downloader.DownloadStarted += Downloader_DownloadStarted;
                _downloader.DownloadProgessChanged += Downloader_DownloadProgessChanged;
                _downloader.DownloadFinished += Downloader_DownloadFinished;

                if (_convert)
                {
                    _downloader.DownloadAndConvert(_video.Title, _output, _format);
                }
                else
                {
                    _downloader.Download(_video.Title, _output);
                }
            }
            else // adaptive stream
            {
                _downloader = new AdaptiveDownloader(_videoStream, _audioStream);
                _downloader.DownloadStarted += Downloader_DownloadStarted;
                _downloader.DownloadProgessChanged += Downloader_DownloadProgessChanged;
                _downloader.DownloadFinished += Downloader_DownloadFinished;

                if (_convert)
                {
                    _downloader.DownloadAndConvert(_video.Title, _output, _format);
                }
                else
                {
                    _downloader.Download(_video.Title, _output);
                }
            }

        }

        private void Downloader_DownloadStarted(object sender, DownloadEventArgs e)
        {
            IsDownloading = true;
            Progress.Visibility = Visibility.Visible;
            Queue.Visibility = Visibility.Collapsed;
        }

        private void Downloader_DownloadProgessChanged(object sender, DownloadEventArgs e)
        {
            if (e.IsConverting)
            {
                //ProgressbarBrush = new SolidColorBrush(Color.FromArgb(255, 244, 244, 89));
                ProgressbarBrush = new SolidColorBrush(Color.FromArgb(255, 245, 236, 45));
            }

            ProgressState = e.Message;
            ProgressValue = e.Percent;
            ProgressPercent = $"Completed: {e.Percent}%";
        }

        private void Downloader_DownloadFinished(object sender, DownloadEventArgs e)
        {
            IsDownloading = false;
            IsComplete = true;

            _output = e.Output;

            Progress.Visibility = Visibility.Collapsed;
            Controls.Visibility = Visibility.Visible;

            DownloadComplete?.Invoke(this, EventArgs.Empty);
        }

        #region User Actions

        private void Remove_OnClick(object sender, RoutedEventArgs e)
        {
            _downloader.Cancel();

            DownloadCanceled?.Invoke(this, EventArgs.Empty);

            if (LogicalTreeHelper.GetParent(this) is StackPanel parent) parent.Children.Remove(this);

            Removed?.Invoke(this, EventArgs.Empty);
        }

        private void ShowInFolder_OnClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (File.Exists(_output))
                {
                    string dir = Path.GetDirectoryName(_output);

                    Process.Start(dir ?? throw new InvalidOperationException());
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void Play_OnClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (File.Exists(_output))
                {
                    Process.Start(_output);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }
        }

        #endregion

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
