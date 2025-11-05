using DataAccessLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataAccess
{
    public class BlogContext : IdentityDbContext<IdentityUser>
    {
        public BlogContext(DbContextOptions<BlogContext> options)
            : base (options)
        {
            
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }


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

                modelBuilder.Entity<UserProfile>(u =>
                {
                    u.ToTable("UserProfiles");
                    u.HasKey(x => x.Id); // Azure Active Directory (AAD) id
                    u.Property(x => x.Id).HasMaxLength(64); 
                    u.Property(x => x.DisplayName).HasMaxLength(128);
                    u.Property(x => x.Email).HasMaxLength(256);
                    //u.Property(x => x.Bio).HasMaxLength(280);
                    u.Property(x => x.CreatedUtc).HasDefaultValueSql("GETUTCDATE()");
                    u.Property(x => x.UpdatedUtc).HasDefaultValueSql("GETUTCDATE()");
                });
            });

            modelBuilder.Entity<BlogPost>(e =>
            {
                e.Property(p => p.Title).HasMaxLength(150).IsRequired();
                e.Property(p => p.OwnerUserId).HasMaxLength(450);
                e.Property(p => p.OwnerUserName).HasMaxLength(256);
                e.Property(p => p.CreatedUtc).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedUtc).HasDefaultValueSql("GETUTCDATE()");

            });

            // AI-Generated section for BlogComments based on the blog above
            modelBuilder.Entity<BlogComment>(entity =>
            {
                entity.ToTable("BlogComments");
                entity.HasKey(c => c.BlogCommentID);

                entity.Property(c => c.Body)
                      .IsRequired()
                      .HasMaxLength(1000);

                // The owner of the comment
                entity.Property(c => c.OwnerUserId).HasMaxLength(450);
                entity.Property(c => c.OwnerUserName).HasMaxLength(256);

                // Server-side timestamp default, leaving this in incase we want to do it in the future
                //entity.Property(c => c.CreatedUtc)
                      //.HasDefaultValueSql("GETUTCDATE()");

                // Each comment belongs to a post
                entity.HasOne(c => c.BlogPost)
                      .WithMany() 
                      .HasForeignKey(c => c.BlogPostID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete comments when a post is deleted

                // Call refernce itself when making replies
                entity.HasOne(c => c.ParentComment)
                      .WithMany(p => p.Replies)
                      .HasForeignKey(c => c.ParentCommentID)
                      .OnDelete(DeleteBehavior.Restrict); // avoid recursive cascades

                // Data indexing
                entity.HasIndex(c => c.OwnerUserId); // a section like a "my comments" section
                entity.HasIndex(c => new { c.BlogPostID, c.BlogCommentID });
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
