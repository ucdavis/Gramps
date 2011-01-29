using System;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.ViewModels;
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

        public ActionResult AddEditor(int? templateId, int? callForProposalId)
        {
            Template template = null;
            CallForProposal callforProposal= null;

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }


            var viewModel = AddEditorViewModel.Create(Repository, template, callforProposal);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddEditor(int? templateId, int? callForProposalId, int userId)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }

            var user = Repository.OfType<User>().GetNullableById(userId);

            var editor = new Editor(user, false);
            editor.CallForProposal = callforProposal;
            editor.Template = template;

            editor.TransferValidationMessagesTo(ModelState);

            if (editor.Template != null && _editorRepository.Queryable.Where(a => a.User == editor.User && a.Template == template).Any())
            {
                ModelState.AddModelError("User", "User already exists");
                Message = "User already exists";
            }

            if (editor.CallForProposal != null && _editorRepository.Queryable.Where(a => a.User == editor.User && a.CallForProposal == callforProposal).Any())
            {
                ModelState.AddModelError("User", "User already exists");
                Message = "User already exists";
            }
            if (ModelState.IsValid)
            {
                Repository.OfType<Editor>().EnsurePersistent(editor);
                Message = "Editor Added";                
            }
            else
            {
                Message = string.Format("Unable to add editor {0}", Message);
            }

            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));

        }

        //
        // GET: /Editor/Create
        public ActionResult CreateReviewer(int? templateId, int? callForProposalId)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }
			var viewModel = EditorViewModel.Create(Repository, template, callforProposal);
            
            return View(viewModel);
        } 

        //
        // POST: /Editor/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateReviewer(int? templateId, int? callForProposalId, Editor editor)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }

            var editorToCreate = new Editor(editor.ReviewerEmail);

            TransferValues(editor, editorToCreate);
            editorToCreate.Template = template;
            editorToCreate.CallForProposal = callforProposal;

            editorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _editorRepository.EnsurePersistent(editorToCreate);

                Message = "Reviewer Created Successfully";

                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else
            {

                var viewModel = EditorViewModel.Create(Repository, template, callforProposal);
                viewModel.Editor = editorToCreate;

                return View(viewModel);
            }
        }

        //
        // GET: /Editor/Edit/5
        public ActionResult EditReviewer(int id, int? templateId, int? callForProposalId)
        {
            var editor = _editorRepository.GetNullableById(id);

            if (editor == null)
            {
                Message = "Reviewer not found";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else if (editor.User != null)
            {
                Message = "Not a reviewer";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            var viewModel = EditorViewModel.Create(Repository, editor.Template, editor.CallForProposal);
			viewModel.Editor = editor;

			return View(viewModel);
        }
        
        //
        // POST: /Editor/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditReviewer(int id, int? templateId, int? callForProposalId, Editor editor)
        {

            var editorToEdit = _editorRepository.GetNullableById(id);

            if (editor == null)
            {
                Message = "Reviewer not found";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else if (editor.User != null)
            {
                Message = "Not a reviewer";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            TransferValues(editor, editorToEdit);

            editorToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _editorRepository.EnsurePersistent(editorToEdit);

                Message = "Editor Edited Successfully";

                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else
            {
                var viewModel = EditorViewModel.Create(Repository, editorToEdit.Template, editorToEdit.CallForProposal);
                viewModel.Editor = editor;

                return View(viewModel);
            }
        }

        //
        // POST: /Editor/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, int? templateId, int? callForProposalId)
        {
			var editorToDelete = _editorRepository.GetNullableById(id);

            if (editorToDelete == null)
            {
                Message = "Editor Not Found";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            if (editorToDelete.IsOwner)
            {
                Message = "Can't delete owner.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            _editorRepository.Remove(editorToDelete);

            Message = "Editor Removed Successfully";

            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResetReviewerId(int id, int? templateId, int? callForProposalId)
        {
            var editorToReset = _editorRepository.GetNullableById(id);

            if (editorToReset == null)
            {
                Message = "Editor Not Found";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            if (editorToReset.User != null)
            {
                Message = "Can't Reset non Reviewers";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            editorToReset.ReviewerId = Guid.NewGuid();

            _editorRepository.EnsurePersistent(editorToReset);

            Message = "Editor Reset Successfully";

            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Editor source, Editor destination)
        {
            destination.ReviewerEmail = source.ReviewerEmail;
            destination.ReviewerName = source.ReviewerName;
            destination.ReviewerId = source.ReviewerId;
        }

    }




}
