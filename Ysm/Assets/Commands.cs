using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using Ysm.Actions;
using Ysm.Assets.Caches;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Core.Ioc;
using Ysm.Core.RelayCommand;
using Ysm.Data;
using Ysm.Models;
using Ysm.Properties;
using Ysm.Views;

namespace Ysm.Assets
{
    public class Commands
    {
        #region YSM

        public RelayCommand LoginCommand { get; }
        public RelayCommand LogoutCommand { get; }
        public RelayCommand AboutCommand { get; }
        public RelayCommand LicenceCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand SettingsCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand FeedbackCommand { get; }
        public RelayCommand ImportCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand UserGuideCommand { get; }
        public RelayCommand TutorialCommand { get; }

        #endregion

        #region Mark Watched & Unwatched

        public RelayCommand MarkWatchedCommand { get; }
        public RelayCommand MarkAllWatchedCommand { get; }
        public RelayCommand MarkAsWatchedCommand { get; }
        public RelayCommand MarkAsUnwatchedCommand { get; }

        public RelayCommand MarkUnwatchedCommand { get; }
        public RelayCommand MarkAllUnwatchedCommand { get; }

        public RelayCommand IterationsCommand { get; }

        #endregion

        #region Download New Subscriptions & Videos

        public RelayCommand DownloadNewSubscriptionsCommand { get; }
        public RelayCommand DownloadAllVideosCommand { get; }
        public RelayCommand DownloadNewVideosCommand { get; }

        #endregion

        #region Edit

        public RelayCommand RemoveCommand { get; }
        public RelayCommand CreateCommand { get; }
        public RelayCommand RenameCommand { get; }
        public RelayCommand CutCommand { get; }
        public RelayCommand PasteCommand { get; }
        public RelayCommand ColorCommand { get; }
        public RelayCommand DefaultColorCommand { get; }
        public RelayCommand<object> AddPlaylistCommand { get; }
        public RelayCommand CopyVideoUrlCommand { get; }
        public RelayCommand CopyVideosPageUrlCommand { get; }
        public RelayCommand CopyChannelPageUrlCommand { get; }
        public RelayCommand ChannelSortTitleCommand { get; }
        public RelayCommand ChannelSortDateCommand { get; }
        public RelayCommand DownloadCommand { get; }
        public RelayCommand DownloadAllCommand { get; }
        public RelayCommand LocateCommand { get; }

        #endregion

        #region Favorites

        public RelayCommand AddToFavoritesCommand { get; }
        public RelayCommand FavoritesRemoveAllCommand { get; }
        public RelayCommand FavoritesRemoveChannelCommand { get; }
        public RelayCommand FavoritesRemoveCommand { get; }

        #endregion

        #region WatchLater

        public RelayCommand WatchLaterCommand { get; }
        public RelayCommand WatchLaterRemoveAllCommand { get; }
        public RelayCommand WatchLaterRemoveChannelCommand { get; }
        public RelayCommand WatchLaterRemoveCommand { get; }

        #endregion

        #region Playlists

        public RelayCommand PlaylistRemoveVideoCommand { get; }
        public RelayCommand NewPlaylistCommand { get; }

        #endregion

        #region History

        public RelayCommand HistoryRemoveAllCommand { get; }
        public RelayCommand HistoryRemoveCommand { get; }
        public RelayCommand HistoryTodayCommand { get; }
        public RelayCommand HistoryYesterdayCommand { get; }
        public RelayCommand HistoryDayBeforedYesterdayCommand { get; }
        public RelayCommand HistorySevenDaysCommand { get; }
        public RelayCommand HistoryThirtyDaysCommand { get; }

        #endregion

        #region Views

        public RelayCommand SubscriptionViewCommand { get; }
        public RelayCommand FavoritesViewCommand { get; }
        public RelayCommand WatchLateViewCommand { get; }
        public RelayCommand HistoryViewCommand { get; }
        public RelayCommand PlaylistViewCommand { get; }
        public RelayCommand BookmarksViewCommand { get; }

        #endregion

        #region Subscriptions & Videos View State

        public RelayCommand ShowAllSubscriptionsCommand { get; }
        public RelayCommand ShowSubscriptionsWithVideoCommand { get; }
        public RelayCommand ShowAllVideosCommand { get; }
        public RelayCommand ShowUnwatchedVideosCommand { get; }

        #endregion

        #region Player

        public RelayCommand PlayerRefreshCommand { get; }
        public RelayCommand PlayerPreviousCommand { get; }
        public RelayCommand PlayerNextCommand { get; }
        public RelayCommand PlayerCloseCommand { get; }
        public RelayCommand PlayerCloseAllCommand { get; }
        public RelayCommand PlayerReopenClosedTabCommand { get; }
        public RelayCommand PlayerContinueCommand { get; }
        public RelayCommand PlayerDownloadVideoCommand { get; }
        public RelayCommand PlayerLocateCommand { get; }
        public RelayCommand PlayerCopyUrlCommand { get; }
        public RelayCommand PlayerCopyChannelUrlCommand { get; }
        public RelayCommand PlayerCopyTextCommand { get; }
        public RelayCommand PlayerSearchGoogleCommand { get; }
        public RelayCommand PlayerWatchLaterCommand { get; }
        public RelayCommand PlayerFavoriteCommand { get; }
        public RelayCommand PlayerBookmarkCommand { get; }

        #endregion

        #region Open

        // Outside 
        public RelayCommand OpenChannelPageInBrowserCCommand { get; }
        public RelayCommand OpenChannelPageInBrowserVCommand { get; }
        public RelayCommand OpenChannelPageInBrowserPCommand { get; }

