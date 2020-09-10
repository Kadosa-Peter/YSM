using System;
using System.Collections.Generic;

namespace Ysm.Assets
{
    public class SearchEventArgs<T> : EventArgs
    {
        public bool Reset { get; set; }

        public IEnumerable<T> Result;
    }
}