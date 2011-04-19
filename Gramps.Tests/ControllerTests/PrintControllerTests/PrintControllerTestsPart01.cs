using System.Linq;
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
        #region ProposalAdmin Tests

        [TestMethod]
        public void TestProposalAdminRedirectsWhenCallNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "me@ucdavis.edu");
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalAdmin(4, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(null, result.RouteValues.ElementAt(2+1).Value);
            }
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestProposalAdminRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@ucdavis.edu");
            SetupDataForTests1();
            AccessService.Expect(a => a.HasAccess(null, 1, "me@ucdavis.edu")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ProposalAdmin(1, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var accessServiceArgs = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, accessServiceArgs[0]);
            Assert.AreEqual(1, accessServiceArgs[1]);
            Assert.AreEqual("me@ucdavis.edu", accessServiceArgs[2]);
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestProposalAdminRedirectsWhenProposalPassedAndNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@ucdavis.edu");
            SetupDataForTests1();
            AccessService.Expect(a => a.HasAccess(null, 1, "me@ucdavis.edu")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ProposalAdmin(1, 6)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal not found.", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestProposalAdminRedirectsWhenProposalPassedbutNotInCall()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@ucdavis.edu");
            SetupDataForTests1();
            AccessService.Expect(a => a.HasAccess(null, 1, "me@ucdavis.edu")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ProposalAdmin(1, 5)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not found with call.", Controller.Message);
            PrintService.AssertWasNotCalled(a => a.Print(Arg<int>.Is.Anything, Arg<int?>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestProposalAdminReturnsFileWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@ucdavis.edu");
            SetupDataForTests1();
            AccessService.Expect(a => a.HasAccess(null, 1, "me@ucdavis.edu")).Return(true).Repeat.Any();
            PrintService.Expect(a => a.Print(1, 2, false)).Return(new FileContentResult(new byte[] {1, 2, 3},
                                                                                        "application/pdf")).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalAdmin(1, 2);
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
        public void TestProposalAdminReturnsFileWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@ucdavis.edu");
            SetupDataForTests1();
            AccessService.Expect(a => a.HasAccess(null, 1, "me@ucdavis.edu")).Return(true).Repeat.Any();
            PrintService.Expect(a => a.Print(1, 2, true)).Return(new FileContentResult(new byte[] { 1, 2, 3 },
                                                                                        "application/pdf")).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalAdmin(1, 2, true);
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
            Assert.AreEqual(true, printServiceArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestProposalAdminReturnsFileWhenValid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@ucdavis.edu");
            SetupDataForTests1();
            AccessService.Expect(a => a.HasAccess(null, 1, "me@ucdavis.edu")).Return(true).Repeat.Any();
            PrintService.Expect(a => a.Print(1, null, true)).Return(new FileContentResult(new byte[] { 1, 2, 3 },
                                                                                        "application/pdf")).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalAdmin(1, null, true);
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
            Assert.AreEqual(true, printServiceArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestProposalAdminReturnsFileWhenValid4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@ucdavis.edu");
            SetupDataForTests1();
            AccessService.Expect(a => a.HasAccess(null, 1, "me@ucdavis.edu")).Return(true).Repeat.Any();
            PrintService.Expect(a => a.Print(1, 0, true)).Return(new FileContentResult(new byte[] { 1, 2, 3 },
                                                                                        "application/pdf")).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ProposalAdmin(1, 0, true);
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
            Assert.AreEqual(true, printServiceArgs[2]);
            #endregion Assert
        }
        #endregion ProposalAdmin Tests
    }
}
