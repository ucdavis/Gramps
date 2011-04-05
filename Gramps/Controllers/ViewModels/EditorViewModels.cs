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
    /// ViewModel for the Editor class
    /// </summary>
    public class EditorViewModel : NavigationViewModel
    {
        public Editor Editor { get; set; }

        public static EditorViewModel Create(IRepository repository, Template template, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new EditorViewModel { Editor = new Editor() };
            if (template != null)
            {
                viewModel.IsTemplate = true;
                viewModel.TemplateId = template.Id;
            }
            else if (callForProposal != null)
            {
                viewModel.IsCallForProposal = true;
                viewModel.CallForProposalId = callForProposal.Id;
            }

            Check.Require(viewModel.IsTemplate || viewModel.IsCallForProposal, "Must have either a template or a call for proposal");

            return viewModel;
        }
    }

    public class EditorListViewModel : NavigationViewModel
    {
        public IQueryable<Editor> EditorList;

        public static EditorListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new EditorListViewModel();

            if (templateId != null && templateId != 0)
            {
                viewModel.IsTemplate = true;
                viewModel.EditorList = repository.OfType<Editor>().Queryable.Where(a => a.Template != null && a.Template.Id == templateId);
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null && callForProposalId != 0)
            {
                viewModel.IsCallForProposal = true;
                viewModel.EditorList = repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal != null && a.CallForProposal.Id == callForProposalId);
                viewModel.CallForProposalId = callForProposalId;
            }

            return viewModel;
        }
    }

    public class AddEditorViewModel : NavigationViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public CallForProposal CallForProposal { get; set; }
        public Template Template { get; set; }
        public User User { get; set; }

        public static AddEditorViewModel Create(IRepository repository, Template template, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository is required.");
            

            var viewModel = new AddEditorViewModel()
            {
                Template = template,
                CallForProposal = callForProposal,
                Users = repository.OfType<User>().Queryable.OrderBy(a => a.LastName).ToList()
            };
            if (template != null)
            {
                viewModel.IsTemplate = true;
                viewModel.TemplateId = template.Id;
            }
            else if (callForProposal != null)
            {
                viewModel.IsCallForProposal = true;
                viewModel.CallForProposalId = callForProposal.Id;
            }

            Check.Require(viewModel.IsTemplate || viewModel.IsCallForProposal, "Must have either a template or a call for proposal");

            return viewModel;
        }
    }

    public class ReviewersSendViewModel : CallNavigationViewModel
    {
        public IQueryable<Editor> EditorsToNotify;
        public bool Immediate;

        public static ReviewersSendViewModel Create(IRepository repository, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new ReviewersSendViewModel { CallForProposal = callForProposal, Immediate = false };

            viewModel.EditorsToNotify = repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal != null && a.CallForProposal == callForProposal && a.User == null);

            return viewModel;
        }
    }
}