using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region Editors Tests

        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEditorsWithAValueOfNullDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.Editors = null;
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(callForProposal);
                Assert.AreEqual(callForProposal.Editors, null);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Editors: may not be null");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEditorsWithAnInvalidValueDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                //callForProposal.Editors = new List<Editor>();
                var invalidEditor = new Editor();
                invalidEditor.ReviewerEmail = "x".RepeatTimes(200);
                callForProposal.AddEditor(invalidEditor);
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(callForProposal);
                Assert.AreEqual(2, callForProposal.Editors.Count);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("EditorsList: One or more invalid editors or reviewers detected");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCallForProposalWithNoOwnerDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.Editors = new List<Editor>();
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(callForProposal);
                Assert.AreEqual(0, callForProposal.Editors.Count);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Owner: Owner is required");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }

        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestCallForProposalWithNoReviewersSaves()
        {
            #region Arrange
            var record = GetValid(9);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Editors);
            Assert.AreEqual(1, record.Editors.Count);
            Assert.IsTrue(record.Editors[0].IsOwner);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCallForProposalsWithManyReviewersSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.IsActive = true;
            var reviewer1 = new Editor();
            reviewer1.ReviewerEmail = "test1@testy.com";
            var reviewer2 = new Editor();
            reviewer2.ReviewerEmail = "test2@testy.com";

            record.AddEditor(reviewer1);
            record.AddEditor(reviewer2);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Editors);
            Assert.AreEqual(3, record.Editors.Count);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCallForProposalsWithInvalidReviewerSavesIfNotActive()
        {
            #region Arrange
            var record = GetValid(9);
            record.IsActive = false;
            var reviewer1 = new Editor();
            reviewer1.ReviewerEmail = "x".RepeatTimes(200);
            var reviewer2 = new Editor();
            reviewer2.ReviewerEmail = "test2@testy.com";

            record.AddEditor(reviewer1);
            record.AddEditor(reviewer2);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Editors);
            Assert.AreEqual(3, record.Editors.Count);
            #endregion Assert
        }
        
        #endregion Valid Tests

        #region Cascade Tests
        [TestMethod]
        public void TestEditorsCascadesSave()
        {
            #region Arrange
            var editorRepository = new Repository<Editor>();
            var editorCount = editorRepository.Queryable.Count();
            var record = GetValid(99);
            record.AddEditor(CreateValidEntities.Editor(1));
            record.AddEditor(CreateValidEntities.Editor(2));
            record.AddEditor(CreateValidEntities.Editor(3));
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Emails);
            Assert.AreEqual(4, record.Editors.Count);
            Assert.AreEqual(4 + editorCount, editorRepository.Queryable.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestEditorsCascadesUpdate()
        {
            #region Arrange
            var editorRepository = new Repository<Editor>();
            var record = GetValid(99);
            record.AddEditor(CreateValidEntities.Editor(1));
            record.AddEditor(CreateValidEntities.Editor(2));
            record.AddEditor(CreateValidEntities.Editor(3));
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            var saveId = record.Editors[1].Id;
            #endregion Arrange

            #region Act
            record.Editors[1].ReviewerEmail = "updated@test.com";
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            NHibernateSessionManager.Instance.GetSession().Evict(record.Editors[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            #endregion Act

            #region Assert
            Assert.AreEqual("updated@test.com", editorRepository.Queryable.Where(a => a.Id == saveId).Single().ReviewerEmail);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditorsCascadesDelete1()
        {
            #region Arrange
            var editorRepository = new Repository<Editor>();
            var editorCount = editorRepository.Queryable.Count();
            var record = GetValid(99);
            record.AddEditor(CreateValidEntities.Editor(1));
            record.AddEditor(CreateValidEntities.Editor(2));
            record.AddEditor(CreateValidEntities.Editor(3));
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Arrange

            #region Act
            record.Editors.RemoveAt(2);
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Editors);
            Assert.AreEqual(3, record.Editors.Count);
            Assert.AreEqual(3 + editorCount, editorRepository.Queryable.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestEditorsCascadesDelete2()
        {
            #region Arrange
            var editorRepository = new Repository<Editor>();
            var editorCount = editorRepository.Queryable.Count();
            var record = GetValid(99);
            record.AddEditor(CreateValidEntities.Editor(1));
            record.AddEditor(CreateValidEntities.Editor(2));
            record.AddEditor(CreateValidEntities.Editor(3));
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            var saveId = record.Id;
            #endregion Arrange

            #region Act

            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.Remove(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNull(CallForProposalRepository.GetNullableById(saveId));
            Assert.AreEqual(editorCount, editorRepository.Queryable.Count());
            #endregion Assert
        }

        #endregion Cascade Tests
        #endregion Editors Tests
    }
}
