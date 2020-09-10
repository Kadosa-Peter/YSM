using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Ysm.Core.Annotations;


namespace Ysm.Core
{
    public class NodeBase : INotifyPropertyChanged, IComparable
    {
        public event EventHandler Rendered;
        public event EventHandler Expanded;
        public event EventHandler Collapsed;
        public event EventHandler Selected;
        public event EventHandler UnSelected;
        public event EventHandler RenameStarted;
        public event EventHandler RenameEnded;

        public NodeBase Parent
        {
            get => _parent;

            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged();
                }
            }
        }
        private NodeBase _parent;

        public string Id
        {
            get => _id;

            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _id;

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

        public int Count
        {
            get => _count;

            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _count;

        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isSelected;

        public bool IsExpanded
        {
            get => _isExpanded;

            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isExpanded;

        public bool HasItems
        {
            get => _hasItems;

            set
            {
                if (_hasItems != value)
                {
                    _hasItems = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _hasItems;

        // TODO: should be true by defaul
        public bool CanCollapse
        {
            get => _canCollapse;

            set
            {
                if (_canCollapse != value)
                {
                    _canCollapse = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _canCollapse;

        public bool IsRenaming
        {
            get => _isRenaming;

            set
            {
                if (_isRenaming != value)
                {
                    _isRenaming = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isRenaming;

        public bool IsDragOver
        {
            get => _isDragOver;

            set
            {
                if (_isDragOver != value)
                {
                    _isDragOver = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isDragOver;

        public bool IsRendered
        {
            get => _isRendered;

            set
            {
                if (_isRendered != value)
                {
                    _isRendered = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isRendered;

        public bool Fade
        {
            get => _fade;

            set
            {
                if (_fade != value)
                {
                    _fade = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _fade;

        public Color Color
        {
            get => _color;

            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }
        private Color _color = ColorHelper.GetDefaultFolderColor();

        public Visibility Visibility
        {
            get => _visibility;

            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private Visibility _visibility;

        public ImageSource Icon
        {
            get => _icon;

            set
            {
                if (!Equals(_icon, value))
                {
                    _icon = value;
                    OnPropertyChanged();
                }
            }
        }
        private ImageSource _icon;

        public bool CanDelete { get; set; } = true;
        public bool CanRename { get; set; } = true;
        public bool CanMove { get; set; } = true;

        public ObservableCollection<NodeBase> Items { get; set; }

        // .ctro
        public NodeBase()
        {
            Items = new ObservableCollection<NodeBase>();
            Items.CollectionChanged += (s, e) => { HasItems = Items.Count > 0; };
        }

        protected virtual void OnUnselected()
        {
            UnSelected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSelected()
        {
            Selected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCollapsed()
        {
            Collapsed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnExpanded()
        {
            Expanded?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRenameStarted()
        {
            RenameStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRenameEnded()
        {
            RenameEnded?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRendered()
        {
            Rendered?.Invoke(this, EventArgs.Empty);
        }

        public int CompareTo(object obj)
        {
            return obj is NodeBase node ? string.Compare(Title, node.Title, StringComparison.OrdinalIgnoreCase) : 0;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == "IsSelected")
            {
                if (IsSelected)
                {
                    OnSelected();
                }
                else
                {
                    OnUnselected();
                }
            }
            else if (propertyName == "IsExpanded")
            {
                if (IsExpanded)
                {
                    OnExpanded();
                }
                else
                {
                    OnCollapsed();
                }
            }
            else if (propertyName == "IsRenaming")
            {
                if (IsRenaming)
                {
                    OnRenameStarted();
                }
                else
                {
                    OnRenameEnded();
                }
            }
            else if (propertyName == "IsRendered")
            {
                if (IsRendered)
                {
                    OnRendered();
                }
            }
        }
    }
}
