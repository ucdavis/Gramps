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
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Proposal
    /// LookupFieldName:	Email
    /// </summary>
    [TestClass]
    public class ProposalRepositoryTests : AbstractRepositoryTests<Proposal, int, ProposalMap>
    {
        /// <summary>
        /// Gets or sets the Proposal repository.
        /// </summary>
        /// <value>The Proposal repository.</value>
        public IRepository<Proposal> ProposalRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ProposalRepositoryTests"/> class.
        /// </summary>
        public ProposalRepositoryTests()
        {
            ProposalRepository = new Repository<Proposal>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Proposal GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Proposal(counter);
            rtValue.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Proposal> GetQuery(int numberAtEnd)
        {
            return ProposalRepository.Queryable.Where(a => a.Email.Contains(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Proposal entity, int counter)
        {
            Assert.AreEqual(string.Format("Email{0}@testy.com", counter), entity.Email);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Proposal entity, ARTAction action)
        {
            const string updateValue = "Updated@toasty.com";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Email);
                    break;
                case ARTAction.Restore:
                    entity.Email = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Email;
                    entity.Email = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(2);
            //LoadTemplates(2);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            ProposalRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ProposalRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region GUID Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestGuidWithADuplicateValueDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Guid = ProposalRepository.GetNullableById(1).Guid;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(proposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("could not insert: [Gramps.Core.Domain.Proposal][SQL: INSERT INTO Proposals (Guid, Email, IsApproved, IsDenied, IsNotified, RequestedAmount, ApprovedAmount, IsSubmitted, CreatedDate, SubmittedDate, NotifiedDate, CallForProposalID) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?); select last_insert_rowid()]", ex.Message);
                throw;
            }	
        }
        #endregion Invalid Tests

        [TestMethod]
        public void TestGuidSavesWithDifferentGuid()
        {
            #region Arrange
            var newGuid = Guid.NewGuid();
            Assert.AreNotEqual(Guid.Empty, newGuid);
            Assert.IsFalse(ProposalRepository.Queryable.Where(a => a.Guid == newGuid).Any());
            var record = ProposalRepository.GetNullableById(2);
            record.Guid = newGuid;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = ProposalRepository.Queryable.Where(a => a.Guid == newGuid).Single();
            Assert.AreEqual(2, record.Id);
            #endregion Assert		
        }

        #endregion GUID Tests

        #region CallForProposal Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithAValueOfNullDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.CallForProposal = null;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                Assert.IsNull(proposal.CallForProposal);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CallForProposal: may not be null");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestCallForProposalWithANewValueDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.CallForProposal = new CallForProposal("test");
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(proposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueGramps.Core.Domain.Proposal.CallForProposal", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestEmailsForCallWithExistingCallForProposalSaves()
        {
            #region Arrange
            Proposal record = GetValid(99);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
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
        public void TestDeleteEmailsForCallDoesNotCascadeToCallForProposal()
        {
            #region Arrange
            Proposal record = GetValid(99);      
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.Remove(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<CallForProposal>().GetNullableById(2));
            Assert.IsNull(ProposalRepository.GetNullableById(saveId));
            #endregion Assert
        }
        #endregion Cascade Tests

        #endregion CallForProposal Tests

        #region Email Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Email with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithNullValueDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Email = null;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Email with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithEmptyStringDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Email = string.Empty;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Email with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithSpacesOnlyDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Email = " ";
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty", "Email: not a well-formed email address");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Email with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithTooLongValueDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Email = "x".RepeatTimes((95 + 1)) + "@t.ca";
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                Assert.AreEqual(100 + 1, proposal.Email.Length);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: length must be between 0 and 100");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithInvalidEmailFormatValueDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Email = "@x.com";
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: not a well-formed email address");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Email with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWith6CharacterSaves()
        {
            #region Arrange
            var proposal = GetValid(9);
            proposal.Email = "x@t.ca";
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var proposal = GetValid(9);
            proposal.Email = "x".RepeatTimes(95) + "@t.ca";
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, proposal.Email.Length);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests

        #region IsApproved Tests

        /// <summary>
        /// Tests the IsApproved is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsApprovedIsFalseSaves()
        {
            #region Arrange

            Proposal proposal = GetValid(9);
            proposal.IsApproved = false;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(proposal.IsApproved);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsApproved is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsApprovedIsTrueSaves()
        {
            #region Arrange

            var proposal = GetValid(9);
            proposal.IsApproved = true;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(proposal.IsApproved);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        #endregion IsApproved Tests

        #region IsDenied Tests

        /// <summary>
        /// Tests the IsDenied is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsDeniedIsFalseSaves()
        {
            #region Arrange

            Proposal proposal = GetValid(9);
            proposal.IsDenied = false;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(proposal.IsDenied);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsDenied is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsDeniedIsTrueSaves()
        {
            #region Arrange

            var proposal = GetValid(9);
            proposal.IsDenied = true;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(proposal.IsDenied);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        #endregion IsDenied Tests

        #region IsNotified Tests

        /// <summary>
        /// Tests the IsNotified is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsNotifiedIsFalseSaves()
        {
            #region Arrange

            Proposal proposal = GetValid(9);
            proposal.IsNotified = false;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(proposal.IsNotified);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsNotified is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsNotifiedIsTrueSaves()
        {
            #region Arrange

            var proposal = GetValid(9);
            proposal.IsNotified = true;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(proposal.IsNotified);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        #endregion IsNotified Tests

        #region IsSubmitted Tests

        /// <summary>
        /// Tests the IsSubmitted is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsSubmittedIsFalseSaves()
        {
            #region Arrange

            Proposal proposal = GetValid(9);
            proposal.IsSubmitted = false;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(proposal.IsSubmitted);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsSubmitted is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsSubmittedIsTrueSaves()
        {
            #region Arrange

            var proposal = GetValid(9);
            proposal.IsSubmitted = true;

            #endregion Arrange

            #region Act

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(proposal.IsSubmitted);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());

            #endregion Assert
        }

        #endregion IsSubmitted Tests

        #region RequestedAmount Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the RequestedAmount with A value of -0.001m does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestRequestedAmountWithAValueOfLessThanZeroDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.RequestedAmount = -0.001m;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                Assert.AreEqual(proposal.RequestedAmount, -0.001m);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RequestedAmount: Minimum amount is zero");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }	
        }
        #endregion Invalid Tests

        [TestMethod]
        public void TestRequestedAmountOfZeroSaves()
        {
            #region Arrange
            var proposal = GetValid(9);
            proposal.RequestedAmount = 0m;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.00m, proposal.RequestedAmount);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestRequestedAmountOfMaxValueSaves()
        {
            #region Arrange
            var proposal = GetValid(9);
            proposal.RequestedAmount = Decimal.MaxValue;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(Decimal.MaxValue, proposal.RequestedAmount);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());
            #endregion Assert
        }
        #endregion RequestedAmount Tests

        #region ApprovedAmount Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the ApprovedAmount with A value of -0.001m does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestApprovedAmountWithAValueOfLessThanZeroDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.ApprovedAmount = -0.001m;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                Assert.AreEqual(proposal.ApprovedAmount, -0.001m);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ApprovedAmount: Minimum amount is zero");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        [TestMethod]
        public void TestApprovedAmountOfZeroSaves()
        {
            #region Arrange
            var proposal = GetValid(9);
            proposal.ApprovedAmount = 0m;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0.00m, proposal.ApprovedAmount);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestApprovedAmountOfMaxValueSaves()
        {
            #region Arrange
            var proposal = GetValid(9);
            proposal.ApprovedAmount = Decimal.MaxValue;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(Decimal.MaxValue, proposal.ApprovedAmount);
            Assert.IsFalse(proposal.IsTransient());
            Assert.IsTrue(proposal.IsValid());
            #endregion Assert
        }
        #endregion ApprovedAmount Tests

        #region CreatedDate Tests

        /// <summary>
        /// Tests the CreatedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Proposal record = GetValid(99);
            record.CreatedDate = compareDate;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
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
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
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
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreatedDate);
            #endregion Assert
        }
        #endregion CreatedDate Tests
   
        #region SubmittedDate Tests

        /// <summary>
        /// Tests the SubmittedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestSubmittedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Proposal record = GetValid(99);
            record.SubmittedDate = compareDate;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.SubmittedDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the SubmittedDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestSubmittedDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.SubmittedDate = compareDate;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.SubmittedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the SubmittedDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestSubmittedDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.SubmittedDate = compareDate;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.SubmittedDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestSubmittedDateWithNullValueWillSave()
        {
            #region Arrange
            var record = GetValid(99);
            record.SubmittedDate = null;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.SubmittedDate);
            #endregion Assert
        }
        #endregion SubmittedDate Tests

        #region NotifiedDate Tests

        /// <summary>
        /// Tests the NotifiedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestNotifiedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Proposal record = GetValid(99);
            record.NotifiedDate = compareDate;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.NotifiedDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the NotifiedDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestNotifiedDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.NotifiedDate = compareDate;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.NotifiedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the NotifiedDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestNotifiedDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.NotifiedDate = compareDate;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.NotifiedDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestNotifiedDateWithNullValueWillSave()
        {
            #region Arrange
            var record = GetValid(99);
            record.NotifiedDate = null;
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.NotifiedDate);
            #endregion Assert
        }
        #endregion NotifiedDate Tests

        #region Comments Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Comments with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCommentsWithAValueOfNullDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Comments = null;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                Assert.AreEqual(proposal.Comments, null);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Comments: may not be null");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }	
        }
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCommentsWithANewValueDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Comments.Add(CreateValidEntities.Comment(9));
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(proposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Comment, Entity: Gramps.Core.Domain.Comment", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestCommentsWithEMptyListSaves()
        {
            #region Arrange
            Proposal record = GetValid(99);
            record.Comments = new List<Comment>();
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(0, record.Comments.Count);
            #endregion Assert				
        }

        [TestMethod]
        public void TestCommentsWithExistingPopulatedValidValuesWillSave()
        {
            #region Arrange
            var record = ProposalRepository.GetNullableById(1);
            Repository.OfType<Editor>().DbContext.BeginTransaction();
            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors.Add(CreateValidEntities.Editor(2));
            editors.Add(CreateValidEntities.Editor(3));
            for (int i = 0; i < 3; i++)
            {
                editors[i].CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Repository.OfType<Editor>().EnsurePersistent(editors[i]);
            }
            Repository.OfType<Editor>().DbContext.CommitTransaction();
            Repository.OfType<Comment>().DbContext.BeginTransaction();
            var comments = new List<Comment>();
            comments.Add(new Comment(record, Repository.OfType<Editor>().GetNullableById(1), "Comment1"));
            comments.Add(new Comment(record, Repository.OfType<Editor>().GetNullableById(2), "Comment2"));
            comments.Add(new Comment(record, Repository.OfType<Editor>().GetNullableById(3), "Comment3"));
            foreach (var comment in comments)
            {
                Repository.OfType<Comment>().EnsurePersistent(comment);
            }
            Repository.OfType<Comment>().DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            Assert.AreEqual(0, record.Comments.Count);
            record = ProposalRepository.GetNullableById(1);
            Assert.AreEqual(3, record.Comments.Count);
            #endregion Arrange

            #region Act
            record.IsNotified = true;
            record.IsDenied = true;
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(3, record.Comments.Count);
            #endregion Assert			
        }
        #endregion Valid Tests

        #region Cascades Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestDeleteProposalDoesNotCascadeToExistingComments()
        {
            Proposal record;
            try
            {
                #region Arrange
                record = ProposalRepository.GetNullableById(1);
                Repository.OfType<Editor>().DbContext.BeginTransaction();
                var editors = new List<Editor>();
                editors.Add(CreateValidEntities.Editor(1));
                editors.Add(CreateValidEntities.Editor(2));
                editors.Add(CreateValidEntities.Editor(3));
                for (int i = 0; i < 3; i++)
                {
                    editors[i].CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                    Repository.OfType<Editor>().EnsurePersistent(editors[i]);
                }
                Repository.OfType<Editor>().DbContext.CommitTransaction();
                Repository.OfType<Comment>().DbContext.BeginTransaction();
                var comments = new List<Comment>();
                comments.Add(new Comment(record, Repository.OfType<Editor>().GetNullableById(1), "Comment1"));
                comments.Add(new Comment(record, Repository.OfType<Editor>().GetNullableById(2), "Comment2"));
                comments.Add(new Comment(record, Repository.OfType<Editor>().GetNullableById(3), "Comment3"));
                foreach (var comment in comments)
                {
                    Repository.OfType<Comment>().EnsurePersistent(comment);
                }
                Repository.OfType<Comment>().DbContext.CommitTransaction();
                NHibernateSessionManager.Instance.GetSession().Evict(record);
                Assert.AreEqual(0, record.Comments.Count);

                #endregion Arrange
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                record = ProposalRepository.GetNullableById(1);
                Assert.AreEqual(3, record.Comments.Count);
                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.Remove(record);
                ProposalRepository.DbContext.CommitChanges();
                #endregion Act
            }
            catch (Exception ex)
            {
                #region Assert
                Assert.IsNotNull(ex);
                Assert.AreEqual("could not delete collection: [Gramps.Core.Domain.Proposal.Comments#1][SQL: UPDATE Comments SET ProposalID = null WHERE ProposalID = @p0]", ex.Message);
                #endregion Assert
                throw;
            } 
        }
        #endregion Cascades Tests
        #endregion Comments Tests

        #region Answers Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Answers with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAnswersWithAValueOfNullDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.Answers = null;
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(proposal);
                Assert.AreEqual(proposal.Answers, null);
                var results = proposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Answers: may not be null");
                Assert.IsTrue(proposal.IsTransient());
                Assert.IsFalse(proposal.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestAnswersWithAQuestionDoesNotSave()
        {
            Proposal proposal = null;
            try
            {
                #region Arrange
                proposal = GetValid(9);
                proposal.AddAnswer(null, "Test");
                #endregion Arrange

                #region Act
                ProposalRepository.DbContext.BeginTransaction();
                ProposalRepository.EnsurePersistent(proposal);
                ProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(proposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueGramps.Core.Domain.QuestionAnswer.Question", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestAnswersWithEmptyListSaves()
        {
            #region Arrange
            Proposal record = GetValid(99);
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(0, record.Answers.Count);
            #endregion Assert			
        }

        [TestMethod]
        public void TestAnswersWithPopulatedListSaves()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            #region CallForProposal Add Questions
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var question = new Question();
            question.Name = "What Color?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            question = new Question();
            question.Name = "What Food?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);

            question = new Question();
            question.Name = "What will you do with it??";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();
            #endregion CallForProposal Add Questions

            Proposal record = GetValid(99);
            record.AddAnswer(callForProposal.Questions[0], "Blue");
            record.AddAnswer(callForProposal.Questions[1], "Pizza");
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(2, record.Answers.Count);
            #endregion Assert
        }

        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestProposaCascadesSaveToAnswers()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            #region CallForProposal Add Questions
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var question = new Question();
            question.Name = "What Color?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            question = new Question();
            question.Name = "What Food?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);

            question = new Question();
            question.Name = "What will you do with it??";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();
            #endregion CallForProposal Add Questions

            //var count = Repository.OfType<QuestionAnswer>().Queryable.Count();

            Proposal record = GetValid(99);
            record.AddAnswer(callForProposal.Questions[0], "Blue");
            record.AddAnswer(callForProposal.Questions[1], "Pizza");
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(callForProposal);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            var answers = Repository.OfType<QuestionAnswer>().GetAll();
            Assert.AreEqual(2, answers.Count);
            Assert.AreEqual("Blue", answers[0].Answer);
            Assert.AreEqual("Pizza", answers[1].Answer);
            #endregion Assert
        }


        [TestMethod]
        public void TestUpdateProposalCascadesToAnswers()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            #region CallForProposal Add Questions
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var question = new Question();
            question.Name = "What Color?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            question = new Question();
            question.Name = "What Food?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);

            question = new Question();
            question.Name = "What will you do with it??";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();
            #endregion CallForProposal Add Questions

            //var count = Repository.OfType<QuestionAnswer>().Queryable.Count();

            Proposal record = GetValid(99);
            record.AddAnswer(callForProposal.Questions[0], "Blue");
            record.AddAnswer(callForProposal.Questions[1], "Pizza");
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(callForProposal);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = ProposalRepository.GetNullableById(saveId);
            record.Answers[1].Answer = "Fish";
            record.AddAnswer(record.CallForProposal.Questions[2], "Because I neeeeed it");

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(callForProposal);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            var answers = Repository.OfType<QuestionAnswer>().GetAll();
            Assert.AreEqual(3, answers.Count);
            Assert.AreEqual("Blue", answers[0].Answer);
            Assert.AreEqual("Fish", answers[1].Answer);
            Assert.AreEqual("Because I neeeeed it", answers[2].Answer);
            #endregion Assert		
        }


        [TestMethod]
        public void TestRemoveAnswerFromListCascadeDeletesToAnswers()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            #region CallForProposal Add Questions
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var question = new Question();
            question.Name = "What Color?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            question = new Question();
            question.Name = "What Food?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);

            question = new Question();
            question.Name = "What will you do with it??";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();
            #endregion CallForProposal Add Questions

            //var count = Repository.OfType<QuestionAnswer>().Queryable.Count();

            Proposal record = GetValid(99);
            record.AddAnswer(callForProposal.Questions[0], "Blue");
            record.AddAnswer(callForProposal.Questions[1], "Pizza");
            #endregion Arrange

            #region Act
            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(callForProposal);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = ProposalRepository.GetNullableById(saveId);
            record.Answers.RemoveAt(0);

            ProposalRepository.DbContext.BeginTransaction();
            ProposalRepository.EnsurePersistent(record);
            ProposalRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(callForProposal);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            var answers = Repository.OfType<QuestionAnswer>().GetAll();
            Assert.AreEqual(1, answers.Count);
            Assert.AreEqual("Pizza", answers[0].Answer);
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion Answers Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorSetsExpectedValues()
        {
            #region Arrange
            var record = new Proposal();
            #endregion Arrange

            #region Assert
            Assert.AreNotEqual(Guid.Empty, record.Guid);
            Assert.AreEqual(DateTime.Now.Date, record.CreatedDate.Date);
            Assert.AreEqual(0m, record.RequestedAmount);
            Assert.AreEqual(0m, record.ApprovedAmount);
            Assert.IsNotNull(record.Comments);
            Assert.IsNotNull(record.Answers);
            Assert.IsFalse(record.IsApproved);
            Assert.IsFalse(record.IsDenied);
            Assert.IsFalse(record.IsNotified);
            Assert.IsFalse(record.IsSubmitted);
            #endregion Assert		
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapProposal1()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal2()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.IsApproved, true)
                .CheckProperty(c => c.IsDenied, true)
                .CheckProperty(c => c.IsNotified, true)
                .CheckProperty(c => c.IsSubmitted, true)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        [TestMethod]
        public void TestCanCorrectlyMapProposal3()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.IsApproved, false)
                .CheckProperty(c => c.IsDenied, false)
                .CheckProperty(c => c.IsNotified, false)
                .CheckProperty(c => c.IsSubmitted, false)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal4()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.ApprovedAmount, 0m)
                .CheckProperty(c => c.RequestedAmount, 0m)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal5()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.ApprovedAmount, 0.01m)
                .CheckProperty(c => c.RequestedAmount, 0.01m)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal6()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.ApprovedAmount, 100000000.99m)
                .CheckProperty(c => c.RequestedAmount, 100000000.99m)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal7()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.ApprovedAmount, 9.9999m)
                .CheckProperty(c => c.RequestedAmount, 9.9999m)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal8()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.ApprovedAmount, -9.9999m)
                .CheckProperty(c => c.RequestedAmount, -9.9999m)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal9()
        {
            #region Arrange
            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var dateCompare1 = new DateTime(2010,12,25);
            var dateCompare2 = dateCompare1.AddDays(1);
            var dateCompare3 = dateCompare1.AddDays(2);
            var guid = Guid.NewGuid();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.CreatedDate, dateCompare1)
                .CheckProperty(c => c.Email, "email@test.com")
                .CheckProperty(c => c.Guid, guid)
                .CheckProperty(c => c.NotifiedDate, dateCompare2)
                .CheckProperty(c => c.SubmittedDate, dateCompare3)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapProposal10()
        {
            #region Arrange
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();

            var id = ProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var proposal = new Proposal();
            proposal.SetIdTo(id);
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            callForProposal.AddEditor(CreateValidEntities.Editor(1));
            callForProposal.AddEditor(CreateValidEntities.Editor(2));
            callForProposal.AddEditor(CreateValidEntities.Editor(3));

            var question = new Question();
            question.Name = "What Food?";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);

            question = new Question();
            question.Name = "What will you do with it??";
            question.QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
            callForProposal.AddQuestion(question);
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);

            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            var answers = new List<QuestionAnswer>();
            answers.Add(new QuestionAnswer {Answer = "Blue", Proposal = proposal, Question = callForProposal.Questions[0]});
            answers.Add(new QuestionAnswer { Answer = "Piuzza", Proposal = proposal, Question = callForProposal.Questions[1]});

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments.Add(CreateValidEntities.Comment(2));
            comments.Add(CreateValidEntities.Comment(3));
            Repository.OfType<Comment>().DbContext.BeginTransaction();
            for (int i = 0; i < 3; i++)
            {
                comments[i].Proposal = proposal;
                comments[i].Editor = callForProposal.Editors[i];
                Repository.OfType<Comment>().EnsurePersistent(comments[i]);
                //record.Comments.Add(comments[i]);
            }
            Repository.OfType<Comment>().DbContext.CommitTransaction();

            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Proposal>(session, new ProposalEqualityComparer())
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.Answers, answers)
                .CheckProperty(c => c.Comments, comments)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
  

        #endregion Fluent Mapping Tests

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
            expectedFields.Add(new NameAndType("Answers", "System.Collections.Generic.IList`1[Gramps.Core.Domain.QuestionAnswer]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ApprovedAmount", "System.Decimal", new List<string>
            {
                "[UCDArch.Core.NHibernateValidator.Extensions.RangeDoubleAttribute(Min = 0, Message = \"Minimum amount is zero\")]"
            }));
            expectedFields.Add(new NameAndType("CallForProposal", "Gramps.Core.Domain.CallForProposal", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Comments", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Comment]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("CreatedDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                "[NHibernate.Validator.Constraints.EmailAttribute()]",
                "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Guid", "System.Guid", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsApproved", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("IsDenied", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("IsNotified", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("IsSubmitted", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("NotifiedDate", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("RequestedAmount", "System.Decimal", new List<string>
            {
                "[UCDArch.Core.NHibernateValidator.Extensions.RangeDoubleAttribute(Min = 0, Message = \"Minimum amount is zero\")]"
            }));
            expectedFields.Add(new NameAndType("SubmittedDate", "System.Nullable`1[System.DateTime]", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Proposal));

        }

        #endregion Reflection of Database.	
		
        public class ProposalEqualityComparer : IEqualityComparer
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param><exception cref="T:System.ArgumentException"><paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.</exception>
            bool IEqualityComparer.Equals(object x, object y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                if (x is IList<Comment> && y is IList<Comment>)
                {
                    var xVal = (IList<Comment>)x;
                    var yVal = (IList<Comment>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Text, yVal[i].Text);
                    }
                    return true;
                }

                if (x is IList<QuestionAnswer> && y is IList<QuestionAnswer>)
                {
                    var xVal = (IList<QuestionAnswer>)x;
                    var yVal = (IList<QuestionAnswer>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Answer, yVal[i].Answer);
                    }
                    return true;
                }

                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}