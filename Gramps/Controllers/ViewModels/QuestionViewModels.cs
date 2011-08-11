using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Gramps.Controllers.ViewModels
{
    /// <summary>
    /// ViewModel for the Question class
    /// </summary>
    public class QuestionListViewModel : NavigationViewModel
    {
        public IQueryable<Question> QuestionList;

        public static QuestionListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new QuestionListViewModel();

            if (templateId != null && templateId != 0)
            {
                viewModel.IsTemplate = true;
                viewModel.QuestionList = repository.OfType<Question>().Queryable.Where(a => a.Template != null && a.Template.Id == templateId.Value).OrderBy(a => a.Order);
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null && callForProposalId != 0)
            {
                viewModel.IsCallForProposal = true;
                viewModel.QuestionList = repository.OfType<Question>().Queryable.Where(a => a.CallForProposal != null && a.CallForProposal.Id == callForProposalId.Value).OrderBy(a => a.Order);
                viewModel.CallForProposalId = callForProposalId;
            }

            return viewModel;
        }
    }

    public class QuestionViewModel : NavigationViewModel
    {
        public Question Question { get; set; }
        public IEnumerable<QuestionType> QuestionTypes { get; set; }
        public IEnumerable<Validator> Validators { get; set; }

        public static QuestionViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new QuestionViewModel
            {
                Question = new Question(),
                QuestionTypes = repository.OfType<QuestionType>().GetAll(), 
                Validators = repository.OfType<Validator>().GetAll()
            };

            if (templateId != null && templateId != 0)
            {
                viewModel.IsTemplate = true;
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null && callForProposalId != 0)
            {
                viewModel.IsCallForProposal = true;
                viewModel.CallForProposalId = callForProposalId;
            }

            Check.Require(viewModel.IsTemplate || viewModel.IsCallForProposal, "Must have either a template or a call for proposal");

            return viewModel;
        }
    }
}