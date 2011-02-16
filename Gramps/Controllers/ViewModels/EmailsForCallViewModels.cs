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
    /// ViewModel for the EmailsForCall class
    /// </summary>
    public class EmailsForCallViewModel : NavigationViewModel
    {
        public EmailsForCall EmailsForCall { get; set; }
        public string BulkLoadEmails { get; set; }

        public static EmailsForCallViewModel Create(IRepository repository, Template template, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new EmailsForCallViewModel { EmailsForCall = new EmailsForCall() };
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

    public class EmailsForCallListViewModel : NavigationViewModel
    {
        public IQueryable<EmailsForCall> EmailsForCallList;

        public static EmailsForCallListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new EmailsForCallListViewModel();

            if (templateId != null && templateId != 0)
            {
                viewModel.IsTemplate = true;
                viewModel.EmailsForCallList = repository.OfType<EmailsForCall>().Queryable.Where(a => a.Template.Id == templateId);
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null && callForProposalId != 0)
            {
                viewModel.IsCallForProposal = true;
                viewModel.EmailsForCallList = repository.OfType<EmailsForCall>().Queryable.Where(a => a.CallForProposal.Id == callForProposalId);
                viewModel.CallForProposalId = callForProposalId;
            }

            return viewModel;
        }
    }

    public class EmailsForCallSendViewModel : CallNavigationViewModel
    {
        public IQueryable<EmailsForCall> EmailsForCallList;
        public bool Immediate;

        public static EmailsForCallSendViewModel Create(IRepository repository, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new EmailsForCallSendViewModel {CallForProposal = callForProposal, Immediate = false};

            viewModel.EmailsForCallList = repository.OfType<EmailsForCall>().Queryable.Where(a => a.CallForProposal == callForProposal);

            return viewModel;
        }
    }
}