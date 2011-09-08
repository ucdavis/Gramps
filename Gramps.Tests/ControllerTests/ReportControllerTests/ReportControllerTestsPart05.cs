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
        public void TestEditForCallPostRedirectsWhenCallIsNull()
        {
            #region Arrange
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, reportToEdit, null, null, reportParms, "Show")
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert

            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallPostRedirectsWhenCallIsZero()
        {
            #region Arrange
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            Controller.EditForCall(4,  reportToEdit, null, 0, reportParms, "show")
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert

            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallPostRedirectsWhenCallNotFound()
        {
            #region Arrange
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            Controller.EditForCall(4,reportToEdit, null, 4, reportParms, "show")
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert

            #endregion Assert
        }


        [TestMethod]
        public void TestEditForCallPostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            Controller.EditForCall(4, reportToEdit, null, 2, reportParms, "show")
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallPostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            Controller.EditForCall(2, reportToEdit, 99, 2, reportParms, "Show")
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallPostRedirectsWhenReportNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            var result = Controller.EditForCall(7,reportToEdit ,  null, 3, reportParms, "Show")
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.CallIndex(3));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditForCallPostRedirectsWhenNotSameId1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            Controller.EditForCall(4,reportToEdit, null, 2, reportParms, "Show")
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
        public void TestEditForCallPostRedirectsWhenNotSameId2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData3();
            var reportToEdit = CreateValidEntities.Report(9);
            var reportParms = new CreateReportParameter[2];
            reportParms[0] = new CreateReportParameter();
            reportParms[1] = new CreateReportParameter();
            reportParms[0].QuestionId = 7;
            reportParms[1].QuestionId = 8; 
            #endregion Arrange

            #region Act
            Controller.EditForCall(4,reportToEdit, 9, 2, reportParms, "Show")
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
        public void TestEditForCallPostWhenNotValid()
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
            var result = Controller.EditForCall(2, reportToEdit, 2, 3, reportParms, "test")
                .AssertViewRendered()
                .WithViewData<CallReportViewModel>();
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
            Assert.AreEqual(null, args[2]);
            Assert.AreEqual(3, args[3]);
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
        public void TestEditForCallPostWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            var report = CreateValidEntities.Report(9);
            for(int i = 0; i < 3; i++)
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
            for(int i = 0; i < 9; i++)
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
            Controller.EditForCall(2, reportToEdit, 2, 3, reportParms, "test")
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.CallIndex(3));
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
            var args = (Report)ReportRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Report>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(3, args.ReportColumns.Count);
            Assert.AreEqual("Name9", args.Name);
            #endregion Assert
        }

        #endregion EditForCall Post Tests
    }
}
