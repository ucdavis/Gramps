using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Models;
using Gramps.Services;
using MvcContrib;

namespace Gramps.Controllers
{
    [HandleError]
    public class PublicController : ApplicationController 
    {
        private readonly IFormsAuthenticationService _formsService;
        private readonly IMembershipService _membershipService;
        private readonly IEmailService _emailService;


        public PublicController(IFormsAuthenticationService formsService, IMembershipService membershipService, IEmailService emailService)
        {
            _formsService = formsService;
            _membershipService = membershipService;
            _emailService = emailService;
        }

        /// <summary>
        /// #1
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if(!string.IsNullOrEmpty(model.UserName))
            {
                model.UserName = model.UserName.Trim().ToLower();
            }
            if (ModelState.IsValid)
            {
                if (_membershipService.ValidateUser(model.UserName, model.Password))
                {
                    _formsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return this.RedirectToAction<ProposalController>(a => a.Home());
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// #3
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            var viewModel = new ForgotPasswordModel();
            
            return View(viewModel);
        }

        /// <summary>
        /// #4
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [CaptchaValidator]
        [HttpPost]
        public ActionResult ForgotPassword(string userName, bool captchaValid )
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Recaptcha value not valid");
                Message = "Unable to reset password";
                return View(new ForgotPasswordModel() { UserName = userName .ToLower()});
            }
            userName = userName.Trim().ToLower();

            if (!_membershipService.DoesUserExist(userName))
            {
                ModelState.AddModelError("UserName", "Email not found");
            }

            CallForProposal callForProposal = null;
            var proposal =
                Repository.OfType<Proposal>().Queryable.Where(a => a.Email == userName && a.CallForProposal.IsActive)
                    .FirstOrDefault();
            if(proposal == null)
            {
                proposal =
                    Repository.OfType<Proposal>().Queryable.Where(a => a.Email == userName).FirstOrDefault();
            }
            if(proposal == null)
            {
                var editor =
                    Repository.OfType<Editor>().Queryable.Where(a => a.ReviewerEmail == userName && a.CallForProposal != null).FirstOrDefault();
                if(editor != null)
                {
                    callForProposal =
                        Repository.OfType<CallForProposal>().Queryable.Where(a => a.Editors.Contains(editor)).
                            FirstOrDefault();
                }
            }
            else
            {
                callForProposal = proposal.CallForProposal;
            }

            if(callForProposal == null)
            {
               ModelState.AddModelError("UserName", "Linked Email not found"); 
            }

            if (ModelState.IsValid)
            {
                var tempPass = _membershipService.ResetPassword(userName);
                _emailService.SendPasswordReset(callForProposal, userName, tempPass);

                Message = "A new password has been sent to your email. It should arrive in a few minutes";
                return this.RedirectToAction<PublicController>(a => a.LogOn());
            }

            Message = "Unable to reset password";
            var viewModel = new ForgotPasswordModel();
            viewModel.UserName = userName;
            return View(viewModel);
        }


       /// <summary>
       /// #5
       /// </summary>
       /// <returns></returns>
        public ActionResult LogOff()
        {
            _formsService.SignOut();

            return this.RedirectToAction<HomeController>(a => a.LoggedOut());
        }

        /// <summary>
        /// #6
        /// </summary>
        /// <returns></returns>
        [PublicAuthorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = _membershipService.MinPasswordLength;
            return View();
        }

        /// <summary>
        /// #7
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PublicAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (_membershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    //return RedirectToAction("ChangePasswordSuccess");
                    return this.RedirectToAction(a => a.ChangePasswordSuccess());
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = _membershipService.MinPasswordLength;
            return View(model);
        }

        /// <summary>
        /// #8
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
