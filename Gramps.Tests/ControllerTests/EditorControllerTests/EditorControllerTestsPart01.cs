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
        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsToHomeControllerIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(null, 5)
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
        public void TestIndexRedirectsToHomeControllerIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        [TestMethod]
        public void TestIndexReturnsViewWithExpectedData1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, 5)
                .AssertViewRendered()
                .WithViewData<EditorListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(3, result.EditorList.Count());
            Assert.AreEqual("ReviewerName1", result.EditorList.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName2", result.EditorList.ElementAt(1).ReviewerName);
            Assert.AreEqual("ReviewerName4", result.EditorList.ElementAt(2).ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsViewWithExpectedData2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(3, null)
                .AssertViewRendered()
                .WithViewData<EditorListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(2, result.EditorList.Count());
            Assert.AreEqual("ReviewerName5", result.EditorList.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName7", result.EditorList.ElementAt(1).ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        #endregion Index Tests

        #region AddEditor Tests
        #region AddedEditor Get Tests                
        [TestMethod]
        public void TestAddEditorGetRedirectsToHomeControllerIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AddEditor(null, 5)
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
        public void TestAddEditorGetRedirectsToHomeControllerIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AddEditor(3, null)
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
        public void TestAddEdditorGetReturnsViewWithExpectedValues1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(null, 5)
                .AssertViewRendered()
                .WithViewData<AddEditorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Users.Count());
            Assert.IsNull(result.User);
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
        public void TestAddEdditorGetReturnsViewWithExpectedValues2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(3, null)
                .AssertViewRendered()
                .WithViewData<AddEditorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Users.Count());
            Assert.IsNull(result.User);
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
        #endregion AddedEditor Get Tests
        #region AddEditor Post Tests

        [TestMethod]
        public void TestAddEditorPostRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AddEditor(null, 5, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddEditorPostRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AddEditor(3, null, 2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        [TestMethod]
        public void TestAddEditorPostRedirectsWithErrorMessageWhenUserIdNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.AddEditor(null, 5, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to add editor User not found", Controller.Message);
            Controller.ModelState.AssertErrorsAre("User not found", "ReviewerEmailRequired: Reviewer must have an email");
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddEditorPostRedirectsWithErrorMessageWhenUserIdNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.AddEditor(3, null, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to add editor User not found", Controller.Message);
            Controller.ModelState.AssertErrorsAre("User not found", "ReviewerEmailRequired: Reviewer must have an email");
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestAddEditorPostRedirectsWithErrorMessageWhenUserAlreadyExists1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(null, 5, 1)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to add editor User already exists", Controller.Message);
            Controller.ModelState.AssertErrorsAre("User already exists");
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

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
        public void TestAddEditorPostRedirectsWithErrorMessageWhenUserAlreadyExists2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(3, null, 1)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Unable to add editor User already exists", Controller.Message);
            Controller.ModelState.AssertErrorsAre("User already exists");
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));

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

        [TestMethod]
        public void TestAddEditorPostRedirectsWithSuccessMessageWhenUserAddIsValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(null, 5, 2)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Editor Added", Controller.Message);           
            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            var args1 = (Editor)EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(5, args1.CallForProposal.Id);
            Assert.IsNull(args1.Template);
            Assert.AreEqual(2, args1.User.Id);
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
        public void TestAddEditorPostRedirectsWithSuccessMessageWhenUserAddIsValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.AddEditor(3, null, 2)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            Assert.AreEqual("Editor Added", Controller.Message);
            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            var args1 = (Editor)EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything))[0][0];
            Assert.IsNotNull(args1);
            Assert.AreEqual(3, args1.Template.Id);
            Assert.IsNull(args1.CallForProposal);
            Assert.AreEqual(2, args1.User.Id);
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
        #endregion AddEditor Post Tests
        #endregion AddEditor Tests
    }
}
