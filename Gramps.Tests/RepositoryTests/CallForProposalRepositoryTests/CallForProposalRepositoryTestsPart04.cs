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
using UCDArch.Testing.Extensions;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region Emails Tests

        #region Invalid Tests
            
        /// <summary>
        /// Tests the Emails with A value of null does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestEmailsWithAValueOfNullDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.Emails = null;
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
                Assert.AreEqual(callForProposal.Emails, null);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Emails: may not be null");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #endregion Emails Tests
    }
}
