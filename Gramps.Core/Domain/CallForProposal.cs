using System;
using System.Collections.Generic;
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

        protected void SetDefaults()
        {
            IsActive = false;
            CreatedDate = DateTime.Now;
            Emails = new List<EmailsForCall>();
            EmailTemplates = new List<EmailTemplate>();
            Editors = new List<Editor>();
            Questions = new List<Question>();
            Proposals = new List<Proposal>();
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

        public virtual IList<EmailsForCall> Emails { get; set; }
        public virtual IList<EmailTemplate> EmailTemplates { get; set; }
        public virtual IList<Editor> Editors { get; set; }
        public virtual IList<Question> Questions { get; set; }
        public virtual IList<Proposal> Proposals { get; set; }
        #endregion Mapped Fields
    }

    public class CallForProposalMap : ClassMap<CallForProposal>
    {
        public CallForProposalMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);
            Map(x => x.CreatedDate);
            Map(x => x.EndDate);
            Map(x => x.CallsSentDate);
            References(x => x.TemplateGeneratedFrom);
            HasMany(x => x.Emails);
            HasMany(x => x.EmailTemplates);
            HasMany(x => x.Editors);
            HasMany(x => x.Questions);
            HasMany(x => x.Proposals);
        }
    }
}
