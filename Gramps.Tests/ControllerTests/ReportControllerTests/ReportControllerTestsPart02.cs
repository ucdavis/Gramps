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
        #region CreateForTemplate Get Tests

        [TestMethod]
        public void TestCreateForTemplateGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.CreateForTemplate(2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateForTemplateGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.CreateForTemplate(2, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForTemplateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.CreateForTemplate(2, null)
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.TemplateId);
            Assert.AreEqual(true, result.IsTemplate);
            Assert.AreEqual(false, result.IsCallForProposal);
            Assert.AreEqual(4, result.Questions.Count());
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForTemplateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.CreateForTemplate(1, null)
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TemplateId);
            Assert.AreEqual(true, result.IsTemplate);
            Assert.AreEqual(false, result.IsCallForProposal);
            Assert.AreEqual(1, result.Questions.Count());
            Assert.AreEqual(1, result.Questions.ElementAt(0).Template.Id);
            AccessService.AssertWasCalled(a => a.HasAccess(1, null, "tester@testy.com"));
            #endregion Assert
        }
        #endregion CreateForTemplate Get Tests

        #region CreateForTemplate Post Tests

        [TestMethod]
        public void TestCreateForTemplatePostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.CreateForTemplate(new Report(), 2, null, new CreateReportParameter[0], "ShowAll")
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForTemplatePostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.CreateForTemplate(2, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateForTemplateWhenNotValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            ReportService.Expect(a =>a.CommonCreate(
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
            var result = Controller.CreateForTemplate(reportToEdit, 2, 3, reportParms, "test")
                .AssertViewRendered()
                .WithViewData<ReportViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("error message");
            Assert.AreEqual("Unable to create report", Controller.Message);
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
        public void TestCreateForTemplateWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var report = CreateValidEntities.Report(9);
            ReportService.Expect(a => a.CommonCreate(
                    Arg<ModelStateDictionary>.Is.Anything,
                    Arg<Report>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<CreateReportParameter[]>.Is.Anything,
                    Arg<string>.Is.Anything)).Return(report);
   

            var reportToEdit = CreateValidEntities.Report(4);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8;

            SetupData3();
            #endregion Arrange

            #region Act
            Controller.CreateForTemplate(reportToEdit, 2, 3, reportParms, "test")
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.TemplateIndex(2, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Report Created Successfully", Controller.Message);
            ReportService.AssertWasCalled(a => a.CommonCreate(
                    Arg<ModelStateDictionary>.Is.Anything,
                    Arg<Report>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<int?>.Is.Anything,
                    Arg<CreateReportParameter[]>.Is.Anything,
                    Arg<string>.Is.Anything));
            ReportRepository.AssertWasCalled(a => a.EnsurePersistent(report));
            #endregion Assert
        }
        #endregion CreateForTemplate Post Tests
    }
}
