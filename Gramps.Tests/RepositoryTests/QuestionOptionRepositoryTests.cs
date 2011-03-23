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
    /// Entity Name:		QuestionOption
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class QuestionOptionRepositoryTests : AbstractRepositoryTests<QuestionOption, int, QuestionOptionMap>
    {
        /// <summary>
        /// Gets or sets the QuestionOption repository.
        /// </summary>
        /// <value>The QuestionOption repository.</value>
        public IRepository<QuestionOption> QuestionOptionRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionOptionRepositoryTests"/> class.
        /// </summary>
        public QuestionOptionRepositoryTests()
        {
            QuestionOptionRepository = new Repository<QuestionOption>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override QuestionOption GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.QuestionOption(counter);
            rtValue.Question = Repository.OfType<Question>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<QuestionOption> GetQuery(int numberAtEnd)
        {
            return QuestionOptionRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(QuestionOption entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(QuestionOption entity, ARTAction action)
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
                    entity.Name  = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<Question>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(1);
            LoadQuestionTypes();
            LoadQuestions(5);
            Repository.OfType<Question>().DbContext.CommitTransaction();
            QuestionOptionRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            QuestionOptionRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = null;
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = " ";
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            QuestionOption questionOption = null;
            try
            {
                #region Arrange
                questionOption = GetValid(9);
                questionOption.Name = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(questionOption);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(questionOption);
                Assert.AreEqual(200 + 1, questionOption.Name.Length);
                var results = questionOption.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 200");
                Assert.IsTrue(questionOption.IsTransient());
                Assert.IsFalse(questionOption.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var questionOption = GetValid(9);
            questionOption.Name = "x";
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.EnsurePersistent(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionOption.IsTransient());
            Assert.IsTrue(questionOption.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var questionOption = GetValid(9);
            questionOption.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.EnsurePersistent(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, questionOption.Name.Length);
            Assert.IsFalse(questionOption.IsTransient());
            Assert.IsTrue(questionOption.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Question Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Question with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithAValueOfNullDoesNotSave()
        {
            QuestionOption record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Question = null;
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(record);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Question, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Question: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestQuestionWithANewValueDoesNotSave()
        {
            QuestionOption record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Question = CreateValidEntities.Question(1);
                #endregion Arrange

                #region Act
                QuestionOptionRepository.DbContext.BeginTransaction();
                QuestionOptionRepository.EnsurePersistent(record);
                QuestionOptionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Question, Entity: Gramps.Core.Domain.Question", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestCommentWithValidQuestionSaves()
        {
            #region Arrange
            var questionOption = GetValid(9);
            questionOption.Question = Repository.OfType<Question>().GetNullableById(2);
            Assert.IsNotNull(questionOption.Question);
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.EnsurePersistent(questionOption);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("Name2", questionOption.Question.Name);
            Assert.IsFalse(questionOption.IsTransient());
            Assert.IsTrue(questionOption.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteQuestionOptionDoesNotCascadeToQuestion()
        {
            #region Arrange
            var questionCount = Repository.OfType<Question>().Queryable.Count();
            Assert.IsTrue(questionCount > 0);
            var record = QuestionOptionRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            QuestionOptionRepository.DbContext.BeginTransaction();
            QuestionOptionRepository.Remove(record);
            QuestionOptionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(questionCount, Repository.OfType<Question>().Queryable.Count());
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion Question Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapQuestionOption()
        {
            #region Arrange
            var id = QuestionOptionRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var question = Repository.OfType<Question>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<QuestionOption>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.Question, question)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        #endregion Fluent Mapping Tests
        
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
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Question", "Gramps.Core.Domain.Question", new List<string>
            {
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(QuestionOption));

        }

        #endregion Reflection of Database.	
		
		
    }
}