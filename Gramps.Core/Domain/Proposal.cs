using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class Proposal : DomainObject
    {
        #region Constructor
        public Proposal()
        {
            SetDefaults();
        }


        protected void SetDefaults()
        {
            Guid = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            RequestedAmount = 0m;
            ApprovedAmount = 0m;
            Comments = new List<Comment>();
            Answers = new List<QuestionAnswer>();
            ReviewedByEditors = new List<ReviewedProposal>();
            Investigators = new List<Investigator>();
            IsApproved = false;
            IsDenied = false;
            IsNotified = false;
            IsSubmitted = false;

        }

        #endregion Constructor

        #region Mapped Fields

        public virtual Guid Guid { get; set; }
        [NotNull]
        public virtual CallForProposal CallForProposal { get; set; }
        [UCDArch.Core.NHibernateValidator.Extensions.Required]
        [Email]
        [Length(100)]
        public virtual string Email { get; set; }

        public virtual bool IsApproved { get; set; }
        public virtual bool IsDenied { get; set; }
        public virtual bool IsNotified { get; set; }
        public virtual bool IsSubmitted { get; set; }
        [RangeDouble(Min = 0.00, Message = "Minimum amount is zero")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public virtual decimal RequestedAmount { get; set; }
        [RangeDouble(Min = 0.00, Message = "Minimum amount is zero")]
        public virtual decimal ApprovedAmount { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? SubmittedDate { get; set; }
        public virtual DateTime? NotifiedDate { get; set; }
        public virtual bool WasWarned { get; set; }
        [NotNull]
        public virtual IList<Comment> Comments { get; set; }
        [NotNull]
        public virtual IList<QuestionAnswer> Answers { get; set; }
        [NotNull]
        public virtual IList<ReviewedProposal> ReviewedByEditors { get; set; }

        [NotNull]
        public virtual IList<Investigator> Investigators { get; set; }

        public virtual int Sequence { get; set; }

        #endregion Mapped Fields

        #region Methods

        public virtual void AddAnswer(Question question, string answer)
        {
            var questionAnswer = new QuestionAnswer();
            questionAnswer.Proposal = this;
            questionAnswer.Question = question;
            questionAnswer.Answer = answer;
            Answers.Add(questionAnswer);
        }

        public virtual void AddInvestigator(Investigator investigator)
        {
            investigator.Proposal = this;
            Investigators.Add(investigator);
        }

        #endregion Methods

    }

    public class ProposalMap : ClassMap<Proposal>
    {
        public ProposalMap()
        {
            Id(x => x.Id);
            Map(x => x.Guid).Unique();
            Map(x => x.Email);
            Map(x => x.IsApproved);
            Map(x => x.IsDenied);
            Map(x => x.IsNotified);
            Map(x => x.RequestedAmount).CustomSqlType("money");
            Map(x => x.ApprovedAmount);
            Map(x => x.IsSubmitted);
            Map(x => x.CreatedDate);
            Map(x => x.SubmittedDate);
            Map(x => x.NotifiedDate);
            Map(x => x.WasWarned);
            Map(x => x.Sequence);

            References(x => x.CallForProposal).Not.Nullable();
            HasMany(x => x.Comments);
            HasMany(x => x.Answers).Cascade.AllDeleteOrphan();
            HasMany(x => x.Investigators).Cascade.AllDeleteOrphan();
            HasMany(x => x.ReviewedByEditors);//.Table("ReviewedProposals");
        }
    }
}
