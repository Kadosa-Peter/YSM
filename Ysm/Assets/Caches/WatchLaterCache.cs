using System;
using System.Linq;
using System.Threading.Tasks;
using Ysm.Data;

namespace Ysm.Assets.Caches
{
    public class WatchLaterCache : CacheBase
    {
        private static readonly Lazy<WatchLaterCache> Instance = new Lazy<WatchLaterCache>(() => new WatchLaterCache());

        public static WatchLaterCache Default => Instance.Value;

        public void Initialize()
        {
            RemoveAll();

            Task.Run(() =>
            {
                return Repository.Default.Playlists.Get("WatchLater").Videos.Select(x => x.VideoId);

            }).ContinueWith(t =>
            {

                AddRange(t.Result);

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
