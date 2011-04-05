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


namespace Gramps.Tests.ControllerTests.EditorControllerTests
{
    public partial class EditorControllerTests
    {
        #region SendCall Tests

        [TestMethod]
        public void TestSendCallRedirectsToCallIndexIfCallForProposalNotFound()
        {
            #region Arrange
            var callFake = new FakeCallForProposals();
            callFake.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.RouteValues.Count);
            Assert.IsNull(result.RouteValues.ElementAt(2).Value);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            #endregion Assert		
        }


        [TestMethod]
        public void TestSendCallRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(false).Repeat.Any();
            var callFake = new FakeCallForProposals();
            callFake.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.SendCall(2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestSendCallReturnsViewWithExpectedValues1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(true).Repeat.Any();
            SetupDataForTests3();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(2)
                .AssertViewRendered()
                .WithViewData<ReviewersSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.CallForProposal.Id);
            Assert.AreEqual(4, result.EditorsToNotify.Count());
            Assert.AreEqual("ReviewerName4", result.EditorsToNotify.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName6", result.EditorsToNotify.ElementAt(1).ReviewerName);
            Assert.AreEqual("ReviewerName8", result.EditorsToNotify.ElementAt(2).ReviewerName);
            Assert.AreEqual("ReviewerName10", result.EditorsToNotify.ElementAt(3).ReviewerName);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSendCallReturnsViewWithExpectedValues2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            SetupDataForTests3();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1)
                .AssertViewRendered()
                .WithViewData<ReviewersSendViewModel>();
            #endregion Act

            #region Assert           
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(1, result.CallForProposal.Id);
            Assert.AreEqual(2, result.EditorsToNotify.Count());
            Assert.AreEqual("ReviewerName7", result.EditorsToNotify.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName9", result.EditorsToNotify.ElementAt(1).ReviewerName);
            #endregion Assert
        }
        #endregion SendCall Tests
    }
}
