using System;
using System.Collections.Generic;

namespace Ysm.Data
{
    public class Videos
    {
        private readonly Schema _schema;

        public Videos(Schema schema)
        {
            _schema = schema;
        }

        public List<Video> Get()
        {
            return VideoQueries.Get();
        }

        public List<Video> Get(List<string> channels)
        {
            return VideoQueries.Get(channels);
        }

        public void Insert(List<Video> videos)
        {
            VideoQueries.Insert(videos);

            _schema.Insert(videos);
        }

        public IEnumerable<Video> Search(string query, List<string> channels, int state)
        {
            return VideoQueries.Search(query, channels, state);
        }

        public int UnwatchedCount()
        {
            return _schema.GetUnwatchedVideoCount();
        }

        public int UnwatchedCount(string parent)
        {
            return _schema.GetUnwatchedVideoCount(parent);
        }

        public void MarkWatched(string video_id, string channel_id)
        {
            VideoQueries.MarkWatched(video_id);

            _schema.MarkVideoWatched(video_id, channel_id);
        }

        public void MarkAllWatched()
        {
            _schema.MarkAllWatched();

            VideoQueries.MarkAllWatched();
        }

        public void MarkWatched(List<string> channelId)
        {
            _schema.MarkWatched(channelId);

            VideoQueries.MarkWatched(channelId);
        }

        public void MarkWatched(DateTime dateTime)
        {
            VideoQueries.MarkWatched(dateTime);

            _schema.Reset();
        }

        public void MarkUnwatched(IEnumerable<DateTimeOffset> dateList)
        {
            VideoQueries.MarkUnwatched(dateList);

            // call video reset
        }

        public void MarkUnwatchedDate(IEnumerable<DateTimeOffset> dateList)
        {
            VideoQueries.MarkUnwatchedDate(dateList);

            // call video reset
        }

        public void ResetVideoStated()
        {
            _schema.Reset();
        }

        public void MarkUnwatchedAfter(DateTime dateTime)
        {
            VideoQueries.MarkUnwatchedAfter(dateTime);

            _schema.Reset();
        }

        public void Remove(List<string> ids)
        {
            if (ids.Count > 0)
            {
                _schema.DeleteVideosByParent(ids);
                VideoQueries.Delete(ids);
            }
        }

        public void MarkUnwatchedByChannelId(List<string> channels)
        {
            VideoQueries.MarkUnwatchedByChannelId(channels);

            _schema.Reset();
        }

        public void MarkAllUnwatched()
        {
            VideoQueries.MarkAllUnwatched();

            _schema.Reset();
        }
    }
}
