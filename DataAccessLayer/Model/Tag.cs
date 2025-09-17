using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class Tag
    {
        public int TagID { get; set; }
        public string? Name { get; set; } = string.Empty;
    }
}
