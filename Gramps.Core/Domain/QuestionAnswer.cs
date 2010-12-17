using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace Gramps.Core.Domain
{
    public class QuestionAnswer : DomainObject
    {
        #region Constructor

        #endregion Constructor

        #region Mapped Fields

        public virtual string Answer { get; set; }
        [NotNull]
        public virtual Proposal Proposal { get; set; }
        [NotNull]
        public virtual Question Question { get; set; }

        #endregion Mapped Fields
    }

    public class QuestionAnswerMap : ClassMap<QuestionAnswer>
    {
        public QuestionAnswerMap()
        {
            Table("Answers");
            Id(x => x.Id);
            Map(x => x.Answer);
            References(x => x.Proposal);
            References(x => x.Question).Not.Nullable();
        }
    }
}
