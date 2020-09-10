using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Chromium;
using Chromium.Event;
using Chromium.Remote;
using Chromium.WebBrowser;
using Ysm.Assets;
using Ysm.Assets.Browser;
using Ysm.Assets.Caches;
using Ysm.Assets.Menu;
using Ysm.Core;
using Ysm.Data;
using Ysm.Windows;

using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using View = Ysm.Assets.View;

namespace Ysm.Views
{
    public partial class PlayerView
    {
        public PlayerHistory History { get; set; }

        public Video Video { get; set; }

        public bool IsMuted { get; set; }

        public int Seconds { get; set; }

        private FullScreenWindow _fullScreenWindow;

        private ChromiumWebBrowser _webBrowser;

        private readonly BrowserSize _browserSize;

        private bool _loaded;


        // CTRO
        public PlayerView(Video video, bool isMuted)
        {
            InitializeComponent();

            Video = video;
            IsMuted = isMuted;

            ViewRepository.Add(this, nameof(PlayerView));

            _browserSize = new BrowserSize();

            SetupBrowser();

            History = new PlayerHistory();

            PlayerMenu playerMenu = new PlayerMenu();
            ContextMenu = playerMenu.Get();
        }

        private void SetupBrowser()
        {
            _webBrowser = new ChromiumWebBrowser();
            _webBrowser.Dock = DockStyle.Fill;
            _webBrowser.KeyboardHandler.CallbacksDisabled = true;

            BoundObject boundObject = new BoundObject();
            _webBrowser.GlobalObject.Add("bound", boundObject);
            boundObject.Add(OnReady, "onReady");
            boundObject.Add(OnPlay, "onPlay");
            boundObject.Add(OnContinue, "onContinue");
            boundObject.Add(OnLog, "onLog");
            boundObject.Add(OnEnded, "onEnded");
            boundObject.Add(OnVolume, "onVolume");
            boundObject.Add(OnError, "onError");
            boundObject.Add(OnMute, "onMute");
            boundObject.Add(OnMarker, "onMarker");

            _webBrowser.KeyboardHandler.OnKeyEvent += KeyboardHandler_OnKeyEvent;
            _webBrowser.DisplayHandler.OnFullscreenModeChange += DisplayHandler_OnFullscreenModeChange;
            _webBrowser.RequestHandler.OnBeforeResourceLoad += RequestHandler_OnBeforeResourceLoad;
            _webBrowser.LifeSpanHandler.OnBeforePopup += LifeSpanHandler_OnBeforePopup;

            FormHost.Child = _webBrowser;
        }

        private void KeyboardHandler_OnKeyEvent(object sender, CfxOnKeyEventEventArgs e)
        {
            if (e.Event.WindowsKeyCode == 27)
            {
                _fullScreenWindow?.CloseFullScreen();
            }
        }

