using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Bookify.Web.Helpers
{
    public class AppUserClaims : UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        public AppUserClaims(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity=await base.GenerateClaimsAsync(user);
            //*************************************************
            //Add new claim
            identity.AddClaim(new Claim("FullName", user.FullName));
            //*************************************************
            return identity;
        }
    }
}
