using System;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class EmailQueue : DomainObject
    {
        #region Constructor
        public EmailQueue()
        {
            SetDefaults();
        }

        public EmailQueue(CallForProposal callForProposal, string emailAddress, string subject, string body)
        {
            SetDefaults();
            CallForProposal = callForProposal;
            EmailAddress = emailAddress;
            Subject = subject;
            Body = body;
        }


        protected void SetDefaults()
        {
            Pending = true;
            Created = DateTime.Now;
            Immediate = false;
        }
        #endregion Constructor

        #region Mapped Fields
        [NotNull]
        public virtual CallForProposal CallForProposal { get; set; }

        [Required]
        [Length(200)]
        public virtual string EmailAddress { get; set; }

        public virtual DateTime Created { get; set; }
        public virtual bool Pending { get; set; }
        public virtual DateTime? SentDateTime { get; set; }

        [Required]
        [Length(200)]
        public virtual string Subject { get; set; }

        [Required]
        public virtual string Body { get; set; }

        public virtual bool Immediate { get; set; }

        #endregion Mapped Fields
    }

    public class EmailQueueMap : ClassMap<EmailQueue>
    {
        public EmailQueueMap()
        {
            Table("EmailQueue");

            Id(x => x.Id);
            References(x => x.CallForProposal);
            Map(x => x.EmailAddress);
            Map(x => x.Created);
            Map(x => x.Pending);
            Map(x => x.SentDateTime);
            Map(x => x.Subject);
            Map(x => x.Body);
            Map(x => x.Immediate);
        }
    }
}
