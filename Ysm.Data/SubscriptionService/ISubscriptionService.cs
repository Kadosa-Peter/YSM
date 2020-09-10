using System;

namespace Ysm.Data
{
    internal interface ISubscriptionService
    {
        event EventHandler<SubscriptionServiceEventArgs> Started;
        event EventHandler<SubscriptionServiceEventArgs> Finished;
        event EventHandler<SubscriptionServiceEventArgs> Cancelled;
        event EventHandler<SubscriptionServiceEventArgs> ProgressChanged;

        bool IsRunning { get; }

        void Run();

        void Cancel();
    }
}
