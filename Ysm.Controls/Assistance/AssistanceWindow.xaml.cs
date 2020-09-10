using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Chromium.WebBrowser;
using Ysm.Core;

namespace Ysm.Controls
{
    public partial class AssistanceWindow
    {
        private readonly ChromiumWebBrowser _webBrowser;
        private readonly string _userGuide;
        private string _document;

        public AssistanceWindow(string userGuide)
        {
            InitializeComponent();

            _userGuide = userGuide;

            _webBrowser = new ChromiumWebBrowser();
            _webBrowser.LoadHandler.OnLoadEnd += LoadHandler_OnLoadEnd;
            _webBrowser.KeyboardHandler.OnPreKeyEvent += KeyboardHandler_OnPreKeyEvent;
            _webBrowser.Dock = DockStyle.Fill;
            WebBrowserHost.Child = _webBrowser;
        }

        private void LoadHandler_OnLoadEnd(object sender, Chromium.Event.CfxOnLoadEndEventArgs e)
        {
            _webBrowser.ExecuteJavascript("window.scrollTo(0, 0);");
        }

        private void KeyboardHandler_OnPreKeyEvent(object sender, Chromium.Event.CfxOnPreKeyEventEventArgs e)
        {
            if (e.Event.WindowsKeyCode == 32)
            {
                e.SetReturnValue(true);
            }
        }


        public void ShowDocument(string document)
        {
            if (document.NotNull() && document != _document)
            {
                _document = document;

                string path = Path.Combine(FileSystem.Startup, "help", "eng", document);

                path = Path.ChangeExtension(path, ".html");

                string html = ReadHtmlString(path);

                if (html.NotNull())
                {
                    _webBrowser.LoadString(html);
                }
            }
        }

        private string ReadHtmlString(string path)
        {
            try
            {
                string html;

                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    html = reader.ReadToEnd();
                }

                return html;
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return string.Empty;
        }

        private void UserGuid_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_userGuide);
        }

        private void Footer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void HeaderBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                HeaderBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
