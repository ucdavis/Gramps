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
        public void SetDefaults()
        {
            IsActive = true;
        }
        #endregion Constructor

        #region Mapped Fields
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        #endregion Mapped Fields
    }

    public class TemplateMap : ClassMap<Template>
    {
        public TemplateMap()
        {
            Map(x => x.Name);
            Map(x => x.IsActive);
        }
    }
}