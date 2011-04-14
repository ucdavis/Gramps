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
    /// Entity Name:		Comment
    /// LookupFieldName:	Text yrjuy
    /// </summary>
    [TestClass]
    public class CommentRepositoryTests : AbstractRepositoryTests<Comment, int, CommentMap>
    {
        /// <summary>
        /// Gets or sets the Comment repository.
        /// </summary>
        /// <value>The Comment repository.</value>
        public IRepository<Comment> CommentRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentRepositoryTests"/> class.
        /// </summary>
        public CommentRepositoryTests()
        {
            CommentRepository = new Repository<Comment>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Comment GetValid(int? counter)
        {
            var rtvalue = CreateValidEntities.Comment(counter);
            rtvalue.Proposal = Repository.OfType<Proposal>().Queryable.First();
            var count = 6;
            if (counter != null && counter <= 5)
            {
                count = (int)counter;
            }
            rtvalue.Editor = Repository.OfType<CallForProposal>().Queryable.First().Editors.ElementAt(count-1);
            return rtvalue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Comment> GetQuery(int numberAtEnd)
        {
            return CommentRepository.Queryable.Where(a => a.Text.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Comment entity, int counter)
        {
            Assert.AreEqual("Text" + counter, entity.Text);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Comment entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Text);
                    break;
                case ARTAction.Restore:
                    entity.Text = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Text;
                    entity.Text = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            Repository.OfType<User>().DbContext.BeginTransaction();
            LoadUsers(3);
            Repository.OfType<User>().DbContext.CommitTransaction();

            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            var callForProposal = CreateValidEntities.CallForProposal(1);
            callForProposal.AddEditor(new Editor() { IsOwner = true, User = Repository.OfType<User>().Queryable.First() });
            callForProposal.AddEditor(CreateValidEntities.Editor(2));
            callForProposal.AddEditor(CreateValidEntities.Editor(3));
            callForProposal.AddEditor(CreateValidEntities.Editor(4));
            callForProposal.AddEditor(CreateValidEntities.Editor(5));
            callForProposal.AddEditor(CreateValidEntities.Editor(9));
            Repository.OfType<CallForProposal>().EnsurePersistent(callForProposal);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = Repository.OfType<CallForProposal>().Queryable.First();
            Repository.OfType<Proposal>().EnsurePersistent(proposal);
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            CommentRepository.DbContext.BeginTransaction();            
            LoadRecords(5);
            CommentRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Text Tests
        #region Invalid Tests

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Text with null value saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithNullValueSaves()
        {
            #region Arrange
            var comment = GetValid(9);
            comment.Text = null;
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(comment);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, comment.Text);
            Assert.IsFalse(comment.IsTransient());
            Assert.IsTrue(comment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithEmptyStringSaves()
        {
            #region Arrange
            var comment = GetValid(9);
            comment.Text = string.Empty;
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(comment);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(comment.IsTransient());
            Assert.IsTrue(comment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with one space saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithOneSpaceSaves()
        {
            #region Arrange
            var comment = GetValid(9);
            comment.Text = " ";
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(comment);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(comment.IsTransient());
            Assert.IsTrue(comment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with one character saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithOneCharacterSaves()
        {
            #region Arrange
            var comment = GetValid(9);
            comment.Text = "x";
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(comment);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(comment.IsTransient());
            Assert.IsTrue(comment.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Text with long value saves.
        /// </summary>
        [TestMethod]
        public void TestTextWithLongValueSaves()
        {
            #region Arrange
            var comment = GetValid(9);
            comment.Text = "x".RepeatTimes(1000);
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(comment);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(1000, comment.Text.Length);
            Assert.IsFalse(comment.IsTransient());
            Assert.IsTrue(comment.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Text Tests

        #region CreatedDate Tests

        /// <summary>
        /// Tests the CreatedDate with past date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedDateWithPastDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(-10);
            Comment record = GetValid(99);
            record.CreatedDate = compareDate;
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(record);
            CommentRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreatedDate);
            #endregion Assert		
        }

        /// <summary>
        /// Tests the CreatedDate with current date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedDateWithCurrentDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now;
            var record = GetValid(99);
            record.CreatedDate = compareDate;
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(record);
            CommentRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreatedDate);
            #endregion Assert
        }

        /// <summary>
        /// Tests the CreatedDate with future date date will save.
        /// </summary>
        [TestMethod]
        public void TestCreatedDateWithFutureDateDateWillSave()
        {
            #region Arrange
            var compareDate = DateTime.Now.AddDays(15);
            var record = GetValid(99);
            record.CreatedDate = compareDate;
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(record);
            CommentRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.AreEqual(compareDate, record.CreatedDate);
            #endregion Assert
        }
        #endregion CreatedDate Tests

        #region Proposal Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Proposal with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestProposalWithAValueOfNullDoesNotSave()
        {
            Comment comment = null;
            try
            {
                #region Arrange
                comment = GetValid(9);
                comment.Proposal = null;
                #endregion Arrange

                #region Act
                CommentRepository.DbContext.BeginTransaction();
                CommentRepository.EnsurePersistent(comment);
                CommentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(comment);
                Assert.AreEqual(comment.Proposal, null);
                var results = comment.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Proposal: may not be null");
                Assert.IsTrue(comment.IsTransient());
                Assert.IsFalse(comment.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestProposalWithANewValueDoesNotSave()
        {
            Comment comment = null;
            try
            {
                #region Arrange
                comment = GetValid(9);
                comment.Proposal = CreateValidEntities.Proposal(1);
                #endregion Arrange

                #region Act
                CommentRepository.DbContext.BeginTransaction();
                CommentRepository.EnsurePersistent(comment);
                CommentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(comment);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueGramps.Core.Domain.Comment.Proposal", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        /// <summary>
        /// Note this test saves a comment where the editor belongs to a different callforproposal compared to the proposal. 
        /// This isn't valid from a conseptual point of view
        /// </summary>
        [TestMethod]
        public void TestCommentWithValidProposalSaves()
        {
            #region Arrange
            Repository.OfType<CallForProposal>().DbContext.BeginTransaction();
            var callForProposal = CreateValidEntities.CallForProposal(9);
            callForProposal.AddEditor(new Editor() { IsOwner = true, User = Repository.OfType<User>().GetNullableById(3) });
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
            var record = GetValid(9);
            record.Proposal = proposal;
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.EnsurePersistent(record);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteCommentDoesNotCascadeToProposal()
        {
            #region Arrange
            var proposalCount = Repository.OfType<Proposal>().Queryable.Count();
            Assert.IsTrue(proposalCount > 0);
            var record = CommentRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.Remove(record);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(proposalCount, Repository.OfType<Proposal>().Queryable.Count());
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
            Comment comment = null;
            try
            {
                #region Arrange
                comment = GetValid(9);
                comment.Editor = null;
                #endregion Arrange

                #region Act
                CommentRepository.DbContext.BeginTransaction();
                CommentRepository.EnsurePersistent(comment);
                CommentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(comment);
                Assert.AreEqual(comment.Editor, null);
                var results = comment.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Editor: may not be null");
                Assert.IsTrue(comment.IsTransient());
                Assert.IsFalse(comment.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.PropertyValueException))]
        public void TestEditorWithANewValueDoesNotSave()
        {
            Comment comment = null;
            try
            {
                #region Arrange
                comment = GetValid(9);
                comment.Editor = CreateValidEntities.Editor(1);
                #endregion Arrange

                #region Act
                CommentRepository.DbContext.BeginTransaction();
                CommentRepository.EnsurePersistent(comment);
                CommentRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(comment);
                Assert.IsNotNull(ex);
                Assert.AreEqual("not-null property references a null or transient valueGramps.Core.Domain.Comment.Editor", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        //Done other places

        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteCommentDoesNotCascadeToEditor()
        {
            #region Arrange
            var editorCount = Repository.OfType<Editor>().Queryable.Count();
            Assert.IsTrue(editorCount > 0);
            var record = CommentRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            #endregion Arrange

            #region Act
            CommentRepository.DbContext.BeginTransaction();
            CommentRepository.Remove(record);
            CommentRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(editorCount, Repository.OfType<Editor>().Queryable.Count());
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion Editor Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorWithoutArgumentsSetsExpectedValues()
        {
            #region Arrange
            var record = new Comment();
            #endregion Arrange
           
            #region Assert
            Assert.AreEqual(DateTime.Now.Date, record.CreatedDate.Date);
            Assert.IsNull(record.Proposal);
            Assert.IsNull(record.Editor);
            Assert.IsNull(record.Text);
            #endregion Assert		
        }

        [TestMethod]
        public void TestConstructorWithArgumentsSetsExpectedValues()
        {
            #region Arrange
            var record = new Comment(CreateValidEntities.Proposal(7), CreateValidEntities.Editor(8), "Test");
            #endregion Arrange

            #region Assert
            Assert.AreEqual(DateTime.Now.Date, record.CreatedDate.Date);
            Assert.AreEqual("email7@testy.com", record.Proposal.Email);
            Assert.AreEqual("ReviewerName8", record.Editor.ReviewerName);
            Assert.AreEqual("Test", record.Text);
            #endregion Assert
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapComment()
        {
            #region Arrange
            var id = CommentRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var compareDate = new DateTime(2010, 12, 14);
            var editor = Repository.OfType<Editor>().GetNullableById(1);
            var proposal = Repository.OfType<Proposal>().GetNullableById(1);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Comment>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.CreatedDate, compareDate)
                .CheckProperty(c => c.Editor, editor)
                .CheckProperty(c => c.Proposal, proposal)
                .CheckProperty(c => c.Text, "Text")                
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
            expectedFields.Add(new NameAndType("CreatedDate", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Editor", "Gramps.Core.Domain.Editor", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Proposal", "Gramps.Core.Domain.Proposal", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Text", "System.String", new List<string>()));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Comment));

        }

        #endregion Reflection of Database.	
	
    }
}