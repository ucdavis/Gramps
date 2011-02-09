using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Gramps.Controllers.ViewModels
{
    public class EmailTemplateListViewModel : NavigationViewModel
    {
        public IQueryable<EmailTemplate> EmailTemplateList;

        public static EmailTemplateListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new EmailTemplateListViewModel();

            if (templateId != null)
            {
                viewModel.IsTemplate = true;
                viewModel.EmailTemplateList = repository.OfType<EmailTemplate>().Queryable.Where(a => a.Template.Id == templateId);
                viewModel.TemplateId = templateId;
            }
            else if (callForProposalId != null)
            {
                viewModel.IsCallForProposal = true;
                viewModel.EmailTemplateList = repository.OfType<EmailTemplate>().Queryable.Where(a => a.CallForProposal.Id == callForProposalId);
                viewModel.CallForProposalId = callForProposalId;
            }

            return viewModel;
        }
    }

    /// <summary>
    /// ViewModel for the EmailTemplate class
    /// </summary>
    public class EmailTemplateViewModel : NavigationViewModel
    {
        public EmailTemplate EmailTemplate { get; set; }

        public static EmailTemplateViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new EmailTemplateViewModel { EmailTemplate = new EmailTemplate() };

            return viewModel;
        }
    }
}