using System;
using System.Collections.Generic;
using System.Linq;
using Ysm.Data;

namespace Ysm.Assets
{
    public class VisibleNodes
    {
        private static readonly Lazy<VisibleNodes> Instance = new Lazy<VisibleNodes>(() => new VisibleNodes());

        public static VisibleNodes Default => Instance.Value;

        private List<string> _channels;

        public event EventHandler Changed;

        public void Initialize()
        {
            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
            {
                _channels = new List<string>(Repository.Default.Channels.Get().Where(x => x.State > 0).Select(x => x.Id));
            }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SubscriptionDisplayMode")
            {
                if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                {
                    if(_channels == null)
                        _channels = new List<string>();

                    _channels.AddRange(Repository.Default.Channels.Get().Where(x => x.State > 0).Select(x => x.Id));
                }
                else
                {
                    _channels.Clear();
                }
            }
        }

        public void Remove(string id)
        {
            _channels.Remove(id);

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Reset(IEnumerable<string> ids)
        {
            _channels.Clear();
            _channels.AddRange(ids);

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool Contains(string id)
        {
            return _channels.Contains(id);
        }
    }
}
