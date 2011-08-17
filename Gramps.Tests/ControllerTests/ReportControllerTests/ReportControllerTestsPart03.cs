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

        #region CreateForCall Post Tests
        [TestMethod]
        public void TestCreateForCallPostRedirectsIfNull()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            Controller.CreateForCall(new Report(), null, null, null, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallPostRedirectsIfZero()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            Controller.CreateForCall(new Report(), 9, 0, null, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }



        [TestMethod]
        public void TestCreateForCallPostWhenNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.CreateForCall(new Report(), null, 4, null, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallPostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.CreateForCall(new Report(), null, 2, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallPostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.CreateForCall(new Report(), 9, 2, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 2, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateForCallPostWhenNotValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
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
            var result = Controller.CreateForCall(reportToEdit, 99, 2, reportParms, "test")
                .AssertViewRendered()
                .WithViewData<CallReportViewModel>();
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
            Assert.IsNull(args[2]);
            Assert.AreEqual(2, args[3]);
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
        public void TestCreateForCallPostWhenValid()
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
            Controller.CreateForCall(reportToEdit, 99, 2, reportParms, "test")
                .AssertActionRedirect()
                .ToAction<ReportController>(a => a.CallIndex(3));
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
        #endregion CreateForCall Post Tests
    }
}
