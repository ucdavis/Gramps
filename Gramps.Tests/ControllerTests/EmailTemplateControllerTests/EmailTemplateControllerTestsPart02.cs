using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.EmailTemplateControllerTests
{
    public partial class EmailTemplateControllerTests
    {
        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetWithoutAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, null, 1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithoutAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, 2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToIndexWhenEmailTemplateNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(22, null, 1)
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(1, result.RouteValues.ElementAt(3).Value);

            //Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetRedirectsToIndexWhenEmailTemplateNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(22, 2, null)
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(2, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            //Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithDiffRedirectsToHome1()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(1).Template,
                            EmailTemplateRepository.GetNullableById(1).CallForProposal, null, 1)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(1, null, 1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWitDiffRedirectsToHome2A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(9).Template,
                            EmailTemplateRepository.GetNullableById(9).CallForProposal, null, 1)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(9, 2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);            
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWitDiffRedirectsToHome2B()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(20).Template,
                            EmailTemplateRepository.GetNullableById(20).CallForProposal, 2, null)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(20, 2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(1, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);            
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Continue tests after the HasSameId code");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        #endregion Edit Get Tests
        #region Edit Post Tests
        
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
