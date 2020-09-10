using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Actions
{
    // CideoView ContextMenu
    public class OpenVideoTab : IAction
    {
        public string Name { get; } = "OpenVideoTab";

        public void Execute(object obj)
        {
            if (obj is Video video)
            {
                ViewRepository.Get<PlayerTabView>()?.OpenTab(video);

                Repository.Default.History.Insert(video.VideoId);

                // make video watched 
                if (video.State == 0)
                {
                    ViewRepository.Get<FooterView>().DecreaseVideoCount(1);

                    video.State = 1;
                    Repository.Default.Videos.MarkWatched(video.VideoId, video.ChannelId);
                    ViewRepository.Get<ChannelView>()?.DecreaseVideoCount(video.ChannelId);
                }
            }
        }
    }
}
