using System.Windows;
using System.Windows.Controls;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Views;

namespace Ysm.Assets.Menu
{
    public class PlayerMenu : MenuBase
    {
        public ContextMenu Get()
        {
            ExtendedContextMenuItem refresh = new ExtendedContextMenuItem();
            refresh.Title = Properties.Resources.Button_PlayerReload;            
            refresh.CommandName = "PlayerRefreshCommand";
            Menu.Items.Add(refresh);

            ExtendedContextMenuItem goBack = new ExtendedContextMenuItem();
            goBack.Title = Properties.Resources.Button_PlayerPrevious;
            goBack.CommandName = "PlayerPreviousCommand";
            goBack.Id = "D1284CC3-9E40-429A-BF80-67DF655E5E5A";
            Menu.Items.Add(goBack);

            ExtendedContextMenuItem goForward = new ExtendedContextMenuItem();
            goForward.Title = Properties.Resources.Button_PlayerNext;
            goForward.CommandName = "PlayerNextCommand";
            goForward.Id = "001AC5B5-2F81-41F2-9A01-CD6CDB603060";
            Menu.Items.Add(goForward);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem close = new ExtendedContextMenuItem();
            close.Title = Properties.Resources.Button_ClosePlayer;
            close.CommandName = "PlayerCloseCommand";
            Menu.Items.Add(close);

            ExtendedContextMenuItem closeAll = new ExtendedContextMenuItem();
            closeAll.Title = Properties.Resources.Button_CloseAllPlayers;
            closeAll.CommandName = "PlayerCloseAllCommand";
            Menu.Items.Add(closeAll);

            ExtendedContextMenuItem reopenClosed = new ExtendedContextMenuItem();
            reopenClosed.Title = Properties.Resources.Button_ReopenClosedTab;
            reopenClosed.CommandName = "PlayerReopenClosedTabCommand";
            Menu.Items.Add(reopenClosed);

            Menu.Items.Add(CreateSeparator());

         
            ExtendedContextMenuItem bookmark = new ExtendedContextMenuItem();
            bookmark.Title = Properties.Resources.Button_Marker;
            bookmark.CommandName = "PlayerBookmarkCommand";
            Menu.Items.Add(bookmark);

            ExtendedContextMenuItem favorites = new ExtendedContextMenuItem();
            favorites.Title = Properties.Resources.Button_Favorites;
            favorites.CommandName = "PlayerFavoriteCommand";
            Menu.Items.Add(favorites);

            ExtendedContextMenuItem watchLater = new ExtendedContextMenuItem();
            watchLater.Title = Properties.Resources.Button_WatchLater;
            watchLater.CommandName = "PlayerWatchLaterCommand";
            Menu.Items.Add(watchLater);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem copyText = new ExtendedContextMenuItem();
            copyText.Title = Properties.Resources.Button_CopyText;
            copyText.Id= "795962C1-30AB-4A6E-8D7F-DBADC0813DEC";
            copyText.CommandName = "PlayerCopyTextCommand";            
            Menu.Items.Add(copyText);

            ExtendedContextMenuItem searchGoogle = new ExtendedContextMenuItem();
            searchGoogle.Title = Properties.Resources.Button_SearchGoogle;
            searchGoogle.CommandName = "PlayerSearchGoogleCommand";
            searchGoogle.Id = "D7EC00B7-0DDE-45E1-8B54-0F8D26813214";
            Menu.Items.Add(searchGoogle);

            Menu.Items.Add(CreateSeparator("B7E502F4-DD13-4CE0-BA11-FAC2B4271CDA"));

            ExtendedContextMenuItem openInBrowser = new ExtendedContextMenuItem();
            openInBrowser.Id = "6ED8552A-4DBD-4F1B-B572-B9D6F71CF1DD";
            openInBrowser.Title = Properties.Resources.Button_OpenInBrowser;
            openInBrowser.CommandName = "OpenVideoInBrowserPCommand";
            Menu.Items.Add(openInBrowser);

            ExtendedContextMenuItem openChannelInBrowser = new ExtendedContextMenuItem();
            openChannelInBrowser.Title = Properties.Resources.Button_OpenChannelPage;
            openChannelInBrowser.CommandName = "OpenChannelPageInBrowserPCommand";
            Menu.Items.Add(openChannelInBrowser);

            Menu.Items.Add(CreateSeparator());

            //ExtendedContextMenuItem contnue = new ExtendedContextMenuItem();
            //contnue.Title = Properties.Resources.Button_Continue;
            //contnue.CommandName = "PlayerContinueCommand";
            //Menu.Items.Add(contnue);

            ExtendedContextMenuItem download = new ExtendedContextMenuItem();
            download.Title = Properties.Resources.Button_Download;
            download.CommandName = "PlayerDownloadVideoCommand";
            Menu.Items.Add(download);

            ExtendedContextMenuItem locate = new ExtendedContextMenuItem();
            locate.Title = Properties.Resources.Button_Locate;
            locate.CommandName = "PlayerLocateCommand";
            Menu.Items.Add(locate);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem copyUrl = new ExtendedContextMenuItem();
            copyUrl.Title = Properties.Resources.Button_CopyUrl;
            copyUrl.CommandName = "PlayerCopyUrlCommand";
            Menu.Items.Add(copyUrl);

            ExtendedContextMenuItem copyChannelUrl = new ExtendedContextMenuItem();
            copyChannelUrl.Title = Properties.Resources.Button_CopyChannelUrl;
            copyChannelUrl.CommandName = "PlayerCopyChannelUrlCommand";
            Menu.Items.Add(copyChannelUrl);

            //Menu.Items.Add(CreateSeparator());

            //ExtendedContextMenuItem openSettings = new ExtendedContextMenuItem();
            //openSettings.Title = Properties.Resources.Button_Settings;
            //openSettings.Hotkey = "CTRL + K";
            //openSettings.CommandName = "SettingsCommand";
            //Menu.Items.Add(openSettings);

            //ExtendedContextMenuItem help = new ExtendedContextMenuItem();
            //help.Title = Properties.Resources.Button_Help;
            //help.Hotkey = "F1";
            //help.CommandName = "HelpCommand";
            //Menu.Items.Add(help);

            //ExtendedContextMenuItem closeApp = new ExtendedContextMenuItem();
            //closeApp.Title = Properties.Resources.Button_Close;
            //closeApp.CommandName = "CloseCommand";
            //Menu.Items.Add(closeApp);

            SetCommands();

            return Menu;
        }

