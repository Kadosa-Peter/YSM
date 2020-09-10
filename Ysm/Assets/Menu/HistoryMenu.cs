using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Assets.Menu
{
    public class HistoryMenu : MenuBase
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

            ExtendedContextMenuItem remove = new ExtendedContextMenuItem();
            remove.Title = Properties.Resources.Button_Remove;
            remove.CommandName = "HistoryRemoveCommand";
            Menu.Items.Add(remove);

            ExtendedContextMenuItem removeAll = new ExtendedContextMenuItem();
            removeAll.Title = Properties.Resources.Button_RemoveAll;
            removeAll.CommandName = "HistoryRemoveAllCommand";
            Menu.Items.Add(removeAll);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem today = new ExtendedContextMenuItem();
            today.Id = "B1E4915E-3984-49B3-866B-90E1E22E9486";
            today.Title = Properties.Resources.Button_Today;
            today.CommandName = "HistoryTodayCommand";
            today.ShowCheckBox = true;
            Menu.Items.Add(today);

            ExtendedContextMenuItem yesterday = new ExtendedContextMenuItem();
            yesterday.Id = "BE692F54-76A5-4E8B-8473-C16F6F01E5D3";
            yesterday.Title = Properties.Resources.Button_Yesterday;
            yesterday.CommandName = "HistoryYesterdayCommand";
            yesterday.ShowCheckBox = true;
            Menu.Items.Add(yesterday);

            ExtendedContextMenuItem daybeforeyesterday = new ExtendedContextMenuItem();
            daybeforeyesterday.Id = "7199389E-8A1D-486B-B040-E4BEA43250F3";
            daybeforeyesterday.Title = Properties.Resources.Button_DayBeforeYesterday;
            daybeforeyesterday.CommandName = "HistoryDayBeforedYesterdayCommand";
            daybeforeyesterday.ShowCheckBox = true;
            Menu.Items.Add(daybeforeyesterday);

            ExtendedContextMenuItem lastSeven = new ExtendedContextMenuItem();
            lastSeven.Id = "E3AE7F2D-5501-4631-8C43-59E55F14CE33";
            lastSeven.Title = Properties.Resources.Button_LastSeven;
            lastSeven.CommandName = "HistorySevenDaysCommand";
            lastSeven.ShowCheckBox = true;
            Menu.Items.Add(lastSeven);

            ExtendedContextMenuItem LastWeek = new ExtendedContextMenuItem();
            LastWeek.Id = "F506AFCA-D362-4A84-9592-8F26C8A4767C";
            LastWeek.Title = Properties.Resources.Button_LastThirty;
            LastWeek.CommandName = "HistoryThirtyDaysCommand";
            LastWeek.ShowCheckBox = true;
            Menu.Items.Add(LastWeek);

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
            ExtendedContextMenuItem today = Find("B1E4915E-3984-49B3-866B-90E1E22E9486");
            ExtendedContextMenuItem yesterday = Find("BE692F54-76A5-4E8B-8473-C16F6F01E5D3");
            ExtendedContextMenuItem daybeforeyesterday = Find("7199389E-8A1D-486B-B040-E4BEA43250F3");
            ExtendedContextMenuItem thisWeek = Find("E3AE7F2D-5501-4631-8C43-59E55F14CE33");
            ExtendedContextMenuItem lastWeek = Find("F506AFCA-D362-4A84-9592-8F26C8A4767C");

            switch (Settings.Default.HistoryFilter)
            {
                // 0=today, 1=yesterday, 2=day before yesterday 3=Last 7 days, 4=Last 30 days
                case 0:
                    {
                        today.IsChecked = true;
                        yesterday.IsChecked = false;
                        daybeforeyesterday.IsChecked = false;
                        thisWeek.IsChecked = false;
                        lastWeek.IsChecked = false;
                    }
                    break;
                case 1:
                    {
                        today.IsChecked = false;
                        yesterday.IsChecked = true;
                        daybeforeyesterday.IsChecked = false;
                        thisWeek.IsChecked = false;
                        lastWeek.IsChecked = false;
                    }
                    break;
                case 2:
                    {
                        today.IsChecked = false;
                        yesterday.IsChecked = false;
                        daybeforeyesterday.IsChecked = true;
                        thisWeek.IsChecked = false;
                        lastWeek.IsChecked = false;
                    }
                    break;
                case 3:
                    {
                        today.IsChecked = false;
                        yesterday.IsChecked = false;
                        daybeforeyesterday.IsChecked = false;
                        thisWeek.IsChecked = true;
                        lastWeek.IsChecked = false;
                    }
                    break;
                case 4:
                {
                    today.IsChecked = false;
                    yesterday.IsChecked = false;
                    daybeforeyesterday.IsChecked = false;
                    thisWeek.IsChecked = false;
                    lastWeek.IsChecked = true;
                }
                    break;
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
