using System.Collections.Generic;
using Ysm.Assets;

namespace Ysm.Actions
{
    public class VideoService : IAction
    {
        public string Name { get; } = "VideoService";

        public void Execute(object obj)
        {
            if (Kernel.Default.SubscriptionService || Kernel.Default.VideoService)
            {
                return;
            }
            
            if (obj is List<string> channels)
            {
                // egy vagy több csatornáról letöltöm az összes videót
                VideoServiceWrapper.Default.Start(channels, true);
            }
            else
            {
                // letöltöm az új feliratkozásokat és videókat
                Downloader downloader = new Downloader();
                downloader.Run();
            }
        }
    }
}
