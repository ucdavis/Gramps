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
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		EmailsForCall
    /// LookupFieldName:	Email
    /// </summary>
    [TestClass]
    public class EmailsForCallRepositoryTests : AbstractRepositoryTests<EmailsForCall, int, EmailsForCallMap>
    {
        /// <summary>
        /// Gets or sets the EmailsForCall repository.
        /// </summary>
        /// <value>The EmailsForCall repository.</value>
        public IRepository<EmailsForCall> EmailsForCallRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailsForCallRepositoryTests"/> class.
        /// </summary>
        public EmailsForCallRepositoryTests()
        {
            EmailsForCallRepository = new Repository<EmailsForCall>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override EmailsForCall GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.EmailsForCall(counter);
            var count = 9;
            if (counter != null)
            {
                count = (int)counter;
            }
            if (count % 2 == 0)
            {
                rtValue.CallForProposal = Repository.OfType<CallForProposal>().Queryable.Where(a => a.Id == 1).Single();
            }
            else
            {
                rtValue.Template = Repository.OfType<Template>().Queryable.Where(a => a.Id == 1).Single();
            }

            return rtValue;

        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<EmailsForCall> GetQuery(int numberAtEnd)
        {
            return EmailsForCallRepository.Queryable.Where(a => a.Email.Contains(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(EmailsForCall entity, int counter)
        {
            Assert.AreEqual(string.Format("test{0}@testy.com", counter), entity.Email);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(EmailsForCall entity, ARTAction action)
        {
            const string updateValue = "Updated@tosty.com";
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
            LoadTemplates(2);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            EmailsForCallRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            EmailsForCallRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Email Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Email with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithNullValueDoesNotSave()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Email = null;
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
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
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Email = string.Empty;
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
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
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Email = " ";
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty", "Email: not a well-formed email address");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
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
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Email = "x".RepeatTimes((95 + 1)) + "@t.ca";
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                Assert.AreEqual(100 + 1, emailsForCall.Email.Length);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: length must be between 0 and 100");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithInvalidEmailFormatValueDoesNotSave()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Email = "@x.com";
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: not a well-formed email address");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
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
            var emailsForCall = GetValid(9);
            emailsForCall.Email = "x@t.ca";
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(emailsForCall);
            EmailsForCallRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailsForCall.IsTransient());
            Assert.IsTrue(emailsForCall.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var emailsForCall = GetValid(9);
            emailsForCall.Email = "x".RepeatTimes(95) + "@t.ca";
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(emailsForCall);
            EmailsForCallRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, emailsForCall.Email.Length);
            Assert.IsFalse(emailsForCall.IsTransient());
            Assert.IsTrue(emailsForCall.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests

        #region HasBeenEmailed Tests

        /// <summary>
        /// Tests the HasBeenEmailed is false saves.
        /// </summary>
        [TestMethod]
        public void TestHasBeenEmailedIsFalseSaves()
        {
            #region Arrange

            EmailsForCall emailsForCall = GetValid(9);
            emailsForCall.HasBeenEmailed = false;

            #endregion Arrange

            #region Act

            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(emailsForCall);
            EmailsForCallRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(emailsForCall.HasBeenEmailed);
            Assert.IsFalse(emailsForCall.IsTransient());
            Assert.IsTrue(emailsForCall.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the HasBeenEmailed is true saves.
        /// </summary>
        [TestMethod]
        public void TestHasBeenEmailedIsTrueSaves()
        {
            #region Arrange

            var emailsForCall = GetValid(9);
            emailsForCall.HasBeenEmailed = true;

            #endregion Arrange

            #region Act

            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(emailsForCall);
            EmailsForCallRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(emailsForCall.HasBeenEmailed);
            Assert.IsFalse(emailsForCall.IsTransient());
            Assert.IsTrue(emailsForCall.IsValid());

            #endregion Assert
        }

        #endregion HasBeenEmailed Tests

        #region EmailedOnDate Tests

        /// <summary>
        /// Tests the EmailedOnDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestEmailedOnDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            EmailsForCall record = GetValid(99);
            record.EmailedOnDate = compareDate;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EmailedOnDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the EmailedOnDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestEmailedOnDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.EmailedOnDate = compareDate;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EmailedOnDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the EmailedOnDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestEmailedOnDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.EmailedOnDate = compareDate;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.EmailedOnDate);
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailedOnDateWithNullValueWillSave()
        {
            #region Arrange
            var record = GetValid(99);
            record.EmailedOnDate = null;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(null, record.EmailedOnDate);
            #endregion Assert
        }
        #endregion EmailedOnDate Tests

        #region Template Tests

        #region Invalid Tests

        
        /// <summary>
        /// Tests the Template with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateWithAValueOfNullDoesNotSaveIfCallForProposalIsNull()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Template = null;
                emailsForCall.CallForProposal = null;
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                Assert.AreEqual(emailsForCall.Template, null);
                Assert.AreEqual(emailsForCall.CallForProposal, null);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateWithAValidValueDoesNotSaveIfCallForProposalIsNotNull()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Template = Repository.OfType<Template>().GetNullableById(1);
                emailsForCall.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                Assert.IsNotNull(emailsForCall.Template);
                Assert.IsNotNull(emailsForCall.CallForProposal);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTemplateWithANewValueDoesNotSave()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Template = CreateValidEntities.Template(9);
                emailsForCall.CallForProposal = null;
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(emailsForCall);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Template, Entity: Gramps.Core.Domain.Template", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestEmailsForCallWithExistingTemplateSaves()
        {
            #region Arrange
            EmailsForCall record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Template);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert				
        }

        [TestMethod]
        public void TestEmailsForCallWithNullTemplateSaves()
        {
            #region Arrange
            EmailsForCall record = GetValid(99);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2); 
            record.Template = null;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNull(record.Template);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteEmailsForCallDoesNotCascadeToTemplate()
        {
            #region Arrange
            EmailsForCall record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.Remove(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<Template>().GetNullableById(2));
            Assert.IsNull(EmailsForCallRepository.GetNullableById(saveId));
            #endregion Assert		
        }
        #endregion Cascade Tests

        #endregion Template Tests

        #region CallForProposal Tests

        #region Invalid Tests


        /// <summary>
        /// Tests the Template with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithAValueOfNullDoesNotSaveIfCallForProposalIsNull()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Template = null;
                emailsForCall.CallForProposal = null;
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                Assert.AreEqual(emailsForCall.Template, null);
                Assert.AreEqual(emailsForCall.CallForProposal, null);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithAValidValueDoesNotSaveIfCallForProposalIsNotNull()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Template = Repository.OfType<Template>().GetNullableById(1);
                emailsForCall.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailsForCall);
                Assert.IsNotNull(emailsForCall.Template);
                Assert.IsNotNull(emailsForCall.CallForProposal);
                var results = emailsForCall.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailsForCall.IsTransient());
                Assert.IsFalse(emailsForCall.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCallForProposalWithANewValueDoesNotSave()
        {
            EmailsForCall emailsForCall = null;
            try
            {
                #region Arrange
                emailsForCall = GetValid(9);
                emailsForCall.Template = null;
                emailsForCall.CallForProposal = new CallForProposal("test");
                #endregion Arrange

                #region Act
                EmailsForCallRepository.DbContext.BeginTransaction();
                EmailsForCallRepository.EnsurePersistent(emailsForCall);
                EmailsForCallRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(emailsForCall);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.CallForProposal, Entity: Gramps.Core.Domain.CallForProposal", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestEmailsForCallWithExistingCallForProposalSaves()
        {
            #region Arrange
            EmailsForCall record = GetValid(99);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            record.Template = null;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CallForProposal);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailsForCallWithNullCallForProposalSaves()
        {
            #region Arrange
            EmailsForCall record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNull(record.CallForProposal);
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
            EmailsForCall record = GetValid(99);
            record.Template = null;
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.EnsurePersistent(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            EmailsForCallRepository.DbContext.BeginTransaction();
            EmailsForCallRepository.Remove(record);
            EmailsForCallRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<CallForProposal>().GetNullableById(2));
            Assert.IsNull(EmailsForCallRepository.GetNullableById(saveId));
            #endregion Assert
        }
        #endregion Cascade Tests

        #endregion CallForProposal Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorSetsExpectedValues()
        {
            #region Arrange
            var record = new EmailsForCall("some@email.com");
            #endregion Arrange

            #region Assert
            Assert.AreEqual("some@email.com", record.Email);
            Assert.IsFalse(record.HasBeenEmailed);
            Assert.IsNull(record.EmailedOnDate);
            Assert.IsNull(record.Template);
            Assert.IsNull(record.CallForProposal);
            #endregion Assert		
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapEmailsForCall1()
        {
            #region Arrange
            var id = EmailsForCallRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailsForCall>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Email, "test@testy.com")
                .CheckProperty(c => c.HasBeenEmailed, false)
                .CheckProperty(c => c.EmailedOnDate, null)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailsForCall2()
        {
            #region Arrange
            var id = EmailsForCallRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var compareDate = new DateTime(2010, 12, 25);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailsForCall>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Email, "test@testy.com")
                .CheckProperty(c => c.HasBeenEmailed, true)
                .CheckProperty(c => c.EmailedOnDate, compareDate)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailsForCall3()
        {
            #region Arrange
            var id = EmailsForCallRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailsForCall>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Email, "test@testy.com")
                .CheckProperty(c => c.HasBeenEmailed, false)
                .CheckProperty(c => c.EmailedOnDate, null)
                .CheckProperty(c => c.CallForProposal, null)
                .CheckReference(c => c.Template, template)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailsForCall4()
        {
            #region Arrange
            var id = EmailsForCallRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailsForCall>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Email, "test@testy.com")
                .CheckProperty(c => c.HasBeenEmailed, false)
                .CheckProperty(c => c.EmailedOnDate, null)
                .CheckProperty(c => c.Template, null)
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
            expectedFields.Add(new NameAndType("CallForProposal", "Gramps.Core.Domain.CallForProposal", new List<string>()));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                "[NHibernate.Validator.Constraints.EmailAttribute()]",
                "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("EmailedOnDate", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("HasBeenEmailed", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("RelatedTable", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Must be related to Template or CallForProposal not both.\")]"
            }));
            expectedFields.Add(new NameAndType("Template", "Gramps.Core.Domain.Template", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(EmailsForCall));

        }

        #endregion Reflection of Database.	
		
		
    }
}