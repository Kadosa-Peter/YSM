using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Ysm.Core;

// ReSharper disable once CheckNamespace
namespace Ysm.Assets
{
    public class Settings : SettingsBase
    {
        #region Setup

        private static volatile Settings _defaultInstance;

        private static readonly object Mutex = new object();

        private static bool _loaded;

        public static Settings Default
        {
            get
            {
                lock (Mutex)
                {
                    if (_defaultInstance == null)
                        _defaultInstance = new Settings();
                }

                return _defaultInstance;
            }
        }

        private Settings()
        {

        }

        static Settings()
        {
            if (File.Exists(FileSystem.Settings))
            {
                Load();
            }
            else
            {
                _defaultInstance = new Settings();
            }

            _loaded = true;
        }

        private static void Load()
        {
            string file = FileSystem.Settings;

            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                string json = sr.ReadToEnd();

                _defaultInstance = JsonConvert.DeserializeObject<Settings>(json);
            }
        }

        public void Save()
        {
            try
            {
                string file = FileSystem.Settings;

                string json = JsonConvert.SerializeObject(_defaultInstance, Formatting.Indented);

                using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    writer.Write(json);
                }
            }
            catch
            {
                // settings file used by another process
                // ignore   
            }
        }

        public void OverWrite(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = path.ReadText();

                    Settings settings = JsonConvert.DeserializeObject<Settings>(json);

                    PropertyInfo[] properties = settings.GetType().GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        if (!property.CanWrite || property.Name == "Default") continue;
                        property.SetValue(this, property.GetValue(settings));
                    }
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }

        public override void SettingsChanged(string propertyName)
        {
            if (_loaded)
            {
                Save();
            }
        }

        #endregion

        public bool FullScreen { get; set; }

        public bool LastPosition { get; set; }

        public bool DefaultPostion { get; set; } = true;

        public bool IsMaximized { get; set; }

        public double Width { get; set; } = 1200;

        public double DefaultWidth { get; set; } = 1200;

        public double Height { get; set; } = 740;

        public double DefaultHeight { get; set; } = 740;

        public double Top { get; set; }

        public double Left { get; set; }

        public bool ShowFavoritesIcon
        {
            get => _showFavoritesIcon;

            set
            {
                if (_showFavoritesIcon != value)
                {
                    _showFavoritesIcon = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showFavoritesIcon = true;

        public bool ShowWatchlaterIcon
        {
            get => _showWatchlaterIcon;

            set
            {
                if (_showWatchlaterIcon != value)
                {
                    _showWatchlaterIcon = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showWatchlaterIcon = true;

        public bool ShowLocateIcon
        {
            get => _showLocateIcon;

            set
            {
                if (_showLocateIcon != value)
                {
                    _showLocateIcon = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showLocateIcon = true;

        public bool ShowDownloadIcon
        {
            get => _showDownloadIcon;

            set
            {
                if (_showDownloadIcon != value)
                {
                    _showDownloadIcon = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showDownloadIcon;

        public bool ShowNewtabIcon
        {
            get => _showNewtabIcon;

            set
            {
                if (_showNewtabIcon != value)
                {
                    _showNewtabIcon = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showNewtabIcon;

        public bool ShowPlaylistIcon
        {
            get => _showPlaylistIcon;

            set
            {
                if (_showPlaylistIcon != value)
                {
                    _showPlaylistIcon = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showPlaylistIcon;

        public double NotifyWindowHeight { get; set; } = 230;

        public bool ShowSidebar
        {
            get => _showSidebar;

            set
            {
                if (_showSidebar != value)
                {
                    _showSidebar = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showSidebar = true;

        public bool AllowGlobalHotkeys
        {
            get => _allowGlobalHotkeys;

            set
            {
                if (_allowGlobalHotkeys != value)
                {
                    _allowGlobalHotkeys = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _allowGlobalHotkeys = true;

        public bool MinimizeToSystemTray
        {
            get => _minimizeToSystemTray;

            set
            {
                if (_minimizeToSystemTray != value)
                {
                    _minimizeToSystemTray = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _minimizeToSystemTray;

        public bool CloseToSystemTray
        {
            get => _closeToSystemTray;

            set
            {
                if (_closeToSystemTray != value)
                {
                    _closeToSystemTray = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _closeToSystemTray;

        public bool StartNotificationArea
        {
            get => _startNotificationArea;

            set
            {
                if (_startNotificationArea != value)
                {
                    _startNotificationArea = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _startNotificationArea;

        public bool StartTaskbar
        {
            get => _startTaskbar;

            set
            {
                if (_startTaskbar != value)
                {
                    _startTaskbar = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _startTaskbar;

        public bool FirstStartup
        {
            get => _firstStartup;

            set
            {
                if (_firstStartup != value)
                {
                    _firstStartup = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _firstStartup = true;

        public bool StartOnSystemStartup
        {
            get => _startOnSystemStartup;

            set
            {
                if (_startOnSystemStartup != value)
                {
                    _startOnSystemStartup = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _startOnSystemStartup;

        public SortType SubscriptionSort
        {
            get => _subscriptionSort;

            set
            {
                if (_subscriptionSort != value)
                {
                    _subscriptionSort = value;
                    OnPropertyChanged();
                }
            }
        }
        private SortType _subscriptionSort;

        public VideoDisplayMode VideoDisplayMode
        {
            get => _videoDisplayMode;

            set
            {
                if (_videoDisplayMode != value)
                {
                    _videoDisplayMode = value;
                    OnPropertyChanged();
                }
            }
        }
        private VideoDisplayMode _videoDisplayMode;

        public SubscriptionDisplayMode SubscriptionDisplayMode
        {
            get => _subscriptionDisplayMode;

            set
            {
                if (_subscriptionDisplayMode != value)
                {
                    _subscriptionDisplayMode = value;
                    OnPropertyChanged();
                }
            }
        }
        private SubscriptionDisplayMode _subscriptionDisplayMode;

        public bool ShowToolbarTooltip
        {
            get => _showToolbarTooltip;

            set
            {
                if (_showToolbarTooltip != value)
                {
                    _showToolbarTooltip = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showToolbarTooltip = true;

        public bool ShowVideoNotifyWindow
        {
            get => _showVideoNotifyWindow;

            set
            {
                if (_showVideoNotifyWindow != value)
                {
                    _showVideoNotifyWindow = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showVideoNotifyWindow = true;

        public bool ShowSubscriptionNotifyWindow
        {
            get => _showSubscriptionNotifyWindow;

            set
            {
                if (_showSubscriptionNotifyWindow != value)
                {
                    _showSubscriptionNotifyWindow = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showSubscriptionNotifyWindow;

        public bool PlayNotifySound
        {
            get => _playNotifySound;

            set
            {
                if (_playNotifySound != value)
                {
                    _playNotifySound = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _playNotifySound = true;

        public bool MarkVideosWatchedStartup
        {
            get => _markVideosWatchedStartup;

            set
            {
                if (_markVideosWatchedStartup != value)
                {
                    _markVideosWatchedStartup = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _markVideosWatchedStartup;

        public bool Autoplay
        {
            get => _autoplay;

            set
            {
                if (_autoplay != value)
                {
                    _autoplay = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _autoplay = true;

        public bool AskToContinuePlayback
        {
            get => _askToContinuePlayback;

            set
            {
                if (_askToContinuePlayback != value)
                {
                    _askToContinuePlayback = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _askToContinuePlayback = true;

        public bool AutoDownload
        {
            get => _autoDownload;

            set
            {
                if (_autoDownload != value)
                {
                    _autoDownload = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _autoDownload = true;

        public int AutoDownloadMinutes
        {
            get => _autoDownloadMinutes;

            set
            {
                if (_autoDownloadMinutes != value)
                {
                    _autoDownloadMinutes = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _autoDownloadMinutes = 30;

        // 0=today, 1=yesterday, 2=day before yesterday 3=Last 7 days, 4=Last 30 days
        public int HistoryFilter
        {
            get => _historyFilter;

            set
            {
                if (_historyFilter != value)
                {
                    _historyFilter = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _historyFilter;

        public bool History
        {
            get => _history;

            set
            {
                if (_history != value)
                {
                    _history = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _history = true;

        public bool RemoveAds
        {
            get => _removeAds;

            set
            {
                if (_removeAds != value)
                {
                    _removeAds = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _removeAds = true;

        public bool MarkAllWatchedDialog
        {
            get => _markAllWatchedDialog;

            set
            {
                if (_markAllWatchedDialog != value)
                {
                    _markAllWatchedDialog = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _markAllWatchedDialog = true;

        public bool AskMarkAllVideoWatched
        {
            get => _askMarkAllVideoWatched;

            set
            {
                if (_askMarkAllVideoWatched != value)
                {
                    _askMarkAllVideoWatched = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _askMarkAllVideoWatched = true;

        public bool ShowPatreonLink
        {
            get => _showPatreonLink;

            set
            {
                if (_showPatreonLink != value)
                {
                    _showPatreonLink = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showPatreonLink = true;

        public bool ShowMarkerBtn
        {
            get => _showMarkerBtn;

            set
            {
                if (_showMarkerBtn != value)
                {
                    _showMarkerBtn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showMarkerBtn = true;

        public bool ShowCommentsBtn
        {
            get => _showCommentsBtn;

            set
            {
                if (_showCommentsBtn != value)
                {
                    _showCommentsBtn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showCommentsBtn = true;

        public bool ShowLocateBtn
        {
            get => _showLocateBtn;

            set
            {
                if (_showLocateBtn != value)
                {
                    _showLocateBtn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showLocateBtn = true;

        public bool ShowDownloadBtn
        {
            get => _showDownloadBtn;

            set
            {
                if (_showDownloadBtn != value)
                {
                    _showDownloadBtn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showDownloadBtn = true;

        public bool ShowWatchLaterBtn
        {
            get => _showWatchLaterBtn;

            set
            {
                if (_showWatchLaterBtn != value)
                {
                    _showWatchLaterBtn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showWatchLaterBtn = true;

        public bool ShowFavoritesBtn
        {
            get => _showFavoritesBtn;

            set
            {
                if (_showFavoritesBtn != value)
                {
                    _showFavoritesBtn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showFavoritesBtn = true;

        public bool ShowPlaylistBtn
        {
            get => _showPlaylistBtn;

            set
            {
                if (_showPlaylistBtn != value)
                {
                    _showPlaylistBtn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showPlaylistBtn = true;

        public string Validated { get; set; }

        public string Trial { get; set; }

        public DateTime Update { get; set; }

        public DateTime ShutDown { get; set; }

        public string Volume { get; set; } = "35";
        
    }
}