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
    /// Entity Name:		Editor
    /// LookupFieldName:	ReviewerName
    /// Had to fiddle with this a little because the 
    /// CallForProposal is the one that actually does the saving, 
    /// so the counts and comparisons were out a little.
    /// </summary>
    [TestClass]
    public class EditorRepositoryTests : AbstractRepositoryTests<Editor, int, EditorMap>
    {
        /// <summary>
        /// Gets or sets the Editor repository.
        /// </summary>
        /// <value>The Editor repository.</value>
        public IRepository<Editor> EditorRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorRepositoryTests"/> class.
        /// </summary>
        public EditorRepositoryTests()
        {
            EditorRepository = new Repository<Editor>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Editor GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Editor(counter);
            var count = 9;
            if (counter != null)
            {
                count = (int) counter;
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
        protected override IQueryable<Editor> GetQuery(int numberAtEnd)
        {
            return EditorRepository.Queryable.Where(a => a.ReviewerName.EndsWith((numberAtEnd-1).ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Editor entity, int counter)
        {
            if(entity.User == null)
            {
                Assert.AreEqual("ReviewerName" + (counter -1), entity.ReviewerName);
            }
            else
            {
                Assert.AreEqual(entity.Id, counter);
            }
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Editor entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.ReviewerName);
                    break;
                case ARTAction.Restore:
                    entity.ReviewerName = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.ReviewerName;
                    entity.ReviewerName = updateValue;
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
            LoadCallForProposals(1);
            LoadTemplates(1);
            Repository.OfType<CallForProposal>().DbContext.CommitTransaction();

            EditorRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            EditorRepository.DbContext.CommitTransaction();
            EntriesAdded = 6;
        }

        #endregion Init and Overrides	

        #region IsOwner Tests

        /// <summary>
        /// Tests the IsOwner is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsOwnerIsFalseSaves()
        {
            #region Arrange

            Editor editor = GetValid(9);
            editor.IsOwner = false;

            #endregion Arrange

            #region Act

            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(editor.IsOwner);
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsOwner is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsOwnerIsTrueSaves()
        {
            #region Arrange

            var editor = GetValid(9);
            editor.IsOwner = true;

            #endregion Arrange

            #region Act

            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(editor.IsOwner);
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());

            #endregion Assert
        }

        #endregion IsOwner Tests

        #region ReviewerEmail Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ReviewerEmail with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReviewerEmailWithTooLongValueDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.ReviewerEmail = "x".RepeatTimes((94 + 1)) + "@t.com";
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                Assert.AreEqual(100 + 1, editor.ReviewerEmail.Length);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReviewerEmail: length must be between 0 and 100");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReviewerEmailWithOneSpaceDoesNotSave1()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.ReviewerEmail = " ";
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                Assert.AreEqual(1, editor.ReviewerEmail.Length);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReviewerEmailRequired: Reviewer must have an email", "ReviewerEmail: not a well-formed email address");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReviewerEmailWithOneSpaceDoesNotSave2()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.ReviewerEmail = " ";
                editor.User = Repository.OfType<User>().GetNullableById(1);
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                Assert.AreEqual(1, editor.ReviewerEmail.Length);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReviewerEmail: not a well-formed email address");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ReviewerEmail with null value saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerEmailWithNullValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerEmail = null;
            editor.User = Repository.OfType<User>().GetNullableById(1);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReviewerEmail with empty strinf saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerEmailWithEmptyStringSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerEmail = string.Empty;
            editor.User = Repository.OfType<User>().GetNullableById(1);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReviewerEmail with 4 characters saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerEmailWith4CharactersSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerEmail = "x@x.x";
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReviewerEmail with long value saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerEmailWithLongValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerEmail = "x".RepeatTimes((93 + 1)) + "@t.com";
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, editor.ReviewerEmail.Length);
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ReviewerEmail Tests
        
        #region ReviewerName Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the ReviewerName with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReviewerNameWithTooLongValueDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.ReviewerName = "x".RepeatTimes((200 + 1));
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                Assert.AreEqual(200 + 1, editor.ReviewerName.Length);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReviewerName: length must be between 0 and 200");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the ReviewerName with null value saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerNameWithNullValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerName = null;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReviewerName with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerNameWithEmptyStringSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerName = string.Empty;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReviewerName with one space saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerNameWithOneSpaceSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerName = " ";
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReviewerName with one character saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerNameWithOneCharacterSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerName = "x";
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the ReviewerName with long value saves.
        /// </summary>
        [TestMethod]
        public void TestReviewerNameWithLongValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.ReviewerName = "x".RepeatTimes(200);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(200, editor.ReviewerName.Length);
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion ReviewerName Tests

        #region ReviewerId Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReviewerIdWithEmptyGuidDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.ReviewerId = Guid.Empty;
                editor.User = null;
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                Assert.AreEqual(Guid.Empty, editor.ReviewerId);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReviewerIdentifier: Reviewer must have a unique identifier");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests
        
        [TestMethod]
        public void TestReviewerIdWithNonEmptyGuidSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ReviewerId = SpecificGuid.GetGuid(9);
            record.User = null;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(SpecificGuid.GetGuid(9), record.ReviewerId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        [TestMethod]
        public void TestReviewerIdWithEmptyGuidAndUserSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ReviewerId = Guid.Empty;
            record.User = Repository.OfType<User>().GetNullableById(1);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(Guid.Empty, record.ReviewerId);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #endregion ReviewerId Tests

        #region Template Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTemplateWithANewValueDoesNotSave()
        {
            Editor record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = CreateValidEntities.Template(9);
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(record);
                EditorRepository.DbContext.CommitTransaction();
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

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateAndCallForProposalWithNullValueDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.Template = null;
                editor.CallForProposal = null;
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTemplateAndCallForProposalWithNeitherNullDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.Template = Repository.OfType<Template>().GetNullableById(1);
                editor.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Assert.IsNotNull(editor.CallForProposal);
                Assert.IsNotNull(editor.Template);
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests
        [TestMethod]
        public void TestTemplateWithNullValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.Template = null;
            editor.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            Assert.IsNotNull(editor.CallForProposal);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestTemplateWithNonNullValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.Template = Repository.OfType<Template>().GetNullableById(1);
            editor.CallForProposal = null;            
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(editor.Template);
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteDoesNotCascadeToTemplate()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.Template = Repository.OfType<Template>().GetNullableById(1);
            editor.CallForProposal = null;
    
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();

            var templateCount = Repository.OfType<Template>().Queryable.Count();
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.Remove(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(templateCount, Repository.OfType<Template>().Queryable.Count());
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion Template Tests

        #region CallForProposal Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCallForProposalWithANewValueDoesNotSave()
        {
            Editor record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Template = null;
                record.CallForProposal = CreateValidEntities.CallForProposal(9);
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(record);
                EditorRepository.DbContext.CommitTransaction();
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
        public void TestCallForProposalWithNullValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.Template = Repository.OfType<Template>().GetNullableById(1);
            editor.CallForProposal = null;
            Assert.IsNotNull(editor.Template);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }

        [TestMethod]
        public void TestCallForProposalWithNonNullValueSaves()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            editor.Template = null;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(editor.CallForProposal);
            Assert.IsFalse(editor.IsTransient());
            Assert.IsTrue(editor.IsValid());
            #endregion Assert
        }
        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        public void TestDeleteDoesNotCascadeToCallForProposals()
        {
            #region Arrange
            var editor = GetValid(9);
            editor.Template = null;
            editor.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);

            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(editor);
            EditorRepository.DbContext.CommitTransaction();

            var callForProposalCount = Repository.OfType<CallForProposal>().Queryable.Count();
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.Remove(editor);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(callForProposalCount, Repository.OfType<CallForProposal>().Queryable.Count());
            #endregion Assert
        }
        #endregion Cascade Tests
        #endregion CallForProposals Tests

        #region User Tests

        #region Invalid Tests
        /// <summary>
        /// Tests the User with A value of new User() does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestUserWithAValueOfNewDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.User = CreateValidEntities.User(9);
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(editor);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.User, Entity: Gramps.Core.Domain.User", ex.Message);
                throw;
            }	
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestUserWithNullValueSavesIfReviewerIsSet()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = null;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }


        [TestMethod]
        public void TestUserWithExistingValueSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = Repository.OfType<User>().GetNullableById(1);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }
        #endregion Valid Tests

        #region Cascade Tests

        [TestMethod]
        public void TestDeleteEditorDoesNotCascadeToUser()
        {
            #region Arrange
            var record = GetValid(9);
            record.User = Repository.OfType<User>().GetNullableById(1);
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            Assert.IsNotNull(record.User);
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.Remove(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsNotNull(Repository.OfType<User>().GetNullableById(1));
            Assert.IsNull(EditorRepository.GetNullableById(saveId));
            #endregion Assert		
        }
        #endregion Cascade Tests
        #endregion User Tests

        #region Comments Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the Comments with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCommentsWithAValueOfNullDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.Comments = null;
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                Assert.AreEqual(editor.Comments, null);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Comments: may not be null");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestCommentsWithANewValueDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.Comments.Add(CreateValidEntities.Comment(9));
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(editor);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Comment, Entity: Gramps.Core.Domain.Comment", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestCommentsWithEmptyListSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Comments = new List<Comment>();
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.Comments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert			
        }


        [TestMethod]
        public void TestCommentsWithPopulatedValuesSaves()
        {
            #region Arrange
            var record = EditorRepository.GetNullableById(1);
            Assert.IsNotNull(record);
            
            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals.Add(CreateValidEntities.Proposal(3));
            
            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            foreach (var proposal in proposals)
            {
                proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
            }
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments.Add(CreateValidEntities.Comment(2));
            comments.Add(CreateValidEntities.Comment(3));
            Repository.OfType<Comment>().DbContext.BeginTransaction();
            for (int i = 0; i < 3; i++)
            {
                comments[i].Proposal = proposals[i];
                comments[i].Editor = record;
                Repository.OfType<Comment>().EnsurePersistent(comments[i]);
                record.Comments.Add(comments[i]);
            }
            Repository.OfType<Comment>().DbContext.CommitTransaction();

            Assert.AreEqual(3, record.Comments.Count);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Comments.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }


        [TestMethod]
        public void TestCanGetRelatedComments()
        {
            #region Arrange
            var record = EditorRepository.GetNullableById(1);
            Assert.IsNotNull(record);

            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals.Add(CreateValidEntities.Proposal(3));

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            foreach (var proposal in proposals)
            {
                proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
            }
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments.Add(CreateValidEntities.Comment(2));
            comments.Add(CreateValidEntities.Comment(3));
            comments.Add(CreateValidEntities.Comment(4));
            Repository.OfType<Comment>().DbContext.BeginTransaction();
            for (int i = 0; i < 4; i++)
            {
                if (i == 3)
                {
                    comments[i].Proposal = proposals[1];
                    comments[i].Editor = EditorRepository.GetNullableById(2);
                }
                else
                {
                    comments[i].Proposal = proposals[i];
                    comments[i].Editor = record;
                }
                Repository.OfType<Comment>().EnsurePersistent(comments[i]);
                record.Comments.Add(comments[i]);
            }
            Repository.OfType<Comment>().DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(comments[0]);
            NHibernateSessionManager.Instance.GetSession().Evict(comments[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(comments[2]);
            NHibernateSessionManager.Instance.GetSession().Evict(comments[3]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            #endregion Arrange

            #region Act
            var record2 = EditorRepository.GetNullableById(1);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<Comment>().Queryable.Count());
            Assert.AreEqual(3, record2.Comments.Count);
            #endregion Assert		
        }

        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestDeleteEditorWithCommentsDoesNotSaveOrCascade()
        {
            Editor record = null;
            #region Arrange
            try
            {               
                record = EditorRepository.GetNullableById(3);
                Assert.IsNotNull(record);

                var proposals = new List<Proposal>();
                proposals.Add(CreateValidEntities.Proposal(1));
                proposals.Add(CreateValidEntities.Proposal(2));
                proposals.Add(CreateValidEntities.Proposal(3));

                Repository.OfType<Proposal>().DbContext.BeginTransaction();
                foreach (var proposal in proposals)
                {
                    proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                    Repository.OfType<Proposal>().EnsurePersistent(proposal);
                }
                Repository.OfType<Proposal>().DbContext.CommitTransaction();

                var comments = new List<Comment>();
                comments.Add(CreateValidEntities.Comment(1));
                comments.Add(CreateValidEntities.Comment(2));
                comments.Add(CreateValidEntities.Comment(3));
                comments.Add(CreateValidEntities.Comment(4));
                Repository.OfType<Comment>().DbContext.BeginTransaction();
                for (int i = 0; i < 4; i++)
                {
                    if (i == 3)
                    {
                        comments[i].Proposal = proposals[1];
                        comments[i].Editor = EditorRepository.GetNullableById(1);
                    }
                    else
                    {
                        comments[i].Proposal = proposals[i];
                        comments[i].Editor = record;
                    }
                    Repository.OfType<Comment>().EnsurePersistent(comments[i]);
                    record.Comments.Add(comments[i]);
                }
                Repository.OfType<Comment>().DbContext.CommitTransaction();
              
            }
            catch (Exception)
            {
                
                Assert.Fail("An exception happened while setting up the data");
            }
            #endregion Arrange

            try
            {
                #region Arrange
                NHibernateSessionManager.Instance.GetSession().Evict(record);  
                record = EditorRepository.GetNullableById(3);
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.Remove(record);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("could not delete collection: [Gramps.Core.Domain.Editor.Comments#3][SQL: UPDATE Comments SET EditorID = null WHERE EditorID = @p0]", ex.Message);
                throw;
            }	
        }
        
        #endregion Cascade Tests
        #endregion Comments Tests

        #region ReviewedProposal Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the ReviewedProposal with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestReviewedProposalsWithAValueOfNullDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.ReviewedProposals = null;
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(editor);
                Assert.AreEqual(editor.ReviewedProposals, null);
                var results = editor.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReviewedProposals: may not be null");
                Assert.IsTrue(editor.IsTransient());
                Assert.IsFalse(editor.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestReviewedProposalsWithANewValueDoesNotSave()
        {
            Editor editor = null;
            try
            {
                #region Arrange
                editor = GetValid(9);
                editor.ReviewedProposals.Add(CreateValidEntities.ReviewedProposal(9));
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.EnsurePersistent(editor);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(editor);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.ReviewedProposal, Entity: Gramps.Core.Domain.ReviewedProposal", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestReviewedProposalsWithEmptyListSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.ReviewedProposals = new List<ReviewedProposal>();
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(0, record.ReviewedProposals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }


        [TestMethod]
        public void TestReviewedProposalsWithPopulatedValuesSaves()
        {
            #region Arrange
            var record = EditorRepository.GetNullableById(1);
            Assert.IsNotNull(record);

            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals.Add(CreateValidEntities.Proposal(3));

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            foreach (var proposal in proposals)
            {
                proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
            }
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            var reviewedProposals = new List<ReviewedProposal>();
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(1));
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(2));
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(3));
            Repository.OfType<ReviewedProposal>().DbContext.BeginTransaction();
            for (int i = 0; i < 3; i++)
            {
                reviewedProposals[i].Proposal = proposals[i];
                reviewedProposals[i].Editor = record;
                Repository.OfType<ReviewedProposal>().EnsurePersistent(reviewedProposals[i]);
                record.ReviewedProposals.Add(reviewedProposals[i]);
            }
            Repository.OfType<ReviewedProposal>().DbContext.CommitTransaction();

            Assert.AreEqual(3, record.ReviewedProposals.Count);
            #endregion Arrange

            #region Act
            EditorRepository.DbContext.BeginTransaction();
            EditorRepository.EnsurePersistent(record);
            EditorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.ReviewedProposals.Count);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert
        }


        [TestMethod]
        public void TestCanGetRelatedReviewedProposals()
        {
            #region Arrange
            var record = EditorRepository.GetNullableById(1);
            Assert.IsNotNull(record);

            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals.Add(CreateValidEntities.Proposal(3));

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            foreach (var proposal in proposals)
            {
                proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
            }
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            var reviewedProposals = new List<ReviewedProposal>();
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(1));
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(2));
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(3));
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(4));
            Repository.OfType<ReviewedProposal>().DbContext.BeginTransaction();
            for (int i = 0; i < 4; i++)
            {
                if (i == 3)
                {
                    reviewedProposals[i].Proposal = proposals[1];
                    reviewedProposals[i].Editor = EditorRepository.GetNullableById(2);
                }
                else
                {
                    reviewedProposals[i].Proposal = proposals[i];
                    reviewedProposals[i].Editor = record;
                }
                Repository.OfType<ReviewedProposal>().EnsurePersistent(reviewedProposals[i]);
                record.ReviewedProposals.Add(reviewedProposals[i]);
            }
            Repository.OfType<ReviewedProposal>().DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(reviewedProposals[0]);
            NHibernateSessionManager.Instance.GetSession().Evict(reviewedProposals[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(reviewedProposals[2]);
            NHibernateSessionManager.Instance.GetSession().Evict(reviewedProposals[3]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            #endregion Arrange

            #region Act
            var record2 = EditorRepository.GetNullableById(1);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, Repository.OfType<ReviewedProposal>().Queryable.Count());
            Assert.AreEqual(3, record2.ReviewedProposals.Count);
            #endregion Assert
        }

        #endregion Valid Tests
        #region Cascade Tests

        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestDeleteEditorWithReviewedProposalsDoesNotSaveOrCascade()
        {
            Editor record = null;
            #region Arrange
            try
            {
                record = EditorRepository.GetNullableById(3);
                Assert.IsNotNull(record);

                var proposals = new List<Proposal>();
                proposals.Add(CreateValidEntities.Proposal(1));
                proposals.Add(CreateValidEntities.Proposal(2));
                proposals.Add(CreateValidEntities.Proposal(3));

                Repository.OfType<Proposal>().DbContext.BeginTransaction();
                foreach (var proposal in proposals)
                {
                    proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                    Repository.OfType<Proposal>().EnsurePersistent(proposal);
                }
                Repository.OfType<Proposal>().DbContext.CommitTransaction();

                var reviewedProposals = new List<ReviewedProposal>();
                reviewedProposals.Add(CreateValidEntities.ReviewedProposal(1));
                reviewedProposals.Add(CreateValidEntities.ReviewedProposal(2));
                reviewedProposals.Add(CreateValidEntities.ReviewedProposal(3));
                reviewedProposals.Add(CreateValidEntities.ReviewedProposal(4));
                Repository.OfType<ReviewedProposal>().DbContext.BeginTransaction();
                for (int i = 0; i < 4; i++)
                {
                    if (i == 3)
                    {
                        reviewedProposals[i].Proposal = proposals[1];
                        reviewedProposals[i].Editor = EditorRepository.GetNullableById(1);
                    }
                    else
                    {
                        reviewedProposals[i].Proposal = proposals[i];
                        reviewedProposals[i].Editor = record;
                    }
                    Repository.OfType<ReviewedProposal>().EnsurePersistent(reviewedProposals[i]);
                    record.ReviewedProposals.Add(reviewedProposals[i]);
                }
                Repository.OfType<ReviewedProposal>().DbContext.CommitTransaction();

            }
            catch (Exception)
            {

                Assert.Fail("An exception happened while setting up the data");
            }
            #endregion Arrange

            try
            {
                #region Arrange
                NHibernateSessionManager.Instance.GetSession().Evict(record);
                record = EditorRepository.GetNullableById(3);
                #endregion Arrange

                #region Act
                EditorRepository.DbContext.BeginTransaction();
                EditorRepository.Remove(record);
                EditorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
                Assert.AreEqual("could not delete collection: [Gramps.Core.Domain.Editor.ReviewedProposals#3][SQL: UPDATE ReviewedProposals SET EditorID = null WHERE EditorID = @p0]", ex.Message);
                throw;
            }
        }

        #endregion Cascade Tests
        #endregion ReviewedProposal Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapEditor1()
        {
            #region Arrange
            var id = EditorRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Editor>(session)
                .CheckProperty(c => c.Id, id)

                .CheckProperty(c => c.IsOwner, true)
                .CheckProperty(c => c.ReviewerEmail, "test@testy.com")
                .CheckProperty(c => c.ReviewerId, Guid.NewGuid())
                .CheckProperty(c => c.ReviewerName, "Name")
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEditor2()
        {
            #region Arrange
            var id = EditorRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Editor>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.IsOwner, false)
                .CheckProperty(c => c.ReviewerEmail, "test@testy.com")
                .CheckProperty(c => c.ReviewerId, Guid.NewGuid())
                .CheckProperty(c => c.ReviewerName, "Name")
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEditor3()
        {
            #region Arrange
            var id = EditorRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Editor>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.IsOwner, true)
                .CheckProperty(c => c.ReviewerEmail, "test@testy.com")
                .CheckProperty(c => c.ReviewerId, Guid.NewGuid())
                .CheckProperty(c => c.ReviewerName, "Name")
                .CheckProperty(c => c.Template, null)
                .CheckProperty(c => c.User, null)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEditor4()
        {
            #region Arrange
            var id = EditorRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
            var template = Repository.OfType<Template>().GetNullableById(1);
            var user = Repository.OfType<User>().GetNullableById(1);

            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Editor>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.CallForProposal, callForProposal)
                .CheckProperty(c => c.IsOwner, true)
                .CheckProperty(c => c.ReviewerEmail, "test@testy.com")
                .CheckProperty(c => c.ReviewerId, Guid.NewGuid())
                .CheckProperty(c => c.ReviewerName, "Name")
                .CheckReference(c => c.Template, template)
                .CheckReference(c => c.User, user)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEditor5()
        {
            #region Arrange
            var id = EditorRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var record = new Editor();
            record.SetIdTo(id);

            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals.Add(CreateValidEntities.Proposal(3));

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            foreach (var proposal in proposals)
            {
                proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
            }
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments.Add(CreateValidEntities.Comment(2));
            comments.Add(CreateValidEntities.Comment(3));
            Repository.OfType<Comment>().DbContext.BeginTransaction();
            for (int i = 0; i < 3; i++)
            {
                comments[i].Proposal = proposals[i];
                comments[i].Editor = record;
                Repository.OfType<Comment>().EnsurePersistent(comments[i]);
                record.Comments.Add(comments[i]);
            }
            Repository.OfType<Comment>().DbContext.CommitTransaction();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Editor>(session, new EditorEqualityComparer())
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Comments, comments)
                .CheckProperty(c => c.IsOwner, true)
                .CheckProperty(c => c.ReviewerEmail, "test@testy.com")
                .CheckProperty(c => c.ReviewerId, Guid.NewGuid())
                .CheckProperty(c => c.ReviewerName, "Name")
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapEditor6()
        {
            #region Arrange
            var id = EditorRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var record = new Editor();
            record.SetIdTo(id);

            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals.Add(CreateValidEntities.Proposal(3));

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            foreach (var proposal in proposals)
            {
                proposal.CallForProposal = Repository.OfType<CallForProposal>().GetNullableById(1);
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
            }
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            var reviewedProposal = new List<ReviewedProposal>();
            reviewedProposal.Add(CreateValidEntities.ReviewedProposal(1));
            reviewedProposal.Add(CreateValidEntities.ReviewedProposal(2));
            reviewedProposal.Add(CreateValidEntities.ReviewedProposal(3));
            Repository.OfType<ReviewedProposal>().DbContext.BeginTransaction();
            for (int i = 0; i < 3; i++)
            {
                reviewedProposal[i].Proposal = proposals[i];
                reviewedProposal[i].Editor = record;
                Repository.OfType<ReviewedProposal>().EnsurePersistent(reviewedProposal[i]);
                record.ReviewedProposals.Add(reviewedProposal[i]);
            }
            Repository.OfType<ReviewedProposal>().DbContext.CommitTransaction();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Editor>(session, new EditorEqualityComparer())
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.ReviewedProposals, reviewedProposal)
                .CheckProperty(c => c.IsOwner, true)
                .CheckProperty(c => c.ReviewerEmail, "test@testy.com")
                .CheckProperty(c => c.ReviewerId, Guid.NewGuid())
                .CheckProperty(c => c.ReviewerName, "Name")
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
            expectedFields.Add(new NameAndType("Comments", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Comment]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsOwner", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("RelatedTable", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Must be related to Template or CallForProposal not both.\")]"
            }));
            expectedFields.Add(new NameAndType("ReviewedProposals", "System.Collections.Generic.IList`1[Gramps.Core.Domain.ReviewedProposal]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("ReviewerEmail", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.EmailAttribute()]",
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]"
            }));
            expectedFields.Add(new NameAndType("ReviewerEmailRequired", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Reviewer must have an email\")]"
            }));
            expectedFields.Add(new NameAndType("ReviewerId", "System.Guid", new List<string>()));
            expectedFields.Add(new NameAndType("ReviewerIdentifier", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Reviewer must have a unique identifier\")]"
            }));
            expectedFields.Add(new NameAndType("ReviewerName", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("Template", "Gramps.Core.Domain.Template", new List<string>()));
            expectedFields.Add(new NameAndType("User", "Gramps.Core.Domain.User", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Editor));

        }

        #endregion Reflection of Database.	
		
        public class EditorEqualityComparer : IEqualityComparer
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

                if (x is IList<Comment> && y is IList<Comment>)
                {
                    var xVal = (IList<Comment>)x;
                    var yVal = (IList<Comment>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Text, yVal[i].Text);
                    }
                    return true;
                }

                if (x is IList<ReviewedProposal> && y is IList<ReviewedProposal>)
                {
                    var xVal = (IList<ReviewedProposal>)x;
                    var yVal = (IList<ReviewedProposal>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].LastViewedDate.Date, yVal[i].LastViewedDate.Date);
                        Assert.AreEqual(xVal[i].FirstViewedDate.Date, yVal[i].FirstViewedDate.Date);
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