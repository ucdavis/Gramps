using System;
using System.Collections.Generic;
using System.Linq;
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
using UCDArch.Web.Validator;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Question class
    /// </summary>
    [UserOnly]
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
            var order = 0;
            if (templateId.HasValue && templateId.Value != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                order = _questionRepository.Queryable.Where(a => a.Template == template).Select(a => a.Order).Max() + 1;
            }
            else if (callForProposalId.HasValue && callForProposalId.Value != 0)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                order = _questionRepository.Queryable.Where(a => a.CallForProposal == callforProposal).Select(a => a.Order).Max() + 1;
            }
            question.Template = template;
            question.CallForProposal = callforProposal;

            question.Order = order;

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

            ValidatorsValidation(question);

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

        

        public ActionResult Edit(int id, int? templateId, int? callForProposalId)
        {

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var questionToEdit = _questionRepository.GetNullableById(id);
            if (questionToEdit == null)
            {
                Message = "Question not found";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            var viewModel = QuestionViewModel.Create(Repository, templateId, callForProposalId);
            viewModel.Question = questionToEdit;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, int? templateId, int? callForProposalId, Question question, string[] questionOptions)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var questionToEdit = _questionRepository.GetNullableById(id);
            if (questionToEdit == null)
            {
                Message = "Question not found.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (!_accessService.HasSameId(questionToEdit.Template, questionToEdit.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            TransferValues(question, questionToEdit, questionOptions);
            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, questionToEdit.ValidationResults());
            ValidatorsValidation(questionToEdit);

            if (ModelState.IsValid)
            {
                Message = "Question updated successfully";
                _questionRepository.EnsurePersistent(questionToEdit);
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }


            var viewModel = QuestionViewModel.Create(Repository, templateId, callForProposalId);
            viewModel.Question = questionToEdit;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var questionToDelete = _questionRepository.GetNullableById(id);
            if (questionToDelete == null)
            {
                Message = "Question not found.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (!_accessService.HasSameId(questionToDelete.Template, questionToDelete.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            //TODO: Add a check to see if there are answers (not needed for a template)

            _questionRepository.Remove(questionToDelete);
            Message = "Question removed";
            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));

        }

        [HttpPost]
        public ActionResult MoveUp(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var questionToReorder = _questionRepository.GetNullableById(id);
            if (questionToReorder == null)
            {
                Message = "Question not found.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (
                !_accessService.HasSameId(questionToReorder.Template, questionToReorder.CallForProposal, templateId,
                                          callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            Question nextSmaller = null; 
            if(templateId.HasValue && templateId != 0)
            {
                var template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                nextSmaller = _questionRepository.Queryable
                    .Where(a => a.Template == template && a.Order < questionToReorder.Order).OrderByDescending(a => a.Order)
                    .FirstOrDefault();
            }
            if (callForProposalId.HasValue && callForProposalId != 0)
            {
                var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                nextSmaller = _questionRepository.Queryable
                    .Where(a => a.CallForProposal == callForProposal && a.Order < questionToReorder.Order).OrderByDescending(a => a.Order).LastOrDefault();
            }
            if (nextSmaller != null)
            {
                var saveOrder = questionToReorder.Order;
                questionToReorder.Order = nextSmaller.Order;
                _questionRepository.EnsurePersistent(questionToReorder);

                nextSmaller.Order = saveOrder;
                _questionRepository.EnsurePersistent(nextSmaller);

                Message = "Moved Up.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            Message = "Not moved.";
            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));

        }


        [HttpPost]
        public ActionResult MoveDown(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var questionToReorder = _questionRepository.GetNullableById(id);
            if (questionToReorder == null)
            {
                Message = "Question not found.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (
                !_accessService.HasSameId(questionToReorder.Template, questionToReorder.CallForProposal, templateId,
                                          callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            Question nextBigger = null;
            if (templateId.HasValue && templateId != 0)
            {
                var template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                nextBigger = _questionRepository.Queryable
                    .Where(a => a.Template == template && a.Order > questionToReorder.Order).OrderBy(a => a.Order)
                    .FirstOrDefault();
            }
            if (callForProposalId.HasValue && callForProposalId != 0)
            {
                var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                nextBigger = _questionRepository.Queryable
                    .Where(a => a.CallForProposal == callForProposal && a.Order > questionToReorder.Order).OrderBy(a => a.Order).LastOrDefault();
            }
            if (nextBigger != null)
            {
                var saveOrder = questionToReorder.Order;
                questionToReorder.Order = nextBigger.Order;
                _questionRepository.EnsurePersistent(questionToReorder);

                nextBigger.Order = saveOrder;
                _questionRepository.EnsurePersistent(nextBigger);

                Message = "Moved Down.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            Message = "Not moved.";
            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));

        }

        #region Private Methods

        private void ValidatorsValidation(Question question)
        {
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
        }

        

        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Question source, Question destination, string[] questionOptions)
        {
            destination.Name = source.Name;
            destination.QuestionType = source.QuestionType;
            destination.Validators = source.Validators;

            destination.Options.Clear();
            if (source.QuestionType != null && source.QuestionType.HasOptions && questionOptions != null)
            {
                foreach (string s in questionOptions)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var option = new QuestionOption(s);
                        destination.AddQuestionOption(option);
                    }
                }
            }
        }
        #endregion Private Methods
    }


}
