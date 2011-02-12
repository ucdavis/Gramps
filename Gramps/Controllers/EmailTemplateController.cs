using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
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
    
        //
        // GET: /EmailTemplate/
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var viewModel = EmailTemplateListViewModel.Create(Repository, templateId, callForProposalId);

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SendTestEmail(string subject, string message)
        {
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginId == CurrentUser.Identity.Name).FirstOrDefault();
            var mail = new MailMessage("automatedemail@caes.ucdavis.edu", user.Email, subject, message);
            mail.IsBodyHtml = true;
            var client = new SmtpClient();
            client.Send(mail);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /EmailTemplate/Edit/5
        public ActionResult Edit(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var emailtemplate = _emailtemplateRepository.GetNullableById(id);

            if (emailtemplate == null) return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            if (!_accessService.HasSameId(emailtemplate.Template, emailtemplate.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = EmailTemplateViewModel.Create(Repository, templateId, callForProposalId);
            viewModel.EmailTemplate = emailtemplate;

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
