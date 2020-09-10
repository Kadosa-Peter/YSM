using System.Collections.Generic;

namespace Ysm.Data
{
    public class History
    {
        public void Insert(string videoId)
        {
            HistoryQueries.Insert(videoId);
        }

        public void Delete(List<string> ids)
        {
            HistoryQueries.Remove(ids);
        }

        public List<Video> Get()
        {
            return HistoryQueries.Get();
        }

        public void RemoveAll()
        {
            HistoryQueries.RemoveAll();
        }

        public void Remove(string id)
        {
            HistoryQueries.Remove(id);
        }
    }
}