        public RelayCommand OpenVideosPageInBrowserCCommand { get; }
        public RelayCommand OpenVideosPageInBrowserVCommand { get; }

        public RelayCommand OpenVideoInBrowserVCommand { get; }
        public RelayCommand OpenVideoInBrowserPCommand { get; }

        // Inside
        public RelayCommand OpenCommand { get; }
        public RelayCommand OpenTabCommand { get; }

        #endregion

        public Commands()
        {
            // YSM
            LoginCommand = new RelayCommand(DoLogin, CanLogin);
            LogoutCommand = new RelayCommand(DoLogout, CanLogout);
            AboutCommand = new RelayCommand(DoAbout, CanAbout);
            LicenceCommand = new RelayCommand(DoLicence, CanLicence);
            CloseCommand = new RelayCommand(DoClose, CanClose);
            SettingsCommand = new RelayCommand(DoSettings, CanSettings);
            UpdateCommand = new RelayCommand(DoUpdate, CanUpdate);
            FeedbackCommand = new RelayCommand(DoFeedback, CanFeedback);
            ImportCommand = new RelayCommand(DoImport, CanImport);
            ExportCommand = new RelayCommand(DoExport, CanExport);
            HelpCommand = new RelayCommand(DoHelp, CanHelp);
            UserGuideCommand = new RelayCommand(DoUserGuide, CanUserGuide);
            TutorialCommand = new RelayCommand(DoTutorial, CanTutorial);

            // Mark Watched & Unwatched
            MarkWatchedCommand = new RelayCommand(DoMarkWatched, CanMarkWatched);
            MarkAllWatchedCommand = new RelayCommand(DoMarkAllWatched, CanMarkAllWatched);
            MarkAsWatchedCommand = new RelayCommand(DoMarkAsWatched, CanMarkAsWatched);
            MarkAsUnwatchedCommand = new RelayCommand(DoMarkAsUnwatched, CanMarkAsUnwatched);
            MarkUnwatchedCommand = new RelayCommand(DoMarkUnwatched, CanMarkUnwatched);
            MarkAllUnwatchedCommand = new RelayCommand(DoMarkAllUnwatched, CanMarkAllUnwatched);
            IterationsCommand = new RelayCommand(DoIterations, CanIterations);


            // Download New Subscriptions & Videos
            DownloadNewSubscriptionsCommand = new RelayCommand(DoDownloadNewSubscriptions, CanDownloadNewSubscriptions);
            DownloadAllVideosCommand = new RelayCommand(DoDownloadAllVideos, CanDownloadAllVideos);
            DownloadNewVideosCommand = new RelayCommand(DoDownloadNewVideos, CanDownloadNewVideos);

            // Edit
            RemoveCommand = new RelayCommand(DoRemove, CanRemove);
            CreateCommand = new RelayCommand(DoCreate, CanCreate);
            RenameCommand = new RelayCommand(DoRename, CanRename);
            CutCommand = new RelayCommand(DoCut, CanCut);
            PasteCommand = new RelayCommand(DoPaste, CanPaste);
            ColorCommand = new RelayCommand(DoColor, CanColor);
            DefaultColorCommand = new RelayCommand(DoDefaultColor, CanDefaultColor);
            AddPlaylistCommand = new RelayCommand<object>(DoAddPlaylist, CanAddPlaylist);
            CopyVideoUrlCommand = new RelayCommand(DoCopyVideoUrl, CanCopyVideoUrl);
            CopyVideosPageUrlCommand = new RelayCommand(DoCopyVideosPageUrl, CanCopyVideosPageUrl);
            CopyChannelPageUrlCommand = new RelayCommand(DoCopyChannelPageUrl, CanCopyChannelPageUrl);
            ChannelSortTitleCommand = new RelayCommand(DoChannelSortTitle, CanChannelSortTitle);
            ChannelSortDateCommand = new RelayCommand(DoChannelSortDate, CanChannelSortDate);
            DownloadCommand = new RelayCommand(DoDownload, CanDownload);
            DownloadAllCommand = new RelayCommand(DoDownloadAll, CanDownloadAll);
            LocateCommand = new RelayCommand(DoLocate, CanLocate);

            // Favorites
            AddToFavoritesCommand = new RelayCommand(DoAddToFavorites, CanAddToFavorites);
            FavoritesRemoveAllCommand = new RelayCommand(DoFavoritesRemoveAll, CanFavoritesRemoveAll);
            FavoritesRemoveChannelCommand = new RelayCommand(DoFavoritesRemoveChannel, CanFavoritesRemoveChannel);
            FavoritesRemoveCommand = new RelayCommand(DoFavoritesRemove, CanFavoritesRemove);

            // WatchLater
            WatchLaterCommand = new RelayCommand(DoWatchLater, CanWatchLater);
            WatchLaterRemoveAllCommand = new RelayCommand(DoWatchLaterRemoveAll, CanWatchLaterRemoveAll);
            WatchLaterRemoveChannelCommand = new RelayCommand(DoWatchLaterRemoveChannel, CanWatchLaterRemoveChannel);
            WatchLaterRemoveCommand = new RelayCommand(DoWatchLaterRemove, CanWatchLaterRemove);

            // Playlists
            PlaylistRemoveVideoCommand = new RelayCommand(DoPlaylistRemoveVideo, CanPlaylistRemoveVideo);
            NewPlaylistCommand = new RelayCommand(DoNewPlaylist, CanNewPlaylist);


            // History
            HistoryRemoveAllCommand = new RelayCommand(DoHistoryRemoveAll, CanHistoryRemoveAll);
            HistoryRemoveCommand = new RelayCommand(DoHistoryRemove, CanHistoryRemove);
            HistoryTodayCommand = new RelayCommand(DoHistoryToday, CanHistoryToday);
            HistoryYesterdayCommand = new RelayCommand(DoHistoryYesterday, CanHistoryYesterday);
            HistoryDayBeforedYesterdayCommand = new RelayCommand(DoHistoryDayBeforedYesterday, CanHistoryDayBeforedYesterday);
            HistorySevenDaysCommand = new RelayCommand(DoHistorySevenDays, CanHistorySevenDays);
            HistoryThirtyDaysCommand = new RelayCommand(DoHistoryThirtyDays, CanHistoryThirtyDays);

            // Views
            SubscriptionViewCommand = new RelayCommand(DoSubscriptionView, CanSubscriptionView);
            FavoritesViewCommand = new RelayCommand(DoFavoritesView, CanFavoritesView);
            WatchLateViewCommand = new RelayCommand(DoWatchLateView, CanWatchLateView);
            HistoryViewCommand = new RelayCommand(DoHistoryView, CanHistoryView);
            PlaylistViewCommand = new RelayCommand(DoPlaylistView, CanPlaylistView);
            BookmarksViewCommand = new RelayCommand(DoBookmarksView, CanBookmarksView);

            //  Subscriptions & Videos View State
            ShowAllVideosCommand = new RelayCommand(DoShowAllVideos, CanShowAllVideos);
            ShowUnwatchedVideosCommand = new RelayCommand(DoShowUnwatchedVideos, CanShowUnwatchedVideos);
            ShowAllSubscriptionsCommand = new RelayCommand(DoShowAllSubscriptions, CanShowAllSubscriptions);
            ShowSubscriptionsWithVideoCommand = new RelayCommand(DoShowSubscriptionsWithVideo, CanShowSubscriptionsWithVideo);

            // Player
            PlayerRefreshCommand = new RelayCommand(DoPlayerRefresh, CanPlayerRefresh);
            PlayerPreviousCommand = new RelayCommand(DoPlayerPrevious, CanPlayerPrevious);
            PlayerNextCommand = new RelayCommand(DoPlayerNext, CanPlayerNext);
            PlayerCloseCommand = new RelayCommand(DoPlayerClose, CanPlayerClose);
            PlayerCloseAllCommand = new RelayCommand(DoPlayerCloseAll, CanPlayerCloseAll);
            PlayerReopenClosedTabCommand = new RelayCommand(DoPlayerReopenClosedTab, CanPlayerReopenClosedTab);
            PlayerContinueCommand = new RelayCommand(DoPlayerContinue, CanPlayerContinue);
            PlayerDownloadVideoCommand = new RelayCommand(DoPlayerDownloadVideo, CanPlayerDownloadVideo);
            PlayerLocateCommand = new RelayCommand(DoPlayerLocate, CanPlayerLocate);
            PlayerCopyUrlCommand = new RelayCommand(DoPlayerCopyUrl, CanPlayerCopyUrl);
            PlayerCopyChannelUrlCommand = new RelayCommand(DoPlayerCopyChannelUrl, CanPlayerCopyChannelUrl);
            PlayerCopyTextCommand = new RelayCommand(DoPlayerCopyText, CanPlayerCopyText);
            PlayerSearchGoogleCommand = new RelayCommand(DoPlayerSearchGoogle, CanPlayerSearchGoogle);
            PlayerFavoriteCommand = new RelayCommand(DoPlayerFavorite, CanPlayerFavorite);
            PlayerWatchLaterCommand = new RelayCommand(DoPlayerWatchLater, CanPlayerWatchLater);
            PlayerBookmarkCommand = new RelayCommand(DoPlayerBookmark, CanPlayerBookmark);

            // outside
            OpenChannelPageInBrowserCCommand = new RelayCommand(DoOpenChannelPageInBrowserC, CanOpenChannelPageInBrowserC);
            OpenChannelPageInBrowserVCommand = new RelayCommand(DoOpenChannelPageInBrowserV, CanOpenChannelPageInBrowserV);
            OpenChannelPageInBrowserPCommand = new RelayCommand(DoOpenChannelPageInBrowserP, CanOpenChannelPageInBrowserP);

            OpenVideosPageInBrowserCCommand = new RelayCommand(DoOpenVideosPageInBrowserC, CanOpenVideosPageInBrowserC);
            OpenVideosPageInBrowserVCommand = new RelayCommand(DoOpenVideosPageInBrowserV, CanOpenVideosPageInBrowserV);

            OpenVideoInBrowserVCommand = new RelayCommand(DoOpenVideoInBrowserV, CanOpenVideoInBrowserV);
            OpenVideoInBrowserPCommand = new RelayCommand(DoOpenVideoInBrowserP, CanOpenVideoInBrowserP);

            // inside
            OpenCommand = new RelayCommand(DoOpen, CanOpen);
            OpenTabCommand = new RelayCommand(DoOpenTab, CanOpenTab);
        }

