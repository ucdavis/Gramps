using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace Gramps.Tests.ControllerTests.CallForProposalControllerTests
{
    public partial class CallForProposalControllerTests
    {
        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsToIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(1, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(2)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditGetRedirectsToHomeControllerIfNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(false).Repeat.Any();
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(1, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditGetReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(1, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1)
                .AssertViewRendered()
                .WithViewData<CallForProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name1", result.CallForProposal.Name);
            Assert.AreEqual(1, result.CallForProposalId);
            Assert.IsNull(result.TemplateId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }       
        #endregion Edit Get Tests
        #region Edit Post Tests

        [TestMethod]
        public void TestEditPostRedirectsToIndexWhenCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(1, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(2, new CallForProposal())
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditPostRedirectsToHomeIndexIfNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(false).Repeat.Any();
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(1, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(1, new CallForProposal())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditPostChecksProposalMaximum()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].AddEditor(CreateValidEntities.Editor(1));
            calls[0].Editors[0].User = CreateValidEntities.User(1);
            calls[0].Editors[0].IsOwner = true;
            calls[0].SetIdTo(1);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
            var editcall = CreateValidEntities.CallForProposal(9);
            editcall.ProposalMaximum = 0m;
            editcall.Description = "Desc";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, editcall)
                .AssertViewRendered()
                .WithViewData<CallForProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("You need to specify a Proposal Maximum of at least a cent");
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.CallForProposal.Name);
            Assert.AreEqual("Desc", result.CallForProposal.Description);
            Assert.AreEqual(1, result.CallForProposalId);
            Assert.IsNull(result.TemplateId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostChecksDescriptionIfActive()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].AddEditor(CreateValidEntities.Editor(1));
            calls[0].Editors[0].User = CreateValidEntities.User(1);
            calls[0].Editors[0].IsOwner = true;
            calls[0].SetIdTo(1);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
            var editcall = CreateValidEntities.CallForProposal(9);
            editcall.Description = " ";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, editcall)
                .AssertViewRendered()
                .WithViewData<CallForProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Please supply a description");
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.CallForProposal.Name);
            Assert.AreEqual(" ", result.CallForProposal.Description);
            Assert.AreEqual(1, result.CallForProposalId);
            Assert.IsNull(result.TemplateId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostChecksIfOwnerIsMissing1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].AddEditor(CreateValidEntities.Editor(1));
            calls[0].Editors[0].User = CreateValidEntities.User(1);
            calls[0].Editors[0].IsOwner = false;
            calls[0].SetIdTo(1);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
            var editcall = CreateValidEntities.CallForProposal(9);
            editcall.IsActive = false;
            editcall.Description = " ";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, editcall)
                .AssertViewRendered()
                .WithViewData<CallForProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Owner: Owner is required");
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.CallForProposal.Name);
            Assert.AreEqual(" ", result.CallForProposal.Description);
            Assert.AreEqual(1, result.CallForProposalId);
            Assert.IsNull(result.TemplateId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostChecksIfOwnerIsMissing2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].AddEditor(CreateValidEntities.Editor(1));
            calls[0].Editors[0].User = CreateValidEntities.User(1);
            calls[0].Editors[0].IsOwner = false;
            calls[0].SetIdTo(1);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
            var editcall = CreateValidEntities.CallForProposal(9);
            editcall.IsActive = true;
            editcall.Description = "Desc";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, editcall)
                .AssertViewRendered()
                .WithViewData<CallForProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Owner: Owner is required");
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.CallForProposal.Name);
            Assert.AreEqual(1, result.CallForProposalId);
            Assert.IsNull(result.TemplateId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostChecksIfOwnerIsMissing3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            //calls[0].AddEditor(CreateValidEntities.Editor(1));
            //calls[0].Editors[0].User = CreateValidEntities.User(1);
            //calls[0].Editors[0].IsOwner = false;
            calls[0].SetIdTo(1);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
            var editcall = CreateValidEntities.CallForProposal(9);
            editcall.IsActive = false;
            editcall.Description = " ";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, editcall)
                .AssertViewRendered()
                .WithViewData<CallForProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Owner: Owner is required");
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.CallForProposal.Name);
            Assert.AreEqual(" ", result.CallForProposal.Description);
            Assert.AreEqual(1, result.CallForProposalId);
            Assert.IsNull(result.TemplateId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostChecksIfOwnerIsMissing4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            //calls[0].AddEditor(CreateValidEntities.Editor(1));
            //calls[0].Editors[0].User = CreateValidEntities.User(1);
            //calls[0].Editors[0].IsOwner = false;
            calls[0].SetIdTo(1);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
            var editcall = CreateValidEntities.CallForProposal(9);
            editcall.IsActive = true;
            editcall.Description = "Desc";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, editcall)
                .AssertViewRendered()
                .WithViewData<CallForProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Owner: Owner is required");
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.CallForProposal.Name);
            Assert.AreEqual(1, result.CallForProposalId);
            Assert.IsNull(result.TemplateId);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            CallForProposalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsToEditGetWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].AddEditor(CreateValidEntities.Editor(1));
            calls[0].Editors[0].User = CreateValidEntities.User(1);
            calls[0].Editors[0].IsOwner = true;
            calls[0].SetIdTo(1);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
            var editcall = CreateValidEntities.CallForProposal(9);
            editcall.IsActive = true;
            editcall.Description = "Updated";
            editcall.ProposalMaximum = 12.97m;
            editcall.EndDate = new DateTime(2009, 12, 23).Date;
            editcall.Name = "UName";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, editcall)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            //Assert.AreEqual(1, result.RouteValues.ElementAt(2));
            Assert.AreEqual("Call For Proposal Edited Successfully.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            CallForProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything));
            var args2 = (CallForProposal) CallForProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args2);
            Assert.AreEqual("UName", args2.Name);
            Assert.AreEqual("Updated", args2.Description);
            Assert.AreEqual(12.97m, args2.ProposalMaximum);
            Assert.AreEqual(new DateTime(2009, 12, 23).Date, args2.EndDate.Date);
            #endregion Assert		
        }
        

        #endregion Edit Post Tests
        #endregion Edit Tests

    }
}
