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
        public CallForProposal(Template template)
        {            
            SetDefaults();
            TemplateGeneratedFrom = template;
            foreach (var emailsForCall in template.Emails)
            {
                AddEmailForCall(emailsForCall.Email);
            }
            //TODO: Go throught the template and populate the call for proposal
        }
        public CallForProposal(string name)
        {
            SetDefaults();
            Name = name;
        }

        public CallForProposal()
        {
            SetDefaults();
        }

        protected void SetDefaults()
        {
            IsActive = true;
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
        
        [NotNull]
        public virtual IList<EmailsForCall> Emails { get; set; }
        
        [NotNull]
        public virtual IList<EmailTemplate> EmailTemplates { get; set; }
        
        [NotNull]
        public virtual IList<Editor> Editors { get; set; }
        
        [NotNull]
        public virtual IList<Question> Questions { get; set; }
        
        [NotNull]
        public virtual IList<Proposal> Proposals { get; set; }
        #endregion Mapped Fields

        #region Methods

        public virtual void AddEmailForCall(string email)
        {
            var emailForCall = new EmailsForCall(email);
            emailForCall.CallForProposal = this;
            //if (emailForCall.IsValid())
            //{ 
            //    Emails.Add(emailForCall);
            //}
            Emails.Add(emailForCall);
        }
        public virtual void RemoveEmailForCall(EmailsForCall emailsForCall)
        {
            if (Emails != null && Emails.Contains(emailsForCall) && !emailsForCall.HasBeenEmailed)
            {
                Emails.Remove(emailsForCall);
            }
        }

        #endregion Methods

        #region ValidationOnlyFields

        [AssertTrue(Message = "One or more invalid emails for call detected")]
        private bool EmailsForCallList
        {
            get
            {
                if (Emails != null)
                {
                    foreach (var emailsForCall in Emails)
                    {
                        if (!emailsForCall.IsValid())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        #endregion ValidationOnlyFields

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
            HasMany(x => x.Emails).Inverse().Cascade.AllDeleteOrphan();
            HasMany(x => x.EmailTemplates);
            HasMany(x => x.Editors);
            HasMany(x => x.Questions);
            HasMany(x => x.Proposals);
        }
    }
}
