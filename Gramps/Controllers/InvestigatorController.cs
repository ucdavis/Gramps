﻿using System;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Investigator class
    /// </summary>
    [PublicAuthorize]
    public class InvestigatorController : ApplicationController
    {
	    private readonly IRepository<Investigator> _investigatorRepository;
        private readonly IRepository<Proposal> _proposalRepository;

        public InvestigatorController(IRepository<Investigator> investigatorRepository, IRepository<Proposal> proposalRepository)
        {
            _investigatorRepository = investigatorRepository;
            _proposalRepository = proposalRepository;
        }
        
        /// <summary>
        /// #1
        /// GET: /Investigator/Create
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        public ActionResult Create(Guid id) //Proposal ID
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
            if (proposal == null)
            {
                Message = string.Format(StaticValues.Message_NotFound, "Your proposal was");
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.Email != CurrentUser.Identity.Name)
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.IsSubmitted)
            {
                Message = string.Format(StaticValues.Message_ProposalSubmitted, "add investigator to", "proposal");
                return this.RedirectToAction<ProposalController>(a => a.Details(id));
            }
            if (!proposal.CallForProposal.IsActive || proposal.CallForProposal.EndDate < DateTime.Now.Date)
            {
                Message = string.Format(StaticValues.Message_ProposalNotActive, "Cannot add investigator");
                return this.RedirectToAction<ProposalController>(a => a.Edit(id));
            }

			var viewModel = InvestigatorViewModel.Create(Repository, proposal);
            
            return View(viewModel);
        } 

        /// <summary>
        /// #2
        /// POST: /Investigator/Create
        /// </summary>
        /// <param name="id"></param>
        /// <param name="investigator"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Guid id, Investigator investigator)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
            if (proposal == null)
            {
                Message = string.Format(StaticValues.Message_NotFound, "Your proposal was");
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.Email != CurrentUser.Identity.Name)
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.IsSubmitted)
            {
                Message = string.Format(StaticValues.Message_ProposalSubmitted, "add investigator to", "proposal");
                return this.RedirectToAction<ProposalController>(a => a.Details(id));
            }
            if (!proposal.CallForProposal.IsActive || proposal.CallForProposal.EndDate < DateTime.Now.Date)
            {
                Message = string.Format(StaticValues.Message_ProposalNotActive, "Cannot add investigator");
                return this.RedirectToAction<ProposalController>(a => a.Edit(id));
            }


            var investigatorToCreate = new Investigator();
            investigatorToCreate.Proposal = proposal;

            TransferValues(investigator, investigatorToCreate);

            investigatorToCreate.TransferValidationMessagesTo(ModelState);
            if (investigatorToCreate.IsPrimary && Repository.OfType<Investigator>().Queryable.Where(a => a.Proposal == proposal && a.IsPrimary).Any())
            {
                ModelState.AddModelError("Investigator.IsPrimary", "There can only be one primary investigator per proposal.");
            }

            if (ModelState.IsValid)
            {
                _investigatorRepository.EnsurePersistent(investigatorToCreate);

                Message = string.Format(StaticValues.Message_CreatedSuccessfully, "Investigator");

                return this.RedirectToAction<ProposalController>(a => a.Edit(id));
            }
            else
            {
				var viewModel = InvestigatorViewModel.Create(Repository, proposal);
                viewModel.Investigator = investigatorToCreate;

                return View(viewModel);
            }
        }


        //
        // GET: /Investigator/Edit/5
        public ActionResult Edit(int id, Guid proposalId)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == proposalId).SingleOrDefault();
            if (proposal == null)
            {
                Message = "Your proposal was not found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.Email != CurrentUser.Identity.Name)
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.IsSubmitted)
            {
                Message = "Cannot edit proposal once submitted.";
                return this.RedirectToAction<ProposalController>(a => a.Details(proposalId));
            }
            if (!proposal.CallForProposal.IsActive || proposal.CallForProposal.EndDate < DateTime.Now.Date)
            {
                Message = "Proposal is not active or end date has passed. Cannot add investigator.";
                return this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
            }


            var investigator = _investigatorRepository.GetNullableById(id);

            if (investigator == null)
            {
                Message = "Investigator Not Found";
                return this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
            }
            if (investigator.Proposal.Guid != proposalId)
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = InvestigatorViewModel.Create(Repository, proposal);
            viewModel.Investigator = investigator;

            return View(viewModel);
        }

        //
        // POST: /Investigator/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Guid proposalId, Investigator investigator)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == proposalId).SingleOrDefault();
            if (proposal == null)
            {
                Message = "Your proposal was not found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.Email != CurrentUser.Identity.Name)
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.IsSubmitted)
            {
                Message = "Cannot edit proposal once submitted.";
                return this.RedirectToAction<ProposalController>(a => a.Details(proposalId));
            }
            if (!proposal.CallForProposal.IsActive || proposal.CallForProposal.EndDate < DateTime.Now.Date)
            {
                Message = "Proposal is not active or end date has passed. Cannot add investigator.";
                return this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
            }

            var investigatorToEdit = _investigatorRepository.GetNullableById(id);

            if (investigatorToEdit == null)
            {
                Message = "Investigator Not Found";
                return this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
            }
            if (investigatorToEdit.Proposal.Guid != proposalId)
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            TransferValues(investigator, investigatorToEdit);

            investigatorToEdit.TransferValidationMessagesTo(ModelState);

            if (investigatorToEdit.IsPrimary && Repository.OfType<Investigator>().Queryable.Where(a => a.Proposal == proposal && a.IsPrimary && a.Id != investigatorToEdit.Id).Any())
            {
                ModelState.AddModelError("Investigator.IsPrimary", "There can only be one primary investigator per proposal.");
            }

            if (ModelState.IsValid)
            {
                _investigatorRepository.EnsurePersistent(investigatorToEdit);

                Message = "Investigator Edited Successfully";

                return this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
            }
            else
            {
                var viewModel = InvestigatorViewModel.Create(Repository, proposal);
                viewModel.Investigator = investigatorToEdit;

                return View(viewModel);
            }
        }


        //
        // POST: /Investigator/Delete/5
        [HttpPost]
        public ActionResult Delete(int investigatorId, Guid proposalId)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == proposalId).SingleOrDefault();
            if (proposal == null)
            {
                Message = "Your proposal was not found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.Email != CurrentUser.Identity.Name)
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.IsSubmitted)
            {
                Message = "Cannot edit proposal once submitted.";
                return this.RedirectToAction<ProposalController>(a => a.Details(proposalId));
            }
            if (!proposal.CallForProposal.IsActive || proposal.CallForProposal.EndDate < DateTime.Now.Date)
            {
                Message = "Proposal is not active or end date has passed. Cannot remove investigator.";
                return this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
            }

            var investigatorToDelete = _investigatorRepository.GetNullableById(investigatorId);

            if (investigatorToDelete == null || investigatorToDelete.Proposal != proposal)
            {
                Message = "Not Deleted";
                this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
            }

            try
            {
                _investigatorRepository.Remove(investigatorToDelete);

                Message = "Investigator Removed Successfully";
            }
            catch (Exception)
            {
                Message = "Not Deleted";
            }
            return this.RedirectToAction<ProposalController>(a => a.Edit(proposalId));
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Investigator source, Investigator destination)
        {
            destination.Address1 =      source.Address1;
            destination.Address2 =      source.Address2;
            destination.Address3 =      source.Address3;
            destination.City =          source.City;
            destination.Email =         source.Email;
            destination.Institution =   source.Institution;
            destination.IsPrimary =     source.IsPrimary;
            destination.Name =          source.Name;
            destination.Phone =         source.Phone;
            destination.State =         source.State.ToUpper();
            destination.Zip =           source.Zip;
            destination.Position =      source.Position;
        }

    }

	/// <summary>
    /// ViewModel for the Investigator class
    /// </summary>
    public class InvestigatorViewModel
	{
		public Investigator Investigator { get; set; }
        public Proposal Proposal { get; set; }
 
		public static InvestigatorViewModel Create(IRepository repository, Proposal proposal)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(proposal != null);
			
			var viewModel = new InvestigatorViewModel {Investigator = new Investigator(), Proposal = proposal};
 
			return viewModel;
		}
	}
}
