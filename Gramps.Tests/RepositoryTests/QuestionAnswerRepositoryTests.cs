using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		QuestionAnswer
    /// LookupFieldName:	Answer
    /// </summary>
    [TestClass]
    public class QuestionAnswerRepositoryTests : AbstractRepositoryTests<QuestionAnswer, int, QuestionAnswerMap>
    {
        /// <summary>
        /// Gets or sets the QuestionAnswer repository.
        /// </summary>
        /// <value>The QuestionAnswer repository.</value>
        public IRepository<QuestionAnswer> QuestionAnswerRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionAnswerRepositoryTests"/> class.
        /// </summary>
        public QuestionAnswerRepositoryTests()
        {
            QuestionAnswerRepository = new Repository<QuestionAnswer>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override QuestionAnswer GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.QuestionAnswer(counter);
            rtValue.Question = Repository.OfType<Question>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<QuestionAnswer> GetQuery(int numberAtEnd)
        {
            return QuestionAnswerRepository.Queryable.Where(a => a.Answer.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(QuestionAnswer entity, int counter)
        {
            Assert.AreEqual("Answer" + counter, entity.Answer);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(QuestionAnswer entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Answer);
                    break;
                case ARTAction.Restore:
                    entity.Answer = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Answer;
                    entity.Answer = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            LoadValidators();
            LoadUsers(3);
            LoadCallForProposals(2);
            LoadQuestions(2);
            LoadProposals(3);
            Repository.OfType<Question>().DbContext.CommitTransaction();
            QuestionAnswerRepository.DbContext.BeginTransaction();            
            LoadRecords(6);
            QuestionAnswerRepository.DbContext.CommitTransaction();
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

            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
                                                                         {
                                                                             "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                                                                             "[System.Xml.Serialization.XmlIgnoreAttribute()]"
                                                                         }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(QuestionAnswer));

        }

        #endregion Reflection of Database.	
		
		
    }
}