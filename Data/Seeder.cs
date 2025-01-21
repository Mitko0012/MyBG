using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.Security.Cryptography;

namespace MyBG.Data
{
    public static class Seeder
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                UserManager<IdentityUser> manager = serviceProvider.GetService<UserManager<IdentityUser>>();
                RoleManager<IdentityRole> roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "User" }; // List of roles to create

                if (context.Users.Any())
                {
                    return;
                }
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var adminUser = await manager.FindByEmailAsync("morkovbul2@gmail.com");
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = "morkovbul2@gmail.com",
                        Email = "morkovbul2@gmail.com"
                    };

                    // Create the admin user
                    var result = await manager.CreateAsync(adminUser, "1234Abc#");

                    if (result.Succeeded)
                    {
                        // Assign the user to the Admin role
                        await manager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
                context.Submissions.Add(new Models.PageModel { Title = "Sofia", TextBody = "Andibul" });
                context.Pages.Add(new Models.PageModel { Title = "Sofia", TextBody = "Abdul" });
                context.SaveChanges();
            }
        }
    }
}
