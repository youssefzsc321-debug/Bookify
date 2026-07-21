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

            if (user is null)
            {
                user = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin",
                    EmailConfirmed = true,
                    IsDeleted = false
                };

                var createResult = await userManager.CreateAsync(user, "753951420Tt");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, AppRoles.Admin);
                }
            }
            else
            {
                user.IsDeleted = false;
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);

                await userManager.RemovePasswordAsync(user);
                var res = await userManager.AddPasswordAsync(user, "753951420Tt");

                if (res.Succeeded)
                {
                    if (!await userManager.IsInRoleAsync(user, AppRoles.Admin))
                    {
                        await userManager.AddToRoleAsync(user, AppRoles.Admin);
                    }
                }
            }


        }
    }
}
