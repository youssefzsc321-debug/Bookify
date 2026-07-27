using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUser(UserManager<AppUser> userManager)
        {

            var adminEmail = "admin@bookify.com";
            var user = await userManager.FindByEmailAsync(adminEmail);


            if (!userManager.Users.Any())
            {
                user = new AppUser
                {
                    UserName = "Admin",
                    Email = "admin@bookify.com",
                    FullName = "Abuzaid",
                    EmailConfirmed = true,
                    IsDeleted = false
                };

                var createResult = await userManager.CreateAsync(user, "753951420Tt#");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, AppRoles.Admin);
                }
            }
         


        }
    }
}
