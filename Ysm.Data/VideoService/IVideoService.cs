using System;
using System.Collections.Generic;
using Ysm.Core;

namespace Ysm.Data
{
    internal interface IVideoService
    {
        event EventHandler<VideoServiceEventArgs> Started;
        event EventHandler<VideoServiceEventArgs> Finished;
        event EventHandler<VideoServiceEventArgs> SegmentFinished;
        event EventHandler<VideoServiceEventArgs> Cancelled;
        event EventHandler<VideoServiceEventArgs> ProgressChanged;

        bool IsRunning { get;  }

        void Run(List<string> subscriptions, SortType sort, bool downloadAll);

        void Cancel();
    }
}
