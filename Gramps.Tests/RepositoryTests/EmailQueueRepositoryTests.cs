using System;
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
    /// Entity Name:		EmailQueue
    /// LookupFieldName:	Subject
    /// </summary>
    [TestClass]
    public class EmailQueueRepositoryTests : AbstractRepositoryTests<EmailQueue, int, EmailQueueMap>
    {
        /// <summary>
        /// Gets or sets the EmailQueue repository.
        /// </summary>
        /// <value>The EmailQueue repository.</value>
        public IRepository<EmailQueue> EmailQueueRepository { get; set; }
        public IRepository<CallForProposal> CallForProposalRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailQueueRepositoryTests"/> class.
        /// </summary>
        public EmailQueueRepositoryTests()
        {
            EmailQueueRepository = new Repository<EmailQueue>();
            CallForProposalRepository = new Repository<CallForProposal>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override EmailQueue GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.EmailQueue(counter);
            rtValue.CallForProposal = CallForProposalRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<EmailQueue> GetQuery(int numberAtEnd)
        {
            return EmailQueueRepository.Queryable.Where(a => a.Subject.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(EmailQueue entity, int counter)
        {
            Assert.AreEqual("Subject" + counter, entity.Subject);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(EmailQueue entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Subject);
                    break;
                case ARTAction.Restore:
                    entity.Subject = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Subject;
                    entity.Subject = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            CallForProposalRepository.DbContext.BeginTransaction();
            LoadUsers(5);
            LoadCallForProposals(3);
            CallForProposalRepository.DbContext.CommitTransaction();
            EmailQueueRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            EmailQueueRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region EmailAddress Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the EmailAddress with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailAddressWithNullValueDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.EmailAddress = null;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailAddress: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the EmailAddress with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailAddressWithEmptyStringDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.EmailAddress = string.Empty;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailAddress: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the EmailAddress with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailAddressWithSpacesOnlyDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.EmailAddress = " ";
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailAddress: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the EmailAddress with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailAddressWithTooLongValueDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.EmailAddress = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                Assert.AreEqual(200 + 1, emailQueue.EmailAddress.Length);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailAddress: length must be between 0 and 200");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the EmailAddress with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEmailAddressWithOneCharacterSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.EmailAddress = "x";
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the EmailAddress with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailAddressWithLongValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.EmailAddress = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, emailQueue.EmailAddress.Length);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion EmailAddress Tests

        #region Created Tests

        /// <summary>
        /// Tests the Created with past date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            EmailQueue record = GetValid(99);
            record.Created = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Created);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the Created with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.Created = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Created);
            #endregion Assert
        }

        /// <summary>
        /// Tests the Created with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.Created = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.Created);
            #endregion Assert
        }
        #endregion Created Tests

        #region Pending Tests

        /// <summary>
        /// Tests the Pending is false saves.
        /// </summary>
        [TestMethod]
        public void TestPendingIsFalseSaves()
        {
            #region Arrange

            EmailQueue emailQueue = GetValid(9);
            emailQueue.Pending = false;

            #endregion Arrange

            #region Act

            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailQueue.Pending);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the Pending is true saves.
        /// </summary>
        [TestMethod]
        public void TestPendingIsTrueSaves()
        {
            #region Arrange

            var emailQueue = GetValid(9);
            emailQueue.Pending = true;

            #endregion Arrange

            #region Act

            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailQueue.Pending);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());

            #endregion Assert
        }

        #endregion Pending Tests

        #region SentDateTime Tests

        [TestMethod]
        public void TestSentDateTimeWithNullDateWillSave()
        {
            #region Arrange
            EmailQueue record = GetValid(99);
            record.SentDateTime = null;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNull(record.SentDateTime);
            #endregion Assert
        }

        /// <summary>
        /// Tests the SentDateTime with past date will save.
        /// </summary>
        [TestMethod]
        public void TestSentDateTimeWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            EmailQueue record = GetValid(99);
            record.SentDateTime = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.SentDateTime);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the SentDateTime with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestSentDateTimeWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.SentDateTime = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.SentDateTime);
            #endregion Assert
        }

        /// <summary>
        /// Tests the SentDateTime with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestSentDateTimeWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.SentDateTime = compareDate;
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(record);
            EmailQueueRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.SentDateTime);
            #endregion Assert
        }
        #endregion SentDateTime Tests
        
        #region Subject Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Subject with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubjectWithNullValueDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Subject = null;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Subject with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubjectWithEmptyStringDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Subject = string.Empty;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Subject with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubjectWithSpacesOnlyDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Subject = " ";
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Subject with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubjectWithTooLongValueDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Subject = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                Assert.AreEqual(200 + 1, emailQueue.Subject.Length);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: length must be between 0 and 200");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Subject with one character saves.
        /// </summary>
        [TestMethod]
        public void TestSubjectWithOneCharacterSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Subject = "x";
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Subject with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSubjectWithLongValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Subject = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, emailQueue.Subject.Length);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Subject Tests

        #region Body Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Body with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestBodyWithNullValueDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Body = null;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Body: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Body with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestBodyWithEmptyStringDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Body = string.Empty;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Body: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Body with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestBodyWithSpacesOnlyDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.Body = " ";
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Body: may not be null or empty");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Body with one character saves.
        /// </summary>
        [TestMethod]
        public void TestBodyWithOneCharacterSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Body = "x";
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Body with long value saves.
        /// </summary>
        [TestMethod]
        public void TestBodyWithLongValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.Body = "x".RepeatTimes(1000);
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1000, emailQueue.Body.Length);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Body Tests

        #region Immediate Tests

        /// <summary>
        /// Tests the Immediate is false saves.
        /// </summary>
        [TestMethod]
        public void TestImmediateIsFalseSaves()
        {
            #region Arrange

            EmailQueue emailQueue = GetValid(9);
            emailQueue.Immediate = false;

            #endregion Arrange

            #region Act

            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailQueue.Immediate);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the Immediate is true saves.
        /// </summary>
        [TestMethod]
        public void TestImmediateIsTrueSaves()
        {
            #region Arrange

            var emailQueue = GetValid(9);
            emailQueue.Immediate = true;

            #endregion Arrange

            #region Act

            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailQueue.Immediate);
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());

            #endregion Assert
        }

        #endregion Immediate Tests

  

        #region CallForProposal Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the CallForProposal with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithAValueOfNullDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.CallForProposal = null;
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailQueue);
                Assert.AreEqual(emailQueue.CallForProposal, null);
                var results = emailQueue.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CallForProposal: may not be null");
                Assert.IsTrue(emailQueue.IsTransient());
                Assert.IsFalse(emailQueue.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCallForProposalWithAValueOfNewDoesNotSave()
        {
            EmailQueue emailQueue = null;
            try
            {
                #region Arrange
                emailQueue = GetValid(9);
                emailQueue.CallForProposal = CreateValidEntities.CallForProposal(1);
                emailQueue.CallForProposal.AddEditor(new Editor(Repository.OfType<User>().Queryable.Where(a => a.Id == 3).Single(), true));
                #endregion Arrange

                #region Act
                EmailQueueRepository.DbContext.BeginTransaction();
                EmailQueueRepository.EnsurePersistent(emailQueue);
                EmailQueueRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(emailQueue);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.CallForProposal, Entity: Gramps.Core.Domain.CallForProposal", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestCallForProposalWithAnExistingValueSaves()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.CallForProposal = CallForProposalRepository.GetNullableById(2);
            #endregion Arrange

            #region Act
            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailQueue.IsTransient());
            Assert.IsTrue(emailQueue.IsValid());
            Assert.AreEqual("Name2", emailQueue.CallForProposal.Name);
            #endregion Assert	
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteEmailQueueDoesNotCascadeToCallForProposal()
        {
            #region Arrange
            var emailQueue = GetValid(9);
            emailQueue.CallForProposal = CallForProposalRepository.GetNullableById(2);

            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.EnsurePersistent(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();

            var saveId = emailQueue.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(emailQueue);
            #endregion Arrange

            #region Act
            emailQueue = EmailQueueRepository.GetNullableById(saveId);
            Assert.IsNotNull(emailQueue);

            EmailQueueRepository.DbContext.BeginTransaction();
            EmailQueueRepository.Remove(emailQueue);
            EmailQueueRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(emailQueue);
            #endregion Act

            #region Assert
            Assert.IsNull(EmailQueueRepository.GetNullableById(saveId));
            Assert.IsNotNull(CallForProposalRepository.GetNullableById(2));
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion CallForProposal Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorWithNoParametersSetsExpectedValues()
        {
            #region Arrange
            var record = new EmailQueue();
            #endregion Arrange

            #region Assert
            Assert.IsTrue(record.Pending);
            Assert.AreEqual(DateTime.Now.Date, record.Created.Date);
            Assert.IsNull(record.Body);
            Assert.IsNull(record.CallForProposal);
            Assert.IsNull(record.EmailAddress);
            Assert.IsNull(record.SentDateTime);
            Assert.IsNull(record.Subject);
            #endregion Assert		
        }

        [TestMethod]
        public void TestConstructorWithParametersSetsExpectedValues()
        {
            #region Arrange
            var record = new EmailQueue(CreateValidEntities.CallForProposal(9), "email", "sub", "boody");
            #endregion Arrange

            #region Assert
            Assert.IsTrue(record.Pending);
            Assert.AreEqual(DateTime.Now.Date, record.Created.Date);
            Assert.AreEqual("boody", record.Body);
            Assert.IsNotNull(record.CallForProposal);
            Assert.AreEqual("Name9", record.CallForProposal.Name);
            Assert.AreEqual("email", record.EmailAddress);
            Assert.IsNull(record.SentDateTime);
            Assert.AreEqual("sub", record.Subject);
            #endregion Assert
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapEmailQueue1()
        {
            #region Arrange
            var id = EmailQueueRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var date1 = new DateTime(2010, 11, 21, 11, 55, 01);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailQueue>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Pending, true)
                .CheckProperty(c => c.SentDateTime, null)
                .CheckProperty(c => c.Subject, "Subject")
                .CheckProperty(c => c.Body, "Body")
                .CheckProperty(c => c.Created, date1)
                .CheckProperty(c => c.EmailAddress, "EmailAddress")
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailQueue2()
        {
            #region Arrange
            var id = EmailQueueRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var date1 = new DateTime(2010, 11, 21, 11, 55, 01);
            var date2 = new DateTime(2010, 11, 21, 11, 57, 01);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailQueue>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Pending, false)
                .CheckProperty(c => c.SentDateTime, date2)
                .CheckProperty(c => c.Subject, "Subject")
                .CheckProperty(c => c.Body, "Body")
                .CheckProperty(c => c.Created, date1)
                .CheckProperty(c => c.EmailAddress, "EmailAddress")
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailQueue3()
        {
            #region Arrange
            var id = EmailQueueRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var date1 = new DateTime(2010, 11, 21, 11, 55, 01);
            var callForProposal = CallForProposalRepository.GetNullableById(3);
            Assert.IsNotNull(callForProposal);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailQueue>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Pending, true)
                .CheckProperty(c => c.SentDateTime, null)
                .CheckProperty(c => c.Subject, "Subject")
                .CheckProperty(c => c.Body, "Body")
                .CheckProperty(c => c.Created, date1)
                .CheckProperty(c => c.EmailAddress, "EmailAddress")
                .CheckReference(c => c.CallForProposal, callForProposal)
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
            expectedFields.Add(new NameAndType("Body", "System.String", new List<string>
            {
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("CallForProposal", "Gramps.Core.Domain.CallForProposal", new List<string>
            {
                 "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Created", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("EmailAddress", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Immediate", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Pending", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("SentDateTime", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("Subject", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(EmailQueue));

        }

        #endregion Reflection of Database.	
				
    }
}