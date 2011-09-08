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
        #region Delete Tests

        [TestMethod]
        public void TestDeleteWhenNoAccessRedirects()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(3, 4, 5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region AssertAssert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(4, 5, "tester@testy.com"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeleteWhenReportNotFoundRedirects1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var fakeReports = new FakeReports();
            fakeReports.Records(2, ReportRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, null, 5)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.CallIndex(5));
            #endregion Act

            #region AssertAssert.AreEqual("Report not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 5, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenReportNotFoundRedirects2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var fakeReports = new FakeReports();
            fakeReports.Records(2, ReportRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, 0, 5)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.CallIndex(5));
            #endregion Act

            #region AssertAssert.AreEqual("Report not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(0, 5, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenReportNotFoundRedirects3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var fakeReports = new FakeReports();
            fakeReports.Records(2, ReportRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, 4, 5)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.TemplateIndex(4, 5));
            #endregion Act

            #region AssertAssert.AreEqual("Report not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(4, 5, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues["templateId"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenReportNotFoundRedirects4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var fakeReports = new FakeReports();
            fakeReports.Records(2, ReportRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, 4, null)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.TemplateIndex(4, null));
            #endregion Act

            #region AssertAssert.AreEqual("Report not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(4, null, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues["templateId"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenNotSameId1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Delete(4, null, 2)
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
        public void TestDeleteRedirectsWhenNotSameId2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Delete(4, 9, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(9, 2, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(9), CallForProposalRepository.GetNullableById(2), 9, 2));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4, null, 2)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.CallIndex(2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("Report removed", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(2), null, 2));
            ReportRepository.AssertWasCalled(a => a.Remove(Arg<Report>.Is.Anything));
            var args = (Report) ReportRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<Report>.Is.Anything))[0][0]; 
            Assert.AreEqual("Name4", args.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(4, 9, 2)
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.TemplateIndex(9,2));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.RouteValues["templateId"]);
            Assert.AreEqual("Report removed", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(9, 2, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(9), CallForProposalRepository.GetNullableById(2), 9, 2));
            ReportRepository.AssertWasCalled(a => a.Remove(Arg<Report>.Is.Anything));
            var args = (Report)ReportRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<Report>.Is.Anything))[0][0];
            Assert.AreEqual("Name4", args.Name);
            #endregion Assert
        }

        #endregion Delete Tests

        #region Launch Tests

        [TestMethod]
        public void TestLaunchRedirctsIfCallIdIsNull()
        {
            Controller.Launch(1, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
        }
        [TestMethod]
        public void TestLaunchRedirctsIfCallIdIsZero()
        {
            Controller.Launch(1, 0)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
        }


        [TestMethod]
        public void TestLaunchRedirectsWhenCallNotFound()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Launch(1, 4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        [TestMethod]
        public void TestLaunchRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Launch(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestLaunchRedirectsWhenReportNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Launch(9, 2)
               .AssertActionRedirect()
               .ToAction<ReportController>(a => a.CallIndex(2));
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestLaunchRedirectsWhenNotSameId()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Launch(6, 3)
               .AssertActionRedirect()
               .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestLaunchReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Launch(6, 2)
                .AssertViewRendered()
                .WithViewData<ReportLaunchViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.ForExport);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }
        #endregion Launch Tests
    }
}
