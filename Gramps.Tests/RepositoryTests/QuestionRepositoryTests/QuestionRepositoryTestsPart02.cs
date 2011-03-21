using System;
using System.Linq;
using Gramps.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests.QuestionRepositoryTests
{

    public partial class QuestionRepositoryTests
    {
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
    }
}
