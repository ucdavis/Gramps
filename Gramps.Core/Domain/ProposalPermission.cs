using System;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace Gramps.Core.Domain
{
    public class ProposalPermission : DomainObject
    {
        #region Constructor
        public ProposalPermission(Proposal proposal)
        {
            SetDefaults();
            Proposal = proposal;
            AllowReview = false;
            AllowEdit = false;
            AllowSubmit = false;

        }

        public ProposalPermission()
        {
            SetDefaults();
        }

        protected void SetDefaults()
        {
            DateAdded = DateTime.Now;
            DateUpdated = DateTime.Now;
        }

        #endregion Constructor

        #region Mapped Fields
        [NotNull]
        public virtual Proposal Proposal { get; set; }

        [UCDArch.Core.NHibernateValidator.Extensions.Required]
        [Email]
        [Length(100)]
        public virtual string Email { get; set; }

        public virtual bool AllowReview { get; set; }
        public virtual bool AllowEdit { get; set; }
        public virtual bool AllowSubmit { get; set; }

        public virtual DateTime DateAdded { get; set; }
        public virtual DateTime DateUpdated { get; set; }


        #endregion Mapped Fields
    }

    public class ProposalPermissionMap : ClassMap<ProposalPermission>
    {
        public ProposalPermissionMap()
        {
            Id(x => x.Id);
            Map(x => x.Email);
            Map(x => x.AllowReview);
            Map(x => x.AllowEdit);
            Map(x => x.AllowSubmit);
            Map(x => x.DateAdded);
            Map(x => x.DateUpdated);

            References(x => x.Proposal).Not.Nullable();

        }
    }
}