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
    /// Entity Name:		EmailTemplate
    /// LookupFieldName:	Subject
    /// </summary>
    [TestClass]
    public class EmailTemplateRepositoryTests : AbstractRepositoryTests<EmailTemplate, int, EmailTemplateMap>
    {
        /// <summary>
        /// Gets or sets the EmailTemplate repository.
        /// </summary>
        /// <value>The EmailTemplate repository.</value>
        public IRepository<EmailTemplate> EmailTemplateRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateRepositoryTests"/> class.
        /// </summary>
        public EmailTemplateRepositoryTests()
        {
            EmailTemplateRepository = new Repository<EmailTemplate>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override EmailTemplate GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.EmailTemplate(counter);
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
        protected override IQueryable<EmailTemplate> GetQuery(int numberAtEnd)
        {
            return EmailTemplateRepository.Queryable.Where(a => a.Subject.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(EmailTemplate entity, int counter)
        {
            Assert.AreEqual("Subject" + counter, entity.Subject);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(EmailTemplate entity, ARTAction action)
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
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(2);
            LoadTemplates(2);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            EmailTemplateRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            EmailTemplateRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region TemplateType Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the TemplateType with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateTypeWithAValueOfNullDoesNotSave()
        {
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.TemplateType = null;
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.AreEqual(emailTemplate.TemplateType, null);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("TemplateType: Must have email template type");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
                throw;
            }	
        }

        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestTemplateWithAValidValueSaves1()
        {
            #region Arrange
            var record = GetValid(9);
            record.TemplateType = EmailTemplateType.InitialCall;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(EmailTemplateType.InitialCall, record.TemplateType);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        [TestMethod]
        public void TestTemplateWithAValidValueSaves2()
        {
            #region Arrange
            var record = GetValid(9);
            record.TemplateType = EmailTemplateType.ProposalApproved;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(EmailTemplateType.ProposalApproved, record.TemplateType);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        [TestMethod]
        public void TestTemplateWithAValidValueSaves3()
        {
            #region Arrange
            var record = GetValid(9);
            record.TemplateType = EmailTemplateType.ProposalConfirmation;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(EmailTemplateType.ProposalConfirmation, record.TemplateType);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        [TestMethod]
        public void TestTemplateWithAValidValueSaves4()
        {
            #region Arrange
            var record = GetValid(9);
            record.TemplateType = EmailTemplateType.ProposalDenied;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(EmailTemplateType.ProposalDenied, record.TemplateType);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        [TestMethod]
        public void TestTemplateWithAValidValueSaves5()
        {
            #region Arrange
            var record = GetValid(9);
            record.TemplateType = EmailTemplateType.ReadyForReview;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(EmailTemplateType.ReadyForReview, record.TemplateType);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        [TestMethod]
        public void TestTemplateWithAValidValueSaves6()
        {
            #region Arrange
            var record = GetValid(9);
            record.TemplateType = EmailTemplateType.ReminderCallIsAboutToClose;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(EmailTemplateType.ReminderCallIsAboutToClose, record.TemplateType);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests

        #endregion TemplateType Tests

        #region Text Tests


        #region Valid Tests

        /// <summary>
        /// Tests the Text with null value saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithNullValueSaves()
        {
            #region Arrange
            var emailTemplate = GetValid(9);
            emailTemplate.Text = null;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(emailTemplate);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailTemplate.IsTransient());
            Assert.IsTrue(emailTemplate.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithEmptyStringSaves()
        {
            #region Arrange
            var emailTemplate = GetValid(9);
            emailTemplate.Text = string.Empty;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(emailTemplate);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailTemplate.IsTransient());
            Assert.IsTrue(emailTemplate.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with one space saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithOneSpaceSaves()
        {
            #region Arrange
            var emailTemplate = GetValid(9);
            emailTemplate.Text = " ";
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(emailTemplate);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailTemplate.IsTransient());
            Assert.IsTrue(emailTemplate.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithOneCharacterSaves()
        {
            #region Arrange
            var emailTemplate = GetValid(9);
            emailTemplate.Text = "x";
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(emailTemplate);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailTemplate.IsTransient());
            Assert.IsTrue(emailTemplate.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithLongValueSaves()
        {
            #region Arrange
            var emailTemplate = GetValid(9);
            emailTemplate.Text = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(emailTemplate);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, emailTemplate.Text.Length);
            Assert.IsFalse(emailTemplate.IsTransient());
            Assert.IsTrue(emailTemplate.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Text Tests
   
        #region Subject Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Subject with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestSubjectWithNullValueDoesNotSave()
        {
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Subject = null;
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: may not be null or empty");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
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
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Subject = string.Empty;
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: may not be null or empty");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
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
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Subject = " ";
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: may not be null or empty");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
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
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Subject = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.AreEqual(100 + 1, emailTemplate.Subject.Length);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Subject: length must be between 0 and 100");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
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
            var emailTemplate = GetValid(9);
            emailTemplate.Subject = "x";
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(emailTemplate);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(emailTemplate.IsTransient());
            Assert.IsTrue(emailTemplate.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Subject with long value saves.
        /// </summary>
        [TestMethod]
        public void TestSubjectWithLongValueSaves()
        {
            #region Arrange
            var emailTemplate = GetValid(9);
            emailTemplate.Subject = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(emailTemplate);
            EmailTemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, emailTemplate.Subject.Length);
            Assert.IsFalse(emailTemplate.IsTransient());
            Assert.IsTrue(emailTemplate.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Subject Tests

        #region Template Tests

        #region Invalid Tests


        /// <summary>
        /// Tests the Template with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateWithAValueOfNullDoesNotSaveIfCallForProposalIsNull()
        {
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Template = null;
                emailTemplate.CallForProposal = null;
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.AreEqual(emailTemplate.Template, null);
                Assert.AreEqual(emailTemplate.CallForProposal, null);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateWithAValidValueDoesNotSaveIfCallForProposalIsNotNull()
        {
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Template = Repository.OfType<Template>().GetNullableById(1);
                emailTemplate.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.IsNotNull(emailTemplate.Template);
                Assert.IsNotNull(emailTemplate.CallForProposal);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTemplateWithANewValueDoesNotSave()
        {
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Template = CreateValidEntities.Template(9);
                emailTemplate.CallForProposal = null;
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Template, Entity: Gramps.Core.Domain.Template", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestEmailTemplateWithExistingTemplateSaves()
        {
            #region Arrange
            EmailTemplate record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Template);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailTemplateWithNullTemplateSaves()
        {
            #region Arrange
            EmailTemplate record = GetValid(99);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            record.Template = null;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitChanges();
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
        public void TestDeleteEmailTemplateDoesNotCascadeToTemplate()
        {
            #region Arrange
            EmailTemplate record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.Remove(record);
            EmailTemplateRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<Template>().GetNullableById(2));
            Assert.IsNull(EmailTemplateRepository.GetNullableById(saveId));
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
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Template = null;
                emailTemplate.CallForProposal = null;
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.AreEqual(emailTemplate.Template, null);
                Assert.AreEqual(emailTemplate.CallForProposal, null);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithAValidValueDoesNotSaveIfCallForProposalIsNotNull()
        {
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Template = Repository.OfType<Template>().GetNullableById(1);
                emailTemplate.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.IsNotNull(emailTemplate.Template);
                Assert.IsNotNull(emailTemplate.CallForProposal);
                var results = emailTemplate.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(emailTemplate.IsTransient());
                Assert.IsFalse(emailTemplate.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCallForProposalWithANewValueDoesNotSave()
        {
            EmailTemplate emailTemplate = null;
            try
            {
                #region Arrange
                emailTemplate = GetValid(9);
                emailTemplate.Template = null;
                emailTemplate.CallForProposal = new CallForProposal("test");
                #endregion Arrange

                #region Act
                EmailTemplateRepository.DbContext.BeginTransaction();
                EmailTemplateRepository.EnsurePersistent(emailTemplate);
                EmailTemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(emailTemplate);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.CallForProposal, Entity: Gramps.Core.Domain.CallForProposal", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestEmailTemplateWithExistingCallForProposalSaves()
        {
            #region Arrange
            EmailTemplate record = GetValid(99);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            record.Template = null;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CallForProposal);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailTemplateWithNullCallForProposalSaves()
        {
            #region Arrange
            EmailTemplate record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitChanges();
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
        public void TestDeleteEmailTemplateDoesNotCascadeToCallForProposal()
        {
            #region Arrange
            EmailTemplate record = GetValid(99);
            record.Template = null;
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.EnsurePersistent(record);
            EmailTemplateRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            EmailTemplateRepository.DbContext.BeginTransaction();
            EmailTemplateRepository.Remove(record);
            EmailTemplateRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<CallForProposal>().GetNullableById(2));
            Assert.IsNull(EmailTemplateRepository.GetNullableById(saveId));
            #endregion Assert
        }
        #endregion Cascade Tests

        #endregion CallForProposal Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorWithoutParametersSetsExpectedValues()
        {
            #region Arrange
            var record = new EmailTemplate();
            #endregion Arrange            

            #region Assert
            Assert.IsNotNull(record);
            Assert.IsNull(record.TemplateType);
            Assert.IsNull(record.Subject);
            Assert.IsNull(record.Text);
            Assert.IsNull(record.Template);
            Assert.IsNull(record.CallForProposal);
            #endregion Assert		
        }

        [TestMethod]
        public void TestConstructorWithParametersSetsExpectedValues()
        {
            #region Arrange
            var record = new EmailTemplate("subject");
            #endregion Arrange

            #region Assert
            Assert.IsNotNull(record);
            Assert.IsNull(record.TemplateType);
            Assert.AreEqual("subject", record.Subject);
            Assert.IsNull(record.Text);
            Assert.IsNull(record.Template);
            Assert.IsNull(record.CallForProposal);
            #endregion Assert
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate1()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.InitialCall)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate2()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.ProposalApproved)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate3()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.ProposalConfirmation)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate4()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.ProposalDenied)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate5()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.ReadyForReview)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate6()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.ReminderCallIsAboutToClose)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate7()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckReference(c => c.Template, template)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.InitialCall)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEmailTemplate8()
        {
            #region Arrange
            var id = EmailTemplateRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<EmailTemplate>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Subject, "subject")
                .CheckProperty(c => c.Text, "Text was here")
                .CheckProperty(c => c.Template, null)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.TemplateType, EmailTemplateType.InitialCall)
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
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("RelatedTable", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Must be related to Template or CallForProposal not both.\")]"
            }));
            expectedFields.Add(new NameAndType("Subject", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));           
            expectedFields.Add(new NameAndType("Template", "Gramps.Core.Domain.Template", new List<string>()));
            expectedFields.Add(new NameAndType("TemplateType", "System.Nullable`1[Gramps.Core.Domain.EmailTemplateType]", new List<string>
            {
                 "[NHibernate.Validator.Constraints.NotNullAttribute(Message = \"Must have email template type\")]"
            })); 
            expectedFields.Add(new NameAndType("Text", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(EmailTemplate));

        }

        #endregion Reflection of Database.	
		
		
    }
}