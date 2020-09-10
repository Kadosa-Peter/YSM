using System;
using System.Collections.Generic;

namespace Ysm.Data
{
    public class VideoServiceEventArgs : EventArgs
    {
        public List<Video> Videos { get; set; }

        public List<string> Errors { get; set; }

        public int All { get; set; }

        public int Finished { get; set; }
    }
}
