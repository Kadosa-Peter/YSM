using Ysm.Core;
using Ysm.Data;

namespace Ysm.Models.Notes
{
    public class Node : NodeBase
    {
        public new string Id => MarkerGroup != null ? MarkerGroup.Id : Marker.Id;

        public MarkerGroup MarkerGroup { get; set; }

        public Marker Marker { get; set; }

        public NodeType NodeType { get; set; }
    }
}
