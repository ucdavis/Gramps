using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class EmailTemplate : DomainObject
    {
        #region Constructor
        public EmailTemplate(string subject)
        {
            Subject = subject;
        }

        public EmailTemplate()
        {
            
        }


        #endregion Constructor

        #region Mapped Fields
        [NotNull(Message = "Must have email template type")]
        public virtual EmailTemplateType? TemplateType { get; set; } 

        public virtual string Text { get; set; }
        [Required]
        [Length(100)]
        public virtual string Subject { get; set; }

        public virtual Template Template { get; set; }
        public virtual CallForProposal CallForProposal { get; set; }

        #endregion Mapped Fields

        #region Validation Fields

        [AssertTrue(Message = "Must be related to Template or CallForProposal not both.")]
        private bool RelatedTable
        {
            get
            {
                if ((Template == null && CallForProposal == null) || (Template != null && CallForProposal != null))
                {
                    return false;
                }
                return true;
            }
        }

        #endregion Validation Fields
    }

    public class EmailTemplateMap : ClassMap<EmailTemplate>
    {
        public EmailTemplateMap()
        {
            Id(x => x.Id);
            Map(x => x.TemplateType).CustomType(typeof (NHibernate.Type.EnumStringType<EmailTemplateType>));
            Map(x => x.Text);
            Map(x => x.Subject);
            References(x => x.Template);
            References(x => x.CallForProposal);
        }
    }

    //Note, database has same values
    public enum EmailTemplateType
    {
        InitialCall,
        ReminderCallIsAboutToClose,
        ProposalApproved,
        ProposalDenied,
        ProposalConfirmation,
        ReadyForReview,
        ProposalUnsubmitted
    }
}
