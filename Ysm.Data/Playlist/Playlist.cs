using System.Collections.Generic;
using System.Linq;

namespace Ysm.Data
{
    public class Playlist
    {
        public Playlist()
        {
            Channels = new List<Channel>();
            Videos = new List<Video>();
        }

        public string Name { get; set; }

        public string Id { get; set; }

        public bool Default { get; set; }

        public string Color { get; set; }

        public List<Channel> Channels { get; set; } 

        public List<Video> Videos { get; set; }

        public void Add(Video video)
        {
            if(Videos.Any(x => x.VideoId == video.VideoId )) return;

            Channel channel = ChannelQueries.Get_By_Id(video.ChannelId);

            if (Channels.Any(x => x.Id == channel.Id) == false)
            {
                Channels.Add(channel);
            }

            if (Videos.Contains(video) == false)
            {
                Videos.Add(video);
            }
        }

        public void Remove(string id)
        {
            Video video = Videos.FirstOrDefault(x => x.VideoId == id);

            if (video != null)
            {
                Videos.Remove(video);

                if (Videos.Count(x => x.ChannelId == video.ChannelId) == 0)
                {
                    Channel channel = Channels.FirstOrDefault(x => x.Id == video.ChannelId);

                    Channels.Remove(channel);
                }
            }
        }

        public void RemoveAll()
        {
            Videos.Clear();
            Channels.Clear();
        }

        public int Count()
        {
            return Videos.Count;
        }

        public int Count(string channelId)
        {
            return Videos.Count(x => x.ChannelId == channelId);
        }

        public void Clear()
        {
            Videos.Clear();
            Channels.Clear();
        }
    }
}
