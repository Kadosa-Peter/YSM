using System.Collections.Generic;

namespace Ysm.Data
{
    public class Markers
    {
        public List<MarkerGroup> Get()
        {
            return MarkerQueries.Get();
        }

        public MarkerGroup Get(string videoId)
        {
            return MarkerQueries.Get(videoId);
        }

        public MarkerGroup Save(Video video, Marker marker)
        {
            return MarkerQueries.Save(video, marker);
        }

        public void Delete(string id)
        {
            MarkerQueries.Delete(id);
        }

        public void Delete(string id, string entryId)
        {
            MarkerQueries.Delete(id, entryId);
        }

        public void Update(string groupId, string markId, string comment)
        {
            MarkerQueries.Update(groupId, markId, comment);
        }
    }
}
