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
}