        #region YSM

        private void DoLogin()
        {
            Dialogs.OpenLoginWindow();
        }
        private bool CanLogin()
        {
            return true;
        }

        private void DoLogout()
        {
            AuthenticationService.Default.Logout();

            VideoServiceWrapper.Default.Cancel();
            SubscriptionServiceWrapper.Default.Cancel();
        }
        private bool CanLogout()
        {
            return true;
        }

        private void DoAbout()
        {
            IAction action = ActionRepository.Find("OpenAbout");
            action?.Execute(null);
        }
        private bool CanAbout()
        {
            return true;
        }

        private void DoLicence()
        {
            IAction action = ActionRepository.Find("Licence");
            action?.Execute(null);
        }
        private bool CanLicence()
        {
            return true;
        }

        private void DoClose()
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
        }
        private bool CanClose()
        {
            return true;
        }

        private void DoSettings()
        {
            IAction action = ActionRepository.Find("OpenSettings");
            action?.Execute(null);
        }
        private bool CanSettings()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoUpdate()
        {
            IAction action = ActionRepository.Find("Update");
            action?.Execute(true);
        }
        private bool CanUpdate()
        {
            return true;
        }

        private void DoFeedback()
        {
            IAction action = ActionRepository.Find("OpenFeedback");
            action?.Execute(null);
        }
        private bool CanFeedback()
        {
            return true;
        }

