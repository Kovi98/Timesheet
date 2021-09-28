using Microsoft.AspNetCore.Identity;
using Portal.Areas.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            if (!(await roleManager.RoleExistsAsync(nameof(Roles.Admin)))) await roleManager.CreateAsync(new IdentityRole(nameof(Roles.Admin)));
            if (!(await roleManager.RoleExistsAsync(nameof(Roles.Member)))) await roleManager.CreateAsync(new IdentityRole(nameof(Roles.Member)));
        }
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                Name = "Admin",
                Surname = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Admin123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Member.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }

            }
        }
    }
    public enum Roles
    {
        Admin,
        Member
    }
}
