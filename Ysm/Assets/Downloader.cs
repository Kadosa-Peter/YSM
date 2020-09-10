using Ysm.Data;

namespace Ysm.Assets
{
    public class Downloader
    {
        public Downloader()
        {
            VideoServiceWrapper.Default.Finished += VideoService_Finished;
            VideoServiceWrapper.Default.Canceled += VideoService_Canceled;
            SubscriptionServiceWrapper.Default.Finished += SubscriptionService_Finished;
            SubscriptionServiceWrapper.Default.Canceled += SubscriptionService_Canceled;
        }

        private void SubscriptionService_Canceled(object sender, SubscriptionServiceEventArgs e)
        {
            VideoServiceWrapper.Default.Finished -= VideoService_Finished;
            SubscriptionServiceWrapper.Default.Finished -= SubscriptionService_Finished;
        }

        private void VideoService_Canceled(object sender, VideoServiceEventArgs e)
        {
            VideoServiceWrapper.Default.Finished -= VideoService_Finished;
            SubscriptionServiceWrapper.Default.Finished -= SubscriptionService_Finished;
        }

        public void Run()
        {
            SubscriptionServiceWrapper.Default.Start();
        }

        private void SubscriptionService_Finished(object sender, SubscriptionServiceEventArgs e)
        {
            VideoServiceWrapper.Default.Start(null);
        }

        private void VideoService_Finished(object sender, VideoServiceEventArgs e)
        {
            VideoServiceWrapper.Default.Finished -= VideoService_Finished;
            SubscriptionServiceWrapper.Default.Finished -= SubscriptionService_Finished;
        }
    }
}
