using System;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Error class
    /// </summary>
    public class ErrorController : ApplicationController
    {

        public ActionResult Index()
        {
            return View();
        }
    }


}
