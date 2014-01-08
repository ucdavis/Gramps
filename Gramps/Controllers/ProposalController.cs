using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Models;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;
using System.Linq;
using File = Gramps.Core.Domain.File;

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
        private readonly IMembershipService _membershipService;
        private readonly IAnswerService _answerService;

        public ProposalController(IRepository<Proposal> proposalRepository, IAccessService accessService, IEmailService emailService, IMembershipService membershipService, IAnswerService answerService)
        {
            _proposalRepository = proposalRepository;
            _accessService = accessService;
            _emailService = emailService;
            _membershipService = membershipService;
            _answerService = answerService;
        }

        /// <summary>
        /// #1
        /// GET: /Proposal/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return this.RedirectToAction<HomeController>(a => a.About());
        }

        #region Admin(User) Methods
        
        /// <summary>
        /// #2
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filterDecission"></param>
        /// <param name="filterNotified"></param>
        /// <param name="filterSubmitted"></param>
        /// <param name="filterWarned"></param>
        /// <param name="filterEmail"></param>
        /// <returns></returns>
        [UserOnly]
        public ActionResult AdminIndex(int id, string filterDecission, string filterNotified, string filterSubmitted, string filterWarned, string filterEmail)
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
            var viewModel = ProposalAdminListViewModel.Create(Repository, 
                callforproposal, 
                CurrentUser.Identity.Name,
                filterDecission,         
                filterNotified,
                filterSubmitted,
                filterWarned,
                filterEmail);

            return View(viewModel);
        }

        /// <summary>
        /// #3
        /// GET: /Proposal/Details/5
        /// </summary>
        /// <param name="id">Proposal Id</param>
        /// <param name="callForProposalId">CallForProposal Id </param>
        /// <returns></returns>
        [UserOnly]
        public ActionResult AdminDetails(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
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

            if (!_accessService.HasSameId(null, proposal.CallForProposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
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

        /// <summary>
        /// #4
        /// </summary>
        /// <param name="id">Proposal Id</param>
        /// <param name="callForProposalId">Call For Proposal Id</param>
        /// <returns>Proposal's PDF if found</returns>
        [UserOnly]
        public ActionResult AdminDownload(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
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
            
            if (!_accessService.HasSameId(null, proposal.CallForProposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (proposal.File == null || proposal.File.Contents == null)
            {
                Message = "Proposal PDF Not Found";
                return this.RedirectToAction(a => a.Index());
            }

            return File(proposal.File.Contents, proposal.File.ContentType, proposal.File.Name);
        }

        /// <summary>
        /// #5
        /// Send Call that the CallForProposals is about to close.
        /// </summary>
        /// <param name="id">CallForProposal Id</param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        [HttpPost]
        [UserOnly]
        public ActionResult SendCall(int id, bool immediate)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (!callforproposal.IsActive || callforproposal.EndDate.Date <= DateTime.Now.Date)
            {
                Message = "Is not active or end date is passed";
                return this.RedirectToAction(a => a.AdminIndex(id, null, null, null, null, null));
            }


            var count = 0;

            var proposals = callforproposal.Proposals.Where(a => !a.IsSubmitted && !a.WasWarned);

            foreach (var proposal in proposals)
            {
                _emailService.SendProposalEmail(Request, Url, proposal, callforproposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.ReminderCallIsAboutToClose).Single(), immediate);
                proposal.WasWarned = true;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
                count++;
            }
            Message = string.Format("{0} Emails Generated", count);

            return this.RedirectToAction(a => a.AdminIndex(id, null, null, null, null, null));
        }

        /// <summary>
        /// #6
        /// </summary>
        /// <param name="id"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        [HttpPost]
        [UserOnly]
        public ActionResult SendDecision(int id, bool immediate)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {

                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (!callforproposal.IsActive || callforproposal.EndDate.Date > DateTime.Now.Date)
            {
                Message = "Is not active or end date is not passed";
                return this.RedirectToAction(a => a.AdminIndex(id, null, null, null, null, null));
            }


            var approvedCount = 0;
            var deniedCount = 0;


            var proposals = callforproposal.Proposals.Where(a => !a.IsNotified && (a.IsApproved || a.IsDenied));

            foreach (var proposal in proposals)
            {
                if(proposal.IsApproved)
                {
                    _emailService.SendProposalEmail(Request, Url, proposal, callforproposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.ProposalApproved).Single(), immediate);
                    approvedCount++;
                }
                else if(proposal.IsDenied)
                {
                    _emailService.SendProposalEmail(Request, Url, proposal, callforproposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.ProposalDenied).Single(), immediate);
                    deniedCount++;
                }
                else
                {
                    continue;
                }
                proposal.IsNotified = true;
                proposal.NotifiedDate = DateTime.Now;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);                
            }
            Message = string.Format("{0} Emails Generated. {1} Approved {2} Denied", approvedCount + deniedCount, approvedCount, deniedCount);

            return this.RedirectToAction(a => a.AdminIndex(id, null, null, null, null, null));
        }

        /// <summary>
        /// #7
        /// </summary>
        /// <param name="id">Proposal Id</param>
        /// <param name="callForProposalId">callForProposal Id</param>
        /// <returns></returns>
        [UserOnly]
        public ActionResult AdminEdit(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
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

            if (!_accessService.HasSameId(null, proposal.CallForProposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
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

            viewModel.Comment = Repository.OfType<Comment>().Queryable
                .Where(a => a.Proposal == proposal && a.Editor == editor).FirstOrDefault();

            return View(viewModel);
        }

        /// <summary>
        /// #8
        /// </summary>
        /// <param name="id">Proposal Id</param>
        /// <param name="callForProposalId">callForProposal Id</param>
        /// <param name="proposal"></param>
        /// <param name="comment"></param>
        /// <param name="approvedDenied"></param>
        /// <returns></returns>
        [UserOnly]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminEdit(int id, int callForProposalId, Proposal proposal, Comment comment, string approvedDenied)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }


            var proposalToEdit = _proposalRepository.GetNullableById(id);

            if (proposalToEdit == null)
            {
                Message = "Proposal Not Found";
                return this.RedirectToAction(a => a.Index());
            }

            if (!_accessService.HasSameId(null, proposalToEdit.CallForProposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var saveIsSubmitted = proposalToEdit.IsSubmitted;


            var editor = Repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal == callforproposal && a.User != null && a.User.LoginId == CurrentUser.Identity.Name).Single();
            var commentToEdit = Repository.OfType<Comment>().Queryable
                .Where(a => a.Proposal == proposalToEdit && a.Editor == editor).FirstOrDefault();
            if (commentToEdit == null)
            {
                commentToEdit = new Comment(proposalToEdit, editor, string.Empty);
            }

            if (approvedDenied == StaticValues.RB_Decission_Approved)
            {
                proposal.IsApproved = true;
                proposal.IsDenied = false;
            }
            else if(approvedDenied == StaticValues.RB_Decission_Denied)
            {
                proposal.IsApproved = false;
                proposal.IsDenied = true;
            }
            else if(approvedDenied == StaticValues.RB_Decission_NotDecided)
            {
                proposal.IsApproved = false;
                proposal.IsDenied = false;
            }
            else
            {
                throw new ApplicationException("Error with parameter");
            }

            if (proposalToEdit.IsNotified && proposal.IsNotified)
            {
                if (proposalToEdit.IsApproved != proposal.IsApproved || proposalToEdit.IsDenied != proposal.IsDenied)
                {
                    ModelState.AddModelError("Proposal.IsNotified", "You should not change the Decission if they have been notified.");
                }
            }


            AdminTransferValues(proposal, proposalToEdit, comment, commentToEdit);

            proposalToEdit.TransferValidationMessagesTo(ModelState);
            commentToEdit.TransferValidationMessagesTo(ModelState);

            if (proposalToEdit.IsApproved && proposalToEdit.IsDenied) //Should not be able to happen
            {
                ModelState.AddModelError("IsApproved", "IsApproved and IsDenied can not both be checked");
            }
            if (!proposalToEdit.IsSubmitted && proposalToEdit.IsApproved)
            {
                ModelState.AddModelError("IsApproved", "Can not approve an unsubmitted proposal");
            }

            if (saveIsSubmitted && !proposalToEdit.IsSubmitted && (proposalToEdit.IsApproved || proposalToEdit.IsDenied))
            {
                ModelState.AddModelError("Proposal.IsSubmitted", "Can not unsubmit unless the decission is undecided");
            }

            if (ModelState.IsValid)
            {
                _proposalRepository.EnsurePersistent(proposalToEdit);
                if(commentToEdit.Text != null) //This shouldn't break anything...
                {
                    Repository.OfType<Comment>().EnsurePersistent(commentToEdit);
                }

                if (saveIsSubmitted && !proposalToEdit.IsSubmitted)
                {
                    _emailService.SendProposalEmail(Request, Url, proposalToEdit, callforproposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.ProposalUnsubmitted).Single(), false);
                }                

                Message = "Proposal successfully edited";
                return this.RedirectToAction(a => a.AdminIndex(callForProposalId, null, null, null, null, null));
            }
            else
            {
                var viewModel = ProposalAdminViewModel.Create(Repository, callforproposal, proposalToEdit);

                viewModel.Comment = commentToEdit;

                return View(viewModel);

            }

            
        }

        #endregion Admin(User) Methods

        #region Reviewer Methods
        /// <summary>
        /// #9
        /// </summary>
        /// <param name="id">CallForProposal Id</param>
        /// <param name="filterDecission"></param>
        /// <param name="filterEmail"></param>
        /// <returns></returns>
        [PublicAuthorize]
        public ActionResult ReviewerIndex(int id, string filterDecission, string filterEmail)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!callforproposal.IsReviewer(CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }
            var viewModel = ProposalReviewerListViewModel.Create(Repository, callforproposal, CurrentUser.Identity.Name, filterDecission, filterEmail);

            return View(viewModel);
        }

        /// <summary>
        /// #10
        /// </summary>
        /// <param name="id">Proposal Id</param>
        /// <param name="callForProposalId">CallForProposal Id</param>
        /// <returns></returns>
        [PublicAuthorize]
        public ActionResult ReviewerDetails(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!callforproposal.IsReviewer(CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }


            var proposal = _proposalRepository.GetNullableById(id);

            if (proposal == null)
            {
                Message = "Proposal Not Found";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!_accessService.HasSameId(null, proposal.CallForProposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!proposal.IsSubmitted)
            {
                Message = "Proposal Not Submitted Yet";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }


            var editor = Repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal == callforproposal && a.ReviewerEmail == CurrentUser.Identity.Name).First();
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

        /// <summary>
        /// #11
        /// </summary>
        /// <param name="id">Proposal Id</param>
        /// <param name="callForProposalId">CallForProposal Id</param>
        /// <returns></returns>
        [PublicAuthorize]
        public ActionResult ReviewerDownload(int id, int callForProposalId)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callforproposal == null)
            {
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!callforproposal.IsReviewer(CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }


            var proposal = _proposalRepository.GetNullableById(id);

            if (proposal == null)
            {
                Message = "Proposal Not Found";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!_accessService.HasSameId(null, proposal.CallForProposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!proposal.IsSubmitted)
            {
                Message = "Proposal Not Submitted Yet";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (proposal.File == null || proposal.File.Contents == null)
            {
                Message = "Proposal PDF Not Found";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            return File(proposal.File.Contents, proposal.File.ContentType, proposal.File.Name);
        }

        #endregion Reviewer Methods

        #region Public Methods (Proposer)
               
        /// <summary>
        /// #12
        /// GET: /Proposal/Create
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null || !callforproposal.IsActive || callforproposal.EndDate.Date < DateTime.Now.Date)
            {
                Message = "Grant No longer Available";
                return this.RedirectToAction<HomeController>(a => a.About());
            }

			var viewModel = ProposalViewModel.Create(Repository, callforproposal);
            
            return View(viewModel);
        }

        
        /// <summary>
        /// #13
        /// POST: /Proposal/Create
        /// </summary>
        /// <param name="id"></param>
        /// <param name="captchaValid"></param>
        /// <param name="proposal"></param>
        /// <returns></returns>
        [HttpPost]
        [CaptchaValidator]
        public ActionResult Create(int id, bool captchaValid, Proposal proposal)
        {

            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null || !callforproposal.IsActive || callforproposal.EndDate.Date < DateTime.Now.Date)
            {
                Message = "Grant No longer Available";
                return this.RedirectToAction<HomeController>(a => a.About());
            }

            var proposalToCreate = new Proposal();
            if (proposal.Email != null) 
            { 
                proposalToCreate.Email = proposal.Email.Trim().ToLower();
            }
            proposalToCreate.CallForProposal = callforproposal;
            proposalToCreate.Sequence =
                Repository.OfType<Proposal>().Queryable
                .Where(a => a.CallForProposal == callforproposal)
                .Max(a => a.Sequence) + 1;

            proposalToCreate.TransferValidationMessagesTo(ModelState);

            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Recaptcha value not valid");
            }

            if (ModelState.IsValid)
            {
                _proposalRepository.EnsurePersistent(proposalToCreate);

                Message = "Proposal Created Successfully";

                string tempPass = null;
                if (!_membershipService.DoesUserExist(proposalToCreate.Email))                
                {
                    _membershipService.CreateUser(proposalToCreate.Email.Trim().ToLower(),
                                                 "Ht548*%KjjY2#",
                                                 proposalToCreate.Email.Trim().ToLower());
                    tempPass = _membershipService.ResetPassword(proposalToCreate.Email.Trim().ToLower());
                }

                _emailService.SendConfirmation(Request, Url, proposalToCreate, proposalToCreate.CallForProposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.ProposalConfirmation).Single(), true, proposalToCreate.Email.Trim().ToLower(), tempPass);

                return this.RedirectToAction(a => a.Confirmation(proposalToCreate.Email));
            }
            else
            {
                var viewModel = ProposalViewModel.Create(Repository, callforproposal);
                viewModel.Proposal = proposalToCreate;

                return View(viewModel);
            }
        }

        /// <summary>
        /// #14
        /// </summary>
        /// <param name="proposalEmail"></param>
        /// <returns></returns>
        public ActionResult Confirmation(string proposalEmail)
        {
            var viewModel = ProposalConfirmationViewModel.Create(proposalEmail);
            return View(viewModel);
        }

        /// <summary>
        /// #15
        /// </summary>
        /// <returns></returns>
        [PublicAuthorize]
        public ActionResult Home()
        {
            var viewModel = ProposalPublicListViewModel.Create(Repository, CurrentUser.Identity.Name);

            return View(viewModel);
        }

        /// <summary>
        /// #16
        /// GET: /Proposal/Edit/5
        /// </summary>
        /// <param name="id">Proposal Guid</param>
        /// <returns></returns>
        [PublicAuthorize]
        public ActionResult Edit(Guid id)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
            if (proposal == null)
            {
                Message = "Your proposal was not found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.Email.Trim().ToLower() != CurrentUser.Identity.Name.Trim().ToLower())
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.IsSubmitted)
            {
                Message = "Cannot edit proposal once submitted.";
                return this.RedirectToAction(a => a.Details(id));
            }
            if (proposal.CallForProposal.EndDate.Date < DateTime.Now.Date)
            {
                Message = "Call for proposal has closed, you will not be able to save changes.";
            }
            else if(!proposal.CallForProposal.IsActive)
            {
                Message = "Call for proposal has been deactivated, you will not be able to save changes.";
            }
            

			var viewModel = ProposalViewModel.Create(Repository, proposal.CallForProposal);
			viewModel.Proposal = proposal;
            viewModel.SaveOptionChoice = StaticValues.RB_SaveOptions_SaveWithValidation;

			return View(viewModel);
        }

        /// <summary>
        /// #17
        /// POST: /Proposal/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="proposal"></param>
        /// <param name="proposalAnswers"></param>
        /// <param name="uploadAttachment"></param>
        /// <param name="saveOptions"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [PublicAuthorize]
        public ActionResult Edit(Guid id, Proposal proposal, QuestionAnswerParameter[] proposalAnswers, HttpPostedFileBase uploadAttachment, string saveOptions)
        {
            var proposalToEdit = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
            if (proposalToEdit == null)
            {
                Message = "Your proposal was not found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposalToEdit.Email.Trim().ToLower() != CurrentUser.Identity.Name.Trim().ToLower())
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if(proposalToEdit.IsSubmitted)
            {
                Message = "Cannot edit proposal once submitted.";
                return this.RedirectToAction(a => a.Details(id));
            }
            if (proposalAnswers == null)
            {
                proposalAnswers = new QuestionAnswerParameter[0];
            }

            if (proposalToEdit.CallForProposal.EndDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Call Closed", "Call for proposal has closed");
            }
            else if (!proposalToEdit.CallForProposal.IsActive)
            {
                ModelState.AddModelError("Call deactivated", "Call for proposal has been deactivated");
            }

            if (uploadAttachment != null)
            {
                if (uploadAttachment.ContentType.ToLower() != "application/pdf".ToLower())
                {
                    ModelState.AddModelError("Proposal.File", "Can only upload PDF files.");
                }
                else
                {
                    var reader = new BinaryReader(uploadAttachment.InputStream);
                    if(proposalToEdit.File == null)
                    {
                        proposalToEdit.File = new File();
                    }
                    else
                    {
                        proposalToEdit.File.DateAdded = DateTime.Now;
                    }
                    proposalToEdit.File.ContentType = uploadAttachment.ContentType;
                    proposalToEdit.File.Contents = reader.ReadBytes(uploadAttachment.ContentLength);
                    proposalToEdit.File.Name = uploadAttachment.FileName;
                }
            }
            var saveWithValidate = false;
            if (saveOptions == StaticValues.RB_SaveOptions_SubmitFinal)
            {
                proposal.IsSubmitted = true;
            }
            else if(saveOptions == StaticValues.RB_SaveOptions_SaveWithValidation)
            {
                proposal.IsSubmitted = false;
                saveWithValidate = true;
            }
            else if(saveOptions == StaticValues.RB_SaveOptions_SaveNoValidate)
            {
                proposal.IsSubmitted = false;
            }


            TransferValues(proposal, proposalToEdit);

            _answerService.ProcessAnswers(proposal, proposalAnswers, saveWithValidate, proposalToEdit, ModelState);

            proposalToEdit.TransferValidationMessagesTo(ModelState);
            if (proposalToEdit.IsSubmitted || saveWithValidate)
            {
                if (proposalToEdit.CallForProposal.HideInvestigators)
                {
                    //Don't check
                }
                else
                {
                    if (Repository.OfType<Investigator>().Queryable.Where(a => a.Proposal == proposalToEdit && a.IsPrimary).Count() != 1)
                    {
                        ModelState.AddModelError("Investigators", "Must have one primary investigator");
                    }
                }
                if (proposalToEdit.CallForProposal.ProposalMaximum > 0)
                {
                    if (proposalToEdit.RequestedAmount > proposalToEdit.CallForProposal.ProposalMaximum)
                    {
                        ModelState.AddModelError(
                            "Proposal.RequestedAmount", string.Format("Requested Amount must be {0} or less", String.Format("{0:C}", proposalToEdit.CallForProposal.ProposalMaximum)));
                    }
                }
                if (proposalToEdit.RequestedAmount <= 0)
                {
                    ModelState.AddModelError("Proposal.RequestedAmount", string.Format("Requested Amount must be entered"));
                }
            }


            if (ModelState.IsValid)
            {
                if (proposalToEdit.IsSubmitted)
                {
                    proposalToEdit.SubmittedDate = DateTime.Now;
                }
                _proposalRepository.EnsurePersistent(proposalToEdit);

                Message = "Proposal Edited Successfully";
                if (!proposalToEdit.IsSubmitted)
                {
                    
                    var viewModel = ProposalViewModel.Create(Repository, proposalToEdit.CallForProposal);
                    viewModel.Proposal = proposalToEdit;
                    viewModel.SaveOptionChoice = saveOptions;

                    return View(viewModel);
                }
                Message = "Proposal Submitted Successfully";
                return this.RedirectToAction(a => a.Details(proposalToEdit.Guid));
            }
            else
            {

                Message = "Unable to save. Please Correct Errors";


                var viewModel = ProposalViewModel.Create(Repository, proposalToEdit.CallForProposal);
                viewModel.Proposal = proposalToEdit;
                viewModel.SaveOptionChoice = saveOptions;
                return View(viewModel);
            }
        }
      

        /// <summary>
        /// #18
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PublicAuthorize]
        public ActionResult Details(Guid id)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
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
            if (!proposal.IsSubmitted)
            {
                Message = "Your proposal is not submitted yet!";
            }

            var viewModel = ProposalViewModel.Create(Repository, proposal.CallForProposal);
            viewModel.Proposal = proposal;

            return View(viewModel);
        }

        [PublicAuthorize]
        public ActionResult ProposalPermissionsIndex(Guid id)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
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

            var viewModel = ProposalPermissionsViewModel.Create(proposal);
            viewModel.Permissions = proposal.ProposalPermissions.ToList();

            return View(viewModel);
        }

        [PublicAuthorize]
        public ActionResult AddPermission(Guid id)
        {
            throw new NotImplementedException();
        }

        [PublicAuthorize]
        public ActionResult EditPermission(int id)
        {
            throw new NotImplementedException();
        }


        #endregion Public Methods (Proposer)

        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Proposal source, Proposal destination)
        {
            destination.RequestedAmount = source.RequestedAmount;
            destination.IsSubmitted = source.IsSubmitted;
        }

        private static void AdminTransferValues(Proposal source, Proposal destination, Comment commentSource, Comment commentDestination)
        {
            destination.ApprovedAmount = source.ApprovedAmount;
            destination.IsSubmitted = source.IsSubmitted;
            destination.IsApproved = source.IsApproved;
            destination.IsDenied = source.IsDenied;
            destination.IsNotified = source.IsNotified;

            commentDestination.Text = commentSource.Text;
        }

    }

    public class QuestionAnswerParameter
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }

        public string[] CblAnswer { get; set; }
    }
}
