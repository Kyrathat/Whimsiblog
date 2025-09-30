using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class BlogComment
    {
        public int BlogCommentID { get; set; }
        [Required(ErrorMessage = "Comment body is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string? Body { get; set; } = string.Empty;
        [Required]
        public User? UserID { get; set; }
    }
}
