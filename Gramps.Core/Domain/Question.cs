using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class Question : DomainObject
    {
        #region Constructor
        public Question()
        {
            SetDefaults();
        }
        public void SetDefaults()
        {
            Options = new List<QuestionOption>();
            Validators = new List<Validator>();
            //Answers = new List<QuestionAnswer>();
        }
        #endregion Constructor

        #region Mapped Fields
        [Required]
        [Length(100)]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual QuestionType QuestionType { get; set; }

        public virtual int Order { get; set; }

        public virtual Template Template { get; set; }
        public virtual CallForProposal CallForProposal { get; set; }

        [NotNull]
        public virtual IList<QuestionOption> Options { get; set; }
        [NotNull]
        public virtual IList<Validator> Validators { get; set; }

        //public virtual IList<QuestionAnswer> Answers { get; set; } //We only care about it from the proposal 

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

    public class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            Map(x => x.Name);
            Map(x => x.Order);

            References(x => x.QuestionType);
            References(x => x.Template);
            References(x => x.CallForProposal);

            HasMany(x => x.Options);
            HasManyToMany(x => x.Validators).Table("QuestionXValidator").Cascade.SaveUpdate();
            //HasMany(x => x.Answers);
        }
    }
}
