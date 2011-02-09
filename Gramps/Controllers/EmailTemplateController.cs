using System;
using System.Web.Mvc;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the EmailTemplate class
    /// </summary>
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
            var viewModel = EmailTemplateListViewModel.Create(Repository, templateId, callForProposalId);

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            return View(viewModel);
        }

        ////
        //// GET: /EmailTemplate/Details/5
        //public ActionResult Details(int id)
        //{
        //    var emailtemplate = _emailtemplateRepository.GetNullableById(id);

        //    if (emailtemplate == null) return this.RedirectToAction(a => a.Index());

        //    return View(emailtemplate);
        //}

        ////
        //// GET: /EmailTemplate/Create
        //public ActionResult Create()
        //{
        //    var viewModel = EmailTemplateViewModel.Create(Repository);
            
        //    return View(viewModel);
        //} 

        ////
        //// POST: /EmailTemplate/Create
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Create(EmailTemplate emailtemplate)
        //{
        //    var emailtemplateToCreate = new EmailTemplate();

        //    TransferValues(emailtemplate, emailtemplateToCreate);

        //    emailtemplateToCreate.TransferValidationMessagesTo(ModelState);

        //    if (ModelState.IsValid)
        //    {
        //        _emailtemplateRepository.EnsurePersistent(emailtemplateToCreate);

        //        Message = "EmailTemplate Created Successfully";

        //        return this.RedirectToAction(a => a.Index());
        //    }
        //    else
        //    {
        //        var viewModel = EmailTemplateViewModel.Create(Repository);
        //        viewModel.EmailTemplate = emailtemplate;

        //        return View(viewModel);
        //    }
        //}

        //
        // GET: /EmailTemplate/Edit/5
        public ActionResult Edit(int id, int? templateId, int? callForProposalId)
        {
            var emailtemplate = _emailtemplateRepository.GetNullableById(id);

            if (emailtemplate == null) return this.RedirectToAction(a => a.Index(templateId, callForProposalId));

            var viewModel = EmailTemplateViewModel.Create(Repository);
            viewModel.EmailTemplate = emailtemplate;

            return View(viewModel);
        }
        
        ////
        //// POST: /EmailTemplate/Edit/5
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Edit(int id, EmailTemplate emailtemplate)
        //{
        //    var emailtemplateToEdit = _emailtemplateRepository.GetNullableById(id);

        //    if (emailtemplateToEdit == null) return this.RedirectToAction(a => a.Index());

        //    TransferValues(emailtemplate, emailtemplateToEdit);

        //    emailtemplateToEdit.TransferValidationMessagesTo(ModelState);

        //    if (ModelState.IsValid)
        //    {
        //        _emailtemplateRepository.EnsurePersistent(emailtemplateToEdit);

        //        Message = "EmailTemplate Edited Successfully";

        //        return this.RedirectToAction(a => a.Index());
        //    }
        //    else
        //    {
        //        var viewModel = EmailTemplateViewModel.Create(Repository);
        //        viewModel.EmailTemplate = emailtemplate;

        //        return View(viewModel);
        //    }
        //}
        
        ////
        //// GET: /EmailTemplate/Delete/5 
        //public ActionResult Delete(int id)
        //{
        //    var emailtemplate = _emailtemplateRepository.GetNullableById(id);

        //    if (emailtemplate == null) return this.RedirectToAction(a => a.Index());

        //    return View(emailtemplate);
        //}

        ////
        //// POST: /EmailTemplate/Delete/5
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Delete(int id, EmailTemplate emailtemplate)
        //{
        //    var emailtemplateToDelete = _emailtemplateRepository.GetNullableById(id);

        //    if (emailtemplateToDelete == null) this.RedirectToAction(a => a.Index());

        //    _emailtemplateRepository.Remove(emailtemplateToDelete);

        //    Message = "EmailTemplate Removed Successfully";

        //    return this.RedirectToAction(a => a.Index());
        //}
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(EmailTemplate source, EmailTemplate destination)
        {
            throw new NotImplementedException();
        }

    }


}
