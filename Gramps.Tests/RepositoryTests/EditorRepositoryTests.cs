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


        [TestMethod]
        public void TestUserTests()
        {
            #region Arrange

            Assert.Inconclusive("Write the user tests and others");

            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert		
        }
        
        //Field Tests
        //Constructor Tests
        //Fluent Mapping Tests
        
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
            expectedFields.Add(new NameAndType("CallForProposal", "", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsOwner", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("ReviewerEmail", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]",
                 ""
            }));
            expectedFields.Add(new NameAndType("ReviewerId", "System.Guid", new List<string>()));
            expectedFields.Add(new NameAndType("ReviewerName", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)200)]"
            }));
            expectedFields.Add(new NameAndType("Template", "", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Editor));

        }

        #endregion Reflection of Database.	
		
		
    }
}