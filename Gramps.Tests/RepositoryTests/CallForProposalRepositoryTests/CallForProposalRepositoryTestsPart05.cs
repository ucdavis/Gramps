using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region EmailTemplates Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the EmailTemplates with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailTemplatesWithAValueOfNullDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.EmailTemplates = null;
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
                Assert.AreEqual(callForProposal.EmailTemplates, null);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailTemplates: may not be null");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailTemplatesWithAnInvalidValueDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.EmailTemplates = new List<EmailTemplate>();
                var invalidEmailTemplate = new EmailTemplate();
                invalidEmailTemplate.TemplateType = null;
                callForProposal.AddEmailTemplate(invalidEmailTemplate);
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
                Assert.AreEqual(1, callForProposal.EmailTemplates.Count);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EmailTemplatesList: One or more invalid email templates detected");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }
        

        #endregion Invalid Tests
        #region Valid Tests
        //I don't know how many email templates we will have

        [TestMethod]
        public void TestEmailTemplateWithAnEmptyListSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.EmailTemplates = new List<EmailTemplate>();
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.EmailTemplates);
            Assert.AreEqual(0, record.EmailTemplates.Count);
            #endregion Assert	
        }

        [TestMethod]
        public void TestEmailTemplateWithValidPopulatedListSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.EmailTemplates = new List<EmailTemplate>();
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(1));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(2));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(3));
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.EmailTemplates);
            Assert.AreEqual(3, record.EmailTemplates.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailTemplateWithInvalidPopulatedListSavesIfNotActive()
        {
            #region Arrange
            var record = GetValid(99);
            record.EmailTemplates = new List<EmailTemplate>();
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(1));
            record.AddEmailTemplate(new EmailTemplate());
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(3));
            record.IsActive = false;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.EmailTemplates);
            Assert.AreEqual(3, record.EmailTemplates.Count);
            #endregion Assert
        }
        

        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestEmailTemplatesCascadesSave()
        {
            #region Arrange
            var emailTemplateRepository = new Repository<EmailTemplate>();
            var emailTemplateCount = emailTemplateRepository.Queryable.Count();
            var record = GetValid(99);
            record.EmailTemplates = new List<EmailTemplate>();
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(1));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(2));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(3));
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
            Assert.AreEqual(3, record.EmailTemplates.Count);
            Assert.AreEqual(3 + emailTemplateCount, emailTemplateRepository.Queryable.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailTemplatesCascadesUpdate()
        {
            #region Arrange
            var emailTemplateRepository = new Repository<EmailTemplate>();
            var record = GetValid(99);
            record.EmailTemplates = new List<EmailTemplate>();
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(1));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(2));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(3));
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            Assert.AreEqual("Subject2", emailTemplateRepository.GetNullableById(2).Subject);

            #endregion Arrange

            #region Act
            record = CallForProposalRepository.GetNullableById(saveId);
            record.EmailTemplates.ElementAt(1).Subject = "Updated Subject";
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            Assert.AreEqual("Updated Subject", emailTemplateRepository.GetNullableById(2).Subject);
            #endregion Assert
        }


        [TestMethod]
        public void TestEmailTemplateCascadesDelete1()
        {
            #region Arrange
            var emailTemplateRepository = new Repository<EmailTemplate>();
            var emailTemplateCount = emailTemplateRepository.Queryable.Count();
            var record = GetValid(99);
            record.EmailTemplates = new List<EmailTemplate>();
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(1));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(2));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(3));
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Arrange

            #region Act
            record.EmailTemplates.RemoveAt(1);
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.EmailTemplates);
            Assert.AreEqual(2, record.EmailTemplates.Count);
            Assert.AreEqual(2 + emailTemplateCount, emailTemplateRepository.Queryable.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestEmailTemplateCascadesDelete2()
        {
            #region Arrange
            var emailTemplateRepository = new Repository<EmailTemplate>();
            var emailTemplateCount = emailTemplateRepository.Queryable.Count();
            var record = GetValid(99);
            record.EmailTemplates = new List<EmailTemplate>();
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(1));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(2));
            record.AddEmailTemplate(CreateValidEntities.EmailTemplate(3));
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
            Assert.AreEqual(emailTemplateCount, emailTemplateRepository.Queryable.Count());
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion EmailTemplates Tests

        #region Reports Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Reports with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReportsWithAValueOfNullDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.Reports = null;
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
                Assert.AreEqual(callForProposal.Reports, null);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Reports: may not be null");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }


        #endregion Invalid Tests
        #region Valid Tests


        [TestMethod]
        public void TestReportsWithAnEmptyListSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.Reports = new List<Report>();
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Reports);
            Assert.AreEqual(0, record.Reports.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestReportsWithValidPopulatedListSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.Reports = new List<Report>();
            record.AddReport(CreateValidEntities.Report(1));
            record.AddReport(CreateValidEntities.Report(2));
            record.AddReport(CreateValidEntities.Report(3));
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Reports);
            Assert.AreEqual(3, record.Reports.Count);
            #endregion Assert
        }



        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestReportsCascadesSave()
        {
            #region Arrange
            var reportRepository = new Repository<Report>();
            var reportCount = reportRepository.Queryable.Count();
            var record = GetValid(99);
            record.Reports = new List<Report>();
            record.AddReport(CreateValidEntities.Report(1));
            record.AddReport(CreateValidEntities.Report(2));
            record.AddReport(CreateValidEntities.Report(3));
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            record = CallForProposalRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Reports);
            Assert.AreEqual(3, record.Reports.Count);
            Assert.AreEqual(3 + reportCount, reportRepository.Queryable.Count());
            #endregion Assert
        }



        [TestMethod]
        public void TestReportCascadesDelete1()
        {
            #region Arrange
            var reportRepository = new Repository<Report>();
            var reportCount = reportRepository.Queryable.Count();
            var record = GetValid(99);
            record.Reports = new List<Report>();
            record.AddReport(CreateValidEntities.Report(1));
            record.AddReport(CreateValidEntities.Report(2));
            record.AddReport(CreateValidEntities.Report(3));
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Arrange

            #region Act
            record.Reports.RemoveAt(1);
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.EmailTemplates);
            Assert.AreEqual(2, record.Reports.Count);
            Assert.AreEqual(2 + reportCount, reportRepository.Queryable.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestReportCascadesDelete2()
        {
            #region Arrange
            var reportRepository = new Repository<Report>();
            var reportCount = reportRepository.Queryable.Count();
            var record = GetValid(99);
            record.Reports = new List<Report>();
            record.AddReport(CreateValidEntities.Report(1));
            record.AddReport(CreateValidEntities.Report(2));
            record.AddReport(CreateValidEntities.Report(3));
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
            Assert.AreEqual(reportCount, reportRepository.Queryable.Count());
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion EmailTemplates Tests
    }
}
