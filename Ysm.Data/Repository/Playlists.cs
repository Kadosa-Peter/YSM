using System.Collections.Generic;

namespace Ysm.Data
{
    public class Playlists
    {
        public Playlists()
        {
            PlaylistQueries.CreateDefaults();
        }

        public void Save(Playlist playlist)
        {
            PlaylistQueries.Save(playlist);
        }

        public Playlist Get(string id)
        {
            return PlaylistQueries.Get(id);
        }

        public List<Playlist> GetAll()
        {
            return PlaylistQueries.GetAll();
        }

        public void Insert(Video video, string id)
        {
            PlaylistQueries.Insert(video, id);
        }

        public void RemoveAll(string id)
        {
            PlaylistQueries.RemoveAll(id);
        }

        public void Remove(string videoId, string playlistId)
        {
            PlaylistQueries.Remove(videoId, playlistId);
        }

        public void Delete(string playlistId)
        {
            PlaylistQueries.Delete(playlistId);
        }

        public void Rename(string id, string name)
        {
            PlaylistQueries.Rename(id, name);
        }
    }
}
