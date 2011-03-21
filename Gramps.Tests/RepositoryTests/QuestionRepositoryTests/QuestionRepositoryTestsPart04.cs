using System;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;

namespace Gramps.Tests.RepositoryTests.QuestionRepositoryTests
{

    public partial class QuestionRepositoryTests
    {
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
    }
}
