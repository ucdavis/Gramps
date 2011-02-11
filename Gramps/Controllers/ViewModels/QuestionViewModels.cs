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

            if (templateId != null)
            {
                viewModel.IsTemplate = true;
                viewModel.QuestionList = repository.OfType<Question>().Queryable.Where(a => a.Template.Id == templateId);
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null)
            {
                viewModel.IsCallForProposal = true;
                viewModel.QuestionList = repository.OfType<Question>().Queryable.Where(a => a.CallForProposal.Id == callForProposalId);
                viewModel.CallForProposalId = callForProposalId;
            }

            return viewModel;
        }
    }
}