using System.Collections.Generic;
using System.Linq;
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
            Reports = new List<Report>();
        }
        #endregion Constructor

        #region Mapped Fields
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }

        [NotNull]
        public virtual IList<EmailsForCall> Emails { get; set; }
        [NotNull]
        public virtual IList<EmailTemplate> EmailTemplates { get; set; }
        [NotNull]
        public virtual IList<Editor> Editors { get; set; }
        [NotNull]
        public virtual IList<Question> Questions { get; set; }
        [NotNull]
        public virtual IList<CallForProposal> CallForProposals { get; set; }
        [NotNull]
        public virtual IList<Report> Reports { get; set; }


        #endregion Mapped Fields

        #region Methods

        public virtual bool IsEditor(string userId)
        {
            return Editors.Where(a => a.User != null && a.User.LoginId == userId).Any();
        }

        #endregion Methods
    }

    public class TemplateMap : ClassMap<Template>
    {
        public TemplateMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);

            HasMany(x => x.Emails).Cascade.DeleteOrphan();
            HasMany(x => x.EmailTemplates).Cascade.DeleteOrphan();
            HasMany(x => x.Editors).Cascade.DeleteOrphan();
            HasMany(x => x.Questions).Cascade.DeleteOrphan();
            HasMany(x => x.CallForProposals).Cascade.None();
            HasMany(x => x.Reports).Cascade.DeleteOrphan();
        }
    }
}