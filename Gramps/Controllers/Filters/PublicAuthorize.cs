using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gramps.Controllers.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PublicAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.HttpContext.Response.Redirect("~/Public/LogOn");
            }
            if (!filterContext.HttpContext.User.Identity.Name.Contains("@"))
            {
                filterContext.HttpContext.Response.Redirect("~/Account/LogOut");
            }
        }
    }
}