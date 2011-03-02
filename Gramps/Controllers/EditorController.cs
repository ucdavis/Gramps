using System;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Models;
using Gramps.Services;
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
    [UserOnly]
    public class EditorController : ApplicationController
    {
	    private readonly IRepository<Editor> _editorRepository;
        private readonly IAccessService _accessService;
        private readonly IEmailService _emailService;

        public EditorController(IRepository<Editor> editorRepository, IAccessService accessService, IEmailService emailService)
        {
            _editorRepository = editorRepository;
            _accessService = accessService;
            _emailService = emailService;
        }
    
        //
        // GET: /Editor/
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = EditorListViewModel.Create(Repository, templateId, callForProposalId);


            return View(viewModel);
        }

        //
        // GET: /Editor/Details/5
        public ActionResult Details(int id)
        {
            //todo: show all editors, emails, etc.
            var editor = _editorRepository.GetNullableById(id);

            if (editor == null) return this.RedirectToAction(a => a.Index(null, null));

            return View(editor);
        }

        public ActionResult AddEditor(int? templateId, int? callForProposalId)
        {
            Template template = null;
            CallForProposal callforProposal= null;

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
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

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
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

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
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

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue && callForProposalId !=0)
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
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

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
            if (!_accessService.HasSameId(editor.Template, editor.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
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
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

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
            if (!_accessService.HasSameId(editorToEdit.Template, editorToEdit.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            TransferValues(editor, editorToEdit);
            if (callForProposalId.HasValue && callForProposalId.Value != 0)
            {
                editorToEdit.HasBeenNotified = editor.HasBeenNotified;
            }

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
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

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
            if (!_accessService.HasSameId(editorToDelete.Template, editorToDelete.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            if (editorToDelete.CallForProposal != null)
            {
                editorToDelete.CallForProposal.RemoveEditor(editorToDelete);
            }

            _editorRepository.Remove(editorToDelete);

            Message = "Editor Removed Successfully";

            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResetReviewerId(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

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
            if (!_accessService.HasSameId(editorToReset.Template, editorToReset.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            editorToReset.ReviewerId = Guid.NewGuid();
            editorToReset.HasBeenNotified = false;

            _editorRepository.EnsurePersistent(editorToReset);

            Message = "Editor Reset Successfully";

            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
        }


        public ActionResult SendCall(int id)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {

                return this.RedirectToAction<CallForProposalController>(a => a.Index());
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = ReviewersSendViewModel.Create(Repository, callforproposal);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SendCall(int id, bool immediate)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {

                return this.RedirectToAction<CallForProposalController>(a => a.Index());
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }



            var viewModel = ReviewersSendViewModel.Create(Repository, callforproposal);
            if (!callforproposal.IsActive)
            {
                Message = "Is not active";
                return View(viewModel);
            }

            var membershipService = new AccountMembershipService();
            var count = 0;
            foreach (var editor in viewModel.EditorsToNotify.Where(a => !a.HasBeenNotified))
            {
                string tempPass = null;
                if (membershipService.DoesUserExist(editor.ReviewerEmail))
                {
                    //Send an email saying you already exist and can view here

                }
                else
                {
                    membershipService.CreateUser(editor.ReviewerEmail.Trim().ToLower(),
                                                 "Ht548*%KjjY2#",
                                                 editor.ReviewerEmail.Trim().ToLower());
                    tempPass = membershipService.ResetPassword(editor.ReviewerEmail.Trim().ToLower());
                }

                _emailService.SendEmail(Request, Url, callforproposal, callforproposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.ReadyForReview).Single(), editor.ReviewerEmail, immediate, tempPass);
                editor.NotifiedDate = DateTime.Now;
                editor.HasBeenNotified = true;
                _editorRepository.EnsurePersistent(editor);
                count++;
            }

            viewModel = ReviewersSendViewModel.Create(Repository, callforproposal);
            Message = string.Format("{0} Emails Generated", count);
            return View(viewModel);
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
