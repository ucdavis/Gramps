using System;
using System.Collections.Generic;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region Details Tests
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDetailsThrowsAnExceptionIfGuidIsNotUnique()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var proposals = new List<Proposal>();
                for (int i = 0; i < 2; i++)
                {
                    proposals.Add(CreateValidEntities.Proposal(i + 1));
                    proposals[i].Guid = SpecificGuid.GetGuid(1);
                }
                var fakeProposals = new FakeProposals();
                fakeProposals.Records(3, ProposalRepository, proposals);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Details(SpecificGuid.GetGuid(1));
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains more than one element", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestDetailsRedirectsWhenProposalNotFound()
        {
            #region Arrange
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Details(SpecificGuid.GetGuid(8))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email3@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Details(SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsReturnsViewWhenNotSubmitted()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email3@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Details(SpecificGuid.GetGuid(3))
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal is not submitted yet!", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(3), results.Proposal.Guid);
            Assert.AreEqual("Name1", results.CallForProposal.Name);           
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email2@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Details(SpecificGuid.GetGuid(2))
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(2), results.Proposal.Guid);
            Assert.AreEqual("Name5", results.CallForProposal.Name);
            #endregion Assert
        }
        #endregion Details Tests
    }
}
