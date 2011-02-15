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
using System.Linq;

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

        //
        // GET: /CallForProposal/
        public ActionResult Index()
        {
            var callforproposalList = Repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal != null && a.User != null && a.User.LoginId == CurrentUser.Identity.Name).Select(x => x.CallForProposal).Distinct();


            return View(callforproposalList);
        }

        ////
        //// GET: /CallForProposal/Details/5
        //public ActionResult Details(int id)
        //{
        //    var callforproposal = _callforproposalRepository.GetNullableById(id);

        //    if (callforproposal == null) return this.RedirectToAction(a => a.Index());

        //    return View(callforproposal);
        //}

        //
        // GET: /CallForProposal/Create
        public ActionResult Create()
        {

			var viewModel = CallForProposalCreateViewModel.Create(Repository, CurrentUser.Identity.Name);
            
            return View(viewModel);
        } 

        //
        // POST: /CallForProposal/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(int templateId)
        {
            var user = Repository.OfType<User>().Queryable.Where(a => a.LoginId == CurrentUser.Identity.Name).Single();
            if (!_accessService.HasAccess(templateId, null, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that template.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var template = Repository.OfType<Template>().GetNullableById(templateId);
            
            var callforproposalToCreate = new CallForProposal(template, user);
            callforproposalToCreate.EndDate = DateTime.Now.AddMonths(1);
            callforproposalToCreate.IsActive = false;

            callforproposalToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _callforproposalRepository.EnsurePersistent(callforproposalToCreate);

                Message = "CallForProposal Created Successfully";

                return this.RedirectToAction(a => a.Index());
            }
            else
            {
                Message = "There was a problem selecting that template";
				var viewModel = CallForProposalCreateViewModel.Create(Repository, CurrentUser.Identity.Name);

                return View(viewModel);
            }
        }

        //
        // GET: /CallForProposal/Edit/5
        public ActionResult Edit(int id)
        {
            var callforproposal = _callforproposalRepository.GetNullableById(id);

            if (callforproposal == null) return this.RedirectToAction(a => a.Index());

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

			var viewModel = CallForProposalViewModel.Create(Repository);
			viewModel.CallForProposal = callforproposal;          
            viewModel.CallForProposalId = callforproposal.Id;
            viewModel.TemplateId = null;

			return View(viewModel);
        }
        
        //
        // POST: /CallForProposal/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, CallForProposal callforproposal)
        {
            var callforproposalToEdit = _callforproposalRepository.GetNullableById(id);

            if (callforproposalToEdit == null) return this.RedirectToAction(a => a.Index());

            TransferValues(callforproposal, callforproposalToEdit);

            callforproposalToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _callforproposalRepository.EnsurePersistent(callforproposalToEdit);

                Message = "CallForProposal Edited Successfully";

                return this.RedirectToAction(a => a.Index());
            }
            else
            {
				var viewModel = CallForProposalViewModel.Create(Repository);
                viewModel.CallForProposal = callforproposal;

                return View(viewModel);
            }
        }
        
        //
        // GET: /CallForProposal/Delete/5 
        public ActionResult Delete(int id)
        {
			var callforproposal = _callforproposalRepository.GetNullableById(id);

            if (callforproposal == null) return this.RedirectToAction(a => a.Index());

            return View(callforproposal);
        }

        //
        // POST: /CallForProposal/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, CallForProposal callforproposal)
        {
			var callforproposalToDelete = _callforproposalRepository.GetNullableById(id);

            if (callforproposalToDelete == null) this.RedirectToAction(a => a.Index());

            _callforproposalRepository.Remove(callforproposalToDelete);

            Message = "CallForProposal Removed Successfully";

            return this.RedirectToAction(a => a.Index());
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(CallForProposal source, CallForProposal destination)
        {
            destination.Name = source.Name;
            destination.EndDate = source.EndDate;
            destination.IsActive = source.IsActive;
        }

    }


}
