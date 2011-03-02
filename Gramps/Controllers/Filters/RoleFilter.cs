using System;
using System.Web.Mvc;

namespace Gramps.Controllers.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserOnlyAttribute : AuthorizeAttribute
    {
        public UserOnlyAttribute()
        {
            Roles = RoleNames.User;    //Set the roles prop to a comma delimited string of allowed roles
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.Name.Contains("@"))
            {
                filterContext.HttpContext.Response.Redirect("~/Proposal/Home");
            }
            base.OnAuthorization(filterContext);
        }
    }
    public class RoleNames
    {
        public static readonly string User = "User";
    }
}