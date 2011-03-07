using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;



namespace Gramps.Controllers.ViewModels
{

    public class CallForProposalCreateViewModel
    {
        public CallForProposal CallForProposal { get; set; }
        public IEnumerable<Template> Templates { get; set; }

        public static CallForProposalCreateViewModel Create(IRepository repository, string loginId)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new CallForProposalCreateViewModel { CallForProposal = new CallForProposal() };
            viewModel.Templates = repository.OfType<Editor>().Queryable
                .Where(a => a.Template != null && a.Template.IsActive && a.User != null && a.User.LoginId == loginId).Select(x => x.Template).Distinct();

            return viewModel;
        }
    }

    public class CallForProposalViewModel : NavigationViewModel
    {
        public CallForProposal CallForProposal { get; set; }


        public static CallForProposalViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new CallForProposalViewModel { CallForProposal = new CallForProposal() };
            viewModel.IsTemplate = false;
            viewModel.IsCallForProposal = true;  
  
            return viewModel;
        }
    }

    public class CallForProposalListViewModel
    {
        public IQueryable<CallForProposal> CallForProposals { get; set; }
        public string FilterActive { get; set; }
        public DateTime? FilterStartCreate { get; set; }
        public DateTime? FilterEndCreate { get; set; }

        public static CallForProposalListViewModel Create(IRepository repository, string loginId, string filterActive, DateTime? filterStartCreate = null, DateTime? filterEndCreate = null)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new CallForProposalListViewModel{FilterActive = filterActive, FilterStartCreate = filterStartCreate, FilterEndCreate = filterEndCreate};
            var callForProposalIds = repository.OfType<Editor>().Queryable
                .Where(a => a.CallForProposal != null && 
                    a.User != null && 
                    a.User.LoginId == loginId)
                .Select(x => x.CallForProposal.Id).ToList();
            viewModel.CallForProposals = repository.OfType<CallForProposal>().Queryable.Where(a => callForProposalIds.Contains(a.Id));
            if (filterActive == "Active")
            {
                viewModel.CallForProposals = viewModel.CallForProposals.Where(a => a.IsActive);
            }
            if (filterActive == "InActive")
            {
                viewModel.CallForProposals = viewModel.CallForProposals.Where(a => !a.IsActive);
            }
            if (filterStartCreate != null)
            {
                viewModel.CallForProposals = viewModel.CallForProposals.Where(a => a.CreatedDate > filterStartCreate);
            }
            if (filterEndCreate != null)
            {
                viewModel.CallForProposals = viewModel.CallForProposals.Where(a => a.CreatedDate < filterEndCreate);
            }

            return viewModel;
        }
    }
}