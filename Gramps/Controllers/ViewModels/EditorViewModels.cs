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
    public class EditorViewModel
    {
        public Editor Editor { get; set; }
        public bool IsTemplate = false;
        public bool IsCallForProposal = false;
        public int? TemplateId = 0;
        public int? CallForProposalId = 0;


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

    public class EditorListViewModel
    {
        public IQueryable<Editor> EditorList;
        public bool IsTemplate = false;
        public bool IsCallForProposal = false;
        public int? TemplateId = 0;
        public int? CallForProposalId = 0;

        public static EditorListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new EditorListViewModel();

            if (templateId != null)
            {
                viewModel.IsTemplate = true;
                viewModel.EditorList = repository.OfType<Editor>().Queryable.Where(a => a.Template.Id == templateId);
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null)
            {
                viewModel.IsCallForProposal = true;
                viewModel.EditorList = repository.OfType<Editor>().Queryable.Where(a => a.CallForProposal.Id == callForProposalId);
                viewModel.CallForProposalId = callForProposalId;
            }

            return viewModel;
        }
    }

    public class AddEditorViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public CallForProposal CallForProposal { get; set; }
        public Template Template { get; set; }
        public User User { get; set; }
        public bool IsTemplate = false;
        public bool IsCallForProposal = false;
        public int? TemplateId = null;
        public int? CallForProposalId = null;

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
}