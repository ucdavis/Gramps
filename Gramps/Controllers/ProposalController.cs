using System;
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
        public ActionResult Details(int id)
        {
            var proposal = _proposalRepository.GetNullableById(id);

            if (proposal == null) return this.RedirectToAction(a => a.Index());

            return View(proposal);
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

                return this.RedirectToAction(a => a.Confirmation(proposalToCreate.Email));
            }
            else
            {
                var viewModel = ProposalViewModel.Create(Repository, callforproposal);
                viewModel.Proposal = proposalToCreate;

                return View(viewModel);
            }
        }

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
