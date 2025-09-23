using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; } = string.Empty;
        //public Tag? TagID { get; set; }
        //public User? UserID { get; set; }
    }
}
