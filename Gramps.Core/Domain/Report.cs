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
    public class Report : DomainObject
    {
        #region Constructor

        #endregion Constructor

        #region Mapped Fields
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
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

    public class ReportMap : ClassMap<Report>
    {
        public ReportMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);

            References(x => x.Template);
            References(x => x.CallForProposal);

        }
    }
}