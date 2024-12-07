using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Models;

namespace TheBlogProject.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<BlogUser>(options)
    {
        public DbSet<Blog>? Blogs { get; set; }
        public DbSet<Post>? Posts { get; set; }
        public DbSet<Comment>? Comments { get; set; }
        public DbSet<Tag>? Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Defining the relationship between Blog and Post
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Blog) // Each Post is associated with a Blog
                .WithMany(b => b.Posts) // A Blog has many Posts
                .HasForeignKey(p => p.BlogId); // Foreign key in Post referencing Blog
        }
    }
}