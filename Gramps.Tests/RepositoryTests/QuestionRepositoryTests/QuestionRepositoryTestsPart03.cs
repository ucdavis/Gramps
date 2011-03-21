using System;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests.QuestionRepositoryTests
{

    public partial class QuestionRepositoryTests
    {
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
    }
}
