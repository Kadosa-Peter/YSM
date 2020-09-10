using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Assets.Menu
{
    public class FavoritesMenu : MenuBase
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


            ExtendedContextMenuItem remove = new ExtendedContextMenuItem();
            remove.Title = Properties.Resources.Button_Remove;
            remove.CommandName = "FavoritesRemoveCommand";
            Menu.Items.Add(remove);

            Menu.Items.Add(CreateSeparator());

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

            ExtendedContextMenuItem locate = new ExtendedContextMenuItem();
            locate.Title = Properties.Resources.Button_Locate;
            locate.CommandName = "LocateCommand";
            Menu.Items.Add(locate);

            ExtendedContextMenuItem download = new ExtendedContextMenuItem();
            download.Title = Properties.Resources.Button_Download;
            download.CommandName = "DownloadCommand";
            Menu.Items.Add(download);

            ExtendedContextMenuItem downloadAll = new ExtendedContextMenuItem();
            downloadAll.Title = Properties.Resources.Button_DownloadAll;
            downloadAll.CommandName = "DownloadAllCommand";
            Menu.Items.Add(downloadAll);

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
