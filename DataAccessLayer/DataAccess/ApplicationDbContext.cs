using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Correct using for EFCore
using DataAccessLayer.Model;

namespace DataAccessLayer.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // THis defines and initalizes properties
        // Needed for a fix
        public DbSet<Blog> Blogs { get; set; } = default!;
        public DbSet<BlogPost> BlogPosts { get; set; } = default!;
        public DbSet<BlogComment> BlogComments { get; set; } = default!;
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
    }
}
