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
        public string? OwnerUserId { get; set; }
    }
}
