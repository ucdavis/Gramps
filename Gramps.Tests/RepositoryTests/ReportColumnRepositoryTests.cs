using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		ReportColumn
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class ReportColumnRepositoryTests : AbstractRepositoryTests<ReportColumn, int, ReportColumnMap>
    {
        /// <summary>
        /// Gets or sets the ReportColumn repository.
        /// </summary>
        /// <value>The ReportColumn repository.</value>
        public IRepository<ReportColumn> ReportColumnRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportColumnRepositoryTests"/> class.
        /// </summary>
        public ReportColumnRepositoryTests()
        {
            ReportColumnRepository = new Repository<ReportColumn>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ReportColumn GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ReportColumn(counter);
            rtValue.Report = Repository.OfType<Report>().Queryable.First();
            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ReportColumn> GetQuery(int numberAtEnd)
        {
            return ReportColumnRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ReportColumn entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ReportColumn entity, ARTAction action)
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
            Repository.OfType<Report>().DbContext.BeginTransaction();
            LoadUsers(3);
            LoadCallForProposals(2);
            LoadTemplates(2);
            LoadReports(3);
            Repository.OfType<Report>().DbContext.CommitTransaction();

            ReportColumnRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ReportColumnRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Report Tests

        #region Invalid Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReportWithAValueOfNullDoesNotSave()
        {
            ReportColumn record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Report = null;
                #endregion Arrange

                #region Act
                ReportColumnRepository.DbContext.BeginTransaction();
                ReportColumnRepository.EnsurePersistent(record);
                ReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.IsNull(record.Report);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Report: may not be null");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestReportWithANewValueDoesNotSave()
        {
            ReportColumn record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Report = new Report();
                #endregion Arrange

                #region Act
                ReportColumnRepository.DbContext.BeginTransaction();
                ReportColumnRepository.EnsurePersistent(record);
                ReportColumnRepository.DbContext.CommitTransaction();
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
        public void TestReportColumnWithExistingReportSaves()
        {
            #region Arrange
            ReportColumn record = GetValid(99);
            record.Report = Repository.OfType<Report>().GetNullableById(2);
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(record);
            ReportColumnRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(record.Report);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteReportColumnDoesNotCascadeToReport()
        {
            #region Arrange
            ReportColumn record = GetValid(99);
            record.Report = Repository.OfType<Report>().GetNullableById(2);
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(record);
            ReportColumnRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Arrange

            #region Act
            record = ReportColumnRepository.GetNullableById(saveId);
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.Remove(record);
            ReportColumnRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<Report>().GetNullableById(2));
            Assert.IsNull(ReportColumnRepository.GetNullableById(saveId));
            #endregion Assert
        }
        #endregion Cascade Tests

        #endregion Report Tests
        
        #region ColumnOrder Tests

        /// <summary>
        /// Tests the ColumnOrder with max int value saves.
        /// </summary>
        [TestMethod]
        public void TestColumnOrderWithMaxIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ColumnOrder = int.MaxValue;
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(record);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MaxValue, record.ColumnOrder);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ColumnOrder with min int value saves.
        /// </summary>
        [TestMethod]
        public void TestColumnOrderWithMinIntValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ColumnOrder = int.MinValue;
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(record);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(int.MinValue, record.ColumnOrder);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }

        #endregion ColumnOrder Tests

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            ReportColumn reportColumn = null;
            try
            {
                #region Arrange
                reportColumn = GetValid(9);
                reportColumn.Name = null;
                #endregion Arrange

                #region Act
                ReportColumnRepository.DbContext.BeginTransaction();
                ReportColumnRepository.EnsurePersistent(reportColumn);
                ReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(reportColumn);
                var results = reportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(reportColumn.IsTransient());
                Assert.IsFalse(reportColumn.IsValid());
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
            ReportColumn reportColumn = null;
            try
            {
                #region Arrange
                reportColumn = GetValid(9);
                reportColumn.Name = string.Empty;
                #endregion Arrange

                #region Act
                ReportColumnRepository.DbContext.BeginTransaction();
                ReportColumnRepository.EnsurePersistent(reportColumn);
                ReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(reportColumn);
                var results = reportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(reportColumn.IsTransient());
                Assert.IsFalse(reportColumn.IsValid());
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
            ReportColumn reportColumn = null;
            try
            {
                #region Arrange
                reportColumn = GetValid(9);
                reportColumn.Name = " ";
                #endregion Arrange

                #region Act
                ReportColumnRepository.DbContext.BeginTransaction();
                ReportColumnRepository.EnsurePersistent(reportColumn);
                ReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(reportColumn);
                var results = reportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(reportColumn.IsTransient());
                Assert.IsFalse(reportColumn.IsValid());
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
            ReportColumn reportColumn = null;
            try
            {
                #region Arrange
                reportColumn = GetValid(9);
                reportColumn.Name = "x".RepeatTimes((500 + 1));
                #endregion Arrange

                #region Act
                ReportColumnRepository.DbContext.BeginTransaction();
                ReportColumnRepository.EnsurePersistent(reportColumn);
                ReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(reportColumn);
                Assert.AreEqual(500 + 1, reportColumn.Name.Length);
                var results = reportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 500");
                Assert.IsTrue(reportColumn.IsTransient());
                Assert.IsFalse(reportColumn.IsValid());
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
            var reportColumn = GetValid(9);
            reportColumn.Name = "x";
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var reportColumn = GetValid(9);
            reportColumn.Name = "x".RepeatTimes(500);
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(500, reportColumn.Name.Length);
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Format Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Format with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestFormatWithTooLongValueDoesNotSave()
        {
            ReportColumn reportColumn = null;
            try
            {
                #region Arrange
                reportColumn = GetValid(9);
                reportColumn.Format = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                ReportColumnRepository.DbContext.BeginTransaction();
                ReportColumnRepository.EnsurePersistent(reportColumn);
                ReportColumnRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(reportColumn);
                Assert.AreEqual(50 + 1, reportColumn.Format.Length);
                var results = reportColumn.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Format: length must be between 0 and 50");
                Assert.IsTrue(reportColumn.IsTransient());
                Assert.IsFalse(reportColumn.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Format with null value saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithNullValueSaves()
        {
            #region Arrange
            var reportColumn = GetValid(9);
            reportColumn.Format = null;
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Format with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithEmptyStringSaves()
        {
            #region Arrange
            var reportColumn = GetValid(9);
            reportColumn.Format = string.Empty;
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Format with one space saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithOneSpaceSaves()
        {
            #region Arrange
            var reportColumn = GetValid(9);
            reportColumn.Format = " ";
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Format with one character saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithOneCharacterSaves()
        {
            #region Arrange
            var reportColumn = GetValid(9);
            reportColumn.Format = "x";
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Format with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFormatWithLongValueSaves()
        {
            #region Arrange
            var reportColumn = GetValid(9);
            reportColumn.Format = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, reportColumn.Format.Length);
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Format Tests

        #region IsProperty Tests

        /// <summary>
        /// Tests the IsProperty is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsPropertyIsFalseSaves()
        {
            #region Arrange

            ReportColumn reportColumn = GetValid(9);
            reportColumn.IsProperty = false;

            #endregion Arrange

            #region Act

            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(reportColumn.IsProperty);
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsProperty is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsPropertyIsTrueSaves()
        {
            #region Arrange

            var reportColumn = GetValid(9);
            reportColumn.IsProperty = true;

            #endregion Arrange

            #region Act

            ReportColumnRepository.DbContext.BeginTransaction();
            ReportColumnRepository.EnsurePersistent(reportColumn);
            ReportColumnRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(reportColumn.IsProperty);
            Assert.IsFalse(reportColumn.IsTransient());
            Assert.IsTrue(reportColumn.IsValid());

            #endregion Assert
        }

        #endregion IsProperty Tests

        #region Constructor Tests

        [TestMethod]
        public void TestContsructorSetsExpectedvalues()
        {
            #region Arrange
            var record = new ReportColumn();
            #endregion Arrange

            #region Assert
            Assert.IsFalse(record.IsProperty);
            Assert.IsNull(record.Report);
            #endregion Assert		
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapReportColumn1()
        {
            #region Arrange
            var id = ReportColumnRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var report = Repository.OfType<Report>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<ReportColumn>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.Report, report)
                .CheckProperty(c => c.IsProperty, true)
                .CheckProperty(c => c.ColumnOrder, 1)
                .CheckProperty(c => c.Format, "Format")
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        [TestMethod]
        public void TestCanCorrectlyMapReportColumn2()
        {
            #region Arrange
            var id = ReportColumnRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var report = Repository.OfType<Report>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<ReportColumn>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.Report, report)
                .CheckProperty(c => c.IsProperty, false)
                .CheckProperty(c => c.ColumnOrder, 99)
                .CheckProperty(c => c.Format, "Format")
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
            expectedFields.Add(new NameAndType("ColumnOrder", "System.Int32", new List<string>()));
            expectedFields.Add(new NameAndType("Format", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsProperty", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)500)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Report", "Gramps.Core.Domain.Report", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ReportColumn));

        }

        #endregion Reflection of Database.	
		
		
    }
}