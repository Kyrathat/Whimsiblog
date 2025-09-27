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

        // Who owns this blog (Identity user key; nvarchar(450) typical)
        public string? PrimaryOwnerUserId { get; set; }

        // Optional snapshot of display name/email at creation, unless we don't want it, I'm leaving it in
        public string? PrimaryOwnerUserName { get; set; }

        // Optional audit timestamp, we can decide if we want to show it or not, but I think it's still nice to have
        public DateTime? CreatedUtc { get; set; }

    }
}
