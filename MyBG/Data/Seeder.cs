using Microsoft.EntityFrameworkCore;

namespace MyBG.Data
{
    public static class Seeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Pages.Any())
                {
                    return;
                }

                context.Submissions.Add(new Models.PageModel { Title = "Sofia", TextBody = "Andibul" });
                context.Pages.Add(new Models.PageModel { Title="Sofia", TextBody = "Abdul"});
                context.SaveChanges();
            }
        }
    }
}
