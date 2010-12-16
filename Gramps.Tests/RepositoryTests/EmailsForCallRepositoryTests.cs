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
            Assert.AreEqual(string.Format("Test{0}@testy.com", counter), entity.Email);
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
            LoadCallForProposals(1);
            LoadTemplates(1);
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
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                "",
                "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("EmailedOnDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("HasBeenEmailed", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(EmailsForCall));

        }

        #endregion Reflection of Database.	
		
		
    }
}