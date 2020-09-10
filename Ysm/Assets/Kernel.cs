using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using Ysm.Data;
using Ysm.Models;

namespace Ysm.Assets
{
    public class Kernel : INotifyPropertyChanged
    {
        private static readonly Lazy<Kernel> _instance = new Lazy<Kernel>(() => new Kernel());

        public static Kernel Default => _instance.Value;

        private Kernel() { }

        public bool SubscriptionService
        {
            get => _subscriptionService;
            set
            {
                if (_subscriptionService != value)
                {
                    _subscriptionService = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _subscriptionService;

        public bool VideoService
        {
            get => _videoService;

            set
            {
                if (_videoService != value)
                {
                    _videoService = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _videoService;

        public bool Import
        {
            get => _import;

            set
            {
                if (_import != value)
                {
                    _import = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _import;

        public Video PlayerVideo
        {
            get => _playerVideo;

            set
            {
                if (_playerVideo != value)
                {
                    _playerVideo = value;
                    OnPropertyChanged();
                }
            }
        }
        private Video _playerVideo;

        //public Channels Channels
        //{
        //    get => _channels;

        //    set
        //    {
        //        if (_channels != value)
        //        {
        //            _channels = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        //private Channels _channels;

        public bool CanCut
        {
            get => _canCut;

            set
            {
                if (_canCut != value)
                {
                    _canCut = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _canCut;

        public bool CanPaste
        {
            get => _canPaste;

            set
            {
                if (_canPaste != value)
                {
                    _canPaste = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _canPaste;

        public bool CanDelete
        {
            get => _canDelete;

            set
            {
                if (_canDelete != value)
                {
                    _canDelete = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _canDelete;

        public bool CanRename
        {
            get => _canRename;

            set
            {
                if (_canRename != value)
                {
                    _canRename = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _canRename;

        public bool LoggedIn
        {
            get => _loggedIn;

            set
            {
                if (_loggedIn != value)
                {
                    _loggedIn = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _loggedIn;

        public View View
        {
            get => _view;

            set
            {
                if (_view != value)
                {
                    _view = value;
                    OnPropertyChanged();
                }
            }
        }
        private View _view;        

        public bool IsRenaming { get; set; }

        public bool IsRootOnlySelected { get; set; }

        public bool IsDragDrop { get; set; }

        public bool Search { get; set; }

        public double VerticalOffset { get; set; }

        public List<string> SelectedChannels { get; set; }

        public VideoItem SelectedVideoItem { get; set; }

        public bool MenuIsOpen { get; set; }

        public bool IsDeleteing { get; set; }

        public SecureString ClientId { get; set; }

        public SecureString ClientSecret { get; set; }

        public SecureString ConnectionString { get; set; }

        public SecureString EncryptionKey { get; set; }

        public bool IsBeta { get; set; }

        public DateTime Beta { get; set; }

        public string Website { get; } = "https://www.yosuma.com";

        public string Faq { get; } = "https://www.yosuma.com/faq";

        public string UserGuide { get; } = "https://www.yosuma.com/userguide/v1/";

        public string Tutorial { get; } = "https://www.youtube.com/playlist?list=PLPBVa7lW1xHzh6eaG23VoKxfU1J-KpVlD";        

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
