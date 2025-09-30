using DataAccessLayer.Model;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Model
{

    public class BlogComment
    {
        public int BlogCommentID { get; set; }

        [Required, StringLength(1000)]
        public string Body { get; set; } = string.Empty;

        // Azure AD ownership fields
        public string? OwnerUserId { get; set; }   // ObjectId or NameIdentifier from claims
        public string? OwnerUserName { get; set; } // Email

        [Required]
        public int BlogPostID { get; set; }
        public BlogPost? BlogPost { get; set; }

        public int? ParentCommentID { get; set; }
        public BlogComment? ParentComment { get; set; }

        public ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();
    }
}
