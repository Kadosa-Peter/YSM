using System.Windows.Controls;
using Ysm.Controls;
using Ysm.Core;

namespace Ysm.Assets.Menu
{
    public class ChannelMenu : MenuBase
    {
        public override void ApplicationSettingsChanged(string propertyName)
        {
            if (propertyName == "SubscriptionSort")
            {
                ExtendedContextMenuItem sortByTitle = Find("544CC825-D950-41B0-8669-A2DDDC79018E");
                ExtendedContextMenuItem sortByDate = Find("1077DC89-770E-4AD2-B3AC-B53C9C59BC45");

                // sort by title
                if (ApplicationSettings.SubscriptionSort == SortType.Title)
                {
                    sortByTitle.IsChecked = true;
                    sortByDate.IsChecked = false;
                }
                // sort by date
                else
                {
                    sortByTitle.IsChecked = false;
                    sortByDate.IsChecked = true;
                }
            }
        }

        public override void MenuOpening()
        {
            ExtendedContextMenuItem sortByTitle = Find("544CC825-D950-41B0-8669-A2DDDC79018E");
            ExtendedContextMenuItem sortByDate = Find("1077DC89-770E-4AD2-B3AC-B53C9C59BC45");

            // sort by title
            if (ApplicationSettings.SubscriptionSort == SortType.Title)
            {
                sortByTitle.IsChecked = true;
                sortByDate.IsChecked = false;
            }
            // sort by date
            else
            {
                sortByTitle.IsChecked = false;
                sortByDate.IsChecked = true;
            }
        }

