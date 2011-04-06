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
using Gramps.Tests.Core.Helpers;
using Gramps.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;



namespace Gramps.Tests.ControllerTests.EmailQueueControllerTests
{
    public partial class EmailQueueControllerTests
    {
        #region Edit Tests
        #region Edit Get Tests
        [TestMethod]
        public void TestEditGetRedirectsToCallForProposalIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4, 5)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToHomeIfNoAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(4, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToIndexIfEmailNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(3, EmailQueueRepository);

            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 3)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Email not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);

            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToHomeIndexIfIdDifferent()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(
                Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything)).Return(false).Repeat.Any();

            var fakeEmail = new List<EmailQueue>();
            for (int i = 0; i < 3; i++)
            {
                fakeEmail.Add(CreateValidEntities.EmailQueue(i + 1));
                fakeEmail[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);

            #endregion Arrange

            #region Act
            Controller.Edit(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(3, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(1, args2[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedResults()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(
                Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything)).Return(true).Repeat.Any();

            var fakeEmail = new List<EmailQueue>();
            for (int i = 0; i < 3; i++)
            {
                fakeEmail.Add(CreateValidEntities.EmailQueue(i + 1));
                fakeEmail[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 3)
                .AssertViewRendered()
                .WithViewData<EmailQueueViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(3, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(3, args2[3]);

            Assert.IsNotNull(result);
            Assert.AreEqual("Subject2", result.EmailQueue.Subject);
            Assert.AreEqual(3, result.CallForProposal.Id);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostRedirectsToCallForProposalIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4, 5, new EmailQueue())
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToHomeIfNoAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(4, 3, new EmailQueue())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToIndexIfEmailNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(3, EmailQueueRepository);

            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 3, new EmailQueue())
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Email not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);

            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToHomeIndexIfIdDifferent()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(
                Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything)).Return(false).Repeat.Any();

            var fakeEmail = new List<EmailQueue>();
            for (int i = 0; i < 3; i++)
            {
                fakeEmail.Add(CreateValidEntities.EmailQueue(i + 1));
                fakeEmail[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);

            #endregion Arrange

            #region Act
            Controller.Edit(1, 3, new EmailQueue())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(3, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(1, args2[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Continue edit post tests (validation and valid edit)");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
