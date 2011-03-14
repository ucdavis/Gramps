using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class Investigator : DomainObject
    {
        #region Constructor

        public Investigator()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            IsPrimary = false;
        }

        #endregion Constructor

        #region Mapped Fields
        [NotNull]
        public virtual Proposal Proposal {get; set;}
        public virtual bool IsPrimary { get; set; }

        [Required]
        [Length(200)]
        public virtual string Name { get; set; }

        [Required]
        [Length(200)]
        public virtual string Institution { get; set; }

        [Required]
        [Length(200)]
        public virtual string Address1 { get; set; }

        [Length(200)]
        public virtual string Address2 { get; set; }

        [Length(200)]
        public virtual string Address3 { get; set; }

        [Required]
        [Length(100)]
        public virtual string City { get; set; }

        [Required]
        [Length(2)]
        public virtual string State { get; set; }

        [Required]
        [Length(11)]
        public virtual string Zip { get; set; }
        
        [Length(50)]
        [Required]
        public virtual string Phone { get; set; }

        [Required]
        [Email]
        [Length(200)]
        public virtual string Email { get; set; }

        public virtual string Notes { get; set; }
        [Length(100)]
        public virtual string Position { get; set; } 

        #endregion Mapped Fields

    }

    public class InvestigatorMap : ClassMap<Investigator>
    {
        public InvestigatorMap()
        {
            Id(x => x.Id);

            References(x => x.Proposal);

            Map(x => x.IsPrimary);
            Map(x => x.Name);
            Map(x => x.Institution);
            Map(x => x.Address1);
            Map(x => x.Address2);
            Map(x => x.Address3);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.Zip);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.Notes);
            Map(x => x.Position);
        }
    }
}
