using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUser(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                AppUser admin = new AppUser()
                {
                    UserName = "admin@bookify.com",
                    Email = "admin@bookify.com",
                    FullName = "Admin",
                    //PasswordHash = "Admin@123",
                    EmailConfirmed = true,
                };

                var res = await userManager.CreateAsync(admin, "Admin@123");
                if (res.Succeeded)
                {

                    await userManager.AddToRoleAsync(admin, AppRoles.Admin);
                }
            }
        }
    }
}
