using System;
using System.Collections.Generic;

namespace Ysm.Data
{
    public class MarkerGroup
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string ChannelTitle { get; set; }

        public string ChannelId { get; set; }

        public string Link { get; set; }

        public string ThumbnailUrl { get; set; }

        public DateTimeOffset Published { get; set; }

        public DateTimeOffset Added { get; set; }

        public TimeSpan Duration { get; set; }

        public List<Marker> Markers { get; set; }

        public  MarkerGroup()
        {
            Markers = new List<Marker>();
        }

    }
}
