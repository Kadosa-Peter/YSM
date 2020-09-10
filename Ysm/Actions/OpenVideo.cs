using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Actions
{
    // CideoView ContextMenu
    public class OpenVideo : IAction
    {
        public string Name { get; } = "OpenVideo";

        public void Execute(object obj)
        {
            if (obj is Video video)
            {
                if (ModifierKeyHelper.IsCtrlDown)
                {
                    ViewRepository.Get<PlayerTabView>()?.OpenTab(video);
                }
                else
                {
                    ViewRepository.Get<PlayerTabView>()?.Open(video);
                }

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
