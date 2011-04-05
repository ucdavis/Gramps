using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EditorControllerTests
{
    public partial class EditorControllerTests
    {
        #region EditReviewer Tests
        #region EditReviewer Get Tests
        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(2, null, 5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(2, 3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenEditorNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(9, null, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer not found.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(5, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenEditorNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(9, 3, null)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(1, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer not found.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenEditorNotReviewer1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(2, null, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Not a reviewer", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(5, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenEditorNotReviewer2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(7, 3, null)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Not a reviewer", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenEditorDoesNotHaveSameIdAsParameter1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, null, 5)).Return(false).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(8, null, 5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template) args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetRedirectsWhenEditorDoesNotHaveSameIdAsParameter2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(5), 3, null)).Return(false).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(1, 3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal) args2[1]).Id);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetReturnsViewWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(1, null, 5)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(5, result.CallForProposalId);
            Assert.AreEqual(0, result.TemplateId);
            Assert.IsNotNull(result.Editor);
            Assert.AreEqual("ReviewerName1", result.Editor.ReviewerName);            

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal) args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerGetReturnsViewWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(5, 3, null)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.AreEqual(3, result.TemplateId);
            Assert.IsNotNull(result.Editor);
            Assert.AreEqual("ReviewerName5", result.Editor.ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(3, ((Template) args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }
        #endregion EditReviewer Get Tests
        #region EditReviewer Post Tests
        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(2, null, 5, new Editor())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(2, 3, null, new Editor())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenEditorNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(9, null, 5, new Editor())
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer not found.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(5, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenEditorNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(9, 3, null, new Editor())
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(1, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer not found.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenEditorNotReviewer1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(2, null, 5, new Editor())
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Not a reviewer", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(5, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenEditorNotReviewer2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(7, 3, null, new Editor())
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Not a reviewer", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenEditorDoesNotHaveSameIdAsParameter1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, null, 5)).Return(false).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(8, null, 5, new Editor())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template) args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenEditorDoesNotHaveSameIdAsParameter2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(5), 3, null)).Return(false).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.EditReviewer(1, 3, null, new Editor())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal) args2[1]).Id);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            SetupDataForTests2();
            var editor = CreateValidEntities.Editor(99);
            editor.HasBeenNotified = true;
            editor.ReviewerEmail = editor.ReviewerEmail.ToUpper();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(1, null, 5, editor)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer Edited Successfully.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(5, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            var args1 = (Editor) EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args1);
            Assert.AreNotEqual(editor.ReviewerEmail.ToLower(), args1.ReviewerEmail);
            Assert.AreEqual(editor.ReviewerId, args1.ReviewerId);
            Assert.AreEqual(editor.ReviewerName, args1.ReviewerName);
            Assert.AreEqual(editor.HasBeenNotified, args1.HasBeenNotified);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal) args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostRedirectsWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            SetupDataForTests2();
            var editor = CreateValidEntities.Editor(99);
            editor.HasBeenNotified = true;
            editor.ReviewerEmail = editor.ReviewerEmail.ToUpper();
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(5, 3, null, editor)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer Edited Successfully.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            var args1 = (Editor)EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(editor.ReviewerEmail.ToLower(), args1.ReviewerEmail);
            Assert.AreNotEqual(editor.ReviewerEmail, args1.ReviewerEmail);
            Assert.AreEqual(editor.ReviewerId, args1.ReviewerId);
            Assert.AreEqual(editor.ReviewerName, args1.ReviewerName);
            Assert.AreNotEqual(editor.HasBeenNotified, args1.HasBeenNotified); //Not changed with a template

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(3, ((Template) args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostReturnsViewWhenNotValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            SetupDataForTests2();
            var editor = CreateValidEntities.Editor(99);
            editor.HasBeenNotified = true;
            editor.ReviewerName = "x".RepeatTimes(201);
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(1, null, 5, editor)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(editor.HasBeenNotified, result.Editor.HasBeenNotified);
            Assert.AreEqual(editor.ReviewerName, result.Editor.ReviewerName);
            Assert.AreNotEqual(editor.ReviewerEmail, result.Editor.ReviewerEmail);

            Controller.ModelState.AssertErrorsAre("ReviewerName: length must be between 0 and 200");

            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal) args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditReviewerPostReturnsViewWhenNotValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            SetupDataForTests2();
            var editor = CreateValidEntities.Editor(99);
            editor.HasBeenNotified = true;
            editor.ReviewerEmail = "";
            #endregion Arrange

            #region Act
            var result = Controller.EditReviewer(5, 3, null, editor)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(editor.HasBeenNotified, result.Editor.HasBeenNotified); //Not changed with a template
            Assert.AreEqual(editor.ReviewerName, result.Editor.ReviewerName);
            Assert.AreEqual(editor.ReviewerEmail, result.Editor.ReviewerEmail);

            Controller.ModelState.AssertErrorsAre("ReviewerEmailRequired: Reviewer must have an email");

            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(3, ((Template) args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }
        #endregion EditReviewer Post Tests
        #endregion EditReviewer Tests
    }
}
