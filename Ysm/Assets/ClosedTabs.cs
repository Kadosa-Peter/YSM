using System;
using System.Collections.Generic;
using System.Linq;
using Ysm.Data;

namespace Ysm.Assets
{
    public class ClosedTabs
    {
        private static readonly Lazy<ClosedTabs> _instance = new Lazy<ClosedTabs>(()=> new ClosedTabs());

        public static ClosedTabs Default => _instance.Value;

        private ClosedTabs(){}

        private readonly List<Video> _videos = new List<Video>();

        public int Count => _videos.Count;

        public Video Get()
        {
            if (_videos.Count > 0)
            {
                Video video = _videos.Last();

                _videos.Remove(video);

                return video;
            }

            return null;
        }

        public void Insert(Video video)
        {
            _videos.Add(video);
        }
    }
}
