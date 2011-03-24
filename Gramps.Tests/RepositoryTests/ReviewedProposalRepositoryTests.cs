using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		ReviewedProposal
    /// LookupFieldName:	Id
    /// </summary>
    [TestClass]
    public class ReviewedProposalRepositoryTests : AbstractRepositoryTests<ReviewedProposal, int, ReviewedProposalMap>
    {
        /// <summary>
        /// Gets or sets the ReviewedProposal repository.
        /// </summary>
        /// <value>The ReviewedProposal repository.</value>
        public IRepository<ReviewedProposal> ReviewedProposalRepository { get; set; }
        public IRepository<CallForProposal> CallForProposalRepository { get; set; }
        public IRepository<Proposal> ProposalRepository { get; set; }
        public IRepository<User> UserRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewedProposalRepositoryTests"/> class.
        /// </summary>
        public ReviewedProposalRepositoryTests()
        {
            ReviewedProposalRepository = new Repository<ReviewedProposal>();
            CallForProposalRepository = new Repository<CallForProposal>();
            ProposalRepository = new Repository<Proposal>();
            UserRepository = new Repository<User>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override ReviewedProposal GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.ReviewedProposal(counter);
            rtValue.Proposal = ProposalRepository.Queryable.Where(a => a.Id == 1).Single();
            var count = 0;
            if(counter.HasValue)
            {
                count = counter.Value;
            }
            var callForProposal = CallForProposalRepository.Queryable.Where(a => a.Id == 1).Single();
            rtValue.Editor = callForProposal.Editors.ElementAt(count);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<ReviewedProposal> GetQuery(int numberAtEnd)
        {
            return ReviewedProposalRepository.Queryable.Where(a => a.Id == numberAtEnd);
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(ReviewedProposal entity, int counter)
        {
            Assert.AreEqual(new DateTime(2011, 01, 01).AddDays(counter), entity.LastViewedDate);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(ReviewedProposal entity, ARTAction action)
        {
            var updateValue = new DateTime(2011, 1, 30);
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.FirstViewedDate);
                    break;
                case ARTAction.Restore:
                    entity.FirstViewedDate = DateTimeRestoreValue;
                    break;
                case ARTAction.Update:
                    DateTimeRestoreValue = entity.FirstViewedDate;
                    entity.FirstViewedDate = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            CallForProposalRepository.DbContext.BeginTransaction();
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

            ReviewedProposalRepository.DbContext.BeginTransaction();            
            LoadRecords(5);
            ReviewedProposalRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region FirstViewedDate Tests

        /// <summary>
        /// Tests the FirstViewedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestFirstViewedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            ReviewedProposal record = GetValid(6);
            record.FirstViewedDate = compareDate;
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.EnsurePersistent(record);
            ReviewedProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.FirstViewedDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the FirstViewedDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestFirstViewedDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(6);
            record.FirstViewedDate = compareDate;
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.EnsurePersistent(record);
            ReviewedProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.FirstViewedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the FirstViewedDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestFirstViewedDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(6);
            record.FirstViewedDate = compareDate;
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.EnsurePersistent(record);
            ReviewedProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.FirstViewedDate);
            #endregion Assert
        }
        #endregion FirstViewedDate Tests

        #region LastViewedDate Tests

        /// <summary>
        /// Tests the LastViewedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestLastViewedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            ReviewedProposal record = GetValid(6);
            record.LastViewedDate = compareDate;
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.EnsurePersistent(record);
            ReviewedProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastViewedDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the LastViewedDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastViewedDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(6);
            record.LastViewedDate = compareDate;
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.EnsurePersistent(record);
            ReviewedProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastViewedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the LastViewedDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestLastViewedDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(6);
            record.LastViewedDate = compareDate;
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.EnsurePersistent(record);
            ReviewedProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.LastViewedDate);
            #endregion Assert
        }
        #endregion LastViewedDate Tests

        #region Proposal Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Proposal with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestProposalWithAValueOfNullDoesNotSave()
        {
            ReviewedProposal reviewedProposal = null;
            try
            {
                #region Arrange
                reviewedProposal = GetValid(5);
                reviewedProposal.Proposal = null;
                #endregion Arrange

                #region Act
                ReviewedProposalRepository.DbContext.BeginTransaction();
                ReviewedProposalRepository.EnsurePersistent(reviewedProposal);
                ReviewedProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(reviewedProposal);
                Assert.AreEqual(reviewedProposal.Proposal, null);
                var results = reviewedProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Proposal: may not be null");
                Assert.IsTrue(reviewedProposal.IsTransient());
                Assert.IsFalse(reviewedProposal.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestProposalWithANewValueDoesNotSave()
        {
            ReviewedProposal reviewedProposal = null;
            try
            {
                #region Arrange
                reviewedProposal = GetValid(5);
                reviewedProposal.Proposal = CreateValidEntities.Proposal(1);
                #endregion Arrange

                #region Act
                ReviewedProposalRepository.DbContext.BeginTransaction();
                ReviewedProposalRepository.EnsurePersistent(reviewedProposal);
                ReviewedProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(reviewedProposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueGramps.Core.Domain.ReviewedProposal.Proposal", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        /// <summary>
        /// FYI this test saves a comment where the editor belongs to a different callforproposal compared to the proposal. 
        /// This isn't valid from a conseptual point of view
        /// </summary>
        [TestMethod]
        public void TestReviewedProposalWithValidProposalSaves()
        {
            #region Arrange
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            var callForProposal = CreateValidEntities.CallForProposal(9);
            callForProposal.AddEditor(new Editor { IsOwner = true, User = Repository.OfType<User>().GetNullableById(1) });
            callForProposal.AddEditor(CreateValidEntities.Editor(2));
            callForProposal.AddEditor(CreateValidEntities.Editor(3));
            callForProposal.AddEditor(CreateValidEntities.Editor(4));
            callForProposal.AddEditor(CreateValidEntities.Editor(5));
            callForProposal.AddEditor(CreateValidEntities.Editor(9));
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            var proposal = CreateValidEntities.Proposal(9);
            proposal.CallForProposal = callForProposal;
            Repository.OfType<Proposal>().EnsurePersistent(proposal);
            Repository.OfType<Proposal>().DbContext.CommitTransaction();
            var record = GetValid(6);
            record.Proposal = proposal;
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.EnsurePersistent(record);
            ReviewedProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteReviewedProposalDoesNotCascadeToProposal()
        {
            #region Arrange
            var proposalCount = Repository.OfType<Proposal>().Queryable.Count();
            Assert.IsTrue(proposalCount > 0);
            var record = ReviewedProposalRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.Remove(record);
            ReviewedProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(proposalCount, Repository.OfType<Proposal>().Queryable.Count());
            Assert.IsNull(ReviewedProposalRepository.GetNullableById(2));
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion Proposal Tests

        #region Editor Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Editor with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEditorWithAValueOfNullDoesNotSave()
        {
            ReviewedProposal reviewedProposal = null;
            try
            {
                #region Arrange
                reviewedProposal = GetValid(5);
                reviewedProposal.Editor = null;
                #endregion Arrange

                #region Act
                ReviewedProposalRepository.DbContext.BeginTransaction();
                ReviewedProposalRepository.EnsurePersistent(reviewedProposal);
                ReviewedProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(reviewedProposal);
                Assert.AreEqual(reviewedProposal.Editor, null);
                var results = reviewedProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Editor: may not be null");
                Assert.IsTrue(reviewedProposal.IsTransient());
                Assert.IsFalse(reviewedProposal.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestEditorWithANewValueDoesNotSave()
        {
            ReviewedProposal reviewedProposal = null;
            try
            {
                #region Arrange
                reviewedProposal = GetValid(5);
                reviewedProposal.Editor = CreateValidEntities.Editor(1);
                #endregion Arrange

                #region Act
                ReviewedProposalRepository.DbContext.BeginTransaction();
                ReviewedProposalRepository.EnsurePersistent(reviewedProposal);
                ReviewedProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(reviewedProposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueGramps.Core.Domain.ReviewedProposal.Editor", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteReviewedProposalDoesNotCascadeToEditor()
        {
            #region Arrange
            var editorCount = Repository.OfType<Editor>().Queryable.Count();
            Assert.IsTrue(editorCount > 0);
            var record = ReviewedProposalRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            ReviewedProposalRepository.DbContext.BeginTransaction();
            ReviewedProposalRepository.Remove(record);
            ReviewedProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(editorCount, Repository.OfType<Editor>().Queryable.Count());
            Assert.IsNull(ReviewedProposalRepository.GetNullableById(2));
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion Editor Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorSetsExpectedvalues()
        {
            #region Arrange
            var record = new ReviewedProposal();
            #endregion Arrange

            #region Act/Assert
            Assert.AreEqual(DateTime.Now.Date, record.LastViewedDate.Date);
            Assert.AreEqual(DateTime.Now.Date, record.FirstViewedDate.Date);
            #endregion Act/Assert
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapReviewedProposal()
        {
            #region Arrange
            var id = ReviewedProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var compareDate1 = new DateTime(2010, 12, 14);
            var compareDate2 = new DateTime(2010, 12, 15);
            var editor = Repository.OfType<Editor>().GetNullableById(1);
            var proposal = Repository.OfType<Proposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<ReviewedProposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.FirstViewedDate, compareDate1)
                .CheckReference(c => c.Editor, editor)
                .CheckReference(c => c.Proposal, proposal)
                .CheckProperty(c => c.LastViewedDate, compareDate2)
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
            expectedFields.Add(new NameAndType("Editor", "Gramps.Core.Domain.Editor", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("FirstViewedDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("LastViewedDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Proposal", "Gramps.Core.Domain.Proposal", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(ReviewedProposal));

        }

        #endregion Reflection of Database.			
    }
}