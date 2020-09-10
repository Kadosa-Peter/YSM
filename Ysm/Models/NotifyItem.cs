using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Ysm.Annotations;

namespace Ysm.Models
{
    public class NotifyItem : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public string Id { get; set; }

        public int Count { get; set; }

        public bool ShowCount { get; set; }

        public BitmapImage Icon { get; set; }

        public bool IsVisible
        {
            get => _isVisible;

            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isVisible = true;

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
