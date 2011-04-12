using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region SendCall Tests
        #region SendCall Get Tests

        [TestMethod]
        public void TestSendCallGetWHereCallNotFoundRegirectsToIndex()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("filterActive", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);

            Assert.AreEqual("filterStartCreate", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            Assert.AreEqual("filterEndCreate", result.RouteValues.ElementAt(4).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(4).Value);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSendCallGetWHereNoAccessRegirectsToHome()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.SendCall(3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        [TestMethod]
        public void TestSendCallReturnsViewWithExpectedValues()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(3)
                .AssertViewRendered()
                .WithViewData<EmailsForCallSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.EmailsForCallList.Count());
            Assert.AreEqual(3, result.EmailsForCallList.ElementAt(0).Id);
            Assert.AreEqual(4, result.EmailsForCallList.ElementAt(1).Id);
            Assert.AreEqual(5, result.EmailsForCallList.ElementAt(2).Id);
            Assert.IsFalse(result.Immediate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }
        #endregion SendCall Get Tests
        #region SendCall Post Tests

        #endregion SendCall Post Tests
        #endregion SendCall Tests
    }
}
