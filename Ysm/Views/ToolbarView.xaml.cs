using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Views
{
    public partial class ToolbarView
    {
        public ToolbarView()
        {
            InitializeComponent();

            Kernel.Default.PropertyChanged += Kernel_PropertyChanged;
        }

        private void Kernel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PlayerVideo")
            {
                if (Kernel.Default.PlayerVideo != null)
                {
                    SetVideoTooltip();
                    SetChannelTooltip();
                }
            }
            else if (e.PropertyName == "View")
            {
                if (Kernel.Default.View == View.Playlists)
                {
                    RenameButton.ToolTip = Properties.Resources.Tooltip_RenamePlaylist;
                    RemoveButton.ToolTip= Properties.Resources.Tooltip_RemovePlaylist;
                    CreateButton.ToolTip = Properties.Resources.Tooltip_NewPlaylist;
                }
                else
                {
                    RenameButton.ToolTip = Properties.Resources.Tooltip_Rename;
                    RemoveButton.ToolTip = Properties.Resources.Tooltip_Remove;
                    CreateButton.ToolTip = Properties.Resources.Tooltip_NewCategory;
                }
            }
        }

        private void SetVideoTooltip()
        {
            string text = Properties.Resources.Title_OpenInBrowser;
            text = text.Replace("{xy}", DefaultBrowser.Get());

            string videoTitle = Kernel.Default.PlayerVideo.Title;

            OpenVideoInBrowserButton.ToolTip = $"{text} {videoTitle}";
        }

        private void SetChannelTooltip()
        {
            string text = Properties.Resources.Title_OpenInBrowser;
            text = text.Replace("{xy}", DefaultBrowser.Get());

            string channelId = Kernel.Default.PlayerVideo.ChannelId;
            string channelTitle = Repository.Default.Channels.Get_By_Id(channelId).Title;

            OpenChannelInBrowserButton.ToolTip = $"{text} {channelTitle}";
        }
    }
}
