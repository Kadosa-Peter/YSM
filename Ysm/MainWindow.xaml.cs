using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ysm.Actions;
using Ysm.Assets;
using Ysm.Assets.Caches;
using Ysm.Assets.Trial;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Core.Ioc;
using Ysm.Core.KeyboadrHook;
using Ysm.Data;
using Ysm.Views;
using Ysm.Windows;

using Licence = Ysm.Assets.Licence;
using Update = Ysm.Assets.Update;

namespace Ysm
{
    public partial class MainWindow
    {
        private AssistanceHelper _assistanceHelper;

        private KeyboardHook _keyboardHook;

        private bool _deleteCache;

        public MainWindow()
        {
            try
            {
                BeforeCtor();

                InitializeComponent();

                AfterCtor();
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

        public void BeforeCtor()
        {
            try
            {
                AppSettings.Read();

                if (AuthenticationService.Default.IsLoggedIn)
                {
                    Database.Setup();

                    Repository.Default.Initialize();
                    FavoritesCache.Default.Initialize();
                    WatchLaterCache.Default.Initialize();
                    VisibleNodes.Default.Initialize();
                    Kernel.Default.LoggedIn = true;
                }

                ChromiumFx.Setup();

                if (DebugMode.IsDebugMode == false)
                {
                    //! ChromiumFx.Setup(); után kell állnia.
                    SingleInstance.Make("YSM", Application.Current);
                }

                //! InitializeComponent elött kell állnia!
                if (DebugMode.IsDebugMode == false && Kernel.Default.LoggedIn && Settings.Default.MarkVideosWatchedStartup && !Settings.Default.AskMarkAllVideoWatched)
                {
                    Repository.Default.Videos.MarkAllWatched();
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

        private void AfterCtor()
        {
            try
            {
                WindowSettings.SetLanguage(this);

                WindowSettings.SetState(this);

                AuthenticationService.Default.LoggedIn += LoggedIn;
                AuthenticationService.Default.LoggedOut += LoggedOut;
                AuthenticationService.Default.Shutdown += Shutdown;
                AuthenticationService.Default.Initialize();

                Settings.Default.PropertyChanged += ApplicationSettings_PropertyChanged;

                _assistanceHelper = new AssistanceHelper(this, Kernel.Default.UserGuide);
                SimpleIoc.Default.Register(() => _assistanceHelper);

                _keyboardHook = new KeyboardHook();
                _keyboardHook.KeyDown += KeyboardHook_KeyDown;
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
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        private void ApplicationSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AutoDownload")
            {
                if (Settings.Default.AutoDownload)
                {
                    DownloadTimer.Default.Start();
                }
                else
                {
                    DownloadTimer.Default.Stop();
                }
            }           
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            void FirstStartup()
            {
                Task.Delay(1000).GetAwaiter().OnCompleted(() =>
                {
                    WelcomeWindow welcomeWindow = new WelcomeWindow();
                    welcomeWindow.Owner = this;
                    welcomeWindow.ShowDialog();

                    LoginWindow loginWindow = new LoginWindow { Owner = this };
                    loginWindow.ShowDialog();
                });
            }

            void RunDownloader()
            {
                if ((DateTime.Now - Settings.Default.ShutDown).TotalMinutes > 5)
                {
                    Downloader downloader = new Downloader();
                    downloader.Run();
                }
            }

            void AskMarkWatched()
            {
                Task.Delay(300).GetAwaiter().OnCompleted(() =>
                {
                    if (Dialogs.OpenDialog(Properties.Resources.Question_MarkAllVideoWatched))
                    {
                        Repository.Default.Videos.MarkAllWatched();
                        ViewRepository.Get<ChannelView>().Reset();
                        ViewRepository.Get<VideoView>().RemoveAll();
                        ViewRepository.Get<FooterView>().UpdateCount();
                    }

                    VideoServiceWrapper.Default.AllowUpdate();
                    Update.Run();

                });
            }

            AllowToRun();

            WindowSettings.SetPosition(this);

            ViewRepository.Get<SidebarView>().SetSidebar();

            if (DebugMode.IsDebugMode) return;

            DownloadTimer.Default.Start();

            if (Settings.Default.FirstStartup)
            {
                FirstStartup();
            }
            else if (Kernel.Default.LoggedIn)
            {
                if (Settings.Default.AskMarkAllVideoWatched && Repository.Default.Videos.UnwatchedCount() > 0)
                {
                    VideoServiceWrapper.Default.ForbidUpdate();
                    RunDownloader();
                    AskMarkWatched();
                }
                else
                {
                    RunDownloader();
                    Update.Run();
                }
            }
        }

        private static void AllowToRun()
        {
            if (DebugMode.IsDebugMode) return;

            if (Licence.IsValid() == false)
            {
                bool trial = Trial.Check();

                // érvényes trial még x óráig
                if (trial)
                {
                    ViewRepository.Get<HeaderView>()?.Trial(Trial.Hours);
                }
                else
                {
                    SerialWindow window = new SerialWindow();
                    window.Owner = Application.Current.MainWindow;
                    window.ShowDialog();

                    if (Licence.IsValid() == false)
                    {
                        // nincs aktiválva-nincs érvényes serial megadva
                        // és mivel a trial nem érvényes kilépek a programból
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        private void LoggedIn(object sender, EventArgs e)
        {
            Kernel.Default.LoggedIn = true;

            Database.Setup();
            Repository.Default.Initialize();

            FavoritesCache.Default.Initialize();
            WatchLaterCache.Default.Initialize();
            VisibleNodes.Default.Initialize();

            ChannelMapper.Reset();

            Kernel.Default.View = View.Subscriptions;

            ViewRepository.Get<ChannelView>().Login();
            ViewRepository.Get<VideoView>().Login();
            ViewRepository.Get<FooterView>().Login();
            ViewRepository.Get<FavoritesView>()?.Login();
            ViewRepository.Get<WatchLaterView>()?.Login();
            ViewRepository.Get<HistoryView>()?.Login();
            ViewRepository.Get<PlaylistView>()?.Login();
            ViewRepository.Get<MarkerView>()?.Login();
            ViewRepository.Get<SidebarView>()?.Login();

            DownloadTimer.Default.Start();

            Downloader downloader = new Downloader();
            downloader.Run();
        }

        private void LoggedOut(object sender, EventArgs e)
        {
            Kernel.Default.LoggedIn = false;

            Kernel.Default.PlayerVideo = null;
            Kernel.Default.SelectedVideoItem = null;

            ViewRepository.Get<ChannelView>()?.Logout();
            ViewRepository.Get<VideoView>()?.Logout();
            ViewRepository.Get<PlayerTabView>()?.Logout();
            ViewRepository.Get<PlayerControlView>()?.Logout();
            ViewRepository.Get<FooterView>()?.Logout();
            ViewRepository.Get<FavoritesView>()?.Logout();
            ViewRepository.Get<WatchLaterView>()?.Logout();
            ViewRepository.Get<HistoryView>()?.Logout();
            ViewRepository.Get<PlaylistView>()?.Logout();
            ViewRepository.Get<MarkerView>()?.Logout();
            ViewRepository.Get<SidebarView>()?.Logout();

            DownloadTimer.Default.Stop();
        }

        private void Shutdown(object sender, EventArgs e)
        {
            _deleteCache = true;

            Application.Current.Shutdown();
        }

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Settings.Default.IsMaximized = true;
            }
            else
            {
                Settings.Default.IsMaximized = false;
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            void Close()
            {
                Settings.Default.Width = ActualWidth;
                Settings.Default.Height = ActualHeight;
                Settings.Default.Top = Top;
                Settings.Default.Left = Left;
                Settings.Default.Save();
                DownloadTimer.Default.Dispose();
            }

            if (Kernel.Default.IsDeleteing)
            {
                if (Dialogs.OpenDialog(Messages.GeUnsubscriptionMessage()))
                {
                    Close();
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                Close();
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Hide();
            ShowInTaskbar = false;
            ViewRepository.Get<PlayerTabView>()
                ?.SelectedPlayerView
                ?.SaveEndSynchronously(); // this will block the UI thread for 0.5-1s

            _keyboardHook?.Dispose();

            _assistanceHelper.Dispose();

            Settings.Default.FirstStartup = false;

            Settings.Default.ShutDown = DateTime.Now;

            Settings.Default.Save();

            ChromiumFx.CleanUp(_deleteCache);

            Logger.Save();
        }
      
        private void KeyboardHook_KeyDown(object sender, KeyHookEventArgs e)
        {
            if (Kernel.Default.LoggedIn == false) return;

            // Global Hotkeys
            if (Settings.Default.AllowGlobalHotkeys)
            {
                if (e.Shift && e.Ctrl && e.Key == Key.Left)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.PreviousVideo();
                    return;
                } // Previoud Video
                if (e.Shift && e.Ctrl && e.Key == Key.Right)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.NextVideo();
                    return;
                } // Next Video
                if (e.Shift && e.Ctrl && e.Key == Key.Up)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.VolumeUp();
                    return;
                } // Volume Up
                if (e.Shift && e.Ctrl && e.Key == Key.Down)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.VolumeDown();
                    return;
                } // Volume Down
                if (e.Shift && e.Ctrl && e.Key == Key.M)
                {
                    ViewRepository.Get<PlayerTabView>().Mute();
                    return;
                } // Mute
                if (e.Shift && e.Ctrl && e.Key == Key.F) 
                {
                    ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.AddToFavorites();
                }// Add to Favorites
                if (e.Shift && e.Ctrl && e.Key == Key.L)
                {
                    ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.WatchLater();
                }// Watch Later
                if (e.Shift && e.Ctrl && e.Key == Key.S)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.Stop();
                    return;
                } // Stop
                if (e.Key == Key.MediaPlayPause)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.Stop();
                    return;
                } // Stop
                if (e.Key == Key.MediaNextTrack)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.NextVideo();
                    return;
                } // Next Video
                if (e.Key == Key.MediaPreviousTrack)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.PreviousVideo();
                    return;
                } // Previoud Video
            }

            if (IsActive && !InputHelper.IsInputFocused())
            {
                if(WindowHelper.GetActiveWindowTitle() != "YSM") return;

                if (ModifierKeyHelper.IsShiftDown && e.Key == Key.Enter)
                {
                    ViewRepository.Get<ChannelView>().MarkAllWatched();
                } // Mark all watched
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.K)
                {
                    IAction action = ActionRepository.Find("OpenSettings");
                    action?.Execute(null);
                } // Open Settings
                else if (e.Key == Key.F5)
                {
                    IAction action = ActionRepository.Find("VideoService");
                    action?.Execute(null);
                } // Video Service
                else if (e.Key == Key.F6)
                {
                    IAction action = ActionRepository.Find("SubscriptionService");
                    action?.Execute(null);
                } // Subscription Service
                else if (e.Key == Key.F8)
                {
                    if (Settings.Default.VideoDisplayMode == VideoDisplayMode.UnwatchedVideos)
                        Settings.Default.VideoDisplayMode = VideoDisplayMode.AllVideos;
                    else
                        Settings.Default.VideoDisplayMode = VideoDisplayMode.UnwatchedVideos;
                } // Video State
                else if (e.Key == Key.F9)
                {
                    if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                        Settings.Default.SubscriptionDisplayMode = SubscriptionDisplayMode.AllSubscriptions;
                    else
                        Settings.Default.SubscriptionDisplayMode = SubscriptionDisplayMode.ActiveSubscriptions;
                } // Subscription View Mode

                #region Player

                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.Right)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.NextVideo();
                } // Next Video 
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.Left)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.NextVideo();
                } // Previous Video
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.Up)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.VolumeUp();
                } // Volume Up
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.Down)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.VolumeDown();
                } // Volume Down    
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.M)
                {
                    ViewRepository.Get<PlayerTabView>().Mute();
                } // Mute
                if (e.Ctrl && e.Key == Key.F)
                {
                    ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.AddToFavorites();
                }// Add to Favorites
                if (e.Ctrl && e.Key == Key.L)
                {
                    ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.WatchLater();
                }// Watch Later
                else if (e.Key == Key.Space)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.Stop();
                } // Stop
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.S)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.Stop();
                } // Stop
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.N) 
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.CreateMarker();
                } // Create Note

                #endregion

                #region SwitchView

                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.D1)
                {
                    Kernel.Default.View = View.Subscriptions;
                } // Subscriptions
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.D2)
                {
                    Kernel.Default.View = View.Favorites;
                } // Favorites
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.D3)
                {
                    Kernel.Default.View = View.WatchLater;
                } // WatchLater
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.D4)
                {
                    Kernel.Default.View = View.History;
                } // History
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.D5)
                {
                    Kernel.Default.View = View.Playlists;
                } // Playlists
                else if (ModifierKeyHelper.IsCtrlDown && e.Key == Key.D6)
                {
                    Kernel.Default.View = View.Markers;
                } // Bookmarks
                else if (e.Key == Key.NumPad1)
                {
                    Kernel.Default.View = View.Subscriptions;
                } // Subscriptions
                else if (e.Key == Key.NumPad2)
                {
                    Kernel.Default.View = View.Favorites;
                } // WatchLater
                else if (e.Key == Key.NumPad3)
                {
                    Kernel.Default.View = View.WatchLater;
                } // WatchLater
                else if (e.Key == Key.NumPad4)
                {
                    Kernel.Default.View = View.History;
                } // History
                else if (e.Key == Key.NumPad5)
                {
                    Kernel.Default.View = View.Playlists;
                } // Playlists
                else if (e.Key == Key.NumPad6)
                {
                    Kernel.Default.View = View.Markers;
                } // Bookmarks

                #endregion
            }
        }

        // TaskbarIcon
        private void TaskbarIcon_OnTrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        public void ShowMainWindow()
        {
            ShowInTaskbar = true;
            SystemTrayIcon.Visibility = Visibility.Collapsed;
            Show();

            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}
