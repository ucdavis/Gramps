using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Core.Resources;
using Gramps.Helpers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using MvcContrib;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Helpers;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the EmailTemplate class
    /// </summary>
    [UserOnly]
    public class EmailTemplateController : ApplicationController
    {
	    private readonly IRepository<EmailTemplate> _emailtemplateRepository;
        private readonly IAccessService _accessService;

        public EmailTemplateController(IRepository<EmailTemplate> emailtemplateRepository, IAccessService accessService)
        {
            _emailtemplateRepository = emailtemplateRepository;
            _accessService = accessService;
        }
    
        /// <summary>
        /// #1
        /// GET: /EmailTemplate/
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var viewModel = EmailTemplateListViewModel.Create(Repository, templateId, callForProposalId);

            return View(viewModel);
        }

        /// <summary>
        /// #2 Not Tested
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="footerText"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SendTestEmail(string subject, string message, string footerText)
        {
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginId == CurrentUser.Identity.Name).FirstOrDefault();
            var mail = new MailMessage("automatedemail@caes.ucdavis.edu", user.Email, subject, message + "<br />" + footerText);
            mail.IsBodyHtml = true;
            mail.Body = mail.Body.Replace(Token(StaticValues.TokenProposalMaximum), String.Format("{0:C}", 99999.98));
            mail.Body = mail.Body.Replace(Token(StaticValues.TokenCloseDate), String.Format("{0:D}", DateTime.Now.Date.AddMonths(1)));
            mail.Body = mail.Body.Replace(Token(StaticValues.TokenApprovedAmount), String.Format("{0:C}", 99999.98));
            mail.Body = mail.Body.Replace(Token(StaticValues.TokenReviewerName), "Jonny Appleseed");
            mail.Body = mail.Body.Replace(Token(StaticValues.TokenProposalLink), GetAbsoluteUrl(Request, Url, "~/Proposal/Edit/" + Guid.NewGuid()));
            mail.Body = mail.Body.Replace(Token(StaticValues.TokenCreateProposalLink), GetAbsoluteUrl(Request, Url, "~/Proposal/Create/123"));
 
            var client = new SmtpClient();
            client.Send(mail);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private static string Token(string token)
        {
            return string.Format("{{{0}}}", token);
        }

        private string GetAbsoluteUrl(HttpRequestBase request, UrlHelper url, string relative)
        {
            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Host, url.Content(relative));
        }

        /// <summary>
        /// #3
        /// GET: /EmailTemplate/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult Edit(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var emailtemplate = _emailtemplateRepository.GetNullableById(id);

            if (emailtemplate == null)
            {
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (!_accessService.HasSameId(emailtemplate.Template, emailtemplate.CallForProposal, templateId, callForProposalId))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = EmailTemplateViewModel.Create(Repository, templateId, callForProposalId);
            viewModel.EmailTemplate = emailtemplate;
            viewModel.FooterText = StaticValues.EmailAutomatedDisclaimer;
            if (emailtemplate.TemplateType == EmailTemplateType.InitialCall)
            {
                viewModel.FooterText = string.Format("{0}<br />{1}<br />This will be replaced with the link to create a proposal", viewModel.FooterText, StaticValues.EmailCreateProposal);

                viewModel.Tokens.Add(StaticValues.TokenProposalMaximum);
                viewModel.Tokens.Add(StaticValues.TokenCloseDate);
                viewModel.Tokens.Add(StaticValues.TokenCreateProposalLink);
            }
            else if (emailtemplate.TemplateType == EmailTemplateType.ReadyForReview)
            {               
                viewModel.FooterText = string.Format("{0}<p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p><p>{5}</p><p>{6}</p><p>{7}</p><p>{8}</p>"
                    , viewModel.FooterText
                    , "An account has been created for you."
                    , "UserName johnnytest@test.com"
                    , "Password bdLJ&SftBN>%oe"
                    , "You may change your password (recommended) after logging in."
                    , "After you have logged in, you may use this link to review submitted proposals for this Grant Request:"
                    , "http://localhost:31701/Proposal/ReviewerIndex/8"
                    , "Or to view all active Call For Proposals you can use this link(Home):"
                    , "http://localhost:31701/Proposal/Home");

                viewModel.AlternateFooterText = string.Format("{0}<br /><p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p><p>{5}</p>"
                    , StaticValues.EmailAutomatedDisclaimer
                    , "You have an existing account. Use your email as the userName to login"
                    , "After you have logged in, you may use this link to review submitted proposals for this Grant Request:"
                    , "http://localhost:31701/Proposal/ReviewerIndex/8"
                    , "Or to view all active Call For Proposals you can use this link(Home):"
                    , "http://localhost:31701/Proposal/Home");

                viewModel.Tokens.Add(StaticValues.TokenReviewerName);
            }
            else if (emailtemplate.TemplateType == EmailTemplateType.ProposalConfirmation)
            {
                viewModel.FooterText = string.Format("{0}<p>{1}</p><p>{2} {3}</p><p>{4} {5}</p><p>{6}</p>"
                    , viewModel.FooterText
                    , "An account has been created for you."
                    , "UserName"
                    , "johnnytest@test.com"
                    , "Password"
                    , "bdLJ&SftBN>%oe"
                    , "You may change your password (recommended) after logging in.");

                viewModel.FooterText = string.Format("{0}<p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p>"
                    , viewModel.FooterText
                    , "After you have logged in, you may use this link to edit your proposal:"
                    , "http://localhost:31701/Proposal/Edit/e18348ee-424c-4754-ab48-a23cc7d177a9"
                    , "Or you may access a list of your proposal(s) here:"
                    , "http://localhost:31701/Proposal/Home");

                viewModel.AlternateFooterText = string.Format("{0}<p>{1}</p>"
                    , viewModel.AlternateFooterText
                    , "You have an existing account. Use your email as the userName to login");
                viewModel.AlternateFooterText = string.Format("{0}<p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p>"
                    , viewModel.AlternateFooterText
                    , "After you have logged in, you may use this link to edit your proposal:"
                    , "http://localhost:31701/Proposal/Edit/e18348ee-424c-4754-ab48-a23cc7d177a9"
                    , "Or you may access a list of your proposal(s) here:"
                    , "http://localhost:31701/Proposal/Home");

                viewModel.Tokens.Add(StaticValues.TokenCloseDate);
            }
            else if(emailtemplate.TemplateType == EmailTemplateType.ProposalApproved)
            {
                
                viewModel.Tokens.Add(StaticValues.TokenApprovedAmount);
                viewModel.Tokens.Add(StaticValues.TokenProposalLink);
            }
            else if (emailtemplate.TemplateType == EmailTemplateType.ProposalDenied)
            {

                viewModel.Tokens.Add(StaticValues.TokenProposalLink);
            }
            else if(emailtemplate.TemplateType == EmailTemplateType.ReminderCallIsAboutToClose)
            {
                
                viewModel.Tokens.Add(StaticValues.TokenCloseDate);
                viewModel.Tokens.Add(StaticValues.TokenProposalLink);
            }
            else if (emailtemplate.TemplateType == EmailTemplateType.ProposalUnsubmitted)
            {

                viewModel.Tokens.Add(StaticValues.TokenCloseDate);
                viewModel.Tokens.Add(StaticValues.TokenProposalLink);
            }

            return View(viewModel);
        }

        //
        // POST: /EmailTemplate/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Edit(int id, int? templateId, int? callForProposalId, EmailTemplate emailtemplate)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var emailtemplateToEdit = _emailtemplateRepository.GetNullableById(id);

            if (emailtemplateToEdit == null)
            {
                Message = "Email Template not found.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (!_accessService.HasSameId(emailtemplateToEdit.Template, emailtemplateToEdit.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            TransferValues(emailtemplate, emailtemplateToEdit);

            emailtemplateToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _emailtemplateRepository.EnsurePersistent(emailtemplateToEdit);

                Message = "EmailTemplate Edited Successfully";

                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else
            {
                var viewModel = EmailTemplateViewModel.Create(Repository, templateId, callForProposalId);
                viewModel.EmailTemplate = emailtemplateToEdit;

                return View(viewModel);
            }
        }
        

        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(EmailTemplate source, EmailTemplate destination)
        {
            destination.Subject = source.Subject;
            destination.Text = source.Text;
        }

    }


}
