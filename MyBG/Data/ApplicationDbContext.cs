using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBG.Models;

namespace MyBG.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<PageModel> Pages { get; set; }
        public DbSet<PFPModel> PFPs { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<ForumQuestion> Posts { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ForumQuestion>()
                        .HasMany(x => x.Comment);
            modelBuilder.Entity<CommentModel>()
                        .HasMany(x => x.PostedOnForums);
        }
    }
}
