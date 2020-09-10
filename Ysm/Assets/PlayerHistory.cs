using System;
using System.Collections.Generic;
using Ysm.Data;

namespace Ysm.Assets
{
    public class PlayerHistory
    {
        public event EventHandler HistoryChanged;

        private readonly List<Video> _list;

        private int _current = -1;

        public bool CanGoBack { get; set; }

        public bool CanGoForward { get; set; }

        public PlayerHistory()
        {
            _list = new List<Video>();
        }

        public void Add(Video video)
        {
            if(_list.Count > _current+1 && Equals(_list[_current], video)) return;

            _list.Add(video);

            _current = _list.Count-1;

            if (_list.Count > 1)
                CanGoBack = true;

            CanGoForward = false;

            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public Video GoForward()
        {
            _current++;

            Video video = _list[_current];

            CanGoForward = _current + 1 <= _list.Count-1;

            CanGoBack = _current - 1 >= 0;

            HistoryChanged?.Invoke(this, EventArgs.Empty);

            return video;
        }

        public Video GoBack()
        {
            _current--;

            Video video = _list[_current];

            CanGoForward = _current + 1 <= _list.Count-1;

            CanGoBack = _current - 1 > 0;

            HistoryChanged?.Invoke(this, EventArgs.Empty);

            return video;
        }

        

    }
}
