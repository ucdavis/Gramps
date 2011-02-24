using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class Investigator : DomainObject
    {
        #region Constructor

        

        #endregion Constructor

        #region Mapped Fields

        public virtual Proposal Proposal {get; set;}
        public virtual bool IsPrimary { get; set; }

        [UCDArch.Core.NHibernateValidator.Extensions.Required]
        [Length(200)]
        public virtual string Name { get; set; }

        [UCDArch.Core.NHibernateValidator.Extensions.Required]
        [Length(200)]
        public virtual string Institution { get; set; }

        [UCDArch.Core.NHibernateValidator.Extensions.Required]
        [Length(200)]
        public virtual string Address1 { get; set; }

        [Length(200)]
        public virtual string Address2 { get; set; }

        [Length(200)]
        public virtual string Address3 { get; set; }

        [Length(50)]
        public virtual string Phone { get; set; }

        [UCDArch.Core.NHibernateValidator.Extensions.Required]
        [Email]
        [Length(200)]
        public virtual string Email { get; set; }

        public virtual string Notes { get; set; }

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
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.Notes);
        }
    }
}
