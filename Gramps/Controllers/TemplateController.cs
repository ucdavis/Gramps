using System;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Template class
    /// </summary>
    [UserOnly]
    public class TemplateController : ApplicationController
    {
	    private readonly IRepository<Template> _templateRepository;

        public TemplateController(IRepository<Template> templateRepository)
        {
            _templateRepository = templateRepository;
        }
    
        //
        // GET: /Template/
        public ActionResult Index()
        {
            var templateList = _templateRepository.Queryable;

            return View(templateList);
        }

        //
        // GET: /Template/Details/5
        public ActionResult Details(int id)
        {
            var template = _templateRepository.GetNullableById(id);

            if (template == null) return this.RedirectToAction(a => a.Index());

            return View(template);
        }

        //
        // GET: /Template/Create
        public ActionResult Create()
        {
            var viewModel = TemplateViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Template/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Template template)
        {
            var templateToCreate = new Template();

            TransferValues(template, templateToCreate, false);

            templateToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _templateRepository.EnsurePersistent(templateToCreate);

                Message = "Template Created Successfully";

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

        //
        // GET: /Template/Edit/5
        public ActionResult Edit(int id)
        {
            var template = _templateRepository.GetNullableById(id);

            if (template == null) return this.RedirectToAction(a => a.Index());

			var viewModel = TemplateViewModel.Create(Repository);
			viewModel.Template = template;

			return View(viewModel);
        }
        
        //
        // POST: /Template/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, Template template)
        {
            var templateToEdit = _templateRepository.GetNullableById(id);

            if (templateToEdit == null) return this.RedirectToAction(a => a.Index());

            TransferValues(template, templateToEdit);

            templateToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _templateRepository.EnsurePersistent(templateToEdit);

                Message = "Template Edited Successfully";

                return this.RedirectToAction(a => a.Index());
            }
            else
            {
				var viewModel = TemplateViewModel.Create(Repository);
                viewModel.Template = template;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Template/Delete/5 
        public ActionResult Delete(int id)
        {
			var template = _templateRepository.GetNullableById(id);

            if (template == null) return this.RedirectToAction(a => a.Index());

            return View(template);
        }

        //
        // POST: /Template/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, Template template)
        {
			var templateToDelete = _templateRepository.GetNullableById(id);

            if (templateToDelete == null) return this.RedirectToAction(a => a.Index());

            _templateRepository.Remove(templateToDelete);

            Message = "Template Removed Successfully";

            return this.RedirectToAction(a => a.Index());
        }
        
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
                throw new NotImplementedException("Need to write the copy for edit");
            }
        }

    }

	/// <summary>
    /// ViewModel for the Template class
    /// </summary>
    public class TemplateViewModel
	{
		public Template Template { get; set; }
 
		public static TemplateViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new TemplateViewModel {Template = new Template()};
 
			return viewModel;
		}
	}
}
