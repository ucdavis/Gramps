using System;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.Validator;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Question class
    /// </summary>
    public class QuestionController : ApplicationController
    {
        private readonly IAccessService _accessService;
        private readonly IRepository<Question> _questionRepository;

        public QuestionController(IRepository<Question> questionRepository, IAccessService accessService)
        {
            _questionRepository = questionRepository;
            _accessService = accessService;
        }

        //
        // GET: /Question/
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = QuestionListViewModel.Create(Repository, templateId, callForProposalId);
            return View(viewModel);
        }

        public ActionResult Create(int? templateId, int? callForProposalId)
        {

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = QuestionViewModel.Create(Repository, templateId, callForProposalId);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(int? templateId, int? callForProposalId, Question question, string[] questionOptions)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
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
            question.Template = template;
            question.CallForProposal = callforProposal;

            // process the options
            if (question.QuestionType != null && question.QuestionType.HasOptions && questionOptions != null)
            {
                foreach (string s in questionOptions)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var option = new QuestionOption(s);
                        question.AddQuestionOption(option);
                    }
                }
            }

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, question.ValidationResults());

            var validatorsSelected = question.Validators.Count(validator => validator.Class.ToLower().Trim() != "required");

            //Validator and Question type validation:
            switch (question.QuestionType.Name)
            {
                case "Text Box":
                    //All possible, but only a combination of required and others
                    if (validatorsSelected > 1)
                    {
                        ModelState.AddModelError("Validators", "Cannot have Email, Url, Date, Phone Number, or zip validators selected together.");
                    }
                    break;
                case "Boolean":
                case "Radio Buttons":
                case "Checkbox List":
                case "Drop Down":
                case "Text Area":
                    if (validatorsSelected > 0) //count of all validators excluding required
                    {
                        ModelState.AddModelError("Validators", string.Format("The only validator allowed for a Question Type of {0} is Required.", question.QuestionType.Name));
                    }
                    break;

                case "Date":
                    foreach (var validator in question.Validators)
                    {
                        if (validator.Class.ToLower().Trim() != "required" && validator.Class.ToLower().Trim() != "date")
                        {
                            ModelState.AddModelError("Validators", string.Format("{0} is not a valid validator for a Question Type of {1}", validator.Name, question.QuestionType.Name));
                        }
                    }
                    break;
                case "No Answer":
                    foreach (var validator in question.Validators)
                    {
                        ModelState.AddModelError("Validators", string.Format("{0} is not a valid validator for a Question Type of {1}", validator.Name, question.QuestionType.Name));
                    }
                    break;
                default:
                    //No checks
                    break;
            }

            if (ModelState.IsValid)
            {
                Message = "Question added successfully";
                _questionRepository.EnsurePersistent(question);
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }


            var viewModel = QuestionViewModel.Create(Repository, templateId, callForProposalId);
            viewModel.Question = question;
            return View(viewModel);
        }

    }


}
