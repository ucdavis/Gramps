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
using NHibernate;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Template
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class TemplateRepositoryTests : AbstractRepositoryTests<Template, int, TemplateMap>
    {
        /// <summary>
        /// Gets or sets the Template repository.
        /// </summary>
        /// <value>The Template repository.</value>
        public IRepository<Template> TemplateRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateRepositoryTests"/> class.
        /// </summary>
        public TemplateRepositoryTests()
        {
            TemplateRepository = new Repository<Template>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Template GetValid(int? counter)
        {
            return CreateValidEntities.Template(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Template> GetQuery(int numberAtEnd)
        {
            return TemplateRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Template entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Template entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            TemplateRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            LoadRecords(5);
            TemplateRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Name = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                var results = template.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Name = string.Empty;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                var results = template.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Name = " ";
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                var results = template.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            Template template = null;
            try
            {
                #region Arrange
                template = GetValid(9);
                template.Name = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(template);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(template);
                Assert.AreEqual(100 + 1, template.Name.Length);
                var results = template.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 100");
                Assert.IsTrue(template.IsTransient());
                Assert.IsFalse(template.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var template = GetValid(9);
            template.Name = "x";
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var template = GetValid(9);
            template.Name = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, template.Name.Length);
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange

            Template template = GetValid(9);
            template.IsActive = false;

            #endregion Arrange

            #region Act

            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(template.IsActive);
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange

            var template = GetValid(9);
            template.IsActive = true;

            #endregion Arrange

            #region Act

            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(template);
            TemplateRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(template.IsActive);
            Assert.IsFalse(template.IsTransient());
            Assert.IsTrue(template.IsValid());

            #endregion Assert
        }

        #endregion IsActive Tests

        #region Emails Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailsWithAValueOfNullDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Emails = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Emails, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Emails: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestEmailsWithANewValueDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Emails.Add(CreateValidEntities.EmailsForCall(1));
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.EmailsForCall, Entity: Gramps.Core.Domain.EmailsForCall", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestEmailsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<EmailsForCall>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailsForCall(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailsForCall>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Emails.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Emails);
            Assert.AreEqual(addedCount, record.Emails.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailsWithEmptyListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Emails);
            Assert.AreEqual(0, record.Emails.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        //[TestMethod]
        //public void TestTemplateCascadesUpdateToEmailsForCall1()
        //{
        //    #region Arrange
        //    var count = Repository.OfType<EmailsForCall>().Queryable.Count();
        //    Template record = GetValid(9);
        //    const int addedCount = 3;
        //    for (int i = 0; i < addedCount; i++)
        //    {
        //        record.Emails.Add(CreateValidEntities.EmailsForCall(i+1));
        //    }

        //    TemplateRepository.DbContext.BeginTransaction();
        //    TemplateRepository.EnsurePersistent(record);
        //    TemplateRepository.DbContext.CommitTransaction();
        //    var saveId = record.Id;
        //    var saveRelatedId = record.Emails[1].Id;
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Arrange

        //    #region Act
        //    record = TemplateRepository.GetNullableById(saveId);
        //    record.Emails[1].Email = "Updated";
        //    TemplateRepository.DbContext.BeginTransaction();
        //    TemplateRepository.EnsurePersistent(record);
        //    TemplateRepository.DbContext.CommitTransaction();
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(count + addedCount, Repository.OfType<EmailsForCall>().Queryable.Count());
        //    var relatedRecord = Repository.OfType<EmailsForCall>().GetNullableById(saveRelatedId);
        //    Assert.IsNotNull(relatedRecord);
        //    Assert.AreEqual("Updated", relatedRecord.Email);
        //    #endregion Assert
        //}

        [TestMethod]
        public void TestTemplateCascadesUpdateToEmailsForCall2()
        {
            #region Arrange
            var count = Repository.OfType<EmailsForCall>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailsForCall>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailsForCall(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailsForCall>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Emails.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Emails[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Emails[1].Email = "Updated";
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<EmailsForCall>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<EmailsForCall>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Email);
            #endregion Assert
        }

        [TestMethod]
        public void TestTemplateCascadesUpdateRemoveEmailsForCall()
        {
            #region Arrange
            var count = Repository.OfType<EmailsForCall>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailsForCall>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailsForCall(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailsForCall>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Emails.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Emails[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Emails.RemoveAt(1);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount - 1), Repository.OfType<EmailsForCall>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<EmailsForCall>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestTemplateCascadesDeleteToEmailsForCall()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailsForCall>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailsForCall(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailsForCall>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Emails.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Emails[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.Remove(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<EmailsForCall>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<EmailsForCall>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion Emails Tests

        #region EmailTemplates Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailTemplatesWithAValueOfNullDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.EmailTemplates = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.EmailTemplates, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailTemplates: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestEmailTemplatesWithANewValueDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.EmailTemplate, Entity: Gramps.Core.Domain.EmailTemplate", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestEmailTemplatesWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<EmailTemplate>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailTemplate(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailTemplate>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailTemplates.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.EmailTemplates);
            Assert.AreEqual(addedCount, record.EmailTemplates.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailTemplatesWithEmptyListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.EmailTemplates);
            Assert.AreEqual(0, record.EmailTemplates.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestTemplateCascadesUpdateToEmailTemplate2()
        {
            #region Arrange
            var count = Repository.OfType<EmailTemplate>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailTemplate>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailTemplate(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailTemplate>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailTemplates.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.EmailTemplates[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.EmailTemplates[1].Subject = "Updated";
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<EmailTemplate>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<EmailTemplate>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Subject);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it 
        /// </summary>
        [TestMethod]
        public void TestTemplateCascadesUpdateRemoveEmailTemplate()
        {
            #region Arrange
            var count = Repository.OfType<EmailTemplate>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailTemplate>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailTemplate(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailTemplate>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailTemplates.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.EmailTemplates[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.EmailTemplates.RemoveAt(1);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<EmailTemplate>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<EmailTemplate>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
    

        [TestMethod]
        public void TestTemplateCascadesDeleteToEmailTemplate()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<EmailTemplate>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.EmailTemplate(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<EmailTemplate>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.EmailTemplates.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.EmailTemplates[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.Remove(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<EmailTemplate>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<EmailTemplate>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests
        #endregion EmailTemplates Tests

        #region Editors Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEditorsWithAValueOfNullDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Editors = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Editors, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Editors: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestEditorsWithANewValueDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Editors.Add(CreateValidEntities.Editor(1));
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Editor, Entity: Gramps.Core.Domain.Editor", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestEditorsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Editor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Editor(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Editor>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Editors.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Editors);
            Assert.AreEqual(addedCount, record.Editors.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestEditorsWithEmptyListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Editors);
            Assert.AreEqual(0, record.Editors.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestTemplateCascadesUpdateToEditor2()
        {
            #region Arrange
            var count = Repository.OfType<Editor>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Editor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Editor(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Editor>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Editors.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Editors[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Editors[1].ReviewerEmail = "Updated";
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Editor>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Editor>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.ReviewerEmail);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it
        /// </summary>
        [TestMethod]
        public void TestTemplateCascadesUpdateRemoveEditor()
        {
            #region Arrange
            var count = Repository.OfType<Editor>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Editor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Editor(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Editor>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Editors.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Editors[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Editors.RemoveAt(1);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<Editor>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Editor>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestTemplateCascadesDeleteToEditor()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Editor>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Editor(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Editor>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Editors.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Editors[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.Remove(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<Editor>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Editor>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }

        #endregion Cascade Tests

        #endregion Editors Tests

        #region Questions Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionsWithAValueOfNullDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Questions = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Questions, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Questions: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestQuestionsWithANewValueDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Questions.Add(CreateValidEntities.Question(1));
                record.Questions[0].QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Question, Entity: Gramps.Core.Domain.Question", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Question>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Question(i + 1));
                relatedRecords[i].Template = record;
                relatedRecords[i].QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
                Repository.OfType<Question>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Questions.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(addedCount, record.Questions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionsWithEmptyListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(0, record.Questions.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestTemplateCascadesUpdateToQuestion2()
        {
            #region Arrange
            var count = Repository.OfType<Question>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Question>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Question(i + 1));
                relatedRecords[i].Template = record;
                relatedRecords[i].QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
                Repository.OfType<Question>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Questions.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Questions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Questions[1].Name = "Updated";
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Question>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Question>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestTemplateCascadesUpdateRemoveQuestion()
        {
            #region Arrange
            var count = Repository.OfType<Question>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Question>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Question(i + 1));
                relatedRecords[i].Template = record;
                relatedRecords[i].QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
                Repository.OfType<Question>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Questions.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Questions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Questions.RemoveAt(1);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<Question>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Question>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestTemplateCascadesDeleteToQuestion()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Question>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Question(i + 1));
                relatedRecords[i].Template = record;
                relatedRecords[i].QuestionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Text Box").Single();
                Repository.OfType<Question>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Questions.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Questions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.Remove(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<Question>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Question>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion Questions Tests

        #region CallForProposals Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalsWithAValueOfNullDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.CallForProposals = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.CallForProposals, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("CallForProposals: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCallForProposalsWithANewValueDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.CallForProposals.Add(CreateValidEntities.CallForProposal(1));
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
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
        public void TestCallForProposalsWithPopulatedExistingListWillSave()
        {
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(3);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();
            #region Arrange
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<CallForProposal>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(Repository.OfType<CallForProposal>().GetNullableById(i+1));
                relatedRecords[i].TemplateGeneratedFrom = record;
                Repository.OfType<CallForProposal>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.CallForProposals.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CallForProposals);
            Assert.AreEqual(addedCount, record.CallForProposals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCallForProposalsWithEmptyListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CallForProposals);
            Assert.AreEqual(0, record.CallForProposals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestTemplateCascadesUpdateToCallForProposal2()
        {
            #region Arrange
            var count = Repository.OfType<CallForProposal>().Queryable.Count();
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(3);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<CallForProposal>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(Repository.OfType<CallForProposal>().GetNullableById(i + 1));
                relatedRecords[i].TemplateGeneratedFrom = record;
                Repository.OfType<CallForProposal>().EnsurePersistent(relatedRecords[i]);
            }

            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = relatedRecords[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.CallForProposals[1].Name = "Updated";
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<CallForProposal>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CallForProposal>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }

        /// <summary>
        /// Does NOT Remove it
        /// </summary>
        [TestMethod]
        public void TestTemplateDoesNotCascadesUpdateRemoveCallForProposal()
        {
            #region Arrange
            var count = Repository.OfType<CallForProposal>().Queryable.Count();

            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(3);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<CallForProposal>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(Repository.OfType<CallForProposal>().GetNullableById(i + 1));
                relatedRecords[i].TemplateGeneratedFrom = record;
                Repository.OfType<CallForProposal>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CallForProposals.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CallForProposals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.CallForProposals.RemoveAt(1);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount), Repository.OfType<CallForProposal>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CallForProposal>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }

        [TestMethod]
        public void TestTemplateDoesNotCascadesDeleteToCallForProposal()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            const int addedCount = 3;
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            LoadUsers(addedCount);
            LoadCallForProposals(3);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();
            
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            
            var relatedRecords = new List<CallForProposal>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(Repository.OfType<CallForProposal>().GetNullableById(i+1));
                relatedRecords[i].TemplateGeneratedFrom = record;
                Repository.OfType<CallForProposal>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.CallForProposals.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.CallForProposals[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.Remove(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<CallForProposal>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<CallForProposal>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            #endregion Assert
        }
		


        #endregion Cascade Tests
        #endregion CallForProposals Tests

        #region Reports Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReportsWithAValueOfNullDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Reports = null;
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Reports, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Reports: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestReportsWithANewValueDoesNotSave()
        {
            Template record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Reports.Add(CreateValidEntities.Report(1));
                #endregion Arrange

                #region Act
                TemplateRepository.DbContext.BeginTransaction();
                TemplateRepository.EnsurePersistent(record);
                TemplateRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Report, Entity: Gramps.Core.Domain.Report", ex.Message);
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestReportsWithPopulatedExistingListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();

            const int addedCount = 3;
            var relatedRecords = new List<Report>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Report(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Report>().EnsurePersistent(relatedRecords[i]);
            }
            #endregion Arrange

            #region Act

            foreach (var relatedRecord in relatedRecords)
            {
                record.Reports.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Reports);
            Assert.AreEqual(addedCount, record.Reports.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestReportsWithEmptyListWillSave()
        {
            #region Arrange
            Template record = GetValid(9);
            #endregion Arrange

            #region Act
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Reports);
            Assert.AreEqual(0, record.Reports.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestTemplateCascadesUpdateToReport2()
        {
            #region Arrange
            var count = Repository.OfType<Report>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Report>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Report(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Report>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Reports.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Reports[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Reports[1].Name = "Updated";
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            foreach (var relatedRecord in relatedRecords)
            {
                NHibernateSessionManager.Instance.GetSession().Evict(relatedRecord);
            }
            #endregion Act

            #region Assert
            Assert.AreEqual(count + addedCount, Repository.OfType<Report>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Report>().GetNullableById(saveRelatedId);
            Assert.IsNotNull(relatedRecord2);
            Assert.AreEqual("Updated", relatedRecord2.Name);
            #endregion Assert
        }

        /// <summary>
        /// Does Remove it (Delete this test, or the one below)
        /// </summary>
        [TestMethod]
        public void TestTemplateCascadesUpdateRemoveReport()
        {
            #region Arrange
            var count = Repository.OfType<Report>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Report>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Report(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Report>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Reports.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Reports[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            record.Reports.RemoveAt(1);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + (addedCount-1), Repository.OfType<Report>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Report>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        [TestMethod]
        public void TestTemplateCascadesDeleteToReport()
        {
            #region Arrange
            var count = Repository.OfType<Report>().Queryable.Count();
            Template record = GetValid(9);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();


            const int addedCount = 3;
            var relatedRecords = new List<Report>();
            for (int i = 0; i < addedCount; i++)
            {
                relatedRecords.Add(CreateValidEntities.Report(i + 1));
                relatedRecords[i].Template = record;
                Repository.OfType<Report>().EnsurePersistent(relatedRecords[i]);
            }
            foreach (var relatedRecord in relatedRecords)
            {
                record.Reports.Add(relatedRecord);
            }
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.EnsurePersistent(record);
            TemplateRepository.DbContext.CommitTransaction();
            var saveId = record.Id;
            var saveRelatedId = record.Reports[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = TemplateRepository.GetNullableById(saveId);
            TemplateRepository.DbContext.BeginTransaction();
            TemplateRepository.Remove(record);
            TemplateRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<Report>().Queryable.Count());
            var relatedRecord2 = Repository.OfType<Report>().GetNullableById(saveRelatedId);
            Assert.IsNull(relatedRecord2);
            #endregion Assert
        }


        #endregion Cascade Tests

        #endregion Reports Tests


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange

            Assert.Inconclusive("Write constructor and fluent tests");

            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert		
        }


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
            expectedFields.Add(new NameAndType("CallForProposals", "System.Collections.Generic.IList`1[Gramps.Core.Domain.CallForProposal]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Editors", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Editor]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Emails", "System.Collections.Generic.IList`1[Gramps.Core.Domain.EmailsForCall]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("EmailTemplates", "System.Collections.Generic.IList`1[Gramps.Core.Domain.EmailTemplate]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Questions", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Question]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Reports", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Report]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Template));

        }

        #endregion Reflection of Database.	
		
		
    }
}