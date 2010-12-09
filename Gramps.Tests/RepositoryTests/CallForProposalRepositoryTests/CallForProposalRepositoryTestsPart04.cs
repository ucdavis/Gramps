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


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region Emails Tests

        #region Invalid Tests
            
        /// <summary>
        /// Tests the Emails with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailsWithAValueOfNullDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.Emails = null;
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(callForProposal);
                Assert.AreEqual(callForProposal.Emails, null);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Emails: may not be null");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Emails with A value of TestValue does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailsWithAnInvalidEmailsDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.AddEmailForCall("test@test.com");
                callForProposal.AddEmailForCall("");
                callForProposal.AddEmailForCall("test2@test.com");
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(callForProposal);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailsForCallList: One or more invalid emails for call detected");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }	
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestEmailsWithAnEmptyListSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.Emails = new List<EmailsForCall>();
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Emails);
            Assert.AreEqual(0, record.Emails.Count);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEmailsWithPopulatedValidEmailsListSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.Emails = new List<EmailsForCall>();
            record.AddEmailForCall("test@test.com");
            record.AddEmailForCall("test2@test.com");
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Emails);
            Assert.AreEqual(2, record.Emails.Count);
            #endregion Assert
        }


        #endregion Valid Tests

        #region Cascade Tests


        [TestMethod]
        public void TestEmailsCascadesSave()
        {
            #region Arrange
            var emailForCallRepository = new Repository<EmailsForCall>();
            var emailCount = emailForCallRepository.Queryable.Count();
            var record = GetValid(99);
            record.Emails = new List<EmailsForCall>();
            record.AddEmailForCall("test@test.com");
            record.AddEmailForCall("testy@test.com");
            record.AddEmailForCall("test2@test.com");
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Emails);
            Assert.AreEqual(3, record.Emails.Count);
            Assert.AreEqual(3 + emailCount, emailForCallRepository.Queryable.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestEmailsCascadesDelete1()
        {
            #region Arrange
            var emailForCallRepository = new Repository<EmailsForCall>();
            var emailCount = emailForCallRepository.Queryable.Count();
            var record = GetValid(99);
            record.Emails = new List<EmailsForCall>();
            record.AddEmailForCall("test@test.com");
            record.AddEmailForCall("testy@test.com");
            record.AddEmailForCall("test2@test.com");
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Arrange

            #region Act
            record.RemoveEmailForCall(record.Emails.ElementAt(1));
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Emails);
            Assert.AreEqual(2, record.Emails.Count);
            Assert.AreEqual(2 + emailCount, emailForCallRepository.Queryable.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailsCascadesDelete2()
        {
            #region Arrange
            var emailForCallRepository = new Repository<EmailsForCall>();
            var emailCount = emailForCallRepository.Queryable.Count();
            var record = GetValid(99);
            record.Emails = new List<EmailsForCall>();
            record.AddEmailForCall("test@test.com");
            record.AddEmailForCall("testy@test.com");
            record.AddEmailForCall("test2@test.com");
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.Remove(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNull(CallForProposalRepository.GetNullableById(saveId));
            Assert.AreEqual(emailCount, emailForCallRepository.Queryable.Count());
            #endregion Assert
        }

        #endregion Cascade Tests

        #endregion Emails Tests
    }
}
