using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests.QuestionRepositoryTests
{
    /// <summary>
    /// Entity Name:		Question
    /// LookupFieldName:	Name yrjuy
    /// </summary>
    [TestClass]
    public partial class QuestionRepositoryTests : AbstractRepositoryTests<Question, int, QuestionMap>
    {
        /// <summary>
        /// Gets or sets the Question repository.
        /// </summary>
        /// <value>The Question repository.</value>
        public IRepository<Question> QuestionRepository { get; set; }

        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionRepositoryTests"/> class.
        /// </summary>
        public QuestionRepositoryTests()
        {
            QuestionRepository = new Repository<Question>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Question GetValid(int? counter)
        {
            Template template = null;
            CallForProposal callForProposal = null;
            if (counter.HasValue && counter.Value % 2 == 0)
            {
                callForProposal = Repository.OfType<CallForProposal>().Queryable.First();
            }
            else
            {
                template = Repository.OfType<Template>().Queryable.First();
            }
            var rtvalue = CreateValidEntities.Question(counter, template, callForProposal);
            rtvalue.QuestionType = Repository.OfType<QuestionType>().Queryable.First();

            return rtvalue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Question> GetQuery(int numberAtEnd)
        {
            return QuestionRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Question entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Question entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            LoadValidators();
            LoadUsers(3);
            LoadTemplates(3);
            LoadCallForProposals(2);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();
            QuestionRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            QuestionRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides



        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("CallForProposal", "Gramps.Core.Domain.CallForProposal", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)500)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            
            expectedFields.Add(new NameAndType("Options", "System.Collections.Generic.IList`1[Gramps.Core.Domain.QuestionOption]", new List<string>()));
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuestionType", "Gramps.Core.Domain.QuestionType", new List<string>
            { 
                 ""
            }));
            expectedFields.Add(new NameAndType("Template", "Gramps.Core.Domain.Template", new List<string>()));
            expectedFields.Add(new NameAndType("Validators", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Validator]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Question));

        }

        #endregion Reflection of Database.


    }
}