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
        [TestMethod]
        public void TestMoveUpRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.MoveUp(3, 2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.MoveUp(3, null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenQuestionNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(13, null, 3)
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
        public void TestMoveUpWhenQuestionNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(13, 2, null)
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
        public void TestMoveUpWhenIdNotSame1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData1();
            #endregion Arrange

            #region Act

            Controller.MoveUp(3, 2, null)
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
        public void TestMoveUpWhenIdNotSame2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(false);
            SetupData1();
            #endregion Arrange

            #region Act

            Controller.MoveUp(8, null, 3)
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
        public void TestMoveUpWhenAtTopAlready1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(6, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Not moved.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenAtTopAlready2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(12, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Not moved.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(5, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question) QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0]; 
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0]; 
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name5", args1.Name);
            Assert.AreEqual("Name6", args2.Name);
            Assert.AreEqual(7, args1.Order);
            Assert.AreEqual(8, args2.Order);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(4, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0];
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name4", args1.Name);
            Assert.AreEqual("Name5", args2.Name);
            Assert.AreEqual(8, args1.Order);
            Assert.AreEqual(9, args2.Order);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(3, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0];
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name3", args1.Name);
            Assert.AreEqual("Name4", args2.Name);
            Assert.AreEqual(9, args1.Order);
            Assert.AreEqual(10, args2.Order);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(1, 2, null)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, 2, null));
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0];
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name1", args1.Name);
            Assert.AreEqual("Name3", args2.Name);
            Assert.AreEqual(10, args1.Order);
            Assert.AreEqual(12, args2.Order);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(10, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0];
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name10", args1.Name);
            Assert.AreEqual("Name12", args2.Name);
            Assert.AreEqual(1, args1.Order);
            Assert.AreEqual(3, args2.Order);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(9, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0];
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name9", args1.Name);
            Assert.AreEqual("Name10", args2.Name);
            Assert.AreEqual(3, args1.Order);
            Assert.AreEqual(4, args2.Order);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(8, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0];
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name8", args1.Name);
            Assert.AreEqual("Name9", args2.Name);
            Assert.AreEqual(2, args1.Order);
            Assert.AreEqual(3, args2.Order);
            #endregion Assert
        }

        [TestMethod]
        public void TestMoveUpWhenNotAtTop8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true);
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.MoveUp(7, null, 3)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Moved Up.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            AccessService.AssertWasCalled(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(3), null, 3));
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            var args2 = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[1][0];
            Assert.IsNotNull(args1);
            Assert.IsNotNull(args2);
            Assert.AreEqual("Name7", args1.Name);
            Assert.AreEqual("Name8", args2.Name);
            Assert.AreEqual(1, args1.Order);
            Assert.AreEqual(2, args2.Order);
            #endregion Assert
        }
    }
}
