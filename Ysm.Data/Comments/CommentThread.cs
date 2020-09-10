using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ysm.Data.Comments
{
    public class CommentThread
    {
        public string NextPageToken { get; set; }

        public List<Comment> Comments { get; set; }

        public CommentThread()
        {
            Comments = new List<Comment>();
        }
    }
}
