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
    /// Entity Name:		Validator
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class ValidatorRepositoryTests : AbstractRepositoryTests<Validator, int, ValidatorMap>
    {
        /// <summary>
        /// Gets or sets the Validator repository.
        /// </summary>
        /// <value>The Validator repository.</value>
        public IRepository<Validator> ValidatorRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorRepositoryTests"/> class.
        /// </summary>
        public ValidatorRepositoryTests()
        {
            ValidatorRepository = new Repository<Validator>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Validator GetValid(int? counter)
        {
            return CreateValidEntities.Validator(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Validator> GetQuery(int numberAtEnd)
        {
            return ValidatorRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Validator entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Validator entity, ARTAction action)
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
            ValidatorRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            ValidatorRepository.DbContext.CommitTransaction();
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.HibernateException))]
        public override void CanDeleteEntity()
        {
            try
            {
                base.CanDeleteEntity();
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("Attempted to delete an object of immutable class: [Gramps.Core.Domain.Validator]", ex.Message);
                throw;
            }

        }

        [TestMethod]
        public override void CanUpdateEntity()
        {
            base.CanUpdateEntity(false);
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
            Validator validator = null;
            try
            {
                #region Arrange
                validator = GetValid(9);
                validator.Name = null;
                #endregion Arrange

                #region Act
                ValidatorRepository.DbContext.BeginTransaction();
                ValidatorRepository.EnsurePersistent(validator);
                ValidatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(validator);
                var results = validator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(validator.IsTransient());
                Assert.IsFalse(validator.IsValid());
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
            Validator validator = null;
            try
            {
                #region Arrange
                validator = GetValid(9);
                validator.Name = string.Empty;
                #endregion Arrange

                #region Act
                ValidatorRepository.DbContext.BeginTransaction();
                ValidatorRepository.EnsurePersistent(validator);
                ValidatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(validator);
                var results = validator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(validator.IsTransient());
                Assert.IsFalse(validator.IsValid());
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
            Validator validator = null;
            try
            {
                #region Arrange
                validator = GetValid(9);
                validator.Name = " ";
                #endregion Arrange

                #region Act
                ValidatorRepository.DbContext.BeginTransaction();
                ValidatorRepository.EnsurePersistent(validator);
                ValidatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(validator);
                var results = validator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(validator.IsTransient());
                Assert.IsFalse(validator.IsValid());
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
            Validator validator = null;
            try
            {
                #region Arrange
                validator = GetValid(9);
                validator.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                ValidatorRepository.DbContext.BeginTransaction();
                ValidatorRepository.EnsurePersistent(validator);
                ValidatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(validator);
                Assert.AreEqual(50 + 1, validator.Name.Length);
                var results = validator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 50");
                Assert.IsTrue(validator.IsTransient());
                Assert.IsFalse(validator.IsValid());
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
            var validator = GetValid(9);
            validator.Name = "x";
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, validator.Name.Length);
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Class Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Class with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestClassWithTooLongValueDoesNotSave()
        {
            Validator validator = null;
            try
            {
                #region Arrange
                validator = GetValid(9);
                validator.Class = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                ValidatorRepository.DbContext.BeginTransaction();
                ValidatorRepository.EnsurePersistent(validator);
                ValidatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(validator);
                Assert.AreEqual(50 + 1, validator.Class.Length);
                var results = validator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Class: length must be between 0 and 50");
                Assert.IsTrue(validator.IsTransient());
                Assert.IsFalse(validator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Class with null value saves.
        /// </summary>
        [TestMethod]
        public void TestClassWithNullValueSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.Class = null;
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Class with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestClassWithEmptyStringSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.Class = string.Empty;
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Class with one space saves.
        /// </summary>
        [TestMethod]
        public void TestClassWithOneSpaceSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.Class = " ";
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Class with one character saves.
        /// </summary>
        [TestMethod]
        public void TestClassWithOneCharacterSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.Class = "x";
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Class with long value saves.
        /// </summary>
        [TestMethod]
        public void TestClassWithLongValueSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.Class = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, validator.Class.Length);
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Class Tests

        #region RegEx Tests

        #region Valid Tests

        /// <summary>
        /// Tests the RegEx with null value saves.
        /// </summary>
        [TestMethod]
        public void TestRegExWithNullValueSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.RegEx = null;
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the RegEx with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestRegExWithEmptyStringSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.RegEx = string.Empty;
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the RegEx with one space saves.
        /// </summary>
        [TestMethod]
        public void TestRegExWithOneSpaceSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.RegEx = " ";
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the RegEx with one character saves.
        /// </summary>
        [TestMethod]
        public void TestRegExWithOneCharacterSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.RegEx = "x";
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the RegEx with long value saves.
        /// </summary>
        [TestMethod]
        public void TestRegExWithLongValueSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.RegEx = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, validator.RegEx.Length);
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion RegEx Tests

        #region ErrorMessage Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ErrorMessage with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestErrorMessageWithTooLongValueDoesNotSave()
        {
            Validator validator = null;
            try
            {
                #region Arrange
                validator = GetValid(9);
                validator.ErrorMessage = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                ValidatorRepository.DbContext.BeginTransaction();
                ValidatorRepository.EnsurePersistent(validator);
                ValidatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(validator);
                Assert.AreEqual(200 + 1, validator.ErrorMessage.Length);
                var results = validator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ErrorMessage: length must be between 0 and 200");
                Assert.IsTrue(validator.IsTransient());
                Assert.IsFalse(validator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ErrorMessage with null value saves.
        /// </summary>
        [TestMethod]
        public void TestErrorMessageWithNullValueSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.ErrorMessage = null;
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ErrorMessage with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestErrorMessageWithEmptyStringSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.ErrorMessage = string.Empty;
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ErrorMessage with one space saves.
        /// </summary>
        [TestMethod]
        public void TestErrorMessageWithOneSpaceSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.ErrorMessage = " ";
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ErrorMessage with one character saves.
        /// </summary>
        [TestMethod]
        public void TestErrorMessageWithOneCharacterSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.ErrorMessage = "x";
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ErrorMessage with long value saves.
        /// </summary>
        [TestMethod]
        public void TestErrorMessageWithLongValueSaves()
        {
            #region Arrange
            var validator = GetValid(9);
            validator.ErrorMessage = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            ValidatorRepository.DbContext.BeginTransaction();
            ValidatorRepository.EnsurePersistent(validator);
            ValidatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, validator.ErrorMessage.Length);
            Assert.IsFalse(validator.IsTransient());
            Assert.IsTrue(validator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ErrorMessage Tests

        
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
            expectedFields.Add(new NameAndType("Class", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("ErrorMessage", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("RegEx", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Validator));

        }

        #endregion Reflection of Database.	
		
		
    }
}