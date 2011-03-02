using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Gramps.Controllers.ViewModels
{
    public class EmailTemplateListViewModel : NavigationViewModel
    {
        public IQueryable<EmailTemplate> EmailTemplateList;
        public Dictionary<EmailTemplateType, string> DescriptionDict;

        public static EmailTemplateListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new EmailTemplateListViewModel();

            if (templateId != null  && templateId != 0)
            {
                viewModel.IsTemplate = true;
                viewModel.EmailTemplateList = repository.OfType<EmailTemplate>().Queryable.Where(a => a.Template.Id == templateId);
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null && callForProposalId != 0)
            {
                viewModel.IsCallForProposal = true;
                viewModel.EmailTemplateList = repository.OfType<EmailTemplate>().Queryable.Where(a => a.CallForProposal.Id == callForProposalId);
                viewModel.CallForProposalId = callForProposalId;
            }
            viewModel.DescriptionDict = new Dictionary<EmailTemplateType, string>(7);
            viewModel.DescriptionDict.Add(EmailTemplateType.InitialCall, StaticValues.InitialCall);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalApproved, StaticValues.ProposalApproved);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalConfirmation, StaticValues.ProposalConfirmation);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalDenied, StaticValues.ProposalDenied);
            viewModel.DescriptionDict.Add(EmailTemplateType.ReadyForReview, StaticValues.ReadyForReview);
            viewModel.DescriptionDict.Add(EmailTemplateType.ReminderCallIsAboutToClose, StaticValues.ReminderCallIsAboutToClose);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalUnsubmitted, StaticValues.ProposalUnsubmitted);

            return viewModel;
        }
    }

    /// <summary>
    /// ViewModel for the EmailTemplate class
    /// </summary>
    public class EmailTemplateViewModel : NavigationViewModel
    {
        public EmailTemplate EmailTemplate { get; set; }
        public Dictionary<EmailTemplateType, string> DescriptionDict;
        public string FooterText { get; set; }
        public string AlternateFooterText { get; set; }

        public static EmailTemplateViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new EmailTemplateViewModel { EmailTemplate = new EmailTemplate() };
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

            viewModel.DescriptionDict = new Dictionary<EmailTemplateType, string>(7);
            viewModel.DescriptionDict.Add(EmailTemplateType.InitialCall, StaticValues.InitialCall);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalApproved, StaticValues.ProposalApproved);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalConfirmation, StaticValues.ProposalConfirmation);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalDenied, StaticValues.ProposalDenied);
            viewModel.DescriptionDict.Add(EmailTemplateType.ReadyForReview, StaticValues.ReadyForReview);
            viewModel.DescriptionDict.Add(EmailTemplateType.ReminderCallIsAboutToClose, StaticValues.ReminderCallIsAboutToClose);
            viewModel.DescriptionDict.Add(EmailTemplateType.ProposalUnsubmitted, StaticValues.ProposalUnsubmitted);

            return viewModel;
        }
    }
}