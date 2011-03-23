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
    /// Entity Name:		Report
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class ReportRepositoryTests : AbstractRepositoryTests<Report, int, ReportMap>
    {
        /// <summary>
        /// Gets or sets the Report repository.
        /// </summary>
        /// <value>The Report repository.</value>
        public IRepository<Report> ReportRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportRepositoryTests"/> class.
        /// </summary>
        public ReportRepositoryTests()
        {
            ReportRepository = new Repository<Report>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Report GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Report(counter);
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
        protected override IQueryable<Report> GetQuery(int numberAtEnd)
        {
            return ReportRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Report entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Report entity, ARTAction action)
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
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(2);
            LoadTemplates(2);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            ReportRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ReportRepository.DbContext.CommitTransaction();
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
            Report report = null;
            try
            {
                #region Arrange
                report = GetValid(9);
                report.Name = null;
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(report);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(report);
                var results = report.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(report.IsTransient());
                Assert.IsFalse(report.IsValid());
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
            Report report = null;
            try
            {
                #region Arrange
                report = GetValid(9);
                report.Name = string.Empty;
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(report);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(report);
                var results = report.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(report.IsTransient());
                Assert.IsFalse(report.IsValid());
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
            Report report = null;
            try
            {
                #region Arrange
                report = GetValid(9);
                report.Name = " ";
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(report);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(report);
                var results = report.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(report.IsTransient());
                Assert.IsFalse(report.IsValid());
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
            Report report = null;
            try
            {
                #region Arrange
                report = GetValid(9);
                report.Name = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(report);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(report);
                Assert.AreEqual(100 + 1, report.Name.Length);
                var results = report.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 100");
                Assert.IsTrue(report.IsTransient());
                Assert.IsFalse(report.IsValid());
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
            var report = GetValid(9);
            report.Name = "x";
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(report.IsTransient());
            Assert.IsTrue(report.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var report = GetValid(9);
            report.Name = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, report.Name.Length);
            Assert.IsFalse(report.IsTransient());
            Assert.IsTrue(report.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region ShowUnsubmitted Tests

        /// <summary>
        /// Tests the ShowUnsubmitted is false saves.
        /// </summary>
        [TestMethod]
        public void TestShowUnsubmittedIsFalseSaves()
        {
            #region Arrange

            Report report = GetValid(9);
            report.ShowUnsubmitted = false;

            #endregion Arrange

            #region Act

            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(report.ShowUnsubmitted);
            Assert.IsFalse(report.IsTransient());
            Assert.IsTrue(report.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the ShowUnsubmitted is true saves.
        /// </summary>
        [TestMethod]
        public void TestShowUnsubmittedIsTrueSaves()
        {
            #region Arrange

            var report = GetValid(9);
            report.ShowUnsubmitted = true;

            #endregion Arrange

            #region Act

            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(report.ShowUnsubmitted);
            Assert.IsFalse(report.IsTransient());
            Assert.IsTrue(report.IsValid());

            #endregion Assert
        }

        #endregion ShowUnsubmitted Tests

        #region Template Tests

        #region Invalid Tests


        /// <summary>
        /// Tests the Template with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateWithAValueOfNullDoesNotSaveIfCallForProposalIsNull()
        {
            Report record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = null;
                record.CallForProposal = null;
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(record);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Template, null);
                Assert.AreEqual(record.CallForProposal, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateWithAValidValueDoesNotSaveIfCallForProposalIsNotNull()
        {
            Report record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = Repository.OfType<Template>().GetNullableById(1);
                record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(record);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(record.Template);
                Assert.IsNotNull(record.CallForProposal);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTemplateWithANewValueDoesNotSave()
        {
            Report record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = CreateValidEntities.Template(9);
                record.CallForProposal = null;
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(record);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Template, Entity: Gramps.Core.Domain.Template", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestReportWithExistingTemplateSaves()
        {
            #region Arrange
            Report record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(record);
            ReportRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Template);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestReportWithNullTemplateSaves()
        {
            #region Arrange
            Report record = GetValid(99);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            record.Template = null;
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(record);
            ReportRepository.DbContext.CommitChanges();
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
        public void TestDeleteReportDoesNotCascadeToTemplate()
        {
            #region Arrange
            Report record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(record);
            ReportRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.Remove(record);
            ReportRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<Template>().GetNullableById(2));
            Assert.IsNull(ReportRepository.GetNullableById(saveId));
            #endregion Assert
        }
        #endregion Cascade Tests

        #endregion Template Tests

        #region CallForProposal Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithAValueOfNullDoesNotSaveIfCallForProposalIsNull()
        {
            Report record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = null;
                record.CallForProposal = null;
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(record);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Template, null);
                Assert.AreEqual(record.CallForProposal, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithAValidValueDoesNotSaveIfCallForProposalIsNotNull()
        {
            Report record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = Repository.OfType<Template>().GetNullableById(1);
                record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(record);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(record.Template);
                Assert.IsNotNull(record.CallForProposal);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCallForProposalWithANewValueDoesNotSave()
        {
            Report record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = null;
                record.CallForProposal = new CallForProposal("test");
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(record);
                ReportRepository.DbContext.CommitTransaction();
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
        public void TestReportWithExistingCallForProposalSaves()
        {
            #region Arrange
            Report record = GetValid(99);
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            record.Template = null;
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(record);
            ReportRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.CallForProposal);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestReportWithNullCallForProposalSaves()
        {
            #region Arrange
            Report record = GetValid(99);
            record.CallForProposal = null;
            record.Template = Repository.OfType<Template>().GetNullableById(2);
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(record);
            ReportRepository.DbContext.CommitChanges();
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
        public void TestDeleteReportDoesNotCascadeToCallForProposal()
        {
            #region Arrange
            Report record = GetValid(99);
            record.Template = null;
            record.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(2);
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(record);
            ReportRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.Remove(record);
            ReportRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<CallForProposal>().GetNullableById(2));
            Assert.IsNull(ReportRepository.GetNullableById(saveId));
            #endregion Assert
        }
        #endregion Cascade Tests

        #endregion CallForProposal Tests

        #region ReportColumns Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReportColumnsWithAValueOfNullDoesNotSave()
        {
            Report record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.ReportColumns = null;
                #endregion Arrange

                #region Act
                ReportRepository.DbContext.BeginTransaction();
                ReportRepository.EnsurePersistent(record);
                ReportRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.ReportColumns, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReportColumns: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestReportColumnsWithPopoulatedListWillSave()
        {
            #region Arrange
            Report report = GetValid(9);
            report.AddReportColumn(CreateValidEntities.ReportColumn(1));
            report.AddReportColumn(CreateValidEntities.ReportColumn(2));
            report.AddReportColumn(CreateValidEntities.ReportColumn(3));
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(report.ReportColumns);
            Assert.AreEqual(3, report.ReportColumns.Count);
            Assert.IsFalse(report.IsTransient());
            Assert.IsTrue(report.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestReportColumnsWithEmptyListWillSave()
        {
            #region Arrange
            Report report = GetValid(9);
            report.AddReportColumn(CreateValidEntities.ReportColumn(1));
            report.AddReportColumn(CreateValidEntities.ReportColumn(2));
            report.AddReportColumn(CreateValidEntities.ReportColumn(3));
            #endregion Arrange

            #region Act
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(report.ReportColumns);
            Assert.AreEqual(3, report.ReportColumns.Count);
            Assert.IsFalse(report.IsTransient());
            Assert.IsTrue(report.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestReportCascadesSaveToReportColumns()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Report report = GetValid(9);
            report.AddReportColumn(CreateValidEntities.ReportColumn(1));
            report.AddReportColumn(CreateValidEntities.ReportColumn(2));
            report.AddReportColumn(CreateValidEntities.ReportColumn(3));

            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            var saveId = report.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(report);
            #endregion Arrange

            #region Act
            report = ReportRepository.GetNullableById(saveId);
            #endregion Act

            #region Assert
            Assert.IsNotNull(report);
            Assert.AreEqual(3, report.ReportColumns.Count);
            Assert.AreEqual(count + 3, Repository.OfType<ReportColumn>().Queryable.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestReportCascadesUpdateToReportColumns()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Report report = GetValid(9);
            report.AddReportColumn(CreateValidEntities.ReportColumn(1));
            report.AddReportColumn(CreateValidEntities.ReportColumn(2));
            report.AddReportColumn(CreateValidEntities.ReportColumn(3));

            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            var saveId = report.Id;
            var saveColumnId = report.ReportColumns[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(report);
            #endregion Arrange

            #region Act
            report = ReportRepository.GetNullableById(saveId);
            report.ReportColumns[1].Name = "Updated";
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(report);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + 3, Repository.OfType<ReportColumn>().Queryable.Count());
            var column = Repository.OfType<ReportColumn>().GetNullableById(saveColumnId);
            Assert.IsNotNull(column);
            Assert.AreEqual("Updated", column.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestReportCascadesUpdateRemoveToReportColumns()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Report report = GetValid(9);
            report.AddReportColumn(CreateValidEntities.ReportColumn(1));
            report.AddReportColumn(CreateValidEntities.ReportColumn(2));
            report.AddReportColumn(CreateValidEntities.ReportColumn(3));

            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            var saveId = report.Id;
            var saveColumnId = report.ReportColumns[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(report);
            #endregion Arrange

            #region Act
            report = ReportRepository.GetNullableById(saveId);
            report.ReportColumns.RemoveAt(1);
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(report);
            #endregion Act

            #region Assert
            Assert.AreEqual(count + 2, Repository.OfType<ReportColumn>().Queryable.Count());
            var column = Repository.OfType<ReportColumn>().GetNullableById(saveColumnId);
            Assert.IsNull(column);
            #endregion Assert
        }

        [TestMethod]
        public void TestReportCascadesDeleteToReportColumns()
        {
            #region Arrange
            var count = Repository.OfType<ReportColumn>().Queryable.Count();
            Report report = GetValid(9);
            report.AddReportColumn(CreateValidEntities.ReportColumn(1));
            report.AddReportColumn(CreateValidEntities.ReportColumn(2));
            report.AddReportColumn(CreateValidEntities.ReportColumn(3));

            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.EnsurePersistent(report);
            ReportRepository.DbContext.CommitTransaction();
            var saveId = report.Id;
            var saveColumnId = report.ReportColumns[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(report);
            #endregion Arrange

            #region Act
            report = ReportRepository.GetNullableById(saveId);
            ReportRepository.DbContext.BeginTransaction();
            ReportRepository.Remove(report);
            ReportRepository.DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(report);
            #endregion Act

            #region Assert
            Assert.AreEqual(count, Repository.OfType<ReportColumn>().Queryable.Count());
            var column = Repository.OfType<ReportColumn>().GetNullableById(saveColumnId);
            Assert.IsNull(column);
            #endregion Assert
        }


        #endregion Cascade Tests
        #endregion ReportColumns Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapReport1()
        {
            #region Arrange
            var id = ReportRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var template = Repository.OfType<Template>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Report>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.Template, template)
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.ShowUnsubmitted, true)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapReport2()
        {
            #region Arrange
            var id = ReportRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Report>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.ShowUnsubmitted, false)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapReport3()
        {
            #region Arrange
            var id = ReportRepository.Queryable.Max(x => x.Id) + 1;
            var report = new Report();
            report.SetIdTo(id);
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var columns = new List<ReportColumn>();
            columns.Add(CreateValidEntities.ReportColumn(1));
            columns.Add(CreateValidEntities.ReportColumn(2));
            columns.Add(CreateValidEntities.ReportColumn(3));
            columns.Add(CreateValidEntities.ReportColumn(4));
            foreach (var reportColumn in columns)
            {
                reportColumn.Report = report;
            }
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Report>(session, new ReportEqualityComparer())
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.ShowUnsubmitted, false)
                .CheckProperty(c => c.ReportColumns, columns)
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
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("RelatedTable", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Must be related to Template or CallForProposal not both.\")]"
            }));
            expectedFields.Add(new NameAndType("ReportColumns", "System.Collections.Generic.IList`1[Gramps.Core.Domain.ReportColumn]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ShowUnsubmitted", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Template", "Gramps.Core.Domain.Template", new List<string>()));           
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Report));

        }

        #endregion Reflection of Database.	
		
        public class ReportEqualityComparer : IEqualityComparer
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

                if (x is IList<ReportColumn> && y is IList<ReportColumn>)
                {
                    var xVal = (IList<ReportColumn>)x;
                    var yVal = (IList<ReportColumn>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Name, yVal[i].Name);
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