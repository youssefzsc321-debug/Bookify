using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {

                await roleManager.CreateAsync(new IdentityRole() { Name = AppRoles.Admin });
                await roleManager.CreateAsync(new IdentityRole() { Name = AppRoles.Archive });
                await roleManager.CreateAsync(new IdentityRole() { Name = AppRoles.Reception });
            }
        }
    }
}
