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

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        public IEmailService EmailService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if(!string.IsNullOrEmpty(model.UserName))
            {
                model.UserName = model.UserName.Trim().ToLower();
            }
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
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

        public ActionResult ForgotPassword()
        {
            var viewModel = new ForgotPasswordModel();
            
            return View(viewModel);
        }

        [CaptchaValidator]
        [HttpPost]
        public ActionResult ForgotPassword(string userName, bool captchaValid )
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Recaptcha value not valid");
            }
            userName = userName.Trim().ToLower();

            if (!MembershipService.DoesUserExist(userName))
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
                var tempPass = MembershipService.ResetPassword(userName);
                EmailService.SendPasswordReset(callForProposal, userName, tempPass);

                Message = "A new password has been sent to your email. It should arrive in a few minutes";
                return this.RedirectToAction<PublicController>(a => a.LogOn());
            }

            Message = "Unable to reset password";
            var viewModel = new ForgotPasswordModel();
            viewModel.UserName = userName;
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            throw new NotImplementedException();
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return this.RedirectToAction<HomeController>(a => a.LoggedOut());
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        //public ActionResult Register()
        //{
        //    ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Attempt to register the user
        //        MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

        //        if (createStatus == MembershipCreateStatus.Success)
        //        {
        //            FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
        //    return View(model);
        //}

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [PublicAuthorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [PublicAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
