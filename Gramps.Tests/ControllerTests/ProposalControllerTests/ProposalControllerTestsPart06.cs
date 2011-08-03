using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region ReviewerDetails Tests

        [TestMethod]
        public void TestReviewerDetailsRedirectsIfCallNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.ReviewerDetails(1, 4)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            #endregion Assert		
        }


        [TestMethod]
        public void TestReviewerDetailsRedirectsIfNotAReviewer()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test9@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.ReviewerDetails(2, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDetailsRedirectsIfNotSame()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ReviewerDetails(2, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDetailsRedirectsIfProposalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ReviewerDetails(11, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDetailsReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();


            #endregion Arrange

            #region Act
            Controller.ReviewerDetails(11, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("continue these tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        #endregion ReviewerDetails Tests
    }
}
