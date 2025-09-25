using DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataAccess
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions options)
            : base (options)
        {
            
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // keep base call (important if you derive from IdentityDbContext)

            // Configure Blog entity/table
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blogs");             // explicit table name (otherwise EF pluralizes by convention)
                entity.HasKey(b => b.BlogId);        // primary key

                entity.Property(b => b.Name)         // Name column config
                      .IsRequired()                  // NOT NULL
                      .HasMaxLength(100);           // nvarchar(100)
                //entity.Property(b => b.Tags)
                    //.HasMaxLength(400);
                //entity.HasIndex(b => b.UserId)
                    //.IsUnique();
            });

        }
    }
}
