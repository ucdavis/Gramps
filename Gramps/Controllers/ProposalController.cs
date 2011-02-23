using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
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

        #region Admin(User) Methods
        
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
        public ActionResult AdminDetails(int id, int callForProposalId)
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
        #endregion Admin(User) Methods

        #region Public Methods (Proposer)
               
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
        public ActionResult Edit(Guid id)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
            if (proposal == null)
            {
                Message = "Your proposal was not found.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (proposal.IsSubmitted)
            {
                Message = "Cannot edit proposal once submitted.";
                return this.RedirectToAction(a => a.Details(id));
            }

			var viewModel = ProposalViewModel.Create(Repository, proposal.CallForProposal);
			viewModel.Proposal = proposal;

			return View(viewModel);
        }

        
        //
        // POST: /Proposal/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        [CaptchaValidator]
        public ActionResult Edit(Guid id, Proposal proposal, QuestionAnswerParameter[] proposalAnswers, bool captchaValid)
        {
            var proposalToEdit = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
            if (proposalToEdit == null)
            {
                Message = "Your proposal was not found.";
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


            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Captcha values are not valid.");
            }

            TransferValues(proposal, proposalToEdit);

            var allQuestions = proposalToEdit.CallForProposal.Questions.ToList();

            foreach (var pa in proposalAnswers)
            {
                var question = allQuestions.Where(a => a.Id == pa.QuestionId).FirstOrDefault();
                if (question != null)
                {
                    var answer = CleanUpAnswer(question.QuestionType.Name, pa, question.ValidationClasses);//, question);
                    if (proposal.IsSubmitted)
                    {
                        foreach (var validator in question.Validators)
                        {
                            string message;
                            if (!Validate(validator, answer, question.Name, out message))
                            {
                                ModelState.AddModelError("Answer", message);
                            }
                        }
                    }
                    var pteAnswers = proposalToEdit.Answers.Where(a => a.Question.Id == question.Id).FirstOrDefault();
                    if(pteAnswers != null)
                    {
                        pteAnswers.Answer = answer;
                    }
                    else if(!string.IsNullOrWhiteSpace(answer))
                    {
                        proposalToEdit.AddAnswer(question, answer);
                    }

                }

            }

            proposalToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _proposalRepository.EnsurePersistent(proposalToEdit);

                Message = "Proposal Edited Successfully";
                if (!proposalToEdit.IsSubmitted)
                {

                    var viewModel = ProposalViewModel.Create(Repository, proposalToEdit.CallForProposal);
                    viewModel.Proposal = proposalToEdit;

                    return View(viewModel);
                }

                return this.RedirectToAction(a => a.Details(proposalToEdit.Guid));
            }
            else
            {
                if (!captchaValid)
                {
                    Message = "Captcha not valid";
                }
                else
                {
                    Message = "Unable to submit final. Please Correct Errors";
                }

                var viewModel = ProposalViewModel.Create(Repository, proposalToEdit.CallForProposal);
                viewModel.Proposal = proposalToEdit;

                return View(viewModel);
            }
        }


        public ActionResult Details(Guid id)
        {
            var proposal = _proposalRepository.Queryable.Where(a => a.Guid == id).SingleOrDefault();
            if (proposal == null)
            {
                Message = "Your proposal was not found.";
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
        #endregion Public Methods (Proposer)

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
            destination.RequestedAmount = source.RequestedAmount;
        }

        private bool Validate(Validator validator, string answer, string fieldName, out string message)
        {
            // set as default so we can return without having to set it individually
            message = string.Empty;

            // check to make sure we have a reg ex
            if (string.IsNullOrEmpty(validator.RegEx)) return true;

            var regExVal = new Regex(validator.RegEx);
            // valid
            // check for when answer is null, because when doing a radio button it is null when nothing is selected
            if (regExVal.IsMatch(answer ?? string.Empty)) return true;

            // not valid input provide error message
            message = string.Format(validator.ErrorMessage, fieldName);
            return false;
        }

        private static string CleanUpAnswer(string name, QuestionAnswerParameter qa, string validationClasses)//, Question question)
        {
            string answer;
            if (name != QuestionTypeText.STR_CheckboxList)
            {
                if (name == QuestionTypeText.STR_Boolean)
                {
                    //Convert unchecked bool of null to false
                    if (string.IsNullOrEmpty(qa.Answer) || qa.Answer.ToLower() == "false")
                    {
                        answer = "false";
                    }
                    else
                    {
                        answer = "true";
                    }
                }
                else if (name == QuestionTypeText.STR_TextArea)
                {
                    answer = qa.Answer;
                }
                else
                {
                    answer = qa.Answer ?? string.Empty;
                    if (validationClasses != null && validationClasses.Contains("email"))
                    {
                        answer = answer.ToLower();
                    }
                }
            }
            else
            {
                if (qa.CblAnswer != null)
                {
                    answer = string.Join(",", qa.CblAnswer);
                }
                else
                {
                    answer = string.Empty;
                }
            }
            return answer;
            //else
            //{
            //    answer = string.Empty;
            //    if (qa.CblAnswer != null)
            //    {
            //        var tempAnswer = string.Empty;
            //        var count = qa.CblAnswer.Count();
            //        for (int i = count-1; i >= 0; i--)
            //        {
            //            if (i > 0)
            //            {
            //                if (qa.CblAnswer[i - 1].ToLower() == "true")
            //                {
            //                    tempAnswer = "+" + tempAnswer ;
            //                    i--;
            //                }
            //                else
            //                {
            //                    tempAnswer = "-" + tempAnswer ;
            //                }
            //            }
            //            else
            //            {
            //                if (qa.CblAnswer[i].ToLower() == "false")
            //                {
            //                    tempAnswer = "-" + tempAnswer;
            //                }
            //            }
            //        }

            //        var answerList = new List<string>();

            //        for (int i = 0; i < tempAnswer.Count(); i++)
            //        {
            //            if (tempAnswer[i] == '+')
            //            {
            //                answerList.Add(question.Options[i].Name);
            //            }
            //        }
            //        if (answerList.Count > 0)
            //        {
            //            answer = string.Join(",", answerList);      
            //        }
            //    }
            //    else
            //    {
            //        answer = string.Empty;
            //    }
            //}
            //return answer;
        }
    }

    public class QuestionAnswerParameter
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }

        public string[] CblAnswer { get; set; }
    }
}
