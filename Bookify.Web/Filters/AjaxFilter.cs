using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Bookify.Web.Filters
{
    public class AjaxFilter : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            var requset = routeContext.HttpContext.Request;
            var isAjax = requset.Headers["x-requested-with"]== "XMLHttpRequest";
            return isAjax;
        }
    }
}
