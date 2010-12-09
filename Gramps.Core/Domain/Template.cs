using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class Template : DomainObject
    {
        #region Constructor
        public Template(string name)
        {
            SetDefaults();
            Name = name;
        }
        public Template()
        {
            SetDefaults();
        }
        protected void SetDefaults()
        {
            IsActive = true;
            Emails = new List<EmailsForCall>();
            EmailTemplates = new List<EmailTemplate>();
            Editors = new List<Editor>();
            Questions = new List<Question>();
            CallForProposals = new List<CallForProposal>();
        }
        #endregion Constructor

        #region Mapped Fields
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual IList<EmailsForCall> Emails { get; set; }
        public virtual IList<EmailTemplate> EmailTemplates { get; set; }
        public virtual IList<Editor> Editors { get; set; }
        public virtual IList<Question> Questions { get; set; }
        public virtual IList<CallForProposal> CallForProposals { get; set; }

        #endregion Mapped Fields
    }

    public class TemplateMap : ClassMap<Template>
    {
        public TemplateMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);

            HasMany(x => x.Emails);
            HasMany(x => x.EmailTemplates);
            HasMany(x => x.Editors);
            HasMany(x => x.Questions);
            HasMany(x => x.CallForProposals);
        }
    }
}