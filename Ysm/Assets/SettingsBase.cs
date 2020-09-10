using System.ComponentModel;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Ysm.Assets
{
    public abstract class SettingsBase : INotifyPropertyChanged
    {
        public string Owner { get; } = "YSM";

        public string Version { get; } = "1.0";

        public virtual void SettingsChanged(string propertyName)
        {
            
        }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            SettingsChanged(propertyName);
        }

        #endregion
    }
}
