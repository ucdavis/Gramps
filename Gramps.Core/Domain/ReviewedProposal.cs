using System;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace Gramps.Core.Domain
{
    public class ReviewedProposal : DomainObject
    {
        #region Constructor

        public ReviewedProposal()
        {
            SetDefaults();
        }

        public ReviewedProposal(Proposal proposal, Editor editor)
        {
            SetDefaults();
            Proposal = proposal;
            Editor = editor;
        }
        protected void SetDefaults()
        {
            FirstViewedDate = DateTime.Now;
            LastViewedDate = DateTime.Now;
        }

        #endregion Constructor

        #region Mapped Fields

        public virtual DateTime FirstViewedDate { get; set; }
        public virtual DateTime LastViewedDate { get; set; }

        [NotNull]
        public virtual Proposal Proposal { get; set; }
        [NotNull]
        public virtual Editor Editor { get; set; }
        #endregion Mapped Fields
    }

    public class ReviewedProposalMap : ClassMap<ReviewedProposal>
    {
        public ReviewedProposalMap()
        {
            Id(x => x.Id);
            Map(x => x.FirstViewedDate);
            Map(x => x.LastViewedDate);
            References(x => x.Proposal).Not.Nullable();
            References(x => x.Editor).Not.Nullable();
        }
    }
}
