using System;
using System.Linq;
using System.Web.Mvc;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Editor class
    /// </summary>
    public class EditorController : ApplicationController
    {
	    private readonly IRepository<Editor> _editorRepository;

        public EditorController(IRepository<Editor> editorRepository)
        {
            _editorRepository = editorRepository;
        }
    
        //
        // GET: /Editor/
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            var viewModel = EditorListViewModel.Create(Repository, templateId, callForProposalId);
            //TODO: Check Editor Access Rights

            return View(viewModel);
        }

        //
        // GET: /Editor/Details/5
        public ActionResult Details(int id)
        {
            var editor = _editorRepository.GetNullableById(id);

            if (editor == null) return this.RedirectToAction(a => a.Index(null, null));

            return View(editor);
        }

        //
        // GET: /Editor/Create
        public ActionResult Create()
        {
			var viewModel = EditorViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Editor/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Editor editor)
        {
            var editorToCreate = new Editor();

            TransferValues(editor, editorToCreate);

            editorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _editorRepository.EnsurePersistent(editorToCreate);

                Message = "Editor Created Successfully";

                return this.RedirectToAction(a => a.Index(null, null));
            }
            else
            {
				var viewModel = EditorViewModel.Create(Repository);
                viewModel.Editor = editor;

                return View(viewModel);
            }
        }

        //
        // GET: /Editor/Edit/5
        public ActionResult Edit(int id)
        {
            var editor = _editorRepository.GetNullableById(id);

            //if (editor == null) return this.RedirectToAction(a => a.Index());

			var viewModel = EditorViewModel.Create(Repository);
			viewModel.Editor = editor;

			return View(viewModel);
        }
        
        //
        // POST: /Editor/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, Editor editor)
        {
            var editorToEdit = _editorRepository.GetNullableById(id);

            //if (editorToEdit == null) return this.RedirectToAction(a => a.Index());

            TransferValues(editor, editorToEdit);

            editorToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _editorRepository.EnsurePersistent(editorToEdit);

                Message = "Editor Edited Successfully";

                //return this.RedirectToAction(a => a.Index());
                var viewModel = EditorViewModel.Create(Repository);
                viewModel.Editor = editor;
                return View(viewModel);
            }
            else
            {
				var viewModel = EditorViewModel.Create(Repository);
                viewModel.Editor = editor;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Editor/Delete/5 
        public ActionResult Delete(int id)
        {
			var editor = _editorRepository.GetNullableById(id);

            //if (editor == null) return this.RedirectToAction(a => a.Index());

            return View(editor);
        }

        //
        // POST: /Editor/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, Editor editor)
        {
			var editorToDelete = _editorRepository.GetNullableById(id);

            if (editorToDelete == null) this.RedirectToAction(a => a.Index(null, null));

            _editorRepository.Remove(editorToDelete);

            Message = "Editor Removed Successfully";

            return this.RedirectToAction(a => a.Index(null, null));
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Editor source, Editor destination)
        {
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the Editor class
    /// </summary>
    public class EditorViewModel
	{
		public Editor Editor { get; set; }
 
		public static EditorViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new EditorViewModel {Editor = new Editor()};
 
			return viewModel;
		}
	}

    public class EditorListViewModel
    {
        public IQueryable<Editor> editorList;
        public bool isTemplate = false;
        public bool isCallForProposal = false;
        public int? templateId = 0;
        public int? callForProposalId = 0;

        public static EditorListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new EditorListViewModel();

            if (templateId != null)
            {
                viewModel.isTemplate = true;
                viewModel.editorList = repository.OfType<Editor>().Queryable.Where(a => a.Template.Id == templateId);
                viewModel.templateId = templateId;
            }
            else if(callForProposalId != null)
            {
                viewModel.isCallForProposal = true;
                viewModel.editorList = repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal.Id == callForProposalId);
                viewModel.callForProposalId = callForProposalId;
            }

            return viewModel;
        }
    }
}
