using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.Routing;

namespace Gramps
{
    public static class RouteRegistrar
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Clear();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            MvcRoute.MappUrl("{controller}/{action}/{id}")
                .WithDefaults(new { controller = "Home", action = "Index", id = "" })
                .AddWithName("Default", routes);
        }
    }
}