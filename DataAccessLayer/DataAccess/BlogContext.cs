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
        public DbSet<Tag> tags { get; set; }

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

            // Configure Tag entity/table
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tags"); // Use existing table name
                entity.HasKey(t => t.TagID);
                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(50);
                
                // Ensure tag names are unique
                entity.HasIndex(t => t.Name).IsUnique();
            });

            // Configure many-to-many relationship between Blog and Tag
            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.Blogs)
                .UsingEntity(j => j.ToTable("BlogTags"));

        }
        public DbSet<Tag> Tags { get; set; }
    }
}
