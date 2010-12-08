using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class QuestionType : DomainObject
    {
        //ReadOnly
        public QuestionType()
        {

        }
        [Required]
        [Length(50)]
        public virtual string Name { get; set; }

        public virtual bool HasOptions { get; set; }
        public virtual bool ExtendedProperty { get; set; }
    }

    public class QuestionTypeMap : ClassMap<QuestionType>
    {
        public QuestionTypeMap()
        {
            ReadOnly();
            Map(x => x.Name);
            Map(x => x.HasOptions);
            Map(x => x.ExtendedProperty);
        }
    }
}
