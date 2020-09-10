using System.Collections.Generic;
using System.Linq;

namespace Ysm.Data
{
    public class Channels
    {
        private readonly Schema _schema;

        public Channels(Schema schema)
        {
            _schema = schema;
        }

        public List<Channel> Get()
        {
            return ChannelQueries.Get();
        }

        public List<Channel> Get(string id)
        {
            return ChannelQueries.Get_By_Parent(id);
        }

        public Channel Get_By_Id(string id)
        {
            return ChannelQueries.Get_By_Id(id);
        }

        public void Insert(List<Channel> channels)
        {
            ChannelQueries.Insert(channels);

            _schema.Insert(channels);
        }

        public void Update(List<Channel> channels)
        {
            _schema.MoveChannel(channels.ToDictionary(x => x.Id, x => x.Parent));

            ChannelQueries.Update(channels);
        }

        public void Move(Dictionary<string, string> channels)
        {
            if (channels.Count > 0)
            {
                ChannelQueries.Move(channels);
                _schema.MoveChannel(channels);
            }
        }

        public void Remove(List<string> ids)
        {
            if (ids.Any())
            {
                _schema.DeleteChannels(ids);

                ChannelQueries.Delete(ids);
            }
        }

        public void Remove()
        {
            ChannelQueries.DeleteAll();
        }

        public int Count()
        {
            return _schema.GetChannelCount();
        }

        public int GetState(string id)
        {
            return _schema.GetChannelState(id);
        }

        public IEnumerable<Channel> Search(string query)
        {
            return ChannelQueries.Search(query);
        }
    }
}
