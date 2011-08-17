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
        #region EditForTemplate Get Tests

        [TestMethod]
        public void TestEditForTemplateGetRedirectsWhenReportNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForTemplate(7, 2, null)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.TemplateIndex(2, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditForTemplateGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, 3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(3, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplateGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, 3, 9)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(3, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplateGetRedirectsWhenNotSameId1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, 3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(3, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 3, null));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplateGetRedirectsWhenNotSameId2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, 3, 9)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(3, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 3, null));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true); 
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForTemplate(2, 2, null)
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.TemplateId);
            Assert.AreEqual(true, result.IsTemplate);
            Assert.AreEqual(false, result.IsCallForProposal);
            Assert.AreEqual(4, result.Questions.Count());
            Assert.AreEqual("Name2", result.Report.Name);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForTemplate(2, 1, null)
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TemplateId);
            Assert.AreEqual(true, result.IsTemplate);
            Assert.AreEqual(false, result.IsCallForProposal);
            Assert.AreEqual(1, result.Questions.Count());
            Assert.AreEqual("Name2", result.Report.Name);
            AccessService.AssertWasCalled(a => a.HasAccess(1, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 1, null));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplateGetReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForTemplate(2, 1, 9)
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TemplateId);
            Assert.AreEqual(true, result.IsTemplate);
            Assert.AreEqual(false, result.IsCallForProposal);
            Assert.AreEqual(1, result.Questions.Count());
            Assert.AreEqual("Name2", result.Report.Name);
            AccessService.AssertWasCalled(a => a.HasAccess(1, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 1, null));
            #endregion Assert
        }
        #endregion EditForTemplate Get Tests
    }
}
