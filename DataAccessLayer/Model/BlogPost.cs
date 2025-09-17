using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class BlogPost
    {
        public int BlogPostID { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Body { get; set; } = string.Empty;
        public Tag? TagID { get; set; }
        public Blog? BlogID { get; set; }
    }
}
