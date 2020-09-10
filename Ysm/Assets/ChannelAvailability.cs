using Ysm.Core;
using Ysm.Data;

namespace Ysm.Assets
{
    public static class ChannelAvailability
    {
        public static bool Check(string channelId)
        {
            if (channelId.IsNull()) return false;
            
            if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.AllSubscriptions)
            {
                return true;
            }
            else
            {
                if (Repository.Default.Channels.GetState(channelId) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
