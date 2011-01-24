using System.Web.Mvc;
using UCDArch.Web.Attributes;

namespace Gramps.Controllers
{

    [HandleTransactionsManually] //If Home controller doesn't access the db
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
