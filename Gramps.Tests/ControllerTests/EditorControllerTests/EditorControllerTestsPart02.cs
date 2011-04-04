using System;
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
        #region CreateReviewer Tests
        #region CreateReviewer Get Tests

        [TestMethod]
        public void TestCreateReviewerGetRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.CreateReviewer(null, 5)
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
        public void TestCreateReviewerGetRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.CreateReviewer(3, null)
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
        public void TestCreateReviewerGetReturnsViewWithExpectedValues1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.CreateReviewer(null, 5)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(5, result.CallForProposalId);
            Assert.AreEqual(0, result.TemplateId);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateReviewerGetReturnsViewWithExpectedValues2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.CreateReviewer(3, null)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.AreEqual(3, result.TemplateId);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCreateReviewerExceptionIsThrownIfBothIdsAreNull()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
                AccessService.Expect(a => a.HasAccess(null, null, "Me")).Return(true).Repeat.Any();
                SetupDataForTests();
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.CreateReviewer(null, null);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Must have either a template or a call for proposal", ex.Message);
                throw;
            }	
        }
        #endregion CreateReviewer Get Tests
        #region CreateReviewer Post Tests
        [TestMethod]
        public void TestCreateReviewerPostRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.CreateReviewer(null, 5, new Editor())
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
        public void TestCreateReviewerPostRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.CreateReviewer(3, null, new Editor())
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
        public void TestCreateReviewerPostReturnsViewWithExpectedDataWhenEditorIsInvalid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            var editor = CreateValidEntities.Editor(1);
            editor.ReviewerEmail = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.CreateReviewer(null, 5, editor)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("ReviewerEmailRequired: Reviewer must have an email");
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            Assert.IsNotNull(result);
            Assert.AreEqual(editor.ReviewerId, result.Editor.ReviewerId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(5, result.CallForProposalId);
            Assert.AreEqual(0, result.TemplateId);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateReviewerPostReturnsViewWithExpectedDataWhenEditorIsInvalid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            var editor = CreateValidEntities.Editor(1);
            editor.ReviewerEmail = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.CreateReviewer(3, null, editor)
                .AssertViewRendered()
                .WithViewData<EditorViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("ReviewerEmailRequired: Reviewer must have an email");
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            Assert.IsNotNull(result);
            Assert.AreEqual(editor.ReviewerId, result.Editor.ReviewerId);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.AreEqual(3, result.TemplateId);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateReviewerPostRedirectsWithSuccessMessageWhenReviewerAddIsValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            var editor = CreateValidEntities.Editor(1);
            #endregion Arrange

            #region Act
            var result = Controller.CreateReviewer(null, 5, editor)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer Created Successfully.", Controller.Message);
            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            var args1 = (Editor)EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(5, args1.CallForProposal.Id);
            Assert.IsNull(args1.Template);
            Assert.AreEqual(editor.ReviewerId, args1.ReviewerId);
            Assert.AreEqual(editor.ReviewerName, args1.ReviewerName);
            Assert.AreEqual(editor.ReviewerEmail, args1.ReviewerEmail);
            Assert.IsFalse(args1.IsOwner);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAtOrDefault(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAtOrDefault(2).Key);
            Assert.AreEqual(5, result.RouteValues.ElementAtOrDefault(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAtOrDefault(3).Key);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateReviewerPostRedirectsWithSuccessMessageWhenReviewerAddIsValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            var editor = CreateValidEntities.Editor(1);
            #endregion Arrange

            #region Act
            var result = Controller.CreateReviewer(3, null, editor)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Reviewer Created Successfully.", Controller.Message);
            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            var args1 = (Editor)EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(3, args1.Template.Id);
            Assert.IsNull(args1.CallForProposal);
            Assert.AreEqual(editor.ReviewerId, args1.ReviewerId);
            Assert.AreEqual(editor.ReviewerName, args1.ReviewerName);
            Assert.AreEqual(editor.ReviewerEmail, args1.ReviewerEmail);
            Assert.IsFalse(args1.IsOwner);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAtOrDefault(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAtOrDefault(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAtOrDefault(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAtOrDefault(3).Key);
            #endregion Assert
        }
        #endregion CreateReviewer Post Tests
        #endregion CreateReviewer Tests
    }
}
