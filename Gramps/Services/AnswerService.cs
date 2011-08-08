using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Core.Resources;

namespace Gramps.Services
{
    public interface IAnswerService
    {
        void ProcessAnswers(Proposal proposal, QuestionAnswerParameter[] proposalAnswers, bool saveWithValidate, Proposal proposalToEdit, ModelStateDictionary modelState);
    }

    public class AnswerService : IAnswerService
    {
        public void ProcessAnswers(Proposal proposal, QuestionAnswerParameter[] proposalAnswers, bool saveWithValidate, Proposal proposalToEdit, ModelStateDictionary modelState)
        {
            var allQuestions = proposalToEdit.CallForProposal.Questions.ToList();

            foreach (var pa in proposalAnswers)
            {
                var question = allQuestions.Where(a => a.Id == pa.QuestionId).FirstOrDefault();
                if (question != null)
                {
                    var answer = CleanUpAnswer(question.QuestionType.Name, pa, question.ValidationClasses); //, question);
                    if (proposal.IsSubmitted || saveWithValidate)
                    {
                        foreach (var validator in question.Validators)
                        {
                            string message;
                            if (question.QuestionType.Name == QuestionTypeText.STR_TextArea)
                            {
                                if (validator.Class == "required" && !string.IsNullOrWhiteSpace(answer))
                                {
                                    continue;
                                }
                            }
                            if (!Validate(validator, answer, question.Name, out message))
                            {
                                modelState.AddModelError(question.Name, message);
                            }
                        }
                        if (question.MaxCharacters != null && question.MaxCharacters > 0)
                        {
                            if (answer.Trim().Length > question.MaxCharacters)
                            {
                                modelState.AddModelError(question.Name, string.Format("{0} must be less than {1} characters long.", question.Name, question.MaxCharacters));
                            }
                        }
                    }
                    var pteAnswers = proposalToEdit.Answers.Where(a => a.Question.Id == question.Id).FirstOrDefault();
                    if (pteAnswers != null)
                    {
                        pteAnswers.Answer = answer;
                    }
                    else if (!string.IsNullOrWhiteSpace(answer))
                    {
                        proposalToEdit.AddAnswer(question, answer);
                    }
                }
            }
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
                    answer = qa.Answer ?? string.Empty;
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

        }
    }
}