using System.Collections.Generic;
using System.Linq;
using Ysm.Data;

namespace Ysm.Assets
{
    public static class ChannelMapper
    {
        private static readonly List<MapObj> List;

        static ChannelMapper()
        {
            List = Repository.Default.Channels.Get().Select(x => new MapObj { Id = x.Id, Title = x.Title, Thumbnail = x.Thumbnail }).ToList();
        }

        public static MapObj Get(string id)
        {
            return List.FirstOrDefault(x => x.Id == id);
        }

        public static void Add(List<Channel> channels)
        {
            List.AddRange(channels.Select(x => new MapObj { Id = x.Id, Title = x.Title, Thumbnail = x.Thumbnail }));
        }

        public static void Add(Channel channel)
        {
            if (List.Any(x => x.Id == channel.Id)) return;

            List.Add(new MapObj { Id = channel.Id, Title = channel.Title, Thumbnail = channel.Thumbnail });
        }

        public static void Reset()
        {
            List.Clear();
            List.AddRange(Repository.Default.Channels.Get().Select(x => new MapObj { Id = x.Id, Title = x.Title, Thumbnail = x.Thumbnail }));
        }
    }
}
