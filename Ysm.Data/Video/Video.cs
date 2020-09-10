using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ysm.Data
{
    public class Video : INotifyPropertyChanged
    {
        public string VideoId
        {
            get => _videoId;

            set
            {
                if (_videoId != value)
                {
                    _videoId = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _videoId;

        public string ChannelId
        {
            get => _channelId;

            set
            {
                if (_channelId != value)
                {
                    _channelId = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _channelId;

        public string Title
        {
            get => _title;

            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _title;

        public string Link
        {
            get => _link;

            set
            {
                if (_link != value)
                {
                    _link = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _link;

        public DateTime Published
        {
            get => _published;

            set
            {
                if (_published != value)
                {
                    _published = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTime _published;

        public DateTime Added
        {
            get => _added;

            set
            {
                if (_added != value)
                {
                    _added = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTime _added;

        public TimeSpan Duration
        {
            get => _duration;

            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }
        private TimeSpan _duration;

        public string ThumbnailUrl
        {
            get => _thumbnailUrl;

            set
            {
                if (_thumbnailUrl != value)
                {
                    _thumbnailUrl = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _thumbnailUrl;

        public int State
        {
            get => _state;

            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _state;

        public int Start
        {
            get => _start;

            set
            {
                if (_start != value)
                {
                    _start = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _start;

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