        private void DisplayHandler_OnFullscreenModeChange(object sender, CfxOnFullscreenModeChangeEventArgs e)
        {
            if (e.Fullscreen)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    FormHost.Child = null;

                    _fullScreenWindow = new FullScreenWindow(_webBrowser);
                    _fullScreenWindow.Show();
                    Keyboard.Focus(_fullScreenWindow);

                    /***/

                }));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_fullScreenWindow != null)
                    {
                        _fullScreenWindow.Close();

                        FormHost.Child = _webBrowser;
                    }

                    /***/

                }));
            }
        }

        private void LifeSpanHandler_OnBeforePopup(object sender, CfxOnBeforePopupEventArgs e)
        {
            e.SetReturnValue(true);
            Process.Start(e.TargetUrl);
        }

        private void RequestHandler_OnBeforeResourceLoad(object sender, CfxOnBeforeResourceLoadEventArgs e)
        {
            e.Request.SetReferrer("https://www.youtube.com/", CfxReferrerPolicy.Default);

            e.SetReturnValue(CfxReturnValue.Continue);
        }

        public void Open(Video video)
        {
            if (Video != null)
            {
                SaveEnd();
            }

            if (video?.VideoId != null)
            {
                Video = video;

                string template = GetTemplate(video);

                BrowserControl.SetVideo(video);

                _webBrowser.LoadString(template, "http://www.youtube.com");

                History.Add(video);
            }
        }

        public void SaveEnd()
        {
            try
            {
                string id = Video.VideoId;
                _webBrowser.EvaluateJavascript("getEndTime();", (e, k) =>
                {
                    if (e != null)
                    {
                        if (e.IsDouble)
                        {
                            int seconds = (int)e.DoubleValue;
                            if (seconds >= 15)
                            {
                                Repository.Default.Continuity.Save(id, seconds);
                            }
                        }
                    }

                });
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

        public void SaveEndSynchronously()
        {
            string id = Video.VideoId;
            object waitLock = new object();

            lock (waitLock)
            {
                bool evaluationStarted = _webBrowser.EvaluateJavascript("getEndTime(true);",
                    // Don't invoke, otherwise the ui thread will deadlock!
                    JSInvokeMode.DontInvoke,
                    (e, k) =>
                    {
                        Monitor.Enter(waitLock);

                        if (e != null)
                        {
                            if (e.IsDouble)
                            {
                                int seconds = (int)e.DoubleValue;
                                if (seconds >= 15)
                                {
                                    Repository.Default.Continuity.Save(id, seconds);
                                }
                            }
                        }

                        Monitor.Exit(waitLock);
                    }
                );

                if (evaluationStarted)
                {
                    Monitor.Wait(waitLock, 1000);
                }
            }
        }

        public void OpenInBrowser()
        {
            try
            {
                _webBrowser.EvaluateJavascript("getCurrentTime();", (e, k) =>
                {
                    // e lehet null, ha akkor akorom megnyitni a videót amikor még nem töltődött be a plyer-be teljesen
                    if (e == null)
                    {
                        Pause();

                        string url = $"https://www.youtube.com/watch?v={Video.VideoId}";

                        Process.Start(url);
                    }
                    else
                    {
                        if (e.IsDouble)
                        {
                            double seconds = e.DoubleValue;

                            int min = (int)seconds / 60;
                            int sec = (int)seconds % 60;

                            Pause();

                            string url = $"https://www.youtube.com/watch?v={Video.VideoId}&t={min}m{sec}s";

                            Process.Start(url);
                        }
                    }



                });
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

        private string GetTemplate(Video video)
        {
            string path = Path.Combine(FileSystem.Startup, "Resources", "template.html");

            if (File.Exists(path))
            {
                string html = path.ReadText();

                html = html.Replace("_videoid_", video.VideoId);

                html = html.Replace("var volume = 35;", $"var volume = {Settings.Default.Volume};");

                html = html.Replace("_width_", _browserSize.Width.ToString());
                html = html.Replace("_height_", _browserSize.Height.ToString());

                if (video.Start > 0)
                    html = html.Replace("\"start\": 0,", $"\"start\": {video.Start},");

                if (IsMuted)
                    html = html.Replace("var mute = false;", "var mute = true;");

                if (Settings.Default.RemoveAds == false)
                    html = html.Replace("var blockAds = true;", "var blockAds = false;");

                if (html.NotNull() && Settings.Default.Autoplay)
                    html = html.Replace("\"autoplay\": 0", "\"autoplay\": 1");

                return html;
            }

            return null;
        }

        private void BrowserView_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (Application.Current.MainWindow.IsActive)
                _webBrowser.Focus();
        }

        private void Root_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double height = e.NewSize.Height;

            if (width >= 1292 && height >= 740)
                SetLargeSize();
            else if (width >= 866 && height >= 500)
                SetMiddleSize();
            else
                SetSmallSize();

            //_webBrowser.Browser?.MainFrame.ExecuteJavaScript("repositionMarkers();", null, 0);
        }

        private void BrowserView_OnLoaded(object sender, RoutedEventArgs e)
        {
            double width = Root.ActualWidth;
            double height = Root.ActualHeight;

            // large
            if (width >= 1292 && height >= 740)
                SetLargeSize();
            else if (width >= 866 && height >= 500)
                SetMiddleSize();
            else
                SetSmallSize();

            if (_loaded == false)
            {
                _loaded = true;

                Open(Video);
            }
        }

        private void SetLargeSize()
        {
            // 1280x720
            if (_browserSize.WebViewSize != WebViewSize.Large)
            {
                _browserSize.WebViewSize = WebViewSize.Large;
                _browserSize.Height = 720;
                _browserSize.Width = 1280;

                FormHost.Height = 720;
                FormHost.Width = 1280;

                BrowserContainer.Width = 1280;
                HostContainer.Width = 1280;
                HostContainer.Height = 720;

                _webBrowser.Browser?.MainFrame.ExecuteJavaScript("setLarge();", null, 0);
            }
        }

        private void SetMiddleSize()
        {
            // 854*480
            if (_browserSize.WebViewSize != WebViewSize.Medium)
            {
                _browserSize.WebViewSize = WebViewSize.Medium;
                _browserSize.Height = 480;
                _browserSize.Width = 854;

                FormHost.Height = 480;
                FormHost.Width = 854;

                BrowserContainer.Width = 854;
                HostContainer.Width = 854;
                HostContainer.Height = 480;

                _webBrowser.Browser?.MainFrame.ExecuteJavaScript("setMiddle();", null, 0);
            }
        }

        private void SetSmallSize()
        {
            // 640*360

            if (_browserSize.WebViewSize != WebViewSize.Small)
            {
                _browserSize.WebViewSize = WebViewSize.Small;
                _browserSize.Height = 360;
                _browserSize.Width = 640;

                FormHost.Height = 360;
                FormHost.Width = 640;

                BrowserContainer.Width = 640;
                HostContainer.Width = 640;
                HostContainer.Height = 360;

                _webBrowser.Browser?.MainFrame.ExecuteJavaScript("setSmall();", null, 0);
            }
        }

        public void SetVolume(string volume)
        {
            _webBrowser.Browser?.MainFrame.ExecuteJavaScript($"player.setVolume({volume}); volume = {volume};", null, 0);
        }

        private void OnEnded(object obj)
        {
            _fullScreenWindow?.CloseFullScreen();

            ViewRepository.Get<VideoView>().VideoEnded(Video.VideoId);
        }

        private void OnLog(object obj)
        {
            try
            {
                CfrV8Value[] arguments = (CfrV8Value[])obj;

                foreach (CfrV8Value argument in arguments)
                {
                    string value = argument.StringValue;
                    if (value.NotNull())
                        Debug.WriteLine(value);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
            }
        }

        private void OnReady(object obj)
        {
            AddMarker();

            Seconds = Repository.Default.Continuity.Get(Video.VideoId);

            if (Seconds >= 15)
            {
                if (Settings.Default.AskToContinuePlayback)
                {
                    _webBrowser.Browser?.MainFrame.ExecuteJavaScript("showContinueLayer();", null, 0);
                }
            }
        }

        private void OnContinue(object obj)
        {
            SeekTo(Seconds);
        }

        private void OnPlay(object obj)
        {
            ViewRepository.Get<VideoView>().VideoStarted(Video.VideoId);
        }

        private void OnError(object obj)
        {
            _fullScreenWindow?.CloseFullScreen();

            ViewRepository.Get<VideoView>().VideoEnded(Video.VideoId);
        }

        private void OnVolume(object obj)
        {
            if (IsMuted) return;

            try
            {
                CfrV8Value[] arguments = (CfrV8Value[])obj;

                foreach (CfrV8Value argument in arguments)
                {
                    string value = argument.StringValue;

                    if (value.NotNull())
                    {
                        Settings.Default.Volume = value;

                        ViewRepository.Get<PlayerTabView>().SetVolume(value, Video.VideoId);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
            }
        }

        private void OnMute(object obj)
        {
            try
            {
                CfrV8Value[] arguments = (CfrV8Value[])obj;

                foreach (CfrV8Value argument in arguments)
                {
                    IsMuted = argument.BoolValue;
                    ViewRepository.Get<PlayerTabView>().IsMuted = IsMuted;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
            }
        }

        private void OnMarker(object obj)
        {
            try
            {
                CfrV8Value[] arguments = (CfrV8Value[])obj;

                string action = arguments[0].StringValue;

                if (action == "seekTo")
                {
                    string arg = arguments[1].StringValue;
                    int seconds = Convert.ToInt32(arg);

                    SeekTo(seconds);
                }
                else if (action == "create")
                {
                    double arg1 = arguments[1].DoubleValue;
                    
                    CreateMarker(arg1);
                }
                else if (action == "delete")
                {
                    string arg = arguments[1].StringValue;

                    RemoveMarker(Video.VideoId, arg);
                }
                else if (action == "update")
                {
                    string arg1 = arguments[1].StringValue; // marker id
                    string arg2 = arguments[2].StringValue; // comment 
                    string arg3 = arguments[3].StringValue; // second
                    UpdateMarker(arg1, arg2, arg3);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
            }
        }
      
        public void Hide()
        {
            FormHost.Visibility = Visibility.Collapsed;
            SaveEnd();
            Pause();
        }

        public void Show()
        {
            FormHost.Visibility = Visibility.Visible;

            Play();
        }

        public void Pause()
        {
            try
            {
                _webBrowser.Browser?.MainFrame.ExecuteJavaScript("pause();", null, 0);
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }
        }

        public void Play()
        {
            try
            {
                _webBrowser.Browser?.MainFrame.ExecuteJavaScript("play();", null, 0);
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }
        }

        public void Close()
        {
            string id = Video.VideoId;

            _webBrowser.EvaluateJavascript("getEndTime(false);", (e, k) =>
            {
                try
                {
                    if (e != null)
                    {
                        if (e.IsDouble)
                        {
                            int seconds = (int)e.DoubleValue;

                            if (seconds >= 15)
                            {
                                Repository.Default.Continuity.Save(id, seconds);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), exception);
                }

                _webBrowser.Dispose();
                _webBrowser = null;

            });
        }

        public void Refresh()
        {
            Open(Video);
        }

        public async void GetSource()
        {
            try
            {
                foreach (long id in _webBrowser.Browser.FrameIdentifiers)
                {
                    CfxFrame frame = _webBrowser.Browser.GetFrame(id);

                    if (frame.Url.Contains("youtube.com"))
                    {
                        string htmlSource = await HtmlSource.GetSourceAsync(frame);

                        HtmlSource.SaveHtmlSource(htmlSource, id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }
        }

        public void NextVideo()
        {
            _fullScreenWindow?.CloseFullScreen();

            ViewRepository.Get<VideoView>().NextVideo(Video.VideoId);
        }

        public void PreviousVideo()
        {
            _fullScreenWindow?.CloseFullScreen();

            ViewRepository.Get<VideoView>().PreviousVideo(Video.VideoId);
        }

        public void VolumeUp()
        {
            _webBrowser.Browser?.MainFrame.ExecuteJavaScript("increaseVolume();", null, 0);
        }

        public void VolumeDown()
        {
            _webBrowser.Browser?.MainFrame.ExecuteJavaScript("decreaseVolume();", null, 0);
        }

        public void ToggleMute()
        {
            _webBrowser.Browser?.MainFrame.ExecuteJavaScript("toggleMute();", null, 0);
        }

        public void Stop()
        {
            _webBrowser.Browser?.MainFrame.ExecuteJavaScript("togglePlayPause();", null, 0);
        }

        private void AddMarker()
        {
            MarkerGroup markerGroup = Repository.Default.Markers.Get(Video.VideoId);

            if (markerGroup != null && markerGroup.Markers.Count > 0)
            {
                foreach (Marker entry in markerGroup.Markers)
                {
                    try
                    {
                        _webBrowser.Browser?.MainFrame.ExecuteJavaScript($"addMarker({entry.Time.TotalSeconds}, '{entry.Id}', '{entry.Comment}');", null, 0);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(MethodBase.GetCurrentMethod(), e);
                    }
                }
            }
        }

        public void AddNote(string id, Marker entry)
        {
            if (Video.VideoId == id)
            {
                try
                {
                    _webBrowser.Browser?.MainFrame.ExecuteJavaScript($"addMarker({entry.Time.TotalSeconds}, '{entry.Id}', '{entry.Comment}');", null, 0);
                }
                catch (Exception e)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), e);
                }
            }
        }

        public void UpdateNote(string videoId, string markerId, string comment)
        {
            if (Video.VideoId == videoId)
            {
                try
                {
                    _webBrowser.Browser?.MainFrame.ExecuteJavaScript($"updateMarker('{markerId}', '{comment}');", null, 0);
                }
                catch (Exception e)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), e);
                }
            }
        }

        private void UpdateMarker(string marker, string comment, string sec)
        {
            if (sec.Contains("."))
            {
                sec = sec.Remove(sec.IndexOf(".", StringComparison.InvariantCulture));
            }
            if (sec.Contains(","))
            {
                sec = sec.Remove(sec.IndexOf(",", StringComparison.InvariantCulture));
            }

            MarkerUpdate update = new MarkerUpdate(Video.Title, Video.VideoId, marker, comment, Convert.ToInt32(sec));
            update.Owner = Application.Current.MainWindow;
            update.ShowDialog();
        }

        private void RemoveMarker(string videoId, string id)
        {
            if (Kernel.Default.View == View.Markers)
            {
                ViewRepository.Get<MarkerView>()?.removeMarker(videoId, id);
                Repository.Default.Markers.Delete(videoId, id);
            }
            else
            {
                Repository.Default.Markers.Delete(videoId, id);
                RemoveMarker(id);
            }
        }

        public void RemoveMarker(string id)
        {
            try
            {
                _webBrowser.Browser?.MainFrame.ExecuteJavaScript($"removeMarker('{id}');", null, 0);
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }
        }

        public void RemoveMarkers(string id)
        {
            if (Video.VideoId == id)
            {
                try
                {
                    _webBrowser.Browser?.MainFrame.ExecuteJavaScript("removeMarkers();", null, 0);
                }
                catch (Exception e)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), e);
                }
            }
        }

        public void CreateMarker(double arg)
        {
            Pause();

            try
            {
                int seconds = Convert.ToInt32(Math.Round(arg));

                NoteWindow window = new NoteWindow(Video, seconds, null);
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
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
            finally
            {
                Play();
            }
        }

        public void CreateMarker()
        {
            Pause();

            try
            {
                _webBrowser.EvaluateJavascript("getCurrentTime();", (e, k) =>
                {
                    if (e != null && e.IsDouble)
                    {
                        int seconds = (int)e.DoubleValue;

                        NoteWindow window = new NoteWindow(Video, seconds, null);
                        window.Owner = Application.Current.MainWindow;
                        window.ShowDialog();
                    }
                });
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
            finally
            {
                Play();
            }
        }

        public void SeekTo(int seconds)
        {
            try
            {
                _webBrowser.Browser?.MainFrame.ExecuteJavaScript($"seekTo({seconds});", null, 0);
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }
        }

        public void AddToFavorites()
        {
            if (!FavoritesCache.Default.Contains(Video.VideoId))
            {
                FavoritesCache.Default.Add(Video.VideoId);
                Repository.Default.Playlists.Insert(Video, "Favorites");
            }
        }

        public void WatchLater()
        {
            if (!WatchLaterCache.Default.Contains(Video.VideoId))
            {
                WatchLaterCache.Default.Add(Video.VideoId);
                Repository.Default.Playlists.Insert(Video, "WatchLater");
            }
        }

    }
}
