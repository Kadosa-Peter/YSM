using Ysm.Core;
using Ysm.Data;

namespace Ysm.Models
{
    public class Node : NodeBase
    {
        public new string Id
        {
            get
            {
                if (Category != null)
                    return Category.Id;

                if (Channel != null)
                    return Channel.Id;

                return string.Empty;
            }
        }

        public Category Category { get; set; }

        public Channel Channel { get; set; }

        public Playlist Playlist { get; set; }

        public MarkerGroup MarkerGroup { get; set; }

        public NodeType NodeType { get; set; }
    }
}
