using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class BlogPost
    {
        public int BlogPostID { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "Title can't be longer than 150 characters.")]
        public string? Title { get; set; } = string.Empty;
        [Required]
        public string? Body { get; set; } = string.Empty;
        //public Tag? TagID { get; set; }
        //public Blog? BlogID { get; set; }

        [StringLength(450)]
        public string? OwnerUserId { get; set; }   // AAD object id

        [StringLength(256)]
        public string? OwnerUserName { get; set; } // Name /!!\ at time of post /!!\
                                                    // Name does not update
        public DateTime CreatedUtc { get; set; }

        public DateTime? UpdatedUtc { get; set; }
    }
}
