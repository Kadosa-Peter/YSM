using System;

namespace Ysm.Data
{
    public class Repository
    {
        #region Singleton

        private static readonly Lazy<Repository> Instance = new Lazy<Repository>(() => new Repository());

        public static Repository Default => Instance.Value;

        private Repository() { }


        #endregion

        public Schema Schema { get; set; }

        public Categories Categories { get; set; }

        public Channels Channels { get; set; }

        public Videos Videos { get; set; }

        public Playlists Playlists { get; set; }

        public History History { get; set; }

        public Markers Markers { get; set; }

        public Continuity Continuity { get; set; }

        public void Initialize()
        {
            Schema = new Schema();

            Categories = new Categories(Schema);
            Channels = new Channels(Schema);
            Videos = new Videos(Schema);
            Playlists = new Playlists();
            History = new History();
            Markers = new Markers();
            Continuity = new Continuity();
        }
    }
}
