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
    /// Entity Name:		Question
    /// LookupFieldName:	Name yrjuy
    /// </summary>
    [TestClass]
    public class QuestionRepositoryTests : AbstractRepositoryTests<Question, int, QuestionMap>
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
            LoadUsers(3);
            LoadTemplates(3);
            LoadCallForProposals(2);
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();
            QuestionRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            QuestionRepository.DbContext.CommitTransaction();
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = " ";
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = "x".RepeatTimes((500 + 1));
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                Assert.AreEqual(500 + 1, question.Name.Length);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 500");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
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
            var question = GetValid(9);
            question.Name = "x";
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Name = "x".RepeatTimes(500);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(500, question.Name.Length);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests
 
        #region QuestionType Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the QuestionType with A value of Null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionTypeWithAValueOfNullDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                Assert.AreEqual(question.QuestionType, null);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionType: may not be null");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithQuestionTypeNoOptionsWhenRequired1()
        {
            Question question = null;
            const string qtype = "Radio Buttons";
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                Assert.AreEqual(question.QuestionType.Name, qtype);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsRequired: The question type requires at least one option.");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithQuestionTypeNoOptionsWhenRequired2()
        {
            Question question = null;
            const string qtype = "Checkbox List";
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                Assert.AreEqual(question.QuestionType.Name, qtype);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsRequired: The question type requires at least one option.");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionWithQuestionTypeNoOptionsWhenRequired3()
        {
            Question question = null;
            const string qtype = "Drop Down";
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                Assert.AreEqual(question.QuestionType.Name, qtype);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsRequired: The question type requires at least one option.");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestQuestionWithNewQuestionTypeDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = new QuestionType();
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(question);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.QuestionType, Entity: Gramps.Core.Domain.QuestionType", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionWithQuestionType1()
        {
            #region Arrange
            const string qtype = "Text Box";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(qtype, question.QuestionType.Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert	
        }

        [TestMethod]
        public void TestQuestionWithQuestionType2()
        {
            #region Arrange
            const string qtype = "Boolean";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(qtype, question.QuestionType.Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionWithQuestionType3()
        {
            #region Arrange
            const string qtype = "Radio Buttons";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
            var questionOption = new QuestionOption("TaDa");
            question.AddQuestionOption(questionOption);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(qtype, question.QuestionType.Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionWithQuestionType4()
        {
            #region Arrange
            const string qtype = "Checkbox List";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
            var questionOption = new QuestionOption("TaDa");
            question.AddQuestionOption(questionOption);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(qtype, question.QuestionType.Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionWithQuestionType5()
        {
            #region Arrange
            const string qtype = "Drop Down";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
            var questionOption = new QuestionOption("TaDa");
            question.AddQuestionOption(questionOption);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(qtype, question.QuestionType.Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionWithQuestionType6()
        {
            #region Arrange
            const string qtype = "Date";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(qtype, question.QuestionType.Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionWithQuestionType7()
        {
            #region Arrange
            const string qtype = "No Answer";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(qtype, question.QuestionType.Name);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteQuestionDoesNotCascadeToQuestionType()
        {
            #region Arrange
            const string qtype = "Text Box";
            var question = GetValid(9);
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).First();
 
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            var saveId = question.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(question);
            #endregion Arrange

            #region Act
            question = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(question);
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.Remove(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNull(QuestionRepository.GetNullableById(saveId));
            Assert.IsNotNull(Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == qtype).FirstOrDefault());
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion QuestionType Tests
        
        #region Order Tests

        /// <summary>
        /// Tests the Order with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestOrderWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = int.MaxValue;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.Order);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Order with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestOrderWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Order = int.MinValue;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.Order);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Order Tests

        #region Template Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTemplateWithANewValueDoesNotSave()
        {
            Question record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = CreateValidEntities.Template(9);
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(record);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Template, Entity: Gramps.Core.Domain.Template", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateAndCallForProposalWithNullValueDoesNotSave()
        {
            Question record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = null;
                record.CallForProposal = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(record);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateAndCallForProposalWithNeitherNullDoesNotSave()
        {
            Question record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = Repository.OfType<Template>().GetNullableById(1);
                record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Assert.IsNotNull(record.CallForProposal);
                Assert.IsNotNull(record.Template);
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(record);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestTemplateWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Template = null;
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            Assert.IsNotNull(record.CallForProposal);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestTemplateWithNonNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Template = Repository.OfType<Template>().GetNullableById(1);
            record.CallForProposal = null;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Template);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteDoesNotCascadeToTemplate()
        {
            #region Arrange
            var record = GetValid(9);
            record.Template = Repository.OfType<Template>().GetNullableById(1);
            record.CallForProposal = null;

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();

            var templateCount = Repository.OfType<Template>().Queryable.Count();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.Remove(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(templateCount, Repository.OfType<Template>().Queryable.Count());
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Template Tests

        #region CallForProposal Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCallForProposalWithANewValueDoesNotSave()
        {
            Question record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = null;
                record.CallForProposal = CreateValidEntities.CallForProposal(9);
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(record);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.CallForProposal, Entity: Gramps.Core.Domain.CallForProposal", ex.Message);
                throw;
            }
        }


        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestCallForProposalWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Template = Repository.OfType<Template>().GetNullableById(1);
            record.CallForProposal = null;
            Assert.IsNotNull(record.Template);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCallForProposalWithNonNullValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            record.Template = null;
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CallForProposal);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteDoesNotCascadeToCallForProposals()
        {
            #region Arrange
            var record = GetValid(9);
            record.Template = null;
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();

            var callForProposalCount = Repository.OfType<CallForProposal>().Queryable.Count();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.Remove(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(callForProposalCount, Repository.OfType<CallForProposal>().Queryable.Count());
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion CallForProposals Tests

        #region Options Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Options = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Options: may not be null");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsWithEmptyListDoesNotSaveWhenQuestionTypeRequiresOptions()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Radio Buttons").Single();
                question.Options = new List<QuestionOption>();
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsRequired: The question type requires at least one option.");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsWithPopulatedListDoesNotSaveWhenQuestionTypeDoesNotRequireOptions()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Area").Single();
                question.AddQuestionOption(CreateValidEntities.QuestionOption(1));
                question.AddQuestionOption(CreateValidEntities.QuestionOption(2));
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsNotAllowed: Options not allowed");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestOptionsWithPopulatedListDoesNotSaveWhenInvalidOptions()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Radio Buttons").Single();
                question.AddQuestionOption(CreateValidEntities.QuestionOption(1));
                question.AddQuestionOption(CreateValidEntities.QuestionOption(2));
                question.AddQuestionOption(CreateValidEntities.QuestionOption(3));
                question.Options[1].Name = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("OptionsNames: One or more options is invalid");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestOptionsWithEmptyListSavesWhenOptionNotRequired()
        {
            #region Arrange
            var record = GetValid(9);
            record.Options = new List<QuestionOption>();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Options.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestOptionsWithPopulatedListSavesWhenOptionRequired()
        {
            #region Arrange
            var record = GetValid(9);
            record.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Radio Buttons").Single();
            record.AddQuestionOption(CreateValidEntities.QuestionOption(1));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(2));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(3));
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Options.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestSaveQuestionCascadesSaveToQuestionOptions()
        {
            #region Arrange
            var count = Repository.OfType<QuestionOption>().Queryable.Count();
            var record = GetValid(9);
            record.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Radio Buttons").Single();
            record.AddQuestionOption(CreateValidEntities.QuestionOption(1));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(2));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(3));
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + 3, Repository.OfType<QuestionOption>().Queryable.Count());
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            Assert.AreEqual(3, record.Options.Count);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSaveQuestionCascadesUpdateToQuestionOptions()
        {
            #region Arrange
            var count = Repository.OfType<QuestionOption>().Queryable.Where(a => a.Name == "Updated").Count();
            var record = GetValid(9);
            record.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Radio Buttons").Single();
            record.AddQuestionOption(CreateValidEntities.QuestionOption(1));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(2));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(3));

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            record.Options[1].Name = "Updated";
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + 1, Repository.OfType<QuestionOption>().Queryable.Where(a => a.Name == "Updated").Count());
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            Assert.AreEqual(3, record.Options.Count);
            Assert.AreEqual("Updated", record.Options[1].Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestSaveQuestionCascadesDeleteToQuestionOptions()
        {
            #region Arrange
            var count = Repository.OfType<QuestionOption>().Queryable.Count();
            var record = GetValid(9);
            record.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Radio Buttons").Single();
            record.AddQuestionOption(CreateValidEntities.QuestionOption(1));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(2));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(3));

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            var saveOptions = new List<QuestionOption>();
            saveOptions.Add(record.Options[0]);
            saveOptions.Add(record.Options[2]);
            record.Options.Clear();
            record.Options.Add(saveOptions[0]);
            record.Options.Add(saveOptions[1]);
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + 2, Repository.OfType<QuestionOption>().Queryable.Count());
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            Assert.AreEqual(2, record.Options.Count);
            Assert.AreEqual("Name1", record.Options[0].Name);
            Assert.AreEqual("Name3", record.Options[1].Name);
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Options Tests

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
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuestionType", "Gramps.Core.Domain.QuestionType", new List<string>
            { 
                 ""
            }));
            expectedFields.Add(new NameAndType("Template", "Gramps.Core.Domain.Template", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Question));

        }

        #endregion Reflection of Database.	
		
		
    }
}