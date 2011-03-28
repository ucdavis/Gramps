using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class Validator : DomainObject
    {
        //ReadOnly
        [Required]
        [Length(50)]
        public virtual string Name { get; set; }
        [Length(50)]
        public virtual string Class { get; set; }

        public virtual string RegEx { get; set; }
        [Length(200)]
        public virtual string ErrorMessage { get; set; }
    }

    public class ValidatorMap : ClassMap<Validator>
    {
        public ValidatorMap()
        {
            ReadOnly();
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Class);
            Map(x => x.RegEx);
            Map(x => x.ErrorMessage);

        }
    }
}
