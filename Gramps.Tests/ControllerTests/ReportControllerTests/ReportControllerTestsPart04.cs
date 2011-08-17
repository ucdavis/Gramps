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

        #region EditForTemplate Post Tests
        [TestMethod]
        public void TestEditForTemplatePostRedirectsWhenReportNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForTemplate(7, new Report(),  2, null, null, null)
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
        public void TestEditForTemplatePostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, new Report(), 3, null, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(3, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplatePostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, new Report(), 3, 9, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(3, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplatePostRedirectsWhenNotSameId1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, new Report(), 3, null, null, null)
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
        public void TestEditForTemplatePostRedirectsWhenNotSameId2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, new Report(), 3, 9, null, null)
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
        public void TestEditForTemplatePostWhenNotValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            ReportService.Expect(a => a.CommonCreate(
                    Arg<ModelStateDictionary>.Is.Anything,
                    Arg<Report>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<CreateReportParameter[]>.Is.Anything,
                    Arg<string>.Is.Anything)).Return(CreateValidEntities.Report(9));
            Controller.ModelState.AddModelError("Fake", @"error message");

            var reportToEdit = CreateValidEntities.Report(4);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8;

            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.EditForTemplate(2,reportToEdit, 2, 3, reportParms, "test")
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("error message");
            Assert.AreEqual("Unable to edit report", Controller.Message);
            ReportService.AssertWasCalled(a => a.CommonCreate(
                    Arg<ModelStateDictionary>.Is.Anything,
                    Arg<Report>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<CreateReportParameter[]>.Is.Anything,
                    Arg<string>.Is.Anything));
            var args = ReportService.GetArgumentsForCallsMadeOn(a => a.CommonCreate(
                    Arg<ModelStateDictionary>.Is.Anything,
                    Arg<Report>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<CreateReportParameter[]>.Is.Anything,
                    Arg<string>.Is.Anything))[0];
            Assert.IsNotNull(args);
            ((ModelStateDictionary)args[0]).AssertErrorsAre("error message");
            Assert.AreEqual("Name4", ((Report)args[1]).Name);
            Assert.AreEqual(2, args[2]);
            Assert.IsNull(args[3]);
            Assert.AreEqual(2, ((CreateReportParameter[])args[4]).Count());
            Assert.AreEqual(7, ((CreateReportParameter[])args[4]).ElementAt(0).QuestionId);
            Assert.AreEqual(8, ((CreateReportParameter[])args[4]).ElementAt(1).QuestionId);
            Assert.AreEqual("test", args[5]);
            ReportRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Report>.Is.Anything));

            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.Report.Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForTemplatePostWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            var report = CreateValidEntities.Report(9);
            for (int i = 0; i < 3; i++)
            {
                report.ReportColumns.Add(CreateValidEntities.ReportColumn(i + 5));
            }
            ReportService.Expect(
                a => a.CommonCreate(
                    Arg<ModelStateDictionary>.Is.Anything,
                    Arg<Report>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<CreateReportParameter[]>.Is.Anything,
                    Arg<string>.Is.Anything)).Return(report);


            var reportToEdit = CreateValidEntities.Report(4);
            for (int i = 0; i < 9; i++)
            {
                reportToEdit.ReportColumns.Add(CreateValidEntities.ReportColumn(i + 20));
            }

            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8;

            SetupData3();
            #endregion Arrange

            #region Act
            Controller.EditForTemplate(2, reportToEdit, 2, 3, reportParms, "test")
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.TemplateIndex(2, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Report Edited Successfully", Controller.Message);
            ReportService.AssertWasCalled(
                a => a.CommonCreate(
                    Arg<ModelStateDictionary>.Is.Anything,
                    Arg<Report>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<CreateReportParameter[]>.Is.Anything,
                    Arg<string>.Is.Anything));
            ReportRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Report>.Is.Anything));
            var args = (Report) ReportRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Report>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.ReportColumns.Count);
            Assert.AreEqual("Name9", args.Name);
            #endregion Assert
        }
        #endregion EditForTemplate Post Tests
    }
}
