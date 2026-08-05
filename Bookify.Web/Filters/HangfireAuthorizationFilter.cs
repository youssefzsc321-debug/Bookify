using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Web.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {

        private string policyName;

        public HangfireAuthorizationFilter(string policyName)
        {
            this.policyName = policyName;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var authService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var isAuthorized = authService.AuthorizeAsync(httpContext.User, policyName)
                                            .ConfigureAwait(false)
                                            .GetAwaiter()
                                            .GetResult().Succeeded; 
            return isAuthorized;
        }
    }
}
