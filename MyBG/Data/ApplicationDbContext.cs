using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBG.Controllers;
using MyBG.Models;

namespace MyBG.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<PageModel> Pages { get; set; }
        public DbSet<PFPModel> PFPs { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<ForumQuestion> Posts { get; set; }
        public DbSet<TransportWay> TransportWays { get; set; }
        public DbSet<EditModel> Edits {get; set;}
        public DbSet<InboxMessage> Messages { get; set; }
        public DbSet<AdminRequest> Requests {get; set;}
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
            modelBuilder.Entity<PageModel>()
                        .HasMany(x => x.TransportWays);
            modelBuilder.Entity<EditModel>()
                        .HasOne(x => x.PageToEdit)
                        .WithMany(x => x.Edits)
                        .HasForeignKey(x => x.PageModelKey);

            modelBuilder.Entity<PFPModel>()
                        .HasMany(x => x.Contributions)
                        .WithOne(x => x.UserCreated)
                        .HasForeignKey(x => x.PFPKey);

            modelBuilder.Entity<PFPModel>()
                        .HasMany(x => x.Inbox)
                        .WithOne(x => x.UserSource);
            modelBuilder.Entity<ForumQuestion>()
                        .HasOne(x => x.User)
                        .WithMany()
                        .HasForeignKey(x => x.ForeignKey)
                        .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<CommentModel>()
                        .HasMany(x => x.Replies);
        }
    }
}
