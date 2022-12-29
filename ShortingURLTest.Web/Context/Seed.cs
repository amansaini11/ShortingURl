using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShortingURLTest.Web.Entity;

namespace ShortingURL.Web.Context
{
    public class Seed
    {
        public static async Task SeedRoles(UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            if (await roleManager.Roles.AnyAsync()) return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Admin"},
                new AppRole{Name = "User"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            var admin = new AppUser
            {
                UserName = "admin",
                Email = "admin@lilliurl.com"
            };
            
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin" });
        }
    }
}
