using System;
using System.Collections.Generic;
using System.Text;
using Ysm.Core;

namespace Ysm.Data
{
    internal class VideoSql
    {
        internal static string Insert()
        {
            return @"INSERT INTO Videos(VideoId, ChannelId, Title, Link, Published, Duration, Added, ThumbnailUrl) VALUES(@VideoId, @ChannelId, @Title, @Link, @Published, @Duration, @Added, @ThumbnailUrl)";
        }

        internal static string Get()
        {
            return "SELECT * FROM Videos";
        }

        internal static string Get(List<string> channels)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT * FROM Videos WHERE ChannelId IN(");

            foreach (string channel in channels)
            {
                builder.Append("'");
                builder.Append(channel);
                builder.Append("',");
            }

            builder.Remove(builder.Length - 1, 1);

            builder.Append(");");

            return builder.ToString();
        }

        internal static string Delete(string id)
        {
            return $"DELETE FROM Videos WHERE ChannelId = '{id}'";
        }

        internal static string Delete()
        {
            return "DELETE FROM Videos";
        }

        internal static string MarkVideoWatched(string id)
        {
            return $"UPDATE Videos SET State=1 WHERE VideoId='{id}'";
        }

        internal static string MarkWatched(string channelId)
        {
            return $"UPDATE Videos SET State=1 WHERE ChannelId='{channelId}'";
        }

        internal static string MarkWatched(DateTime dateTime)
        {
            return $"UPDATE Videos SET State=1 WHERE Published < '{dateTime.ToSqlDateTime()}'";
        }

        internal static string MarkAllWatched()
        {
            return "UPDATE Videos SET State=1";
        }

        internal static string MarkAllUnwatched()
        {
            return "UPDATE Videos SET State=0";
        }

        internal static string MarkUnwatchedAfter(DateTime dateTime)
        {

            return $"UPDATE Videos SET State=0 WHERE Published > '{dateTime.ToSqlDateTime()}'";
        }

        internal static string MarkUnwatchedDate(DateTime date)
        {
            return $"UPDATE Videos SET State=0 WHERE DATE(added) = '{date:yyyy-MM-dd}'";
        }

        internal static string MarkUnwatched(DateTime dateTime)
        {
            DateTime d1 = dateTime.AddSeconds(-1);
            DateTime d2 = dateTime.AddSeconds(1);

            return $"UPDATE Videos SET State=0 WHERE Added > '{d1.ToSqlDateTime()}' AND Added < '{d2.ToSqlDateTime()}'";
        }

        internal static string MarkUnwatchedByChannel(string channel)
        {
            return $"UPDATE Videos SET State=0 WHERE ChannelId = '{channel}'";
        }

        internal static string Search(string query, List<string> channels, int state)
        {
            StringBuilder builder = new StringBuilder();

            // only unwatched videos 
            if (state == 0)
            {
                builder.Append($"SELECT * FROM Videos WHERE Title LIKE '%{query}%' And State=0 AND ChannelId IN(");
            }
            // all videos
            else
            {
                builder.Append($"SELECT * FROM Videos WHERE Title LIKE '%{query}%' AND ChannelId IN(");
            }

            foreach (string channel in channels)
            {
                builder.Append("'");
                builder.Append(channel);
                builder.Append("',");
            }

            builder.Remove(builder.Length - 1, 1);

            builder.Append(");");

            return builder.ToString();
        }
    }
}
