using System;
using System.Collections.Generic;

namespace Ysm.Data
{
    public class SubscriptionServiceEventArgs : EventArgs
    {
        public int All { get; set; }

        public int Finished { get; set; }

        public List<Channel> Channels { get; set; }

        public List<string> Removed { get; set; }

        public bool NoNewSubscription { get; set; }
    }
}
