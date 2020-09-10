using System;
using Ysm.Core;

namespace Ysm.Models.Iteration
{
    public class Node : NodeBase
    {
        public NodeType NodeType { get; set; }

        public DateTimeOffset DateTime { get; set; }
    }
}
