using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Chromium.Event;
using Chromium.WebBrowser;
using Ysm.Core;

using Application = System.Windows.Application;

namespace Ysm.Windows
{
    public partial class WelcomeWindow 
    {
        private FullScreenWindow _fullScreenWindow;

        private ChromiumWebBrowser _webBrowser;

        public WelcomeWindow()
        {
            InitializeComponent();

            _webBrowser = new ChromiumWebBrowser();
            _webBrowser.Dock = DockStyle.Fill;
            _webBrowser.DisplayHandler.OnFullscreenModeChange += DisplayHandler_OnFullscreenModeChange;

            FormHost.Child = _webBrowser;
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

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WelcomeWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string template = GetTemplate();

            if (template != null)
            {
                _webBrowser.LoadString(template, "http://www.youtube.com");
            }
            else
            {
                Close();
            }

        }

        private string GetTemplate()
        {
            try
            {
                string path = Path.Combine(FileSystem.Startup, "Resources", "welcome.html");

                string html;

                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    html = reader.ReadToEnd();
                }

                return html;
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

            return null;
        }

        private void WelcomeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _webBrowser.Dispose();
        }
    }
}
