using System;
using System.Threading.Tasks;
using Chromium;

namespace Ysm.Assets.Browser
{
    public class TaskStringVisitor : CfxStringVisitor
    {
        private readonly TaskCompletionSource<string> taskCompletionSource;

        public TaskStringVisitor()
        {
            taskCompletionSource = new TaskCompletionSource<string>();
            Visit += TaskStringVisitor_Visit;
        }

        void TaskStringVisitor_Visit(object sender, Chromium.Event.CfxStringVisitorVisitEventArgs e)
        {
            try
            {
                taskCompletionSource.SetResult(e.String);
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
        }

        public Task<string> Task => taskCompletionSource.Task;
    }
}

