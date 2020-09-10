using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ysm.Core;
using Ysm.Models;

namespace Ysm.Assets
{
    public class VideoList : ObservableCollection<VideoItem>
    {
        private List<VideoItem> _items;

        private List<string> _history;

        private int _index;

        public  VideoList()
        {
            _history = new List<string>();
            _items = new List<VideoItem>();
        }

        public void Set(VideoItem item)
        {
            // ez a metódus videó elindulása után fut le
            // ez a metódus akkor is lefut amikor leállítok egy videót és újraindítom
            _items.Remove(item);

            _index = Items.IndexOf(item);

            if (Enumerable.LastOrDefault(_history) != item.Id)
                _history.Add(item.Id);
        }

        public void Reset(bool clearHistory)
        {
            _items = Items.ToList();

            if (clearHistory)
            {
                _history.Clear();
            }
        }

        public void Remove(string id)
        {
            // Shuffle-nél használom. Eltávolitom azt a videót, amelyiket akkor játszok le, amikor kiválasztom a shuffle opciót.
            VideoItem item = _items.FirstOrDefault(x => x.Id == id);
            _items.Remove(item);
        }

        public VideoItem RandomNext()
        {
            if (_items.Count == 0)
                return null;

            Random random = new Random(Environment.TickCount);

            int index = random.Next(0, _items.Count);

            VideoItem item = _items[index];

            _items.Remove(item);

            if (Enumerable.LastOrDefault(_history) != item.Id)
                _history.Add(item.Id);

            return item;
        }

        public VideoItem Next()
        {
            if (Items.Count == 0) return null;

            int index = _index + 1;

            if (index > Items.Count - 1)
                return null;

            // Itt is beállítom az _index értéket, mert a Set metódus csak néhány másodperc után fut le
            _index = index;

            VideoItem item = Items[index];

            if (Enumerable.LastOrDefault(_history) != item.Id)
                _history.Add(item.Id);

            return item;
        }

        public VideoItem Previous()
        {
            _history.RemoveLast();

            string id = _history.TakeLast();

            if (id.NotNull())
            {
                VideoItem item = Items.FirstOrDefault(x => x.Id == id);

                if (item != null)
                {
                    _index = Items.IndexOf(item);

                    return item;
                }
            }

            return null;

           
        }

        public void AddNew(VideoItem item)
        {
            _items.Add(item);
        }
    }
}
