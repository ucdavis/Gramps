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
        #region Edit Tests
        #region Edit Get Tests
        [TestMethod]
        public void TestEditGetWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, null, 3)
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
        public void TestEditGetWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, 4, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWhenNotFoundRedirectsToIndex1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(11, null, 3)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Emails For Call not found.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWhenNotFoundRedirectsToIndex2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(11, 4, null)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Emails For Call not found.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithNoDiffIdRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Edit(2, null, 3)
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(2, ((CallForProposal)args1[1]).Id);            
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithNoDiffIdRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(TemplateRepository.GetNullableById(4), null, 4, null)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Edit(2, 4, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];            
            Assert.AreEqual(2, ((Template)args1[0]).Id);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
