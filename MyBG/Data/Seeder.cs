using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using MyBG.Models;
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
                var roles = new[] { "Manager", "Admin", "User" }; // List of roles to create

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

                var adminUser = await manager.FindByNameAsync("Manager");
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = "Manager"
                    };

                    // Create the admin user
                    var result = await manager.CreateAsync(adminUser, "Abc1234!");
                    var pfp = new PFPModel { UserName = adminUser.UserName };
                    using(var memoryStream = new MemoryStream())
                    {
                        FileStream fileStream = System.IO.File.Open("ServerTextures/DefaultPfp.png", FileMode.Open);
                        fileStream.CopyTo(memoryStream);
                        pfp.Image = memoryStream.ToArray();
                        fileStream.Close();
                        memoryStream.Close(); 
                    }
                    context.PFPs.Add(pfp);

                    if (result.Succeeded)
                    {
                        // Assign the user to the Admin role
                        await manager.AddToRoleAsync(adminUser, "Manager");
                    }
                }
                context.SaveChanges();
            }
        }
        public static void RegisterPFP(string username, ApplicationDbContext context)
        {
            var pfp = new PFPModel() { UserName = username };
            context.SaveChanges();
        }
    }
}
