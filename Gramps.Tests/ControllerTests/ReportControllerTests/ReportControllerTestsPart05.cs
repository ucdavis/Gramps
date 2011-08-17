﻿using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.ReportControllerTests
{
    public partial class ReportControllerTests
    {
        #region EditForCall Get Tests
        [TestMethod]
        public void TestEditForCallGetRedirectsWhenCallIsNull()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, null, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert

            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetRedirectsWhenCallIsZero()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, null, 0)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert

            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetRedirectsWhenCallNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, null, 4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert

            #endregion Assert
        }


        [TestMethod]
        public void TestEditForCallGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, null, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForCall(2, 99, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetRedirectsWhenReportNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForCall(7, null, 3)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.CallIndex(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetRedirectsWhenNotSameId1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, null, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(2), null, 2));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetRedirectsWhenNotSameId2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, 9, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(2), null, 2));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForCall(4, null, 2)
                .AssertViewRendered()
                .WithViewData<CallReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.CallForProposal.Id);            
            Assert.AreEqual(4, result.Questions.Count());
            Assert.AreEqual("Name4", result.Report.Name);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(2), null, 2));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForCall(4, null, 1)
                .AssertViewRendered()
                .WithViewData<CallReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CallForProposal.Id);
            Assert.AreEqual(1, result.Questions.Count());
            Assert.AreEqual("Name4", result.Report.Name);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 1, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(2), null, 1));
            #endregion Assert
        }

        #endregion EditForCall Get Tests

        #region EditForCall Post Tests

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Write these tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        } 
        #endregion EditForCall Post Tests
    }
}
