using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ysm.Data.Comments
{
    public class Comment
    {
        public Comment(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Published { get; set; }
    }
}
