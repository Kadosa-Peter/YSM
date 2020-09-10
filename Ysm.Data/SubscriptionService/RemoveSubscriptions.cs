using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Ysm.Core;

namespace Ysm.Data
{
    public static class RemoveSubscriptions
    {
        public static event EventHandler<RemoveSubscriptionsEventArgs> Changed;

        public static async Task Execute(IEnumerable<string> ids)
        {
            try
            {
                int all = ids.Count();
                int current = 0;

                Changed?.Invoke(null, new RemoveSubscriptionsEventArgs { Started = true, Finished = false, All = all, Current = 0 });

                YouTubeService youTubeService = AuthenticationService.Default.YouTubeService;

                foreach (string id in ids)
                {
                    SubscriptionsResource.DeleteRequest request = youTubeService.Subscriptions.Delete(id);
                    await request.ExecuteAsync();

                    current++;
                    Changed?.Invoke(null, new RemoveSubscriptionsEventArgs { Started = false, Finished = false, All = all, Current = current });
                }

                Changed?.Invoke(null, new RemoveSubscriptionsEventArgs { Started = false, Finished = true, All = all, Current = 0 });
            }
            catch (AggregateException ae)
            {
                foreach (Exception exception in ae.InnerExceptions)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
        }
    }

    public class RemoveSubscriptionsEventArgs : EventArgs
    {
        public bool Started { get; set; }

        public bool Finished { get; set; }

        public int All { get; set; }

        public int Current { get; set; }
    }

}
