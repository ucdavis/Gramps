﻿using System;
using System.Web;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;
using System.Linq;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Proposal class
    /// </summary>
    public class ProposalController : ApplicationController
    {
	    private readonly IRepository<Proposal> _proposalRepository;
        private readonly IAccessService _accessService;
        private readonly IEmailService _emailService;

        public ProposalController(IRepository<Proposal> proposalRepository, IAccessService accessService, IEmailService emailService)
        {
            _proposalRepository = proposalRepository;
            _accessService = accessService;
            _emailService = emailService;
        }

        //
        // GET: /Proposal/
        public ActionResult Index()
        {
            return this.RedirectToAction<HomeController>(a => a.About());
        }

        [UserOnly]
        public ActionResult AdminIndex(int id)
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
            var viewModel = ProposalAdminListViewModel.Create(Repository, callforproposal, CurrentUser.Identity.Name);

            return View(viewModel);
        }

        //
        // GET: /Proposal/Details/5
        [UserOnly]
        public ActionResult Details(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index());
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            if(!_accessService.HasSameId(null, callforproposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var proposal = _proposalRepository.GetNullableById(id);

            if (proposal == null)
            {
                Message = "Proposal Not Found";
                return this.RedirectToAction(a => a.Index());
            }
            var editor = Repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal == callforproposal && a.User != null && a.User.LoginId == CurrentUser.Identity.Name).Single();
            var reviewed = Repository.OfType<ReviewedProposal>().Queryable.Where(a => a.Proposal == proposal && a.Editor == editor).FirstOrDefault();
            if (reviewed == null)
            {
                reviewed = new ReviewedProposal(proposal, editor);
            }
            else
            {
                reviewed.LastViewedDate = DateTime.Now;
            }
            Repository.OfType<ReviewedProposal>().EnsurePersistent(reviewed);

            var viewModel = ProposalAdminViewModel.Create(Repository, callforproposal, proposal);

            return View(viewModel);
        }

        [HttpPost]
        [UserOnly]
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

            if (!callforproposal.IsActive || callforproposal.EndDate.Date <= DateTime.Now.Date)
            {
                Message = "Is not active or end date is passed";
                return this.RedirectToAction(a => a.AdminIndex(id));
            }


            var count = 0;

            var proposals = callforproposal.Proposals.Where(a => !a.IsSubmitted && !a.WasWarned);

            foreach (var proposal in proposals)
            {
                _emailService.SendEmail(callforproposal, callforproposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.ReminderCallIsAboutToClose).Single(), proposal.Email, immediate);
                proposal.WasWarned = true;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
                count++;
            }
            Message = string.Format("{0} Emails Generated", count);

            return this.RedirectToAction(a => a.AdminIndex(id));
        }


        //
        // GET: /Proposal/Create
        public ActionResult Create(int id)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null || !callforproposal.IsActive || callforproposal.EndDate.Date <= DateTime.Now.Date)
            {
                Message = "Grant No longer Available";
                return this.RedirectToAction<HomeController>(a => a.About());
            }

			var viewModel = ProposalViewModel.Create(Repository, callforproposal);
            
            return View(viewModel);
        }

        
        // POST: /Proposal/Create
        [AcceptVerbs(HttpVerbs.Post)]
        [CaptchaValidator]
        public ActionResult Create(int id, bool captchaValid, Proposal proposal)
        {

            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null || !callforproposal.IsActive || callforproposal.EndDate.Date <= DateTime.Now.Date)
            {
                Message = "Grant No longer Available";
                return this.RedirectToAction<HomeController>(a => a.About());
            }

            var proposalToCreate = new Proposal();

            proposalToCreate.Email = proposal.Email;
            proposalToCreate.CallForProposal = callforproposal;

            proposalToCreate.TransferValidationMessagesTo(ModelState);

            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Recaptcha value not valid");
            }

            if (ModelState.IsValid)
            {
                _proposalRepository.EnsurePersistent(proposalToCreate);

                Message = "Proposal Created Successfully";

                _emailService.SendConfirmation(Request, Url, proposalToCreate, proposalToCreate.CallForProposal.EmailTemplates.Where(a => a.TemplateType==EmailTemplateType.ProposalConfirmation).Single(), true);

                return this.RedirectToAction(a => a.Confirmation(proposalToCreate.Email));
            }
            else
            {
                var viewModel = ProposalViewModel.Create(Repository, callforproposal);
                viewModel.Proposal = proposalToCreate;

                return View(viewModel);
            }
        }

        //private string GetAbsoluteUrl(string relative)
        //{
        //    return string.Format("{0}://{1}:{2}{3}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port, Url.Content(relative));
        //}



        public ActionResult Confirmation(string proposalEmail)
        {
            var viewModel = ProposalConfirmationViewModel.Create(proposalEmail);
            return View(viewModel);
        }

        //
        // GET: /Proposal/Edit/5
        public ActionResult Edit(int id)
        {
            var proposal = _proposalRepository.GetNullableById(id);

            if (proposal == null) return this.RedirectToAction(a => a.Index());

			var viewModel = ProposalViewModel.Create(Repository, null);
			viewModel.Proposal = proposal;

			return View(viewModel);
        }
        
        //
        // POST: /Proposal/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, Proposal proposal)
        {
            var proposalToEdit = _proposalRepository.GetNullableById(id);

            if (proposalToEdit == null) return this.RedirectToAction(a => a.Index());

            TransferValues(proposal, proposalToEdit);

            proposalToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _proposalRepository.EnsurePersistent(proposalToEdit);

                Message = "Proposal Edited Successfully";

                return this.RedirectToAction(a => a.Index());
            }
            else
            {
				var viewModel = ProposalViewModel.Create(Repository, null);
                viewModel.Proposal = proposal;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Proposal/Delete/5 
        public ActionResult Delete(int id)
        {
			var proposal = _proposalRepository.GetNullableById(id);

            if (proposal == null) return this.RedirectToAction(a => a.Index());

            return View(proposal);
        }

        //
        // POST: /Proposal/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, Proposal proposal)
        {
			var proposalToDelete = _proposalRepository.GetNullableById(id);

            if (proposalToDelete == null) this.RedirectToAction(a => a.Index());

            _proposalRepository.Remove(proposalToDelete);

            Message = "Proposal Removed Successfully";

            return this.RedirectToAction(a => a.Index());
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Proposal source, Proposal destination)
        {
            throw new NotImplementedException();
        }

    }


}
