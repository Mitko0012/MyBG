using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBG.Models;

namespace MyBG.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<PageModel> Submissions { get; set; }
        public DbSet<PageModel> Pages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
