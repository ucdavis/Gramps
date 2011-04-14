using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            rtValue.Proposal = Repository.OfType<Proposal>().Queryable.First();

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
        
        #region Answer Tests        

        #region Valid Tests

        /// <summary>
        /// Tests the Answer with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithNullValueSaves()
        {
            #region Arrange
            var questionAnswer = GetValid(9);
            questionAnswer.Answer = null;
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.EnsurePersistent(questionAnswer);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionAnswer.IsTransient());
            Assert.IsTrue(questionAnswer.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithEmptyStringSaves()
        {
            #region Arrange
            var questionAnswer = GetValid(9);
            questionAnswer.Answer = string.Empty;
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.EnsurePersistent(questionAnswer);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionAnswer.IsTransient());
            Assert.IsTrue(questionAnswer.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with one space saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithOneSpaceSaves()
        {
            #region Arrange
            var questionAnswer = GetValid(9);
            questionAnswer.Answer = " ";
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.EnsurePersistent(questionAnswer);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionAnswer.IsTransient());
            Assert.IsTrue(questionAnswer.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithOneCharacterSaves()
        {
            #region Arrange
            var questionAnswer = GetValid(9);
            questionAnswer.Answer = "x";
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.EnsurePersistent(questionAnswer);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(questionAnswer.IsTransient());
            Assert.IsTrue(questionAnswer.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Answer with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAnswerWithLongValueSaves()
        {
            #region Arrange
            var questionAnswer = GetValid(9);
            questionAnswer.Answer = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.EnsurePersistent(questionAnswer);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, questionAnswer.Answer.Length);
            Assert.IsFalse(questionAnswer.IsTransient());
            Assert.IsTrue(questionAnswer.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Answer Tests

        #region Proposal Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Proposal with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestProposalWithAValueOfNullDoesNotSave()
        {
            QuestionAnswer record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Proposal = null;
                #endregion Arrange

                #region Act
                QuestionAnswerRepository.DbContext.BeginTransaction();
                QuestionAnswerRepository.EnsurePersistent(record);
                QuestionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Proposal, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Proposal: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestProposalWithANewValueDoesNotSave()
        {
            QuestionAnswer record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Proposal = CreateValidEntities.Proposal(1);
                #endregion Arrange

                #region Act
                QuestionAnswerRepository.DbContext.BeginTransaction();
                QuestionAnswerRepository.EnsurePersistent(record);
                QuestionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Proposal, Entity: Gramps.Core.Domain.Proposal", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionAnswerWithValidProposalSaves()
        {
            #region Arrange
            var questionAnswer = GetValid(9);
            questionAnswer.Proposal = Repository.OfType<Proposal>().GetNullableById(3);
            Assert.IsNotNull(questionAnswer.Proposal);
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.EnsurePersistent(questionAnswer);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("email3@testy.com", questionAnswer.Proposal.Email);
            Assert.IsFalse(questionAnswer.IsTransient());
            Assert.IsTrue(questionAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteQuestionAnswerDoesNotCascadeToProposal()
        {
            #region Arrange
            var proposalCount = Repository.OfType<Proposal>().Queryable.Count();
            Assert.IsTrue(proposalCount > 0);
            var record = QuestionAnswerRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.Remove(record);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(proposalCount, Repository.OfType<Proposal>().Queryable.Count());
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion Proposal Tests

        #region Question Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Question with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithAValueOfNullDoesNotSave()
        {
            QuestionAnswer record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Question = null;
                #endregion Arrange

                #region Act
                QuestionAnswerRepository.DbContext.BeginTransaction();
                QuestionAnswerRepository.EnsurePersistent(record);
                QuestionAnswerRepository.DbContext.CommitTransaction();
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
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestQuestionWithANewValueDoesNotSave()
        {
            QuestionAnswer record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Question = CreateValidEntities.Question(1);
                #endregion Arrange

                #region Act
                QuestionAnswerRepository.DbContext.BeginTransaction();
                QuestionAnswerRepository.EnsurePersistent(record);
                QuestionAnswerRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueGramps.Core.Domain.QuestionAnswer.Question", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionAnswerWithValidQuestionSaves()
        {
            #region Arrange
            var questionAnswer = GetValid(9);
            questionAnswer.Question = Repository.OfType<Question>().GetNullableById(2);
            Assert.IsNotNull(questionAnswer.Question);
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.EnsurePersistent(questionAnswer);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual("Name2", questionAnswer.Question.Name);
            Assert.IsFalse(questionAnswer.IsTransient());
            Assert.IsTrue(questionAnswer.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteQuestionAnswerDoesNotCascadeToQuestion()
        {
            #region Arrange
            var questionCount = Repository.OfType<Question>().Queryable.Count();
            Assert.IsTrue(questionCount > 0);
            var record = QuestionAnswerRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            QuestionAnswerRepository.DbContext.BeginTransaction();
            QuestionAnswerRepository.Remove(record);
            QuestionAnswerRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(questionCount, Repository.OfType<Question>().Queryable.Count());
            #endregion Assert
        }


        #endregion Cascade Tests
        #endregion Question Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapQuestionAnswer()
        {
            #region Arrange
            var id = QuestionAnswerRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var question = Repository.OfType<Question>().GetNullableById(1);
            var proposal = Repository.OfType<Proposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<QuestionAnswer>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Answer, "Answer")
                .CheckProperty(c => c.Question, question)
                .CheckProperty(c => c.Proposal, proposal)
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
            expectedFields.Add(new NameAndType("Answer", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Proposal", "Gramps.Core.Domain.Proposal", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Question", "Gramps.Core.Domain.Question", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(QuestionAnswer));

        }

        #endregion Reflection of Database.	
		
		
    }
}