using System.Collections.Generic;
using System.Linq;
using Ysm.Assets;

namespace Ysm.Actions
{
    internal static class ActionRepository
    {
        private static readonly List<IAction> _actions;

        static ActionRepository()
        {
            _actions = new List<IAction>
            {
                new OpenSettings(),
                new VideoService(),
                new SubscriptionService(),
                new OpenVideo(),
                new OpenVideoTab(),
                new OpenVideosPageInBrowser(),
                new OpenChannelPageInBrowser(),
                new OpenVideoInBrowser(),
                new ImportAction(),
                new ExportAction(),
                new DownloadVideo(),
                new DownloadAllVideos(),
                new OpenFeedback(),
                new Update(),
                new OpenAbout(),
                new Licence()
            
            };
        }

        internal static IAction Find(string name)
        {
            return _actions.FirstOrDefault(action => action.Name == name);
        }
    }
}
