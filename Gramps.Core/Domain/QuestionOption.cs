using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class QuestionOption : DomainObject
    {
        [Required]
        [Length(200)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }
    }

    public class QuestionOptionMap : ClassMap<QuestionOption>
    {
        public QuestionOptionMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            References(x => x.Question);
        }
    }
}
