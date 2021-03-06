﻿using System.Web.Mvc;
using System.Web.Security;
using Gramps.Helpers;

namespace Gramps.Controllers
{
    public class AccountController : ApplicationController
    {
        /// <summary>
        /// #1
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult LogOn(string returnUrl)
        {
            string resultUrl = CASHelper.Login(); //Do the CAS Login

            if (resultUrl != null)
            {
                return Redirect(resultUrl);
            }

            TempData["URL"] = returnUrl;


            return View();
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("https://cas.ucdavis.edu/cas/logout");
        }
    }
}