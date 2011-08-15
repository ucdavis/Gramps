using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region Delete Tests
        [TestMethod]
        public void TestDeleteRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(3, 2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Delete(3, null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenQuestionNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(13, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question not found.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenQuestionNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(13, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question not found.", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteWhenIdNotSame1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData1();
            #endregion Arrange

            #region Act

            Controller.Delete(3, 2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenIdNotSame2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData1();
            #endregion Arrange

            #region Act

            Controller.Delete(8, null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteFailsWhenRelatedAnswers()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(8, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question has a related answer. Unable to Delete", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            QuestionRepository.AssertWasNotCalled(a => a.Remove(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(9, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question removed", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            QuestionRepository.AssertWasCalled(a => a.Remove(Arg<Question>.Is.Anything));
            var args = (Question) QuestionRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<Question>.Is.Anything))[0][0]; 
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(8, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question removed", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            //AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null)); //This one actually isn't called 
            QuestionRepository.AssertWasCalled(a => a.Remove(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(result);
            Assert.AreEqual("Name8", args.Name);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenValid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(3, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Question removed", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            QuestionRepository.AssertWasCalled(a => a.Remove(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", args.Name);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            #endregion Assert
        }
        #endregion Delete Tests
    }
}
