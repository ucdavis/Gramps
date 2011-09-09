using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using MvcContrib;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Helpers;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Template class
    /// </summary>
    [UserOnly]
    public class TemplateController : ApplicationController
    {
	    private readonly IRepository<Template> _templateRepository;
        private readonly IAccessService _accessService;

        public TemplateController(IRepository<Template> templateRepository, IAccessService accessService)
        {
            _templateRepository = templateRepository;
            _accessService = accessService;
        }
    
        /// <summary>
        /// #1
        /// GET: /Template/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var viewModel = TemplateListViewModel.Create(Repository, CurrentUser.Identity.Name);

            return View(viewModel);
        }

        /// <summary>
        /// #2
        /// GET: /Template/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var viewModel = TemplateViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        /// <summary>
        /// #3
        /// POST: /Template/Create
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Template template)
        {
            var templateToCreate = new Template();
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginId == CurrentUser.Identity.Name).Single();
            

            TransferValues(template, templateToCreate, false);

            templateToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _templateRepository.EnsurePersistent(templateToCreate);

                Message = "Template Created Successfully";

                var editor = new Editor(user, true);
                editor.Template = templateToCreate;
                Repository.OfType<Editor>().EnsurePersistent(editor);

                LoadEmailTemplates(templateToCreate);

                //return RedirectToAction("Index");
                return this.RedirectToAction(a => a.Index());
            }
            else
            {
				var viewModel = TemplateViewModel.Create(Repository);
                viewModel.Template = template;

                return View(viewModel);
            }
        }

        private void LoadEmailTemplates(Template template)
        {
            var dict = new Dictionary<int, EmailTemplateType>();
            dict.Add(0, EmailTemplateType.InitialCall);
            dict.Add(1, EmailTemplateType.ProposalApproved);
            dict.Add(2, EmailTemplateType.ProposalConfirmation);
            dict.Add(3, EmailTemplateType.ProposalDenied);
            dict.Add(4, EmailTemplateType.ReadyForReview);
            dict.Add(5, EmailTemplateType.ReminderCallIsAboutToClose);
            dict.Add(6, EmailTemplateType.ProposalUnsubmitted);
            for (int i = 0; i < 7; i++)
            {
                var emailTemplate = new EmailTemplate(dict[i].ToString() + " Edit This Subject");
                emailTemplate.TemplateType = dict[i];
                emailTemplate.Template = template;

                Repository.OfType<EmailTemplate>().EnsurePersistent(emailTemplate);
            }
        }

        //
        // GET: /Template/Edit/5
        public ActionResult Edit(int id)
        {
            var template = _templateRepository.GetNullableById(id);

            if (template == null) return this.RedirectToAction(a => a.Index());

            if (!_accessService.HasAccess(template.Id, null, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

			var viewModel = TemplateViewModel.Create(Repository);
			viewModel.Template = template;
            viewModel.TemplateId = template.Id;
            viewModel.CallForProposalId = null;

			return View(viewModel);
        }
        
        //
        // POST: /Template/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, Template template)
        {
            var templateToEdit = _templateRepository.GetNullableById(id);

            if (templateToEdit == null) return this.RedirectToAction(a => a.Index());
            if (!_accessService.HasAccess(templateToEdit.Id, null, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            TransferValues(template, templateToEdit);

            templateToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _templateRepository.EnsurePersistent(templateToEdit);

                Message = "Template Edited Successfully";
                var viewModel = TemplateViewModel.Create(Repository);
                viewModel.Template = templateToEdit;
                viewModel.TemplateId = templateToEdit.Id;
                viewModel.CallForProposalId = null;
                return View(viewModel);
                //return this.RedirectToAction(a => a.Index());
            }
            else
            {
				var viewModel = TemplateViewModel.Create(Repository);
                viewModel.Template = templateToEdit;

                return View(viewModel);
            }
        }

        
        ////
        //// GET: /Template/Delete/5 
        //public ActionResult Delete(int id)
        //{
        //    var template = _templateRepository.GetNullableById(id);

        //    if (template == null) return this.RedirectToAction(a => a.Index());

        //    return View(template);
        //}

        ////
        //// POST: /Template/Delete/5
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Delete(int id, Template template)
        //{
        //    var templateToDelete = _templateRepository.GetNullableById(id);

        //    if (templateToDelete == null) return this.RedirectToAction(a => a.Index());

        //    _templateRepository.Remove(templateToDelete);

        //    Message = "Template Removed Successfully";

        //    return this.RedirectToAction(a => a.Index());
        //}
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Template source, Template destination, bool forEdit = true)
        {
            destination.Name = source.Name;
            destination.IsActive = false;
            if (forEdit)
            {
                destination.IsActive = source.IsActive;
                //throw new NotImplementedException("Need to write the copy for edit");
            }
        }

    }


}
