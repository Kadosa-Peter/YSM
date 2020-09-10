using System;
using System.Linq;
using System.Threading.Tasks;
using Ysm.Data;

namespace Ysm.Assets.Caches
{
    public class FavoritesCache : CacheBase
    {
        private static readonly Lazy<FavoritesCache> Instance = new Lazy<FavoritesCache>(() => new FavoritesCache());

        public static FavoritesCache Default => Instance.Value;

        public void Initialize()
        {
            RemoveAll();

            Task.Run(() =>
            {
                return Repository.Default.Playlists.Get("Favorites").Videos.Select(x => x.VideoId);

            }).ContinueWith(t =>
            {

                AddRange(t.Result);

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
