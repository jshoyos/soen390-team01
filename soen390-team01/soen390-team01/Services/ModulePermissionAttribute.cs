using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace soen390_team01.Services
{
    public class ModulePermissionAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roles = Roles.Split(',').ToList();
            if (roles.Any(role => context.HttpContext.User.IsInRole(role)))
            {
                return;
            }

            string actionName = (string)context.RouteData.Values["action"];
            if (actionName.Equals("Index", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Authentication", action = "PermissionDenied" }));
            }
            else
            {
                var result = new JsonResult(null)
                {
                    StatusCode = 401
                };
                context.Result = result;
            }
        }
    }
}
