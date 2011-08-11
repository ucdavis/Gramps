using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Index(2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Index(null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }


        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(1, null)
                .AssertViewRendered()
                .WithViewData<QuestionListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsTemplate);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(1, result.TemplateId);
            Assert.AreEqual(1, result.QuestionList.Count());
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(2, 0)
                .AssertViewRendered()
                .WithViewData<QuestionListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsTemplate);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(2, result.TemplateId);
            Assert.AreEqual(5, result.QuestionList.Count());
            Assert.AreEqual("Name6", result.QuestionList.ElementAt(0).Name);
            Assert.AreEqual("Name5", result.QuestionList.ElementAt(1).Name);
            Assert.AreEqual("Name4", result.QuestionList.ElementAt(2).Name);
            Assert.AreEqual("Name3", result.QuestionList.ElementAt(3).Name);
            Assert.AreEqual("Name1", result.QuestionList.ElementAt(4).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(0, 2)
                .AssertViewRendered()
                .WithViewData<QuestionListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsTemplate);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(2, result.CallForProposalId);
            Assert.AreEqual(1, result.QuestionList.Count());
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, 3)
                .AssertViewRendered()
                .WithViewData<QuestionListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsTemplate);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.AreEqual(5, result.QuestionList.Count());
            Assert.AreEqual("Name12", result.QuestionList.ElementAt(0).Name);
            Assert.AreEqual("Name10", result.QuestionList.ElementAt(1).Name);
            Assert.AreEqual("Name9", result.QuestionList.ElementAt(2).Name);
            Assert.AreEqual("Name8", result.QuestionList.ElementAt(3).Name);
            Assert.AreEqual("Name7", result.QuestionList.ElementAt(4).Name);
            #endregion Assert
        }
        #endregion Index Tests
    }
}
