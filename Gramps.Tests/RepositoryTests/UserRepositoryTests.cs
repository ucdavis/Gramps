using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		User
    /// LookupFieldName:	LoginId
    /// </summary>
    [TestClass]
    public class UserRepositoryTests : AbstractRepositoryTests<User, int, UserMap>
    {
        /// <summary>
        /// Gets or sets the User repository.
        /// </summary>
        /// <value>The User repository.</value>
        public IRepository<User> UserRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepositoryTests"/> class.
        /// </summary>
        public UserRepositoryTests()
        {
            UserRepository = new Repository<User>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override User GetValid(int? counter)
        {
            return CreateValidEntities.User(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<User> GetQuery(int numberAtEnd)
        {
            return UserRepository.Queryable.Where(a => a.LoginId.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(User entity, int counter)
        {
            Assert.AreEqual("LoginId" + counter, entity.LoginId);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(User entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.LoginId);
                    break;
                case ARTAction.Restore:
                    entity.LoginId = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.LoginId;
                    entity.LoginId = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            UserRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            UserRepository.DbContext.CommitTransaction();
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
                Assert.AreEqual("Attempted to delete an object of immutable class: [Gramps.Core.Domain.User]", ex.Message);
                throw;
            }

        }

        [TestMethod]
        public override void CanUpdateEntity()
        {
            base.CanUpdateEntity(false);
        }

        #endregion Init and Overrides	
        
        #region LoginId Tests

        #region Valid Tests

        /// <summary>
        /// Tests the LoginId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLoginIdWithNullValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LoginId = null;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LoginId with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLoginIdWithEmptyStringSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LoginId = string.Empty;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LoginId with one space saves.
        /// </summary>
        [TestMethod]
        public void TestLoginIdWithOneSpaceSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LoginId = " ";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LoginId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLoginIdWithOneCharacterSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LoginId = "x";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LoginId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLoginIdWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LoginId = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, user.LoginId.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion LoginId Tests

        #region FirstName Tests

        #region Valid Tests

        /// <summary>
        /// Tests the FirstName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithNullValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.FirstName = null;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithEmptyStringSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.FirstName = string.Empty;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithOneSpaceSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.FirstName = " ";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithOneCharacterSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.FirstName = "x";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestFirstNameWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.FirstName = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, user.FirstName.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion FirstName Tests
        
        #region LastName Tests

        #region Valid Tests

        /// <summary>
        /// Tests the LastName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithNullValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LastName = null;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithEmptyStringSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LastName = string.Empty;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithOneSpaceSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LastName = " ";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithOneCharacterSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LastName = "x";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLastNameWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.LastName = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, user.LastName.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion LastName Tests

        #region Email Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Email with null value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithNullValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.Email = null;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithEmptyStringSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.Email = string.Empty;
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with one space saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithOneSpaceSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.Email = " ";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with one character saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithOneCharacterSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.Email = "x";
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var user = GetValid(9);
            user.Email = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            UserRepository.DbContext.BeginTransaction();
            UserRepository.EnsurePersistent(user);
            UserRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, user.Email.Length);
            Assert.IsFalse(user.IsTransient());
            Assert.IsTrue(user.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests

        #region FullName Tests

        [TestMethod]
        public void TestFullNameReturnsExpectedResult1()
        {
            #region Arrange
            var record = new User();
            record.FirstName = "First";
            record.LastName = "Last";
            #endregion Arrange

            #region Act
            var result = record.FullName;
            #endregion Act

            #region Assert
            Assert.AreEqual("First Last", result);
            #endregion Assert		
        }

        #endregion FullName Tests

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
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("FirstName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("FullName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("LastName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("LoginId", "System.String", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(User));

        }

        #endregion Reflection of Database.	

    }
}