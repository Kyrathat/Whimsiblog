using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class BlogComment
    {
        public int BlogCommentID { get; set; }
        public string? Body { get; set; } = string.Empty;
        //public user? UserID { get; set; }
    }
}
