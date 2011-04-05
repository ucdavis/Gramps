using System.Collections.Generic;
using System.Linq;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EditorControllerTests
{
    public partial class EditorControllerTests
    {
        #region Delete Tests
        [TestMethod]
        public void TestDeleteRedirectsWhenNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(2, null, 5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(2, 3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenEditorNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(9, null, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, 5));
            #endregion Act

            #region Assert
            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));
            Assert.AreEqual("Editor not found.", Controller.Message);

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
        public void TestDeleteRedirectsWhenEditorNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(9, 3, null)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(1, null));
            #endregion Act

            #region Assert
            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));
            Assert.AreEqual("Editor not found.", Controller.Message);

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
        public void TestDeleteRedirectsWhenEditorDoesNotHaveSameIdAsParameter1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(TemplateRepository.GetNullableById(2), null, null, 5)).Return(false).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.Delete(8, null, 5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenEditorDoesNotHaveSameIdAsParameter2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(null, CallForProposalRepository.GetNullableById(5), 3, null)).Return(false).Repeat.Any();
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            Controller.Delete(1, 3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenOwner1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].IsOwner = true;
            editors[0].User = CreateValidEntities.User(1);
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(5);
            editors[0].Template = null;
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);

            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, null, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Can't delete owner.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(5, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);

            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenOwner2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].IsOwner = true;
            editors[0].User = CreateValidEntities.User(1);
            editors[0].CallForProposal = null;
            editors[0].Template = TemplateRepository.GetNullableById(3);
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, 3, null)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            EditorRepository.AssertWasNotCalled(a => a.Remove(Arg<Editor>.Is.Anything));
            Assert.AreEqual("Can't delete owner.", Controller.Message);

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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(3, ((Template)args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteWhenEditorIsUserAndvalid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].User = CreateValidEntities.User(1);
            editors[0].CallForProposal = null;
            editors[0].Template = TemplateRepository.GetNullableById(3);
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, 3, null)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert            
            Assert.AreEqual("Editor Removed Successfully.", Controller.Message);
           EditorRepository.AssertWasCalled(a => a.Remove(editors[0]));


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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(3, ((Template)args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeleteWhenEditorIsUserAndvalid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].User = CreateValidEntities.User(1);
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(5);
            editors[0].Template = null;
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, null, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Editor Removed Successfully.", Controller.Message);
            EditorRepository.AssertWasCalled(a => a.Remove(editors[0]));


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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenEditorIsNotUserAndvalid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].User = null;
            editors[0].CallForProposal = null;
            editors[0].Template = TemplateRepository.GetNullableById(3);
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, 3, null)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Editor Removed Successfully.", Controller.Message);
            EditorRepository.AssertWasCalled(a => a.Remove(editors[0]));


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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(3, ((Template)args2[0]).Id);
            Assert.AreEqual(null, args2[1]);
            Assert.AreEqual(3, args2[2]);
            Assert.AreEqual(null, args2[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenEditorIsNotUserAndvalid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].User = null;
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(5);
            editors[0].Template = null;
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
            #endregion Arrange

            #region Act
            var result = Controller.Delete(1, null, 5)
                .AssertActionRedirect()
                .ToAction<EditorController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Editor Removed Successfully.", Controller.Message);
            EditorRepository.AssertWasCalled(a => a.Remove(editors[0]));


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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(5, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(5, args2[3]);
            #endregion Assert
        }
        #endregion Delete Tests
    }
}
