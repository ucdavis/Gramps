using System;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
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

        #region Description Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Description with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithNullValueSaves()
        {
            #region Arrange
            var callForProposal = GetValid(9);
            callForProposal.Description = null;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(callForProposal.IsTransient());
            Assert.IsTrue(callForProposal.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithEmptyStringSaves()
        {
            #region Arrange
            var callForProposal = GetValid(9);
            callForProposal.Description = string.Empty;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(callForProposal.IsTransient());
            Assert.IsTrue(callForProposal.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithOneSpaceSaves()
        {
            #region Arrange
            var callForProposal = GetValid(9);
            callForProposal.Description = " ";
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(callForProposal.IsTransient());
            Assert.IsTrue(callForProposal.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithOneCharacterSaves()
        {
            #region Arrange
            var callForProposal = GetValid(9);
            callForProposal.Description = "x";
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(callForProposal.IsTransient());
            Assert.IsTrue(callForProposal.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Description with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDescriptionWithLongValueSaves()
        {
            #region Arrange
            var callForProposal = GetValid(9);
            callForProposal.Description = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, callForProposal.Description.Length);
            Assert.IsFalse(callForProposal.IsTransient());
            Assert.IsTrue(callForProposal.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Description Tests

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
