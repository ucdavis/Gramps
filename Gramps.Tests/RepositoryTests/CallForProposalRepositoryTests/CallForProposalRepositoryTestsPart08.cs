using System;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region Proposal Tests

        [TestMethod]
        public void TestExistingProposalsArePopulatedIntoList()
        {
            #region Arrange
            var record = CallForProposalRepository.GetNullableById(2);
            Assert.IsNotNull(record);
            Assert.AreEqual(0, record.Proposals.Count);

            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = record;
            Repository.OfType<Proposal>().EnsurePersistent(proposal);

            proposal = CreateValidEntities.Proposal(2);
            proposal.CallForProposal = record;
            Repository.OfType<Proposal>().EnsurePersistent(proposal);

            proposal = CreateValidEntities.Proposal(3);
            proposal.CallForProposal = record;
            Repository.OfType<Proposal>().EnsurePersistent(proposal);

            Repository.OfType<Proposal>().DbContext.CommitTransaction();
            NHibernateSessionManager.Instance.GetSession().Evict(record);
            NHibernateSessionManager.Instance.GetSession().Evict(proposal);
            #endregion Arrange

            #region Act
            record = CallForProposalRepository.GetNullableById(2);
            #endregion Act

            #region Assert
            Assert.IsNotNull(record);
            Assert.AreEqual(3, record.Proposals.Count);
            #endregion Assert		
        }

        #region Cascade Tests


        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void TestProposalDoesNotCascadeSave()
        {
            CallForProposal record = null;
            try
            {
                #region Arrange
                record = CallForProposalRepository.GetNullableById(2);
                Assert.IsNotNull(record);
                Assert.AreEqual(0, record.Proposals.Count);

                record.Proposals.Add(CreateValidEntities.Proposal(1));
                record.Proposals.Add(CreateValidEntities.Proposal(2));
                record.Proposals[0].CallForProposal = record;
                record.Proposals[1].CallForProposal = record;
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(record);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Gramps.Core.Domain.Proposal, Entity: Gramps.Core.Domain.Proposal", ex.Message);
                throw;
            }
        }


        //[TestMethod] //It saves anyway.
        //public void TestCallForProposalDoesNotUpdate()
        //{
        //    var record = CallForProposalRepository.GetNullableById(2);
        //    Assert.IsNotNull(record);
        //    Assert.AreEqual(0, record.Proposals.Count);

        //    Repository.OfType<Proposal>().DbContext.BeginTransaction();
        //    var proposal = CreateValidEntities.Proposal(1);
        //    proposal.CallForProposal = record;
        //    Repository.OfType<Proposal>().EnsurePersistent(proposal);

        //    proposal = CreateValidEntities.Proposal(2);
        //    proposal.CallForProposal = record;
        //    Repository.OfType<Proposal>().EnsurePersistent(proposal);

        //    proposal = CreateValidEntities.Proposal(3);
        //    proposal.CallForProposal = record;
        //    Repository.OfType<Proposal>().EnsurePersistent(proposal);

        //    Repository.OfType<Proposal>().DbContext.CommitTransaction();
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    NHibernateSessionManager.Instance.GetSession().Evict(proposal);

        //    record = CallForProposalRepository.GetNullableById(2);
        //    Assert.AreEqual(3, record.Proposals.Count);

        //    #region Act
        //    record.Proposals[1].Email = "updated@testy.com";
        //    CallForProposalRepository.DbContext.BeginTransaction();
        //    CallForProposalRepository.EnsurePersistent(record);
        //    CallForProposalRepository.DbContext.CommitTransaction();
        //    NHibernateSessionManager.Instance.GetSession().Evict(record);
        //    NHibernateSessionManager.Instance.GetSession().Evict(proposal);
        //    #endregion Act

        //    #region Assert
        //    Assert.IsFalse(Repository.OfType<Proposal>().Queryable.Where(a => a.Email == "updated@testy.com").Any());
        //    #endregion Assert		
        //}

        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestProposalDoesNotCascadeDelete1()
        {
            CallForProposal record = null;
            #region Arrange
            try
            {
                record = CallForProposalRepository.GetNullableById(2);
                Assert.IsNotNull(record);
                Assert.AreEqual(0, record.Proposals.Count);

                Repository.OfType<Proposal>().DbContext.BeginTransaction();
                var proposal = CreateValidEntities.Proposal(1);
                proposal.CallForProposal = record;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);

                proposal = CreateValidEntities.Proposal(2);
                proposal.CallForProposal = record;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);

                proposal = CreateValidEntities.Proposal(3);
                proposal.CallForProposal = record;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);

                Repository.OfType<Proposal>().DbContext.CommitTransaction();
                NHibernateSessionManager.Instance.GetSession().Evict(proposal);
                NHibernateSessionManager.Instance.GetSession().Evict(record);
            }
            catch (Exception)
            {              
                Assert.IsFalse(true, "Exception at wrong place");
            }
            #endregion Arrange
            try
            {
                record = CallForProposalRepository.GetNullableById(2);
                Assert.IsNotNull(record);
                Assert.AreEqual(3, record.Proposals.Count);

                record.Proposals.RemoveAt(1);
                

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(record);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("could not delete collection rows: [Gramps.Core.Domain.CallForProposal.Proposals#2][SQL: UPDATE Proposals SET CallForProposalID = null WHERE CallForProposalID = @p0 AND ID = @p1]", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NHibernate.Exceptions.GenericADOException))]
        public void TestProposalDoesNotCascadeDelete2()
        {
            CallForProposal record = null;
            #region Arrange
            try
            {
                record = CallForProposalRepository.GetNullableById(2);
                Assert.IsNotNull(record);
                Assert.AreEqual(0, record.Proposals.Count);

                Repository.OfType<Proposal>().DbContext.BeginTransaction();
                var proposal = CreateValidEntities.Proposal(1);
                proposal.CallForProposal = record;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);

                proposal = CreateValidEntities.Proposal(2);
                proposal.CallForProposal = record;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);

                proposal = CreateValidEntities.Proposal(3);
                proposal.CallForProposal = record;
                Repository.OfType<Proposal>().EnsurePersistent(proposal);

                Repository.OfType<Proposal>().DbContext.CommitTransaction();
                NHibernateSessionManager.Instance.GetSession().Evict(proposal);
                NHibernateSessionManager.Instance.GetSession().Evict(record);
            }
            catch (Exception)
            {
                Assert.IsFalse(true, "Exception at wrong place");
            }
            #endregion Arrange
            try
            {
                record = CallForProposalRepository.GetNullableById(2);
                Assert.IsNotNull(record);
                Assert.AreEqual(3, record.Proposals.Count);

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.Remove(record);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(record);
                Assert.IsNotNull(ex);
                Assert.AreEqual("could not delete collection: [Gramps.Core.Domain.CallForProposal.Proposals#2][SQL: UPDATE Proposals SET CallForProposalID = null WHERE CallForProposalID = @p0]", ex.Message);
                throw;
            }
        }


        #endregion Cascade Tests
        

        #endregion Proposal Tests

        #region ProposalMaximum Tests

        /// <summary>
        /// Tests the ProposalMaximum with A value of 0.00m does not save.
        /// </summary>
        [TestMethod, Ignore]
        [ExpectedException(typeof(ApplicationException))]
        public void TestProposalMaximumWithAValueOfZeroeroDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.ProposalMaximum = -0.01m;
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
                Assert.AreEqual(callForProposal.ProposalMaximum, -0.01m);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ProposalMaximum: Minimum amount is one cent");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }	
        }

        #endregion ProposalMaximum Tests

    }
}