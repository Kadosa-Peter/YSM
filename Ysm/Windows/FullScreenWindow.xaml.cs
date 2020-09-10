using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Chromium;
using Chromium.WebBrowser;
using Ysm.Core;
using Ysm.Core.KeyboadrHook;

namespace Ysm.Windows
{
    public partial class FullScreenWindow
    {
        private readonly ChromiumWebBrowser _webBrowser;

        private KeyboardHook _keybodarHook;

        public FullScreenWindow(ChromiumWebBrowser webBrowser)
        {
            InitializeComponent();

            _webBrowser = webBrowser;

            _keybodarHook = new KeyboardHook();
            _keybodarHook.KeyDown+= KeybodarHook_KeyDown;
        }

        private void KeybodarHook_KeyDown(object sender, KeyHookEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseFullScreen();
            }
            else if (e.Key == Key.Space)
            {
                _webBrowser.Browser?.MainFrame.ExecuteJavaScript("togglePlayPause();", null, 0);
            }
        }

        private void FullScreenWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            CurrentScreen cs = ScreenHelper.GetScreen(Application.Current.MainWindow);
            Left = cs.DeviceBounds.Left;
            WindowState = WindowState.Maximized;

            Add();
        }

        private void FullScreenWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _keybodarHook.Dispose();
            _keybodarHook = null;

            Remove();
        }

        private void Add()
        {
            FormsHost.Child = _webBrowser;

            Task.Delay(100).GetAwaiter().OnCompleted(() =>
            {
                _webBrowser.Focus();

            });
        }

        private void Remove()
        {
            FormsHost.Child = null;
        }

        public void CloseFullScreen()
        {
            List<long> identifiers = _webBrowser.Browser.FrameIdentifiers.ToList();

            foreach (long identifier in identifiers)
            {
                CfxFrame frame = _webBrowser.Browser.GetFrame(identifier);

                string url = frame.Url;

                if (url.StartsWith("https://www.youtube.com"))
                {
                    frame.ExecuteJavaScript("var y=document.getElementsByClassName('ytp-fullscreen-button')[0]; y.click();", null, 0);
                }
            }
        }

      
    }
}
