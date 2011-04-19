using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.PrintControllerTests
{
    public partial class PrintControllerTests
    {
        #region ProposalReviewer Tests

        [TestMethod]
        public void TestProposalReviewerRedirectsWhenCallNotFound()
        {
            #region Arrange
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.ProposalReviewer(4, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestProposalReviewerRedirectsWhenNoAccess()
        {
            #region Arrange
            SetupDataForTests2();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "test2@testy.com");
            #endregion Arrange

            #region Act
            Controller.ProposalReviewer(1, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestProposalReviewerRedirectsWhenProposalPassedAndNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test4@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasAccess(null, 1, "test4@testy.com")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ProposalReviewer(1, 6)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal not found.", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestProposalReviewerRedirectsWhenProposalPassedbutNotInCall()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test4@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasAccess(null, 1, "test4@testy.com")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ProposalReviewer(1, 5)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not found with call.", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestProposalReviewerReturnsFileWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test4@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasAccess(null, 1, "test4@testy.com")).Return(true).Repeat.Any();
            PrintService.Expect(a => a.Print(1, 2, false)).Return(new FileContentResult(new byte[] { 1, 2, 3 },
                                                                                        "application/pdf")).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalReviewer(1, 2);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", ((FileContentResult)result).FileContents.ByteArrayToString());
            Assert.AreEqual("application/pdf", ((FileContentResult)result).ContentType);

            PrintService.AssertWasCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            var printServiceArgs =
                PrintService.GetArgumentsForCallsMadeOn(
                    a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything))[0];
            Assert.IsNotNull(printServiceArgs);
            Assert.AreEqual(1, printServiceArgs[0]);
            Assert.AreEqual(2, printServiceArgs[1]);
            Assert.AreEqual(false, printServiceArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestProposalReviewerReturnsFileWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test4@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasAccess(null, 1, "test4@testy.com")).Return(true).Repeat.Any();
            PrintService.Expect(a => a.Print(1, null, false)).Return(new FileContentResult(new byte[] { 1, 2, 3 },
                                                                                        "application/pdf")).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalReviewer(1, null);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", ((FileContentResult)result).FileContents.ByteArrayToString());
            Assert.AreEqual("application/pdf", ((FileContentResult)result).ContentType);

            PrintService.AssertWasCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            var printServiceArgs =
                PrintService.GetArgumentsForCallsMadeOn(
                    a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything))[0];
            Assert.IsNotNull(printServiceArgs);
            Assert.AreEqual(1, printServiceArgs[0]);
            Assert.AreEqual(null, printServiceArgs[1]);
            Assert.AreEqual(false, printServiceArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestProposalReviewerReturnsFileWhenValid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test4@testy.com");
            SetupDataForTests2();
            AccessService.Expect(a => a.HasAccess(null, 1, "test4@testy.com")).Return(true).Repeat.Any();
            PrintService.Expect(a => a.Print(1, 0, false)).Return(new FileContentResult(new byte[] { 1, 2, 3 },
                                                                                        "application/pdf")).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalReviewer(1, 0);
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", ((FileContentResult)result).FileContents.ByteArrayToString());
            Assert.AreEqual("application/pdf", ((FileContentResult)result).ContentType);

            PrintService.AssertWasCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            var printServiceArgs =
                PrintService.GetArgumentsForCallsMadeOn(
                    a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything))[0];
            Assert.IsNotNull(printServiceArgs);
            Assert.AreEqual(1, printServiceArgs[0]);
            Assert.AreEqual(0, printServiceArgs[1]);
            Assert.AreEqual(false, printServiceArgs[2]);
            #endregion Assert
        }

        #endregion ProposalReviewer Tests
    }
}
