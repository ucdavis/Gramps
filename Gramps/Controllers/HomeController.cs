using System.Web.Mvc;
using Gramps.Controllers.Filters;
using UCDArch.Web.Attributes;

namespace Gramps.Controllers
{
    
    [HandleTransactionsManually] //If Home controller doesn't access the db
    public class HomeController : ApplicationController
    {
        [UserOnly]
        public ActionResult Index()
        {
            

            return View();
        }
        
        public ActionResult About()
        {
            return View();
        }

        public ActionResult LoggedOut()
        {
            return View();
        }
    }
}
