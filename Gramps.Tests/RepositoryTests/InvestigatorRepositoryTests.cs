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
    /// Entity Name:		Investigator
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class InvestigatorRepositoryTests : AbstractRepositoryTests<Investigator, int, InvestigatorMap>
    {
        /// <summary>
        /// Gets or sets the Investigator repository.
        /// </summary>
        /// <value>The Investigator repository.</value>
        public IRepository<Investigator> InvestigatorRepository { get; set; }
        public IRepository<CallForProposal> CallForProposalRepository { get; set; }
        public IRepository<Proposal> ProposalRepository { get; set; }
        public IRepository<User> UserRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="InvestigatorRepositoryTests"/> class.
        /// </summary>
        public InvestigatorRepositoryTests()
        {
            InvestigatorRepository = new Repository<Investigator>();
            CallForProposalRepository = new Repository<CallForProposal>();
            ProposalRepository = new Repository<Proposal>();
            UserRepository = new Repository<User>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Investigator GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Investigator(counter);
            rtValue.Proposal = ProposalRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Investigator> GetQuery(int numberAtEnd)
        {
            return InvestigatorRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Investigator entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Investigator entity, ARTAction action)
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
            ProposalRepository.DbContext.BeginTransaction();
            LoadUsers(1);
            var callForProposal = CreateValidEntities.CallForProposal(1);
            var owner = new Editor();
            owner.IsOwner = true;
            owner.User = UserRepository.Queryable.Where(a => a.Id == 1).Single();
            callForProposal.AddEditor(owner);
            for (int i = 0; i < 9; i++)
            {
                var editor = new Editor(string.Format("jason{0}@test.com", (i + 1)));
                callForProposal.AddEditor(editor);
            }
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();

            ProposalRepository.DbContext.BeginTransaction();
            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CallForProposalRepository.Queryable.Where(a => a.Id == 1).Single();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            InvestigatorRepository.DbContext.BeginTransaction();            
            LoadRecords(5);
            InvestigatorRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region IsPrimary Tests

        /// <summary>
        /// Tests the IsPrimary is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsPrimaryIsFalseSaves()
        {
            #region Arrange

            Investigator investigator = GetValid(9);
            investigator.IsPrimary = false;

            #endregion Arrange

            #region Act

            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(investigator.IsPrimary);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsPrimary is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsPrimaryIsTrueSaves()
        {
            #region Arrange

            var investigator = GetValid(9);
            investigator.IsPrimary = true;

            #endregion Arrange

            #region Act

            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(investigator.IsPrimary);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());

            #endregion Assert
        }

        #endregion IsPrimary Tests

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Name = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
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
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Name = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
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
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Name = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
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
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Name = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(200 + 1, investigator.Name.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 200");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
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
            var investigator = GetValid(9);
            investigator.Name = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Name = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, investigator.Name.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests
    
        #region Institution Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Institution with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInstitutionWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Institution = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Institution: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Institution with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInstitutionWithEmptyStringDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Institution = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Institution: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Institution with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInstitutionWithSpacesOnlyDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Institution = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Institution: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Institution with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestInstitutionWithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Institution = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(200 + 1, investigator.Institution.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Institution: length must be between 0 and 200");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Institution with one character saves.
        /// </summary>
        [TestMethod]
        public void TestInstitutionWithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Institution = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Institution with long value saves.
        /// </summary>
        [TestMethod]
        public void TestInstitutionWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Institution = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, investigator.Institution.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Institution Tests
 
        #region Address1 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Address1 with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddress1WithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Address1 = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Address1: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Address1 with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddress1WithEmptyStringDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Address1 = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Address1: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Address1 with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddress1WithSpacesOnlyDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Address1 = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Address1: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Address1 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddress1WithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Address1 = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(200 + 1, investigator.Address1.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Address1: length must be between 0 and 200");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Address1 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAddress1WithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address1 = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address1 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAddress1WithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address1 = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, investigator.Address1.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Address1 Tests

        #region Address2 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Address2 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddress2WithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Address2 = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(200 + 1, investigator.Address2.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Address2: length must be between 0 and 200");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Address2 with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithNullValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address2 = null;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address2 with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithEmptyStringSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address2 = string.Empty;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address2 with one space saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithOneSpaceSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address2 = " ";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address2 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address2 = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address2 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAddress2WithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address2 = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, investigator.Address2.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Address2 Tests
  
        #region Address3 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Address3 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAddress3WithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Address3 = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(200 + 1, investigator.Address3.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Address3: length must be between 0 and 200");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Address3 with null value saves.
        /// </summary>
        [TestMethod]
        public void TestAddress3WithNullValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address3 = null;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address3 with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestAddress3WithEmptyStringSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address3 = string.Empty;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address3 with one space saves.
        /// </summary>
        [TestMethod]
        public void TestAddress3WithOneSpaceSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address3 = " ";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address3 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestAddress3WithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address3 = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Address3 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestAddress3WithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Address3 = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, investigator.Address3.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Address3 Tests
    
        #region City Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the City with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.City = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("City: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithEmptyStringDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.City = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("City: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithSpacesOnlyDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.City = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("City: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.City = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(100 + 1, investigator.City.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("City: length must be between 0 and 100");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the City with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.City = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the City with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.City = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, investigator.City.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion City Tests

        #region State Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the State with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.State = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("State: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithEmptyStringDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.State = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("State: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithSpacesOnlyDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.State = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("State: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.State = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(2 + 1, investigator.State.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("State: length must be between 0 and 2");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the State with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.State = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the State with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.State = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, investigator.State.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion State Tests
   
        #region Zip Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Zip with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Zip = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Zip: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithEmptyStringDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Zip = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Zip: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithSpacesOnlyDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Zip = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Zip: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Zip = "x".RepeatTimes((11 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(11 + 1, investigator.Zip.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Zip: length must be between 0 and 11");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Zip with one character saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Zip = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Zip with long value saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Zip = "x".RepeatTimes(11);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(11, investigator.Zip.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Zip Tests

        #region Phone Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Phone with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Phone = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Phone: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Phone with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneWithEmptyStringDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Phone = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Phone: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Phone with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneWithSpacesOnlyDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Phone = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Phone: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Phone with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPhoneWithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Phone = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(50 + 1, investigator.Phone.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Phone: length must be between 0 and 50");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Phone with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneWithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Phone = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Phone with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPhoneWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Phone = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, investigator.Phone.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Phone Tests

        #region Email Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Email with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Email = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
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
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Email = string.Empty;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
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
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Email = " ";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: may not be null or empty", "Email: not a well-formed email address");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
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
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Email = "x".RepeatTimes((195 + 1)) + "@t.ca";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(200 + 1, investigator.Email.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: length must be between 0 and 200");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailWithPoorlyFormedEmailDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Email = "james@t@t.com";
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Email: not a well-formed email address");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Email with minimal characters saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithMinimalCharactersSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Email = "x@t.c";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Email with long value saves.
        /// </summary>
        [TestMethod]
        public void TestEmailWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Email = "x".RepeatTimes(195) + "@t.ca";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, investigator.Email.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Email Tests

        #region Notes Tests //This is not currently used, and may be removed


        #region Valid Tests

        /// <summary>
        /// Tests the Notes with null value saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithNullValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Notes = null;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithEmptyStringSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Notes = string.Empty;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with one space saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithOneSpaceSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Notes = " ";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Notes = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Notes with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNotesWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Notes = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, investigator.Notes.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Notes Tests

        #region Position Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Position with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestPositionWithTooLongValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Position = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                Assert.AreEqual(100 + 1, investigator.Position.Length);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Position: length must be between 0 and 100");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Position with null value saves.
        /// </summary>
        [TestMethod]
        public void TestPositionWithNullValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Position = null;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Position with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestPositionWithEmptyStringSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Position = string.Empty;
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Position with one space saves.
        /// </summary>
        [TestMethod]
        public void TestPositionWithOneSpaceSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Position = " ";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Position with one character saves.
        /// </summary>
        [TestMethod]
        public void TestPositionWithOneCharacterSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Position = "x";
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Position with long value saves.
        /// </summary>
        [TestMethod]
        public void TestPositionWithLongValueSaves()
        {
            #region Arrange
            var investigator = GetValid(9);
            investigator.Position = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            InvestigatorRepository.DbContext.BeginTransaction();
            InvestigatorRepository.EnsurePersistent(investigator);
            InvestigatorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, investigator.Position.Length);
            Assert.IsFalse(investigator.IsTransient());
            Assert.IsTrue(investigator.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Position Tests

        #region Proposal Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestProposalWithNullValueDoesNotSave()
        {
            Investigator investigator = null;
            try
            {
                #region Arrange
                investigator = GetValid(9);
                investigator.Proposal = null;
                #endregion Arrange

                #region Act
                InvestigatorRepository.DbContext.BeginTransaction();
                InvestigatorRepository.EnsurePersistent(investigator);
                InvestigatorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(investigator);
                var results = investigator.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Proposal: may not be null");
                Assert.IsTrue(investigator.IsTransient());
                Assert.IsFalse(investigator.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        #endregion Valid Tests

        #region Cascade Tests
        
        #endregion Cascade Tests
        #endregion Proposal Tests


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
            expectedFields.Add(new NameAndType("Address1", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Address2", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("Address3", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("City", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Email", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Institution", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsPrimary", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Phone", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)50)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Position", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]"
            }));
            expectedFields.Add(new NameAndType("State", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)2)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Zip", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)11)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Investigator));

        }

        #endregion Reflection of Database.	
		
		
    }
}