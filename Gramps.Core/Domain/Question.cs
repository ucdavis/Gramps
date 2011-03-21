using System;
using System.Collections.Generic;
using System.Linq;
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
        protected void SetDefaults()
        {
            Options = new List<QuestionOption>();
            Validators = new List<Validator>();
            //Answers = new List<QuestionAnswer>();
        }
        #endregion Constructor

        #region Mapped Fields
        [Required]
        [Length(500)]
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

        [AssertTrue(Message = "The question type requires at least one option.")]
        private bool OptionsRequired
        {
            get
            {
                if (QuestionType != null && QuestionType.HasOptions)
                {
                    if (Options != null && Options.Count > 0)
                    {
                        return Options.Any(o => o.Name != null && !string.IsNullOrEmpty(o.Name.Trim()));
                    }
                    return false; //Fail it, they are required
                }
                return true;
            }
        }

        [AssertTrue(Message = "Options not allowed")]
        private bool OptionsNotAllowed
        {
            get
            {
                if (QuestionType != null && QuestionType.HasOptions)
                {
                    return true;
                }
                else
                {
                    if (Options != null && Options.Count > 0)
                    {
                        return false; //Fail it, they are not allowed
                    }
                }
                return true;
            }
        }

        [AssertTrue(Message = "One or more options is invalid")]
        private bool OptionsNames
        {
            get
            {
                if (QuestionType != null && QuestionType.HasOptions)
                {
                    if (Options != null && Options.Count > 0)
                    {
                        return Options.All(o => o.Name != null && !string.IsNullOrEmpty(o.Name.Trim()));
                    }
                }
                return true;
            }
        }
        #endregion Validation Fields

        #region Methods
        public virtual void AddQuestionOption(QuestionOption questionOption)
        {
            var newQuestionOption = new QuestionOption();
            newQuestionOption.Question = this;
            newQuestionOption.Name = questionOption.Name;
            Options.Add(newQuestionOption);
        }

        public virtual string ValidationClasses
        {
            get
            {
                return string.Join(" ", Validators.Select(a => a.Class).ToArray());
            }
        }

        public virtual string OptionChoices
        {
            get { return string.Join(" ", Options.Select(a => a.Name).ToArray()); }
        }

        public virtual void Addvalidators(Validator validator)
        {
            Validators.Add(validator);
        }

        #endregion Methods


    }

    public class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Order).Column("`Order`");

            References(x => x.QuestionType);
            References(x => x.Template);
            References(x => x.CallForProposal);

            HasMany(x => x.Options).Inverse().Cascade.AllDeleteOrphan();
            //HasManyToMany(x => x.Validators).Table("QuestionXValidator").Cascade.SaveUpdate();
            HasManyToMany(x => x.Validators).NotFound.Exception();// .Cascade.SaveUpdate();
            //HasMany(x => x.Answers);
        }
    }
}