        public override void MenuOpening()
        {
            ExtendedContextMenuItem goBack = Find("D1284CC3-9E40-429A-BF80-67DF655E5E5A");
            ExtendedContextMenuItem goForward = Find("001AC5B5-2F81-41F2-9A01-CD6CDB603060");
            ExtendedContextMenuItem copyText = Find("795962C1-30AB-4A6E-8D7F-DBADC0813DEC");
            ExtendedContextMenuItem searchGoogle = Find("D7EC00B7-0DDE-45E1-8B54-0F8D26813214");


            if (ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView.History.CanGoBack == true)
            {
                goBack.IsEnabled = true;
            }
            else
            {
                goBack.IsEnabled = false;
            }

            if (ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView.History.CanGoForward == true)
            {
                goForward.IsEnabled = true;
            }
            else
            {
                goForward.IsEnabled = false;
            }

            string text = ViewRepository.Get<PlayerTabView>().
                SelectedPlayerView.
                BrowserControl.
                Description.
                SelectedText;

            if (text.IsNull())
            {
                copyText.Visibility = Visibility.Collapsed;
                searchGoogle.Visibility = Visibility.Collapsed;

                Find("B7E502F4-DD13-4CE0-BA11-FAC2B4271CDA").Visibility = Visibility.Collapsed; // Separator
            }
            else
            {
                copyText.Visibility = Visibility.Visible;
                searchGoogle.Visibility = Visibility.Visible;

                searchGoogle.Title = $"{Properties.Resources.Button_SearchGoogle} \'{text.Truncate(20)}\'";

                Find("B7E502F4-DD13-4CE0-BA11-FAC2B4271CDA").Visibility = Visibility.Visible; // Separator
            }

            // open in default browser
            ExtendedContextMenuItem openInBrowser = Find("6ED8552A-4DBD-4F1B-B572-B9D6F71CF1DD");
            string browser = DefaultBrowser.Get();
            if (browser == "Unknown") browser = "webbrowser";
            string title = Properties.Resources.Button_OpenInBrowser;
            title = title.Replace("{xy}", browser);
            openInBrowser.Title = title;
        }
    }
}
