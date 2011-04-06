using System;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the EmailQueue class
    /// </summary>
    [UserOnly]
    public class EmailQueueController : ApplicationController
    {        
        private readonly IRepository<EmailQueue> _emailqueueRepository;
        private readonly IAccessService _accessService;

        public EmailQueueController(IRepository<EmailQueue> emailqueueRepository, IAccessService accessService)
        {
            _emailqueueRepository = emailqueueRepository;
            _accessService = accessService;
        }

        /// <summary>
        /// #1
        /// GET: /EmailQueue/
        /// </summary>
        /// <param name="id">CallForProposal Id</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var viewModel = EmailQueueListViewModel.Create(Repository, callforproposal);

            return View(viewModel);
        }

        /// <summary>
        /// #2
        /// GET: /EmailQueue/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult Details(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var emailqueue = _emailqueueRepository.GetNullableById(id);

            if (emailqueue == null)
            {
                Message = string.Format(StaticValues.Message_NotFound, "Email");
                return this.RedirectToAction(a => a.Index(callForProposalId));
            }
            if (!_accessService.HasSameId(null, callforproposal, null, emailqueue.CallForProposal.Id))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = EmailQueueViewModel.Create(Repository, callforproposal);
            viewModel.EmailQueue = emailqueue;

            return View(viewModel);
        }

        /// <summary>
        /// #3
        /// GET: /EmailQueue/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult Edit(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var emailqueue = _emailqueueRepository.GetNullableById(id);
            if (emailqueue == null)
            {
                Message = string.Format(StaticValues.Message_NotFound, "Email");
                return this.RedirectToAction(a => a.Index(callForProposalId));
            }
            if (!_accessService.HasSameId(null, callforproposal, null, emailqueue.CallForProposal.Id))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }


            var viewModel = EmailQueueViewModel.Create(Repository, callforproposal);
            viewModel.EmailQueue = emailqueue;

            return View(viewModel);
        }

        //
        // POST: /EmailQueue/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, int callForProposalId, EmailQueue emailqueue)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            
            var emailqueueToEdit = _emailqueueRepository.GetNullableById(id);

            if (emailqueueToEdit == null)
            {
                Message = "Email not found";
                return this.RedirectToAction(a => a.Index(callForProposalId));
            }
            if (!_accessService.HasSameId(null, callforproposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var previousPendingState = emailqueueToEdit.Pending;

            TransferValues(emailqueue, emailqueueToEdit);

            emailqueueToEdit.TransferValidationMessagesTo(ModelState);

            if (!emailqueue.Pending && !previousPendingState)
            {
                ModelState.AddModelError("Pending", "Can't edit an email that was already sent");
            }

            if (ModelState.IsValid)
            {
                _emailqueueRepository.EnsurePersistent(emailqueueToEdit);

                Message = "EmailQueue Edited Successfully";

                return this.RedirectToAction(a => a.Index(callForProposalId));
            }
            else
            {
                var viewModel = EmailQueueViewModel.Create(Repository, callforproposal);
                viewModel.EmailQueue = emailqueueToEdit;

                return View(viewModel);
            }
        }



        //
        // POST: /EmailQueue/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, int callForProposalId)
        {
            if (!_accessService.HasAccess(null, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var emailqueueToDelete = _emailqueueRepository.GetNullableById(id);

            if (emailqueueToDelete == null || emailqueueToDelete.CallForProposal == null || emailqueueToDelete.CallForProposal.Id != callForProposalId)
            {
                Message = "Email not deleted";
                this.RedirectToAction(a => a.Index(callForProposalId));
            }
            if (emailqueueToDelete.Pending)
            {
                Message = "Email already sent, can't delete";
                this.RedirectToAction(a => a.Index(callForProposalId));

            }

            _emailqueueRepository.Remove(emailqueueToDelete);

            Message = "EmailQueue Removed Successfully";

            return this.RedirectToAction(a => a.Index(callForProposalId));
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(EmailQueue source, EmailQueue destination)
        {
            destination.Body = source.Body;
            destination.Immediate = source.Immediate;
            destination.Pending = source.Pending;
            destination.Subject = source.Subject;
        }

    }


}
