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
    }
    public class RoleNames
    {
        public static readonly string User = "User";
    }
}