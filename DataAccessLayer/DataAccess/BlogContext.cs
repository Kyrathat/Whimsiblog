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
        public BlogContext(DbContextOptions<BlogContext> options)
            : base (options)
        {
            
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Blog entity/table
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blogs");
                entity.HasKey(b => b.BlogId); // primary key

                entity.Property(b => b.Name) // Name column config
                      .IsRequired() // NOT NULL
                      .HasMaxLength(100);
                //entity.Property(b => b.Tags)
                //.HasMaxLength(400);

                entity.Property(b => b.Description) // Description config
                      .HasMaxLength(1000);

                entity.Property(b => b.PrimaryOwnerUserId).HasMaxLength(450);
                entity.Property(b => b.PrimaryOwnerUserName).HasMaxLength(256);
                entity.Property(b => b.CreatedUtc).HasDefaultValueSql("GETUTCDATE()");

                // Helpful for querying by owner later, I imagine we'll be doing a good amount of queries
                entity.HasIndex(b => b.PrimaryOwnerUserId);
            });

            // Creating a Join Table for BlogPost_Tags.  Shows a relationship where you can have Many Tags with many Posts.
            //Then Saves the results to a BlogPost_Tags table. 
            modelBuilder.Entity<BlogPost>()
        .HasMany(bp => bp.Tags)
        .WithMany(t => t.BlogPosts)
        .UsingEntity(j => j.ToTable("BlogPost_Tags"));

        }
    }
}
