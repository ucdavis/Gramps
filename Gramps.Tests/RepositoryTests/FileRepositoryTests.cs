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
    /// Entity Name:		File
    /// LookupFieldName:	Name yrjuy
    /// </summary>
    [TestClass]
    public class FileRepositoryTests : AbstractRepositoryTests<File, int, FileMap>
    {
        /// <summary>
        /// Gets or sets the File repository.
        /// </summary>
        /// <value>The File repository.</value>
        public IRepository<File> FileRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRepositoryTests"/> class.
        /// </summary>
        public FileRepositoryTests()
        {
            FileRepository = new Repository<File>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override File GetValid(int? counter)
        {
            return CreateValidEntities.File(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<File> GetQuery(int numberAtEnd)
        {
            return FileRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(File entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(File entity, ARTAction action)
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
            FileRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            FileRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region ContentType Tests


        #region Valid Tests

        /// <summary>
        /// Tests the ContentType with null value saves.
        /// </summary>
        [TestMethod]
        public void TestContentTypeWithNullValueSaves()
        {
            #region Arrange
            var file = GetValid(9);
            file.ContentType = null;
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(file);
            FileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(file.IsTransient());
            Assert.IsTrue(file.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ContentType with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestContentTypeWithEmptyStringSaves()
        {
            #region Arrange
            var file = GetValid(9);
            file.ContentType = string.Empty;
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(file);
            FileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(file.IsTransient());
            Assert.IsTrue(file.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ContentType with one space saves.
        /// </summary>
        [TestMethod]
        public void TestContentTypeWithOneSpaceSaves()
        {
            #region Arrange
            var file = GetValid(9);
            file.ContentType = " ";
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(file);
            FileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(file.IsTransient());
            Assert.IsTrue(file.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ContentType with one character saves.
        /// </summary>
        [TestMethod]
        public void TestContentTypeWithOneCharacterSaves()
        {
            #region Arrange
            var file = GetValid(9);
            file.ContentType = "x";
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(file);
            FileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(file.IsTransient());
            Assert.IsTrue(file.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ContentType with long value saves.
        /// </summary>
        [TestMethod]
        public void TestContentTypeWithLongValueSaves()
        {
            #region Arrange
            var file = GetValid(9);
            file.ContentType = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(file);
            FileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, file.ContentType.Length);
            Assert.IsFalse(file.IsTransient());
            Assert.IsTrue(file.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ContentType Tests

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            File file = null;
            try
            {
                #region Arrange
                file = GetValid(9);
                file.Name = null;
                #endregion Arrange

                #region Act
                FileRepository.DbContext.BeginTransaction();
                FileRepository.EnsurePersistent(file);
                FileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(file);
                var results = file.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(file.IsTransient());
                Assert.IsFalse(file.IsValid());
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
            File file = null;
            try
            {
                #region Arrange
                file = GetValid(9);
                file.Name = string.Empty;
                #endregion Arrange

                #region Act
                FileRepository.DbContext.BeginTransaction();
                FileRepository.EnsurePersistent(file);
                FileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(file);
                var results = file.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(file.IsTransient());
                Assert.IsFalse(file.IsValid());
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
            File file = null;
            try
            {
                #region Arrange
                file = GetValid(9);
                file.Name = " ";
                #endregion Arrange

                #region Act
                FileRepository.DbContext.BeginTransaction();
                FileRepository.EnsurePersistent(file);
                FileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(file);
                var results = file.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(file.IsTransient());
                Assert.IsFalse(file.IsValid());
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
            File file = null;
            try
            {
                #region Arrange
                file = GetValid(9);
                file.Name = "x".RepeatTimes((512 + 1));
                #endregion Arrange

                #region Act
                FileRepository.DbContext.BeginTransaction();
                FileRepository.EnsurePersistent(file);
                FileRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(file);
                Assert.AreEqual(512 + 1, file.Name.Length);
                var results = file.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 512");
                Assert.IsTrue(file.IsTransient());
                Assert.IsFalse(file.IsValid());
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
            var file = GetValid(9);
            file.Name = "x";
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(file);
            FileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(file.IsTransient());
            Assert.IsTrue(file.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var file = GetValid(9);
            file.Name = "x".RepeatTimes(512);
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(file);
            FileRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(512, file.Name.Length);
            Assert.IsFalse(file.IsTransient());
            Assert.IsTrue(file.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region DateAdded Tests

        /// <summary>
        /// Tests the DateAdded with past date will save.
        /// </summary>
        [TestMethod]
        public void TestDateAddedWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            File record = GetValid(99);
            record.DateAdded = compareDate;
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(record);
            FileRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateAdded);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the DateAdded with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateAddedWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.DateAdded = compareDate;
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(record);
            FileRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateAdded);
            #endregion Assert
        }

        /// <summary>
        /// Tests the DateAdded with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestDateAddedWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.DateAdded = compareDate;
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(record);
            FileRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.DateAdded);
            #endregion Assert
        }
        #endregion DateAdded Tests

        #region Contents Tests

        [TestMethod]
        public void TestContentsSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Contents = new byte[]{1,2,3,4,5};
            #endregion Arrange

            #region Act
            FileRepository.DbContext.BeginTransaction();
            FileRepository.EnsurePersistent(record);
            FileRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual("12345", record.Contents.ByteArrayToString());
            #endregion Assert	
        }
        #endregion Contents Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapFile()
        {
            #region Arrange
            var id = FileRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var compareDate =new  DateTime(2011, 01, 20);
            var cont = new byte[] {1, 2, 3, 4, 5};
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<File>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Contents, cont)
                .CheckProperty(c => c.ContentType, "ContentType")
                .CheckProperty(c => c.DateAdded, compareDate)
                .CheckProperty(c => c.Name, "Name")

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
            expectedFields.Add(new NameAndType("Contents", "System.Byte[]", new List<string>()));
            expectedFields.Add(new NameAndType("ContentType", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("DateAdded", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)512)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(File));

        }

        #endregion Reflection of Database.	
		
		
    }
}