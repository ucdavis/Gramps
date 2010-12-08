using System;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class CallForProposal : DomainObject
    {
        #region Constructor

        public CallForProposal()
        {
            SetDefaults();
        }

        public void SetDefaults()
        {
            IsActive = false;
            CreatedDate = DateTime.Now;
        }

        #endregion Constructor

        #region Mapped Fields
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        [NotNull]
        public virtual DateTime CreatedDate { get; set; }
        [NotNull]
        public virtual DateTime EndDate { get; set; }

        public virtual DateTime? CallsSentDate { get; set; }

        public virtual Template TemplateGeneratedFrom { get; set; }
        #endregion Mapped Fields
    }

    public class CallForProposalMap : ClassMap<CallForProposal>
    {
        public CallForProposalMap()
        {
            Map(x => x.Name);
            Map(x => x.IsActive);
            Map(x => x.CreatedDate);
            Map(x => x.EndDate);
            Map(x => x.CallsSentDate);
            References(x => x.TemplateGeneratedFrom);
        }
    }
}
