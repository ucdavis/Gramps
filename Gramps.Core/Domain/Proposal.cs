﻿using System;
using System.Collections.Generic;
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
            Comments = new List<Comment>();
            Answers = new List<QuestionAnswer>();
        }

        #endregion Constructor

        #region Mapped Fields

        public virtual Guid Guid { get; set; }
        [NotNull]
        public virtual CallForProposal CallForProposal { get; set; }
        [Required]
        [Email]
        [Length(100)]
        public virtual string Email { get; set; }

        public virtual bool IsApproved { get; set; }
        public virtual bool IsDenied { get; set; }
        public virtual bool IsNotified { get; set; }
        public virtual bool IsSubmitted { get; set; }

        public virtual decimal RequestedAmount { get; set; }
        public virtual decimal ApprovedAmount { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? SubmittedDate { get; set; }
        public virtual DateTime? NotifiedDate { get; set; }
        public virtual IList<Comment> Comments { get; set; }
        public virtual IList<QuestionAnswer> Answers { get; set; }

        #endregion Mapped Fields
    }

    public class ProposalMap : ClassMap<Proposal>
    {
        public ProposalMap()
        {
            Id(x => x.Id);
            Map(x => x.Guid);
            Map(x => x.Email);
            Map(x => x.IsApproved);
            Map(x => x.IsDenied);
            Map(x => x.IsNotified);
            Map(x => x.RequestedAmount);
            Map(x => x.ApprovedAmount);
            Map(x => x.IsSubmitted);
            Map(x => x.CreatedDate);
            Map(x => x.SubmittedDate);
            Map(x => x.NotifiedDate);

            References(x => x.CallForProposal);
            HasMany(x => x.Comments);
            HasMany(x => x.Answers);
        }
    }
}