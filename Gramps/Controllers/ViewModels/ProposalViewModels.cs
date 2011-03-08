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
    public class ProposalPublicListViewModel
    {
        public bool IsReviewer = false;
        public IList<CallForProposal> CallForProposals { get; set; } //Will only be populated if user is a reviewer
        public IList<Proposal> UsersProposals { get; set; }

        public static ProposalPublicListViewModel Create(IRepository repository, string login)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new ProposalPublicListViewModel();
            viewModel.IsReviewer = repository.OfType<Editor>()
                .Queryable.Where(a => a.CallForProposal != null && a .CallForProposal.IsActive && a.User == null && a.ReviewerEmail == login).Any();
            if (viewModel.IsReviewer)
            {
                var callForProposalIds =
                    repository.OfType<Editor>().Queryable.Where(
                        a =>
                        a.CallForProposal != null && a.CallForProposal.IsActive && a.User == null &&
                        a.ReviewerEmail == login).Select(x => x.CallForProposal.Id).ToList();
                viewModel.CallForProposals =
                    repository.OfType<CallForProposal>().Queryable.Where(a => callForProposalIds.Contains(a.Id)).ToList();
            }
            viewModel.UsersProposals = repository.OfType<Proposal>().Queryable.Where(a => a.Email == login).OrderByDescending(a => a.CreatedDate).ToList();

            return viewModel;
        }
    }

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
        public string FilterDecission { get; set; }
        public string FilterNotified { get; set; }
        public string FilterSubmitted { get; set; }
        public string FilterWarned { get; set; }
        public string FilterEmail { get; set; }

        public static ProposalAdminListViewModel Create(IRepository repository, 
            CallForProposal callForProposal, 
            string login,
            string filterDecission,
            string filterNotified,
            string filterSubmitted,
            string filterWarned,
            string filterEmail)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(callForProposal != null, "Grant to apply for must be supplied (CallForProposal)");

            var viewModel = new ProposalAdminListViewModel 
            {
                CallForProposal = callForProposal,
                FilterDecission = filterDecission,
                FilterNotified = filterNotified,
                FilterSubmitted = filterSubmitted,
                FilterWarned = filterWarned,
                FilterEmail = filterEmail
            };
            viewModel.Editor = repository.OfType<Editor>()
                .Queryable.Where(a => a.CallForProposal == callForProposal && a.User != null && a.User.LoginId == login).Single();
            var tempToFilter = repository.OfType<Proposal>()
                .Queryable.Where(a => a.CallForProposal == callForProposal);

            if (filterDecission == StaticValues.RB_Decission_Approved)
            {
                tempToFilter = tempToFilter.Where(a => a.IsApproved && !a.IsDenied);
            }
            else if (filterDecission == StaticValues.RB_Decission_Denied)
            {
                tempToFilter = tempToFilter.Where(a => !a.IsApproved && a.IsDenied);
            }
            else if (filterDecission == StaticValues.RB_Decission_NotDecided)
            {
                tempToFilter = tempToFilter.Where(a => !a.IsApproved && !a.IsDenied);
            }
            
            if (filterNotified == StaticValues.RB_Notified_Notified)
            {
                tempToFilter = tempToFilter.Where(a => a.IsNotified);
            }
            else if (filterNotified == StaticValues.RB_Notified_NotNotified)
            {
                tempToFilter = tempToFilter.Where(a => !a.IsNotified);
            }

            if (filterSubmitted == StaticValues.RB_Submitted_Submitted)
            {
                tempToFilter = tempToFilter.Where(a => a.IsSubmitted);
            }
            else if (filterSubmitted == StaticValues.RB_Submitted_NotSubmitted)
            {
                tempToFilter = tempToFilter.Where(a => !a.IsSubmitted);
            }

            if (filterWarned == StaticValues.RB_Warned_Warned)
            {
                tempToFilter = tempToFilter.Where(a => a.WasWarned);
            }
            else if (filterWarned == StaticValues.RB_Warned_NotWarned)
            {
                tempToFilter = tempToFilter.Where(a => !a.WasWarned);
            }
            if (!string.IsNullOrWhiteSpace(filterEmail))
            {
                filterEmail = filterEmail.ToLower();
                tempToFilter = tempToFilter.Where(a => a.Email.Contains(filterEmail));
            }

            var temp = tempToFilter.ToList();
            viewModel.Proposals = temp.Select(x => new ProposalList
                                 {
                                     Id = x.Id, 
                                     Email = x.Email, 
                                     CreatedDate = x.CreatedDate, 
                                     Approved = x.IsApproved,
                                     Denied = x.IsDenied,
                                     Submitted = x.IsSubmitted,
                                     SubmittedDate = x.SubmittedDate,  
                                     WarnedOfClosing = x.WasWarned,
                                     NotifiedOfDecission = x.IsNotified,
                                     Seq = x.Sequence
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

    public class ProposalReviewerListViewModel : CallNavigationViewModel
    {
        public IList<ProposalList> Proposals { get; set; }
        public Editor Editor { get; set; }
        public bool Immediate { get; set; }
        public string FilterDecission { get; set; }
        public string FilterEmail { get; set; }

        public static ProposalReviewerListViewModel Create(IRepository repository, CallForProposal callForProposal, string login, string filterDecission, string filterEmail)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(callForProposal != null, "Grant to apply for must be supplied (CallForProposal)");

            var viewModel = new ProposalReviewerListViewModel { CallForProposal = callForProposal, FilterDecission = filterDecission, FilterEmail = filterEmail};
            viewModel.Editor = repository.OfType<Editor>()
                .Queryable.Where(a => a.CallForProposal == callForProposal && a.ReviewerEmail == login).First();
            var tempToFilter = repository.OfType<Proposal>()
                .Queryable.Where(a => a.CallForProposal == callForProposal && a.IsSubmitted);

            if (filterDecission == StaticValues.RB_Decission_Approved)
            {
                tempToFilter = tempToFilter.Where(a => a.IsApproved && !a.IsDenied);
            }
            else if (filterDecission == StaticValues.RB_Decission_Denied)
            {
                tempToFilter = tempToFilter.Where(a => !a.IsApproved && a.IsDenied);
            }
            else if (filterDecission == StaticValues.RB_Decission_NotDecided)
            {
                tempToFilter = tempToFilter.Where(a => !a.IsApproved && !a.IsDenied);
            }

            if (!string.IsNullOrWhiteSpace(filterEmail))
            {
                filterEmail = filterEmail.ToLower();
                tempToFilter = tempToFilter.Where(a => a.Email.Contains(filterEmail));
            }


            var temp = tempToFilter.ToList();

            viewModel.Proposals = temp.Select(x => new ProposalList
            {
                Id = x.Id,
                Email = x.Email,
                CreatedDate = x.CreatedDate,
                Approved = x.IsApproved,
                Denied = x.IsDenied,
                Submitted = x.IsSubmitted,
                SubmittedDate = x.SubmittedDate,
                WarnedOfClosing = x.WasWarned,
                NotifiedOfDecission = x.IsNotified,
                Seq = x .Sequence
            }).ToList();
            foreach (var proposal in viewModel.Proposals)
            {
                var viewDate =
                    temp.Where(a => a.Id == proposal.Id).Single().ReviewedByEditors.Where(
                        x => x.Editor == viewModel.Editor).FirstOrDefault();
                if (viewDate != null)
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
        public Comment Comment;
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
        public bool NotifiedOfDecission;
        public int Seq;


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