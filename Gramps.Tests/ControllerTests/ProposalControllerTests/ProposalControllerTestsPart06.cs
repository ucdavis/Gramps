using System;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


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
        public void TestReviewerDetailsRedirectsIfProposalNotSubmitted()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ReviewerDetails(1, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Submitted Yet", Controller.Message);
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
            var result = Controller.ReviewerDetails(4, 1)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Comment);
            Assert.AreEqual("email4@testy.com", result.Proposal.Email);
            Assert.AreEqual("Name1", result.CallForProposal.Name);
            ReviewedProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything));
            var args = (ReviewedProposal) ReviewedProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("email4@testy.com", args.Proposal.Email);
            Assert.AreEqual(DateTime.Now.Date, args.LastViewedDate.Date);
            Assert.AreEqual("test2@testy.com", args.Editor.ReviewerEmail);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDetailsReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();


            #endregion Arrange

            #region Act
            var result = Controller.ReviewerDetails(5, 1)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Comment);
            Assert.AreEqual("email5@testy.com", result.Proposal.Email);
            Assert.AreEqual("Name1", result.CallForProposal.Name);
            ReviewedProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything));
            var args = (ReviewedProposal)ReviewedProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("email5@testy.com", args.Proposal.Email);
            Assert.AreEqual(DateTime.Now.Date, args.LastViewedDate.Date);
            Assert.AreEqual("test2@testy.com", args.Editor.ReviewerEmail);
            #endregion Assert
        }


        #endregion ReviewerDetails Tests

        #region ReviewerDownload Tests
        [TestMethod]
        public void TestReviewerDownloadRedirectsIfCallNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.ReviewerDownload(1, 4)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            #endregion Assert
        }


        [TestMethod]
        public void TestReviewerDownloadRedirectsIfNotAReviewer()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test9@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.ReviewerDownload(2, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDownloadRedirectsIfNotSame()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ReviewerDownload(2, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDownloadRedirectsIfProposalNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ReviewerDownload(11, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDownloadRedirectsIfFileNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ReviewerDownload(4, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal PDF Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDownloadRedirectsIfFileContentsNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ReviewerDownload(5, 1)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal PDF Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerDownloadReturnsFile()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(1), null, 1))
                .Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ReviewerDownload(6, 1)
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("FakeType", result.ContentType);
            Assert.AreEqual("Name44", result.FileDownloadName);
            Assert.AreEqual("236", result.FileContents.ByteArrayToString());
            #endregion Assert
        }
        #endregion ReviewerDetails Tests
    }
}
