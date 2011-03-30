using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.CallForProposalControllerTests
{
    public partial class CallForProposalControllerTests
    {
        #region Launch Tests


        [TestMethod]
        public void TestLaunchRedirectsToIdexIfCallNotFound()
        {
            #region Arrange
            SetupDataForTests();
            Assert.IsNull(CallForProposalRepository.GetNullableById(8));
            #endregion Arrange

            #region Act
            Controller.Launch(8)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestLaunchRedirectsToHomeIndexIfNotAnEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            Assert.IsNotNull(CallForProposalRepository.GetNullableById(5));
            #endregion Arrange

            #region Act
            Controller.Launch(5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        [TestMethod]
        public void TestLaunchReturnsViewWhenCallFoundWithAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            Assert.IsNotNull(CallForProposalRepository.GetNullableById(5));
            #endregion Arrange

            #region Act
            var result = Controller.Launch(5)
                .AssertViewRendered()
                .WithViewData<CallNavigationViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name5", result.CallForProposal.Name);

            Assert.IsNull(Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }
        #endregion Launch Tests
    }
}
