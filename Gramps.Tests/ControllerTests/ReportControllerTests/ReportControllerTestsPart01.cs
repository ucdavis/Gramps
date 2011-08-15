using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.ReportControllerTests
{
    public partial class ReportControllerTests
    {
        #region TemplateIndex Tests
        [TestMethod]
        public void TestTemplateIndexRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.TemplateIndex(2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestTemplateIndexRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.TemplateIndex(2, 5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }


        [TestMethod]
        public void TestTemplateIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.TemplateIndex(2, null)
                .AssertViewRendered()
                .WithViewData<TemplateReportListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsTemplate);
            Assert.AreEqual(false, result.IsCallForProposal);
            Assert.AreEqual(2, result.TemplateId);
            Assert.AreEqual(3, result.ReportList.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestTemplateIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.TemplateIndex(1, null)
                .AssertViewRendered()
                .WithViewData<TemplateReportListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsTemplate);
            Assert.AreEqual(false, result.IsCallForProposal);
            Assert.AreEqual(1, result.TemplateId);
            Assert.AreEqual(1, result.ReportList.Count());
            #endregion Assert
        }
        #endregion TemplateIndex Tests

        #region CallIndex Tests
        [TestMethod]
        public void TestWriteMethodTests()
        {
            #region Arrange
            Assert.Inconclusive("Need to write these tests");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert
        }
        #endregion CallIndex Tests




    }
}
