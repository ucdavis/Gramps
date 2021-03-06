﻿using System;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Services;
using MvcContrib;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Helpers;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the CallForProposal class
    /// </summary>
    [UserOnly]
    public class CallForProposalController : ApplicationController
    {
	    private readonly IRepository<CallForProposal> _callforproposalRepository;
        private readonly IAccessService _accessService;

        public CallForProposalController(IRepository<CallForProposal> callforproposalRepository, IAccessService accessService)
        {
            _callforproposalRepository = callforproposalRepository;
            _accessService = accessService;
        }

        /// <summary>
        /// #1
        /// GET: /CallForProposal/
        /// </summary>
        /// <param name="filterActive"></param>
        /// <param name="filterStartCreate"></param>
        /// <param name="filterEndCreate"></param>
        /// <returns></returns>
        public ActionResult Index(string filterActive, DateTime? filterStartCreate, DateTime? filterEndCreate)
        {
            //var callforproposalList = Repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal != null && a.User != null && a.User.LoginId == CurrentUser.Identity.Name).Select(x => x.CallForProposal).Distinct();
            var viewModel = CallForProposalListViewModel.Create(Repository, CurrentUser.Identity.Name, filterActive, filterStartCreate, filterEndCreate);

            return View(viewModel);
        }

      
        /// <summary>
        /// #2
        /// Launch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Launch(int id)
        {
            var callforproposal = _callforproposalRepository.GetNullableById(id);

            if (callforproposal == null) return this.RedirectToAction(a => a.Index(null, null, null));

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var viewModel = CallNavigationViewModel.Create(callforproposal);

            return View(viewModel);

        }

        /// <summary>
        /// #3
        /// GET: /CallForProposal/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
			var viewModel = CallForProposalCreateViewModel.Create(Repository, CurrentUser.Identity.Name);
            
            return View(viewModel);
        } 

        /// <summary>
        /// #4
        /// POST: /CallForProposal/Create
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int templateId)
        {
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginId == CurrentUser.Identity.Name).Single();
            if (!_accessService.HasAccess(templateId, null, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that template");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var template = Repository.OfType<Template>().GetNullableById(templateId);
            
            var callforproposalToCreate = new CallForProposal(template, user);
            callforproposalToCreate.EndDate = DateTime.Now.AddMonths(1).Date;
            callforproposalToCreate.IsActive = false;

            callforproposalToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _callforproposalRepository.EnsurePersistent(callforproposalToCreate);

                Message = string.Format(StaticValues.Message_CreatedSuccessfully, "Call For Proposal");

                return this.RedirectToAction(a => a.Edit(callforproposalToCreate.Id));
            }
            else
            {
                Message = string.Format(StaticValues.Message_ProblemSelecting, "that template");
				var viewModel = CallForProposalCreateViewModel.Create(Repository, CurrentUser.Identity.Name);

                return View(viewModel);
            }
        }

        /// <summary>
        /// #5
        /// GET: /CallForProposal/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var callforproposal = _callforproposalRepository.GetNullableById(id);

            if (callforproposal == null) return this.RedirectToAction(a => a.Index(null, null, null));

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

			var viewModel = CallForProposalViewModel.Create(Repository);
			viewModel.CallForProposal = callforproposal;          
            viewModel.CallForProposalId = callforproposal.Id;
            viewModel.TemplateId = null;

			return View(viewModel);
        }
        
        /// <summary>
        /// #6
        /// POST: /CallForProposal/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callforproposal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, CallForProposal callforproposal)
        {
            var callforproposalToEdit = _callforproposalRepository.GetNullableById(id);

            if (callforproposalToEdit == null)
            {
                return this.RedirectToAction(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposalToEdit.Id, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            TransferValues(callforproposal, callforproposalToEdit);

            callforproposalToEdit.TransferValidationMessagesTo(ModelState);

            //if (callforproposalToEdit.ProposalMaximum < 0.01m)
            //{
            //    ModelState.AddModelError("CallForProposal.ProposalMaximum", "You need to specify a Proposal Maximum of at least a cent");
            //}

            if (callforproposalToEdit.IsActive && string.IsNullOrWhiteSpace(callforproposalToEdit.Description))
            {
                ModelState.AddModelError("CallForProposal.Description", "Please supply a description");
            }

            if (ModelState.IsValid)
            {
                _callforproposalRepository.EnsurePersistent(callforproposalToEdit);

                Message = string.Format(StaticValues.Message_EditedSuccessfully, "Call For Proposal");//"CallForProposal Edited Successfully";

                return this.RedirectToAction(a => a.Edit(callforproposalToEdit.Id));
            }
            else
            {
				var viewModel = CallForProposalViewModel.Create(Repository);
                viewModel.CallForProposal = callforproposalToEdit;
                viewModel.CallForProposalId = callforproposalToEdit.Id;
                viewModel.TemplateId = null;

                return View(viewModel);
            }
        }       
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(CallForProposal source, CallForProposal destination)
        {
            destination.Name = source.Name;
            destination.EndDate = source.EndDate;
            destination.IsActive = source.IsActive;
            destination.ProposalMaximum = source.ProposalMaximum;
            destination.Description = source.Description;
            destination.HideInvestigators = source.HideInvestigators;
        }

    }


}
