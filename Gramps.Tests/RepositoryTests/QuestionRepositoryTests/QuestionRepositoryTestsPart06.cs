using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests.QuestionRepositoryTests
{

    public partial class QuestionRepositoryTests
    {
        #region Validators Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestValidatorsWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Validators = null;
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
                results.AssertErrorsAre("Validators: may not be null");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestValidatorsWithNewValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Validators.Add(new Validator());
                question.Validators[0].Class = "class";
                question.Validators[0].Name = "name";
                question.Validators[0].ErrorMessage = "err";
                question.Validators[0].RegEx = "test";
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
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Validator, Entity: Gramps.Core.Domain.Validator", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionWithEmptyValidatorsSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Validators = new List<Validator>();
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Validators.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestQuestionWithPopulatedExistingValidatorsSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(1));
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(2));
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(3));
            for (int i = 0; i < 3; i++)
            {
                Assert.IsNotNull(record.Validators[i]);
            }
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Validators.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestQuestionsWithValidatorsDoesNotCascadeDeleteValidators()
        {
            #region Arrange
            var count = Repository.OfType<Validator>().Queryable.Count();
            Assert.IsTrue(count > 0);
            var record = GetValid(9);
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(1));
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(2));
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(3));
            for (int i = 0; i < 3; i++)
            {
                Assert.IsNotNull(record.Validators[i]);
            }

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            Assert.AreEqual(3, record.Validators.Count);
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.Remove(record);
            QuestionRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.IsNull(QuestionRepository.GetNullableById(saveId));
            Assert.AreEqual(count, Repository.OfType<Validator>().Queryable.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestQuestionsWithValidatorsDoesNotCascadeDeleteValidatorsWhenUpdated()
        {
            #region Arrange
            var count = Repository.OfType<Validator>().Queryable.Count();
            Assert.IsTrue(count > 0);
            var record = GetValid(9);
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(1));
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(2));
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(3));
            for (int i = 0; i < 3; i++)
            {
                Assert.IsNotNull(record.Validators[i]);
            }

            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            Assert.AreEqual(3, record.Validators.Count);
            record.Validators.Clear();
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(1));
            record.Validators.Add(Repository.OfType<Validator>().GetNullableById(3));
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(record);
            QuestionRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            record = QuestionRepository.GetNullableById(saveId);
            Assert.IsNotNull(record);
            Assert.AreEqual(2, record.Validators.Count);
            Assert.AreEqual(count, Repository.OfType<Validator>().Queryable.Count());
            #endregion Assert
        }


        #endregion Cascade Tests
        #endregion Validators Tests
    }
}
