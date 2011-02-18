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
    /// ViewModel for the Proposal class
    /// </summary>
    public class ProposalViewModel
    {
        public Proposal Proposal { get; set; }
        public CallForProposal CallForProposal { get; set; }

        public static ProposalViewModel Create(IRepository repository, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new ProposalViewModel { Proposal = new Proposal(), CallForProposal = callForProposal};

            return viewModel;
        }
    }

    public class ProposalAdminListViewModel : CallNavigationViewModel
    {
        public IList<ProposalList> Proposals { get; set; }
        public Editor Editor { get; set; }
        public bool Immediate { get; set; }

        public static ProposalAdminListViewModel Create(IRepository repository, CallForProposal callForProposal, string login)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(callForProposal != null, "Grant to apply for must be supplied (CallForProposal)");

            var viewModel = new ProposalAdminListViewModel {CallForProposal = callForProposal};
            viewModel.Editor = repository.OfType<Editor>()
                .Queryable.Where(a => a.CallForProposal == callForProposal && a.User != null && a.User.LoginId == login).Single();
            var temp = repository.OfType<Proposal>()
                .Queryable.Where(a => a.CallForProposal == callForProposal).ToList();

            viewModel.Proposals = temp.Select(x => new ProposalList
                                 {
                                     Id = x.Id, 
                                     Email = x.Email, 
                                     CreatedDate = x.CreatedDate, 
                                     Approved = x.IsApproved,
                                     Denied = x.IsDenied,
                                     Submitted = x.IsSubmitted,
                                     SubmittedDate = x.SubmittedDate,  
                                     WarnedOfClosing = x.WasWarned
                                 }).ToList();
            foreach (var proposal in viewModel.Proposals)
            {
                var viewDate =
                    temp.Where(a => a.Id == proposal.Id).Single().ReviewedByEditors.Where(
                        x => x.Editor == viewModel.Editor).FirstOrDefault();
                if(viewDate != null)
                {
                    proposal.LastViewedDate = viewDate.LastViewedDate;
                }
                
            }

            viewModel.Immediate = false;

            return viewModel;
        }
    }

    public class ProposalAdminViewModel : CallNavigationViewModel
    {
        public Proposal Proposal;
        public static ProposalAdminViewModel Create(IRepository repository, CallForProposal callForProposal, Proposal proposal)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(callForProposal != null, "Grant to apply for must be supplied (CallForProposal)");
            return new ProposalAdminViewModel {CallForProposal = callForProposal, Proposal = proposal};

        }
    }

    public class ProposalConfirmationViewModel
    {
        public string Message1 { get; set; }
        public string Message2 { get; set; }

        public static ProposalConfirmationViewModel Create(string email)
        {
            var viewModel = new ProposalConfirmationViewModel();
            viewModel.Message1 =
                "Thank you. An email with a link to complete your proposal will be sent to shortly. It will be sent to " +
                email;
            viewModel.Message2 =
                "Note: This email will be sent from the following address so if you do not receive it please check your email filters. automatedemail@caes.ucdavis.edu";

            return viewModel;
        }
    }

    public class ProposalList
    {
        public int Id;
        public string Email;
        public DateTime CreatedDate;
        public bool Approved;
        public bool Denied;
        public bool Submitted;
        public DateTime? SubmittedDate;
        public DateTime? LastViewedDate;
        public bool WarnedOfClosing;


        //        col.Bound(x => x.Email);
        //col.Bound(x => x.CreatedDate);
        //col.Bound(x => x.IsApproved).Title("Approved");
        //col.Bound(x => x.IsDenied).Title("Denied");
        //col.Bound(x => x.IsSubmitted).Title("Submitted");
        //col.Bound(x => x.SubmittedDate);
        //col.Bound(
        //    x =>
        //    x.ReviewedByEditors.Where(a => a.Editor == Model.Editor).FirstOrDefault().
        //        LastViewedDate);

    }
}