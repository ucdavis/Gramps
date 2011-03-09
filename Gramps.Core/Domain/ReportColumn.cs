using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class ReportColumn : DomainObject
    {
        #region Constructor
        public ReportColumn()
        {
            SetDefaults();
        }

        protected void SetDefaults()
        {
            IsProperty = false;
        }

        #endregion Constructor
        #region Mapped Fields

        [NotNull]
        public virtual Report Report { get; set; }

        public virtual int ColumnOrder { get; set; }
        [Required]
        [Length(500)]
        public virtual string Name { get; set; }
        [Length(50)]
        public virtual string Format { get; set; }

        public virtual bool IsProperty { get; set; }
        
        #endregion Mapped Fields
    }

    public class ReportColumnMap : ClassMap<ReportColumn>
    {
        public ReportColumnMap()
        {
            Id(x => x.Id);            
            Map(x => x.ColumnOrder);
            Map(x => x.Name);
            Map(x => x.Format);
            Map(x => x.IsProperty);

            References(x => x.Report);            
        }
    }
}