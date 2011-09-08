using Gramps.Controllers;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.ReportControllerTests
{
    public partial class ReportControllerTests
    {
        #region ExportExcell Tests
        [TestMethod]
        public void TestExportExcellRedirctsIfCallIdIsNull()
        {
            Controller.ExportExcell(1, null)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
        }
        [TestMethod]
        public void TestExportExcellRedirctsIfCallIdIsZero()
        {
            Controller.ExportExcell(1, 0)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
        }


        [TestMethod]
        public void TestExportExcellRedirectsWhenCallNotFound()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.ExportExcell(1, 4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestExportExcellRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.ExportExcell(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestExportExcellRedirectsWhenReportNotFound()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.ExportExcell(9, 2)
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
        public void TestExportExcellRedirectsWhenNotSameId()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.ExportExcell(6, 3)
               .AssertActionRedirect()
               .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }

        //Note: Rest of tests for this action not done. (Would probably need a service).
        #endregion ExportExcell Tests
    }
}
