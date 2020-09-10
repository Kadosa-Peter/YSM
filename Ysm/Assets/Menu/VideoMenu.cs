using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ysm.Assets.Caches;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Assets.Menu
{
    public class VideoMenu : MenuBase
    {
        public ContextMenu Get()
        {
            ExtendedContextMenuItem open = new ExtendedContextMenuItem();
            open.Title = Properties.Resources.Button_Open;
            open.CommandName = "OpenCommand";
            Menu.Items.Add(open);

            ExtendedContextMenuItem openTab = new ExtendedContextMenuItem();
            openTab.Title = Properties.Resources.Button_OpenTab;
            openTab.CommandName = "OpenTabCommand";
            Menu.Items.Add(openTab);

            ExtendedContextMenuItem openInBrowser = new ExtendedContextMenuItem();
            openInBrowser.Id = "6ED8552A-4DBD-4F1B-B572-B9D6F71CF1DD";
            openInBrowser.Title = Properties.Resources.Button_OpenInBrowser;
            openInBrowser.CommandName = "OpenVideoInBrowserVCommand";
            Menu.Items.Add(openInBrowser);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem locate = new ExtendedContextMenuItem();
            locate.Title = Properties.Resources.Button_Locate;
            locate.CommandName = "LocateCommand";
            Menu.Items.Add(locate);

            ExtendedContextMenuItem copyUrl = new ExtendedContextMenuItem();
            copyUrl.Title = Properties.Resources.Button_CopyUrl;
            copyUrl.CommandName = "CopyVideoUrlCommand";
            Menu.Items.Add(copyUrl);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem download = new ExtendedContextMenuItem();
            download.Title = Properties.Resources.Button_Download;
            download.CommandName = "DownloadCommand";
            Menu.Items.Add(download);

            ExtendedContextMenuItem downloadAll = new ExtendedContextMenuItem();
            downloadAll.Title = Properties.Resources.Button_DownloadAll;
            downloadAll.CommandName = "DownloadAllCommand";
            Menu.Items.Add(downloadAll);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem addToFavorites = new ExtendedContextMenuItem();
            addToFavorites.Id = "CB0CB65B-D265-4668-9AB0-9007BF3BD5F8";
            addToFavorites.Title = Properties.Resources.Button_AddToFavorites;
            addToFavorites.CommandName = "AddToFavoritesCommand";
            Menu.Items.Add(addToFavorites);

            ExtendedContextMenuItem watchLater = new ExtendedContextMenuItem();
            watchLater.Id = "2C747262-D603-4868-BB02-3BCCCC5A4C5A";
            watchLater.Title = Properties.Resources.Button_WatchLater;
            watchLater.CommandName = "WatchLaterCommand";
            Menu.Items.Add(watchLater);

            ExtendedContextMenuItem playlists = new ExtendedContextMenuItem();
            playlists.Title = Properties.Resources.Button_Playlists;
            playlists.SubmenuOpened += Playlists_SubmenuOpened;
            Menu.Items.Add(playlists);

            ExtendedContextMenuItem createPlaylist = new ExtendedContextMenuItem();
            createPlaylist.Title = Properties.Resources.Button_NewPlaylist;
            createPlaylist.CommandName = "NewPlaylistCommand";
            createPlaylist.Id = "do-not-delete"; // minden egyes submenu lenyitásnál törlöm a submenu elemeit
            playlists.Items.Add(createPlaylist);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem openChannels = new ExtendedContextMenuItem();
            openChannels.Title = Properties.Resources.Button_OpenChannelPage;
            openChannels.CommandName = "OpenChannelPageInBrowserVCommand";
            Menu.Items.Add(openChannels);

            ExtendedContextMenuItem openVideos = new ExtendedContextMenuItem();
            openVideos.Title = Properties.Resources.Button_OpenVideosPage;
            openVideos.CommandName = "OpenVideosPageInBrowserVCommand";
            Menu.Items.Add(openVideos);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem showAll = new ExtendedContextMenuItem();
            showAll.Id = "C9732370-EB13-4A9F-B476-0789F30056F4";
            showAll.Title = Properties.Resources.Button_AllVideos;
            showAll.CommandName = "ShowAllVideosCommand";
            showAll.ShowCheckBox = true;
            Menu.Items.Add(showAll);

            ExtendedContextMenuItem showUnwatched = new ExtendedContextMenuItem();
            showUnwatched.Id = "3254BCF6-4B64-4B8C-B73C-B1FC1F08CFB1";
            showUnwatched.Title = Properties.Resources.Button_UnwatchedVideos;
            showUnwatched.CommandName = "ShowUnwatchedVideosCommand";
            showUnwatched.ShowCheckBox = true;
            Menu.Items.Add(showUnwatched);

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

            //ExtendedContextMenuItem close = new ExtendedContextMenuItem();
            //close.Title = Properties.Resources.Button_Close;
            //close.CommandName = "CloseCommand";
            //Menu.Items.Add(close);

            SetCommands();

            return Menu;
        }

        public override void MenuOpening()
        {
            ExtendedContextMenuItem favorites = Find("CB0CB65B-D265-4668-9AB0-9007BF3BD5F8");
            ExtendedContextMenuItem watchlater = Find("2C747262-D603-4868-BB02-3BCCCC5A4C5A");

            if (Kernel.Default.SelectedVideoItem != null)
            {
                if (FavoritesCache.Default.Contains(Kernel.Default.SelectedVideoItem.Video.VideoId))
                {
                    favorites.IsChecked = true;
                }
                else
                {
                    favorites.IsChecked = false;
                }

                if (WatchLaterCache.Default.Contains(Kernel.Default.SelectedVideoItem.Video.VideoId))
                {
                    watchlater.IsChecked = true;
                }
                else
                {
                    watchlater.IsChecked = false;
                }
            }

            // show unwatched
            if (ApplicationSettings.VideoDisplayMode == 0)
            {
                Find("3254BCF6-4B64-4B8C-B73C-B1FC1F08CFB1").IsChecked = true;
                Find("C9732370-EB13-4A9F-B476-0789F30056F4").IsChecked = false;
            }
            else
            {
                Find("3254BCF6-4B64-4B8C-B73C-B1FC1F08CFB1").IsChecked = false;
                Find("C9732370-EB13-4A9F-B476-0789F30056F4").IsChecked = true;
            }

            // open in default browser
            ExtendedContextMenuItem openInBrowser = Find("6ED8552A-4DBD-4F1B-B572-B9D6F71CF1DD");
            string browser = DefaultBrowser.Get();
            if (browser == "Unknown") browser = "webbrowser";
            string title = Properties.Resources.Button_OpenInBrowser;
            title = title.Replace("{xy}", browser);
            openInBrowser.Title = title;
        }

        private void Playlists_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (sender is ExtendedContextMenuItem parent)
            {
                Clear();

                List<Playlist> playLists = Repository.Default.Playlists.GetAll();

                if (playLists.Any(x => x.Default == false))
                    parent.Items.Insert(0, CreateSeparator());

                foreach (Playlist playlist in playLists.OrderByDescending(x => x.Name))
                {
                    if (playlist.Default) continue;

                    ExtendedContextMenuItem item = new ExtendedContextMenuItem();
                    item.Title = playlist.Name;
                    item.CommandName = "AddPlaylistCommand";
                    item.CommandParameter = playlist.Id;
                    parent.Items.Insert(0, item);

                    SetCommand(item);
                }
            }

            void Clear()
            {
                List<ExtendedContextMenuItem> items = new List<ExtendedContextMenuItem>();

                foreach (ExtendedContextMenuItem item in parent.Items)
                {
                    if (item.Id == null)
                    {
                        items.Add(item);
                    }
                }

                foreach (ExtendedContextMenuItem item in items)
                {
                    parent.Items.Remove(item);
                }
            }
        }
    }
}
