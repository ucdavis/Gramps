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

    public partial class QuestionRepositoryTests
    {
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
    }
}
