using System.Linq;
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
        #region CreateForCall Get Tests

        [TestMethod]
        public void TestCreateForCallGetRedirectsIfNull()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            Controller.CreateForCall(null, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateForCallGetRedirectsIfZero()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            Controller.CreateForCall(9, 0)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }



        [TestMethod]
        public void TestCreateForCallGetWhenNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.CreateForCall(null, 4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateForCallGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.CreateForCall(null, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.CreateForCall(9, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.CreateForCall(null, 2)
                .AssertViewRendered()
                .WithViewData<CallReportViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.CallForProposal.Name);
            Assert.AreEqual(4, result.Questions.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.CreateForCall(9, 2)
                .AssertViewRendered()
                .WithViewData<CallReportViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.CallForProposal.Name);
            Assert.AreEqual(4, result.Questions.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.CreateForCall(null, 1)
                .AssertViewRendered()
                .WithViewData<CallReportViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(null, 1, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual("Name1", result.CallForProposal.Name);
            Assert.AreEqual(1, result.Questions.Count());
            #endregion Assert
        }
        #endregion CreateForCall Get Tests
    }
}
