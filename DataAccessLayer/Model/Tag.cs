using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class Tag
    {
        public int TagID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters.")]
        public string? Name { get; set; } = string.Empty;

        // Many-to-many relationship with Blogs
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    } 
}
