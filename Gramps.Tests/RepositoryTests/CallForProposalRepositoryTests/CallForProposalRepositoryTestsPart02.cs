using System;
using Gramps.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region CreatedDate Tests

        /// <summary>
        /// Tests the CreatedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            CallForProposal record = GetValid(99);
            record.CreatedDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreatedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the CreatedDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.CreatedDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreatedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the CreatedDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.CreatedDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreatedDate);
            #endregion Assert
        }
        #endregion CreatedDate Tests

        #region EndDate Tests

        /// <summary>
        /// Tests the EndDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestEndDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            CallForProposal record = GetValid(99);
            record.EndDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EndDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the EndDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestEndDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.EndDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EndDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the EndDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestEndDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.EndDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EndDate);
            #endregion Assert
        }
        #endregion EndDate Tests

        #region CallsSentDate Tests

        /// <summary>
        /// Tests the CallsSentDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestCallsSentDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            CallForProposal record = GetValid(99);
            record.CallsSentDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CallsSentDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the CallsSentDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestCallsSentDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.CallsSentDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CallsSentDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the CallsSentDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestCallsSentDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.CallsSentDate = compareDate;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CallsSentDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestCallsSentDateWithNullDateDateWillSave()
        {
            #region Arrange
            var record = GetValid(99);
            record.CallsSentDate = null;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNull(record.CallsSentDate);
            #endregion Assert
        }
        #endregion CallsSentDate Tests
    }
}
