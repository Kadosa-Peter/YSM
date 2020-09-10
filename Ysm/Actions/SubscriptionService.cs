using Ysm.Assets;

namespace Ysm.Actions
{
    public class SubscriptionService : IAction
    {
        public string Name { get; } = "SubscriptionService";
        
        public void Execute(object obj)
        {
            if (Kernel.Default.SubscriptionService || Kernel.Default.VideoService)
            {
                return;
            }
            
            SubscriptionServiceWrapper.Default.Start(true);
        }
    }
}
