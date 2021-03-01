using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace soen390_team01.Services
{
    public class ModulePermission : ActionFilterAttribute
    {
        public string Roles { get; set; }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            bool isAuthorized = false;
            var roles = Roles.Split(',').ToList();
            foreach (var role in roles)
            {
                if (filterContext.HttpContext.User.IsInRole(role))
                {
                    isAuthorized = true;
                    break;
                }
            }
            if (!isAuthorized)
            {
                (filterContext.Controller as Controller).HttpContext.Response.Redirect("Authentication\\PermissionDenied");
            }
        }
    }
}
