using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class EmailsForCall : DomainObject
    {
        #region Constructor
        public EmailsForCall(string emailAddress)
        {
            SetDefaults();
            Email = emailAddress.ToLower();
        }
        public EmailsForCall()
        {
            SetDefaults();
        }

        protected void SetDefaults()
        {
            HasBeenEmailed = false;
        }

        #endregion Constructor

        #region Mapped Fields

        [Required]
        [Email]
        [Length(100)]
        public virtual string Email { get; set; }

        public virtual Template Template { get; set; }
        public virtual CallForProposal CallForProposal { get; set; }

        public virtual bool HasBeenEmailed { get; set; }
        public virtual DateTime? EmailedOnDate { get; set; }

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

    public class EmailsForCallMap : ClassMap<EmailsForCall>
    {
        public EmailsForCallMap()
        {
            Table("Emails");
            Id(x => x.Id);
            Map(x => x.Email);
            Map(x => x.HasBeenEmailed);
            Map(x => x.EmailedOnDate);
            References(x => x.Template);
            References(x => x.CallForProposal);
        }
    }
}
