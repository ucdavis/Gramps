using System;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region TemplateGeneratedFrom Tests
        #region Invalid Tests
        /// <summary>
        /// Tests the TemplateGeneratedFrom with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTemplateGeneratedFromWithNewValueDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.TemplateGeneratedFrom = new Template();
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(callForProposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Template, Entity: Gramps.Core.Domain.Template", ex.Message);
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        [TestMethod]
        public void TestTemplateGeneratedFromWithNullValueSaves()
        {
            #region Arrange
            var record = GetValid(99);
            record.TemplateGeneratedFrom = null;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNull(record.TemplateGeneratedFrom);
            #endregion Assert
        }

        [TestMethod]
        public void TestTemplateGeneratedFromWithExistingTemplateSaves()
        {
            #region Arrange
            var templateRepository = new Repository<Template>();
            templateRepository.DbContext.BeginTransaction();
            LoadTemplates(3);
            templateRepository.DbContext.CommitTransaction();
            var record = GetValid(99);
            record.TemplateGeneratedFrom = templateRepository.GetNullableById(1);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.TemplateGeneratedFrom);
            #endregion Assert
        }

        #endregion Valid Tests

        #region Cascade Tests
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestTemplateGeneratedFromWithUnsavedValidValueDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.TemplateGeneratedFrom = CreateValidEntities.Template(99);
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(callForProposal);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Template, Entity: Gramps.Core.Domain.Template", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestDeleteCallForProposalDoesNotCascadeToTemplate()
        {
            #region Arrange
            var templateRepository = new Repository<Template>();
            templateRepository.DbContext.BeginTransaction();
            LoadTemplates(3);
            templateRepository.DbContext.CommitTransaction();
            var record = GetValid(99);
            record.TemplateGeneratedFrom = templateRepository.GetNullableById(1);

            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitChanges();

            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.TemplateGeneratedFrom);

            var templateCount = templateRepository.Queryable.Count();
            Assert.IsTrue(templateCount > 0);
            var saveId = record.Id;
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.Remove(record);
            CallForProposalRepository.DbContext.CommitChanges();
            #endregion Act

            #region Assert
            Assert.IsNull(CallForProposalRepository.GetNullableById(saveId));
            Assert.AreEqual(templateCount, templateRepository.Queryable.Count());
            #endregion Assert
        }
        #endregion Cascade Tests

        #endregion TemplateGeneratedFrom Tests

    }
}
