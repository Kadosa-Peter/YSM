using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ysm.Core.Annotations;

namespace Ysm.Data
{
    public class Marker : INotifyPropertyChanged
    {
        public string Comment
        {
            get => _comment;

            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _comment;

        public string GroupId { get; set; }

        public string Id { get; set; }

        public TimeSpan Time { get; set; }

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
