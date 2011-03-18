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
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)500)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Order", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("QuestionType", "Gramps.Core.Domain.QuestionType", new List<string>
            { 
                 ""
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Question));

        }

        #endregion Reflection of Database.	
		
		
    }
}