        public ContextMenu Get()
        {
            ExtendedContextMenuItem createCategory = new ExtendedContextMenuItem();
            createCategory.Title = Properties.Resources.Button_CreateCategory;
            createCategory.Hotkey = "F3";
            createCategory.CommandName = "CreateCommand";
            Menu.Items.Add(createCategory);

            ExtendedContextMenuItem remove = new ExtendedContextMenuItem();
            remove.Title = Properties.Resources.Button_Remove;
            remove.Hotkey = "Del";
            remove.CommandName = "RemoveCommand";
            Menu.Items.Add(remove);

            ExtendedContextMenuItem rename = new ExtendedContextMenuItem();
            rename.Title = Properties.Resources.Button_Rename;
            rename.Hotkey = "F2";
            rename.CommandName = "RenameCommand";
            Menu.Items.Add(rename);

            ExtendedContextMenuItem colors = new ExtendedContextMenuItem();
            colors.Title = Properties.Resources.Button_Colors;
            Menu.Items.Add(colors);

            ExtendedContextMenuItem customColors = new ExtendedContextMenuItem();
            customColors.Title = Properties.Resources.Button_CustomColors;
            customColors.CommandName = "ColorCommand";
            colors.Items.Add(customColors);

            ExtendedContextMenuItem defaultColor = new ExtendedContextMenuItem();
            defaultColor.Title = Properties.Resources.Button_DefaultColor;
            defaultColor.CommandName = "DefaultColorCommand";
            colors.Items.Add(defaultColor);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem cut = new ExtendedContextMenuItem();
            cut.Title = Properties.Resources.Button_Cut;
            cut.Hotkey = "CTRL + X";
            cut.CommandName = "CutCommand";
            Menu.Items.Add(cut);

            ExtendedContextMenuItem paste = new ExtendedContextMenuItem();
            paste.Title = Properties.Resources.Button_Paste;
            paste.Hotkey = "CTRL + V";
            paste.CommandName = "PasteCommand";
            Menu.Items.Add(paste);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem mark = new ExtendedContextMenuItem();
            mark.Title = Properties.Resources.Button_Mark;
            Menu.Items.Add(mark);

            /**/ ExtendedContextMenuItem markWatched = new ExtendedContextMenuItem();
            /**/ markWatched.Title = Properties.Resources.Button_MarkWatched;
            /**/ markWatched.Hotkey = "Enter";
            /**/ markWatched.CommandName = "MarkWatchedCommand";
            /**/ mark.Items.Add(markWatched);
            /**/ 
            /**/ ExtendedContextMenuItem allWatched = new ExtendedContextMenuItem();
            /**/ allWatched.Title = Properties.Resources.Button_AllWatched;
            /**/ allWatched.Hotkey = "SHIFT + Enter";
            /**/ allWatched.CommandName = "MarkAllWatchedCommand";
            /**/ mark.Items.Add(allWatched);
            /**/ 
            /**/ mark.Items.Add(CreateSeparator());
            /**/ 
            /**/ ExtendedContextMenuItem markUnwatched = new ExtendedContextMenuItem();
            /**/ markUnwatched.Title = Properties.Resources.Button_MarkUnwatched;
            /**/ markUnwatched.CommandName = "MarkUnwatchedCommand";
            /**/ mark.Items.Add(markUnwatched);
            /**/ 
            /**/ ExtendedContextMenuItem markAllUnwatched = new ExtendedContextMenuItem();
            /**/ markAllUnwatched.Title = Properties.Resources.Button_AllUnwatched;
            /**/ markAllUnwatched.CommandName = "MarkAllUnwatchedCommand";
            /**/ mark.Items.Add(markAllUnwatched);

            ExtendedContextMenuItem downloadNewVideos = new ExtendedContextMenuItem();
            downloadNewVideos.Title = Properties.Resources.Button_DownloadNewVideos;
            downloadNewVideos.Hotkey = "F5";
            downloadNewVideos.CommandName = "DownloadNewVideosCommand";
            Menu.Items.Add(downloadNewVideos);

            ExtendedContextMenuItem downloadAllVideos = new ExtendedContextMenuItem();
            downloadAllVideos.Title = Properties.Resources.Button_DownloadAllVideos;
            downloadAllVideos.CommandName = "DownloadAllVideosCommand";
            Menu.Items.Add(downloadAllVideos);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem copyChannels = new ExtendedContextMenuItem();
            copyChannels.Title = Properties.Resources.Button_CopyChannelUrl;
            copyChannels.CommandName = "CopyChannelPageUrlCommand";
            Menu.Items.Add(copyChannels);

            ExtendedContextMenuItem copyVideos = new ExtendedContextMenuItem();
            copyVideos.Title = Properties.Resources.Button_CopyVideoUrl;
            copyVideos.CommandName = "CopyVideosPageUrlCommand";
            Menu.Items.Add(copyVideos);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem openChannels = new ExtendedContextMenuItem();
            openChannels.Id = "8b6ee790-3e8b-4af6-b976-70b6990914f5";
            openChannels.Title = Properties.Resources.Button_OpenChannelPage;
            openChannels.CommandName = "OpenChannelPageInBrowserCCommand";
            Menu.Items.Add(openChannels);

            ExtendedContextMenuItem openVideos = new ExtendedContextMenuItem();
            openVideos.Id = "61e0f2e8-5493-4b35-a250-16970440461a";
            openVideos.Title = Properties.Resources.Button_OpenVideosPage;
            openVideos.CommandName = "OpenVideosPageInBrowserCCommand";
            Menu.Items.Add(openVideos);

            Menu.Items.Add(CreateSeparator());

            ExtendedContextMenuItem sortTitle = new ExtendedContextMenuItem();
            sortTitle.Id = "544CC825-D950-41B0-8669-A2DDDC79018E";
            sortTitle.Title = Properties.Resources.Button_SortTitle;
            sortTitle.ShowCheckBox = true;
            sortTitle.Click += (s, e) => { Settings.Default.SubscriptionSort = SortType.Title; };
            Menu.Items.Add(sortTitle);

            ExtendedContextMenuItem sortDate = new ExtendedContextMenuItem();
            sortDate.Id = "1077DC89-770E-4AD2-B3AC-B53C9C59BC45";
            sortDate.Title = Properties.Resources.Button_SortDate;
            sortDate.ShowCheckBox = true;
            sortDate.Click += (s, e) => { Settings.Default.SubscriptionSort = SortType.Published; };
            Menu.Items.Add(sortDate);

            SetCommands();

            return Menu;
        }
    }
}
