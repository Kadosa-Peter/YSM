using System;
using System.Windows;
using System.Windows.Forms;
using Chromium.WebBrowser;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Windows
{
    public partial class LoginWindow
    {
        private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";

        private bool _ = true;

        private ChromiumWebBrowser _webBrowser;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _webBrowser = new ChromiumWebBrowser(AuthenticatioHelper.GetAutenticationUri(Kernel.Default.ClientId, RedirectUri));
            _webBrowser.DisplayHandler.OnTitleChange += DisplayHandler_OnTitleChange;
            _webBrowser.Dock = DockStyle.Fill;
                                                                                                                                                                                                    
            FormHost.Child = _webBrowser;
        }

        private void DisplayHandler_OnTitleChange(object sender, Chromium.Event.CfxOnTitleChangeEventArgs e)
        {
            if (e.Title.IsNull()) return;

            if (e.Title.StartsWith("Success") && _)
            {
                _ = false;

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(Hide));

                string title = e.Title;

                string authCode = title.Remove(0, 13);

                AuthenticationService.Default.CreateUser(authCode, RedirectUri);

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(Close));
            }
            else if (e.Title.StartsWith("Denied") && _)
            {
                _ = false;

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(Hide));

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(Close));
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