        private void DoImport()
        {
            IAction action = ActionRepository.Find("ImportAction");
            action?.Execute(null);
        }
        private bool CanImport()
        {
            return true;
        }

        private void DoExport()
        {
            IAction action = ActionRepository.Find("ExportAction");
            action?.Execute(null);
        }
        private bool CanExport()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoHelp()
        {
            if (SimpleIoc.Default.IsRegistered<AssistanceHelper>())
            {
                SimpleIoc.Default.GetInstance<AssistanceHelper>()?.CreateWindow();
            }
        }
        private bool CanHelp()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoUserGuide()
        {
            Process.Start(Kernel.Default.UserGuide);
        }
        private bool CanUserGuide()
        {
            return true;
        }

        private void DoTutorial()
        {
            try
            {
                Process.Start(Kernel.Default.Tutorial);
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
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }
        private bool CanTutorial()
        {
            return true;
        }

        #endregion

        #region Mark Watched & Unwatched

        private void DoMarkWatched()
        {
            ViewRepository.Get<ChannelView>().MarkWatched();
        }
        private bool CanMarkWatched()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.SelectedChannels?.Count > 0;
        }

        private void DoMarkAllWatched()
        {
            ViewRepository.Get<ChannelView>().MarkAllWatched();
        }
        private bool CanMarkAllWatched()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View != View.Subscriptions)
                return false;

            return true;
        }

        private void DoMarkUnwatched()
        {
            ViewRepository.Get<ChannelView>()?.MarkUnwatched();
        }
        private bool CanMarkUnwatched()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.SelectedChannels?.Count > 0;
        }

        private void DoMarkAllUnwatched()
        {
            ViewRepository.Get<ChannelView>()?.MarkAllUnwatched();
        }
        private bool CanMarkAllUnwatched()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoIterations()
        {
            Kernel.Default.View = View.Subscriptions;
            Dialogs.OpenIterationsWindow();
        }
        private bool CanIterations()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoMarkAsWatched()
        {
            Kernel.Default.View = View.Subscriptions;
            Dialogs.OpenWatchedWindow();
        }
        private bool CanMarkAsWatched()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoMarkAsUnwatched()
        {
            Kernel.Default.View = View.Subscriptions;
            Dialogs.OpenUnwatchedWindow();
        }
        private bool CanMarkAsUnwatched()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        #endregion

        #region Download New Subscriptions & Videos

        private void DoDownloadNewSubscriptions()
        {
            IAction action = ActionRepository.Find("SubscriptionService");
            action?.Execute(null);
        }
        private bool CanDownloadNewSubscriptions()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.SubscriptionService == false
                   && Kernel.Default.VideoService == false;
        }

        private void DoDownloadAllVideos()
        {
            List<string> channels = new List<string>();

            Kernel.Default.SelectedChannels.CopyToList(channels);

            if (channels.Count > 0)
            {
                IAction action = ActionRepository.Find("VideoService");
                action.Execute(channels);
            }
        }
        private bool CanDownloadAllVideos()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.SubscriptionService == false
                   && Kernel.Default.VideoService == false
                   && Kernel.Default.SelectedChannels?.Count > 0;
        }

        private void DoDownloadNewVideos()
        {
            IAction action = ActionRepository.Find("VideoService");
            action?.Execute(null);
        }
        private bool CanDownloadNewVideos()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View != View.Subscriptions)
                return false;

            return Kernel.Default.SubscriptionService == false
                   && Kernel.Default.VideoService == false;
        }

        #endregion

        #region Edit
        private void DoRemove()
        {
            if (Kernel.Default.View == View.Playlists)
                ViewRepository.Get<PlaylistView>().Delete();
            else if (Kernel.Default.View == View.Subscriptions)
                ViewRepository.Get<ChannelView>().Delete();
            else if (Kernel.Default.View == View.Markers)
                ViewRepository.Get<MarkerView>().Remove();
        }
        private bool CanRemove()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.History
                || Kernel.Default.View == View.Favorites
                || Kernel.Default.View == View.WatchLater)
                return false;

            if (Kernel.Default.View == View.Markers)
                return ViewRepository.Get<MarkerView>().CanRemove();

            if (Kernel.Default.View == View.Playlists)
                return ViewRepository.Get<PlaylistView>().CanRemove();

            return Kernel.Default.CanDelete;
        }

        private void DoCreate()
        {
            if (Kernel.Default.View == View.Subscriptions)
            {
                ViewRepository.Get<ChannelView>().CreateCategory();
            }
            else if (Kernel.Default.View == View.Playlists)
            {
                ViewRepository.Get<PlaylistView>().CreatePlaylist();
            }
        }
        private bool CanCreate()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.History
                || Kernel.Default.View == View.Favorites
                || Kernel.Default.View == View.WatchLater
                || Kernel.Default.View == View.Markers)
                return false;

            if (Kernel.Default.Search)
                return false;

            return true;
        }

        private void DoRename()
        {
            if (Kernel.Default.View == View.Subscriptions)
            {
                ViewRepository.Get<ChannelView>().Rename();
            }
            else if (Kernel.Default.View == View.Playlists)
            {
                ViewRepository.Get<PlaylistView>().Rename();
            }
            else if (Kernel.Default.View == View.Markers)
            {
                ViewRepository.Get<MarkerView>().Edit();
            }


        }
        private bool CanRename()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.History
                || Kernel.Default.View == View.Favorites
                || Kernel.Default.View == View.WatchLater)
                return false;

            if (Kernel.Default.Search)
                return false;

            if (Kernel.Default.View == View.Markers)
                return ViewRepository.Get<MarkerView>().CanEdit();

            if (Kernel.Default.View == View.Playlists)
                return ViewRepository.Get<PlaylistView>().CanEdit();

            return Kernel.Default.CanRename;
        }

        private void DoCut()
        {
            ViewRepository.Get<ChannelView>().Cut();
        }
        private bool CanCut()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View != View.Subscriptions)
                return false;

            //if (Kernel.Default.Search)
            //    return false;

            return Kernel.Default.CanCut;
        }

        private void DoPaste()
        {
            ViewRepository.Get<ChannelView>().Paste();
        }
        private bool CanPaste()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View != View.Subscriptions)
                return false;

            if (Kernel.Default.Search)
                return false;

            return Kernel.Default.CanPaste;
        }

        private void DoColor()
        {
            if (Kernel.Default.View == View.Subscriptions)
            {
                ViewRepository.Get<ChannelView>().ChangeColor();
            }
            else if (Kernel.Default.View == View.Playlists)
            {
                ViewRepository.Get<PlaylistView>().ChangeColor();
            }
        }
        private bool CanColor()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Subscriptions)
            {
                return ViewRepository.Get<ChannelView>().
                    SelectedNodes.
                    Cast<Node>().
                    Any(x => x.NodeType == NodeType.Category);
            }

            if (Kernel.Default.View == View.Playlists)
            {
                return ViewRepository.Get<PlaylistView>().
                    SelectedNodes.
                    Cast<Node>().
                    Any(x => x.NodeType == NodeType.Playlist);
            }

            return false;
        }

        private void DoDefaultColor()
        {
            if (Kernel.Default.View == View.Subscriptions)
            {
                ViewRepository.Get<ChannelView>().DefaultColor();
            }
            else if (Kernel.Default.View == View.Playlists)
            {
                ViewRepository.Get<PlaylistView>().DefaultColor();
            }
        }
        private bool CanDefaultColor()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Subscriptions)
            {
                return ViewRepository.Get<ChannelView>().
                    SelectedNodes.
                    Cast<Node>().
                    Any(x => x.NodeType == NodeType.Category);
            }

            if (Kernel.Default.View == View.Playlists)
            {
                return ViewRepository.Get<PlaylistView>().
                    SelectedNodes.
                    Cast<Node>().
                    Any(x => x.NodeType == NodeType.Playlist);
            }

            return false;
        }

        private void DoAddPlaylist(object parameter)
        {
            string playlistId = parameter.ToString();

            Video video = Kernel.Default.SelectedVideoItem.Video;

            Playlist playlist = Repository.Default.Playlists.Get(playlistId);

            if (playlist == null)
            {
                Repository.Default.Playlists.Insert(video, playlistId);

                if (Kernel.Default.View == View.Playlists)
                {
                    ViewRepository.Get<PlaylistView>().Video_Added(video, playlistId);

                    ViewRepository.Get<MainViewHost>().ShowNotifyLayer(Resources.Title_VideoAddedPlaylist);
                }
            }
            else
            {
                if (playlist.Videos.Any(x => x.VideoId == video.VideoId))
                {
                    string message = Resources.Title_PlaylistContainsVideo;
                    message = message.Replace("_XY_", playlist.Name);

                    Dialogs.OpenInfo(Messages.PlaylistContainsVideo(message, video.Title));
                }
                else
                {
                    Repository.Default.Playlists.Insert(video, playlistId);

                    if (Kernel.Default.View == View.Playlists)
                        ViewRepository.Get<PlaylistView>().Video_Added(video, playlistId);

                    string text = Resources.Text_VideoAddedPlaylist;
                    text = text.Replace("xy", playlist.Name);

                    ViewRepository.Get<MainViewHost>().ShowNotifyLayer(text);
                }
            }
        }
        private bool CanAddPlaylist(object parameter)
        {
            return true;
        }

        private void DoCopyVideoUrl()
        {
            Clipboard.SetText(UrlHelper.GetVideoUrl(Kernel.Default.SelectedVideoItem.Id));
        }
        private bool CanCopyVideoUrl()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.SelectedVideoItem?.Video.Link != null;
        }

        private void DoCopyVideosPageUrl()
        {
            ViewRepository.Get<ChannelView>().CopyChannelPageUrls();
        }
        private bool CanCopyVideosPageUrl()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.SelectedChannels != null)
                return Kernel.Default.SelectedChannels.Any();

            return false;
        }

        private void DoCopyChannelPageUrl()
        {
            ViewRepository.Get<ChannelView>().CopyChannelPageUrls();
        }
        private bool CanCopyChannelPageUrl()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.SelectedChannels != null)
                return Kernel.Default.SelectedChannels.Any();

            return false;
        }

        private void DoChannelSortTitle()
        {
            Settings.Default.SubscriptionSort = SortType.Title;
        }
        private bool CanChannelSortTitle()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoChannelSortDate()
        {
            Settings.Default.SubscriptionSort = SortType.Published;
        }
        private bool CanChannelSortDate()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoDownload()
        {
            IAction action = ActionRepository.Find("DownloadVideo");
            action?.Execute(null);
        }
        private bool CanDownload()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoDownloadAll()
        {
            List<string> ids = ViewRepository.Get<VideoView>().Videos.Select(x => x.Id).ToList();

            IAction action = ActionRepository.Find("DownloadAllVideos");
            action?.Execute(ids);
        }
        private bool CanDownloadAll()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoLocate()
        {
            Video video = Kernel.Default.SelectedVideoItem.Video;
            ViewRepository.Get<ChannelView>().Locate(video.ChannelId, video.VideoId);
        }
        private bool CanLocate()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.SelectedVideoItem != null;
        }

        #endregion

        #region Favorites

        private void DoAddToFavorites()
        {
            ViewRepository.Get<VideoView>().AddToFavorites();
        }
        private bool CanAddToFavorites()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoFavoritesRemoveAll()
        {
            if (Dialogs.OpenDialog(Resources.Question_Favorites_RemoveAll))
            {
                ViewRepository.Get<FavoritesView>().RemoveAll();
                ViewRepository.Get<VideoView>().Videos.Clear();

                FavoritesCache.Default.RemoveAll();
                Repository.Default.Playlists.RemoveAll("Favorites");
            }
        }
        private bool CanFavoritesRemoveAll()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoFavoritesRemoveChannel()
        {
            ViewRepository.Get<FavoritesView>().RemoveChannel();
        }
        private bool CanFavoritesRemoveChannel()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return ViewRepository.Get<FavoritesView>().SelectedNodes.Cast<Node>()
                .Any(x => x.NodeType == NodeType.Channel);
        }

        private void DoFavoritesRemove()
        {
            if (Kernel.Default.SelectedVideoItem != null)
            {
                Video video = Kernel.Default.SelectedVideoItem.Video;

                //ViewRepository.Get<FavoritesView>().Remove(video.ChannelId);
                ViewRepository.Get<VideoView>().Remove(video.VideoId);

                FavoritesCache.Default.Remove(video.VideoId);
                Repository.Default.Playlists.Remove(video.VideoId, "Favorites");
            }
        }
        private bool CanFavoritesRemove()
        {
            return Kernel.Default.SelectedVideoItem != null;
        }

        #endregion

        #region WatchLater

        private void DoWatchLater()
        {
            ViewRepository.Get<VideoView>().AddToWatchLater();
        }
        private bool CanWatchLater()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return true;
        }

        private void DoWatchLaterRemoveAll()
        {
            if (Dialogs.OpenDialog(Resources.Question_WatchLater_RemoveAll))
            {
                ViewRepository.Get<WatchLaterView>().RemoveAll();
                ViewRepository.Get<VideoView>().Videos.Clear();

                WatchLaterCache.Default.RemoveAll();
                Repository.Default.Playlists.RemoveAll("WatchLater");
            }
        }
        private bool CanWatchLaterRemoveAll()
        {
            return true;
        }

        private void DoWatchLaterRemoveChannel()
        {
            ViewRepository.Get<WatchLaterView>().RemoveChannel();
        }
        private bool CanWatchLaterRemoveChannel()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return ViewRepository.Get<WatchLaterView>().SelectedNodes.Cast<Node>()
                .Any(x => x.NodeType == NodeType.Channel);
        }

        private void DoWatchLaterRemove()
        {
            if (Kernel.Default.SelectedVideoItem != null)
            {
                Video video = Kernel.Default.SelectedVideoItem.Video;

                //ViewRepository.Get<WatchLaterView>().Remove(video.ChannelId);
                ViewRepository.Get<VideoView>().Remove(video.VideoId);

                WatchLaterCache.Default.Remove(video.VideoId);
                Repository.Default.Playlists.Remove(video.VideoId, "WatchLater");
            }
        }
        private bool CanWatchLaterRemove()
        {
            return Kernel.Default.SelectedVideoItem != null;
        }

        #endregion

        #region Playlists

        private void DoPlaylistRemoveVideo()
        {
            string videoId = Kernel.Default.SelectedVideoItem.Video.VideoId;
            string channelId = Kernel.Default.SelectedVideoItem.Video.ChannelId;

            ViewRepository.Get<PlaylistView>().Remove(videoId, channelId);
            ViewRepository.Get<VideoView>().Remove(videoId);
        }
        private bool CanPlaylistRemoveVideo()
        {
            return true;
        }

        private void DoNewPlaylist()
        {
            Dialogs.OpenPlaylistWindow(Kernel.Default.SelectedVideoItem.Video);
        }
        private bool CanNewPlaylist()
        {
            return Kernel.Default.SelectedVideoItem != null;
        }

        #endregion

        #region History

        private void DoHistoryRemoveAll()
        {
            ViewRepository.Get<HistoryView>().RemoveAll();
        }
        private bool CanHistoryRemoveAll()
        {
            return true;
        }

        private void DoHistoryRemove()
        {
            ViewRepository.Get<HistoryView>().Remove();
        }
        private bool CanHistoryRemove()
        {
            return true;
        }

        private void DoHistoryThirtyDays()
        {
            // 0=today, 1=yesterday, 2=day before yesterday 3=Last 7 days, 4=Last 30 days
            Settings.Default.HistoryFilter = 4;
        }
        private bool CanHistoryThirtyDays()
        {
            return true;
        }

        private void DoHistorySevenDays()
        {
            // 0=today, 1=yesterday, 2=day before yesterday 3=Last 7 days, 4=Last 30 days
            Settings.Default.HistoryFilter = 3;
        }
        private bool CanHistorySevenDays()
        {
            return true;
        }

        private void DoHistoryDayBeforedYesterday()
        {
            // 0=today, 1=yesterday, 2=day before yesterday 3=Last 7 days, 4=Last 30 days
            Settings.Default.HistoryFilter = 2;
        }
        private bool CanHistoryDayBeforedYesterday()
        {
            return true;
        }

        private void DoHistoryYesterday()
        {
            // 0=today, 1=yesterday, 2=day before yesterday 3=Last 7 days, 4=Last 30 days
            Settings.Default.HistoryFilter = 1;
        }
        private bool CanHistoryYesterday()
        {
            return true;
        }

        private void DoHistoryToday()
        {
            // 0=today, 1=yesterday, 2=day before yesterday 3=Last 7 days, 4=Last 30 days
            Settings.Default.HistoryFilter = 0;
        }
        private bool CanHistoryToday()
        {
            return true;
        }



        #endregion

        #region Views

        private void DoSubscriptionView()
        {
            Kernel.Default.View = View.Subscriptions;
        }
        private bool CanSubscriptionView()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Subscriptions)
                return false;

            return true;
        }

        private void DoFavoritesView()
        {
            Kernel.Default.View = View.Favorites;
        }
        private bool CanFavoritesView()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Favorites)
                return false;

            return true;
        }

        private void DoWatchLateView()
        {
            Kernel.Default.View = View.WatchLater;
        }
        private bool CanWatchLateView()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.WatchLater)
                return false;

            return true;
        }

        private void DoHistoryView()
        {
            Kernel.Default.View = View.History;
        }
        private bool CanHistoryView()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.History)
                return false;

            return true;
        }

        private void DoPlaylistView()
        {
            Kernel.Default.View = View.Playlists;
        }
        private bool CanPlaylistView()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Playlists)
                return false;

            return true;
        }

        private void DoBookmarksView()
        {
            Kernel.Default.View = View.Markers;
        }
        private bool CanBookmarksView()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Markers)
                return false;

            return true;
        }



        #endregion

        #region Subscriptions & Videos View State

        private void DoShowAllSubscriptions()
        {
            Settings.Default.SubscriptionDisplayMode = SubscriptionDisplayMode.AllSubscriptions;
        }
        private bool CanShowAllSubscriptions()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                return true;
            return false;
        }

        private void DoShowSubscriptionsWithVideo()
        {
            Settings.Default.SubscriptionDisplayMode = SubscriptionDisplayMode.ActiveSubscriptions;
        }
        private bool CanShowSubscriptionsWithVideo()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                return false;
            return true;
        }

        private void DoShowAllVideos()
        {
            Settings.Default.VideoDisplayMode = VideoDisplayMode.AllVideos;
        }
        private bool CanShowAllVideos()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Settings.Default.VideoDisplayMode == VideoDisplayMode.UnwatchedVideos;
        }

        private void DoShowUnwatchedVideos()
        {
            Settings.Default.VideoDisplayMode = VideoDisplayMode.UnwatchedVideos;
        }
        private bool CanShowUnwatchedVideos()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Settings.Default.VideoDisplayMode == VideoDisplayMode.AllVideos;
        }

        #endregion

        #region Player

        private void DoPlayerNext()
        {
            ViewRepository.Get<PlayerTabView>()?.GoForward();
        }
        private bool CanPlayerNext()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            PlayerView view = ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView;

            return view != null && view.History.CanGoForward;
        }

        private void DoPlayerPrevious()
        {
            ViewRepository.Get<PlayerTabView>()?.GoBack();
        }
        private bool CanPlayerPrevious()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            PlayerView view = ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView;

            return view != null && view.History.CanGoBack;
        }

        private void DoPlayerRefresh()
        {
            ViewRepository.Get<PlayerTabView>()?.Refresh();
        }
        private bool CanPlayerRefresh()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView != null;
        }

        private void DoPlayerClose()
        {
            ViewRepository.Get<PlayerTabView>()?.Close();
        }
        private bool CanPlayerClose()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView != null;
        }

        private void DoPlayerCloseAll()
        {
            ViewRepository.Get<PlayerTabView>()?.CloseAll();
        }
        private bool CanPlayerCloseAll()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView != null;
        }

        private void DoPlayerReopenClosedTab()
        {
            Video video = ClosedTabs.Default.Get();

            if(video != null)
                ViewRepository.Get<PlayerTabView>().OpenTab(video);
        }
        private bool CanPlayerReopenClosedTab()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return ClosedTabs.Default.Count > 0;
        }

        private void DoPlayerContinue()
        {
            int seconds = ViewRepository.Get<PlayerTabView>().SelectedPlayerView.Seconds;
            ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView.SeekTo(seconds);
        }
        private bool CanPlayerContinue()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.Seconds >= 15;
        }

        private void DoPlayerDownloadVideo()
        {
            IAction action = ActionRepository.Find("DownloadVideo");
            action?.Execute(Kernel.Default.PlayerVideo.VideoId);
        }
        private bool CanPlayerDownloadVideo()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.PlayerVideo != null;
        }

        private void DoPlayerLocate()
        {
            ViewRepository.Get<ChannelView>().Locate(Kernel.Default.PlayerVideo.ChannelId, Kernel.Default.PlayerVideo.VideoId);
        }
        private bool CanPlayerLocate()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.PlayerVideo != null;
        }

        private void DoPlayerCopyUrl()
        {
            Clipboard.SetText(Kernel.Default.PlayerVideo.Link);
        }
        private bool CanPlayerCopyUrl()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.PlayerVideo != null;
        }

        private void DoPlayerCopyChannelUrl()
        {
            string channel_url = UrlHelper.GetChannelUrl(Kernel.Default.PlayerVideo.ChannelId);

            Clipboard.SetText(channel_url);
        }
        private bool CanPlayerCopyChannelUrl()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            return Kernel.Default.PlayerVideo != null;
        }

        private void DoPlayerSearchGoogle()
        {
            ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.BrowserControl.SearchGoogle();
        }
        private bool CanPlayerSearchGoogle()
        {
            return true;
        }

        private void DoPlayerCopyText()
        {
            ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.BrowserControl.CopyText();
        }
        private bool CanPlayerCopyText()
        {
            return true;
        }

        private void DoPlayerFavorite()
        {
            FavoritesCache.Default.Add(Kernel.Default.PlayerVideo.VideoId);
            Repository.Default.Playlists.Insert(Kernel.Default.PlayerVideo, "Favorites");
        }
        private bool CanPlayerFavorite()
        {
            return true;
        }

        private void DoPlayerWatchLater()
        {
            FavoritesCache.Default.Add(Kernel.Default.PlayerVideo.VideoId);
            Repository.Default.Playlists.Insert(Kernel.Default.PlayerVideo, "WatchLater");
        }
        private bool CanPlayerWatchLater()
        {
            return true;
        }

        private void DoPlayerBookmark()
        {
            ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.CreateMarker();
        }
        private bool CanPlayerBookmark()
        {
            return true;
        }

        #endregion

        #region Open

        // Outside Open Channel Page
        private void DoOpenChannelPageInBrowserC()
        {
            IAction action = ActionRepository.Find("OpenChannelPageInBrowser");
            action.Execute(Kernel.Default.SelectedChannels);
        }
        private bool CanOpenChannelPageInBrowserC()
        {
            if (!AuthenticationService.Default.IsLoggedIn) return false;
            return Kernel.Default.SelectedChannels.NotEmpty();
        }

        private void DoOpenChannelPageInBrowserV()
        {
            IAction action = ActionRepository.Find("OpenChannelPageInBrowser");
            action.Execute(Kernel.Default.SelectedVideoItem.Video.ChannelId.Wrap());
        }
        private bool CanOpenChannelPageInBrowserV()
        {
            if (!AuthenticationService.Default.IsLoggedIn) return false;
            return Kernel.Default.SelectedVideoItem != null;
        }

        private void DoOpenChannelPageInBrowserP()
        {
            IAction action = ActionRepository.Find("OpenChannelPageInBrowser");
            action.Execute(Kernel.Default.PlayerVideo.ChannelId.Wrap());
        }
        private bool CanOpenChannelPageInBrowserP()
        {
            if (!AuthenticationService.Default.IsLoggedIn) return false;
            return Kernel.Default.PlayerVideo != null;
        }


        // Outside Open Videos Page
        private void DoOpenVideosPageInBrowserV()
        {
            IAction action = ActionRepository.Find("OpenVideosPageInBrowser");
            action?.Execute(Kernel.Default.SelectedVideoItem.Video.ChannelId.Wrap());
        }
        private bool CanOpenVideosPageInBrowserV()
        {
            if (!AuthenticationService.Default.IsLoggedIn) return false;
            return Kernel.Default.SelectedVideoItem != null;
        }

        private void DoOpenVideosPageInBrowserC()
        {
            IAction action = ActionRepository.Find("OpenVideosPageInBrowser");
            action?.Execute(Kernel.Default.SelectedChannels);
        }
        private bool CanOpenVideosPageInBrowserC()
        {
            if (!AuthenticationService.Default.IsLoggedIn) return false;
            return Kernel.Default.SelectedChannels.NotEmpty();
        }


        // Outside Open Video Page
        private void DoOpenVideoInBrowserV()
        {
            IAction action = ActionRepository.Find("OpenVideoInBrowser");
            action?.Execute(Kernel.Default.SelectedVideoItem.Id);
        }
        private bool CanOpenVideoInBrowserV()
        {
            return Kernel.Default.SelectedVideoItem != null;
        }

        private void DoOpenVideoInBrowserP()
        {
            ViewRepository.Get<PlayerTabView>().SelectedPlayerView.OpenInBrowser();
        }
        private bool CanOpenVideoInBrowserP()
        {
            return ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView != null;
        }

        // Inside
        private void DoOpen()
        {
            if (Kernel.Default.View == View.Markers)
            {
                ViewRepository.Get<MarkerView>().Open();
            }
            else
            {
                IAction action = ActionRepository.Find("OpenVideo");
                action?.Execute(Kernel.Default.SelectedVideoItem.Video);
            }
        }
        private bool CanOpen()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Markers)
                return ViewRepository.Get<MarkerView>().CanOpen();

            return Kernel.Default.SelectedVideoItem?.Video.VideoId != null;
        }

        private void DoOpenTab()
        {
            if (Kernel.Default.View == View.Markers)
            {
                ViewRepository.Get<MarkerView>().OpenTab();
            }
            else
            {
                IAction action = ActionRepository.Find("OpenVideoTab");
                action?.Execute(Kernel.Default.SelectedVideoItem.Video);
            }

        }
        private bool CanOpenTab()
        {
            if (Kernel.Default.LoggedIn == false)
                return false;

            if (Kernel.Default.View == View.Markers)
                return ViewRepository.Get<MarkerView>().CanOpen();

            return Kernel.Default.SelectedVideoItem?.Video.VideoId != null;
        }

        #endregion

    }
}

