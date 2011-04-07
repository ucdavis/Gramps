using System.Collections.Generic;
using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


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
            EmailQueueRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything));
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
            EmailQueueRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything));
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
            EmailQueueRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything));
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

            EmailQueueRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid1()
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
            var email = CreateValidEntities.EmailQueue(99);
            email.Subject = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 3, email)
                .AssertViewRendered()
                .WithViewData<EmailQueueViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Subject: may not be null or empty");
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.EmailQueue.Subject);
            Assert.AreEqual("Body99", result.EmailQueue.Body);
            EmailQueueRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything));

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
            #endregion Assert	
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid2()
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
                fakeEmail[i].Pending = false;
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);
            var email = CreateValidEntities.EmailQueue(99);
            email.Pending = false;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 3, email)
                .AssertViewRendered()
                .WithViewData<EmailQueueViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Can't edit an email that was already sent");
            Assert.IsNotNull(result);
            Assert.AreEqual("Subject99", result.EmailQueue.Subject);
            Assert.AreEqual("Body99", result.EmailQueue.Body);
            EmailQueueRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything));

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
            #endregion Assert
        }


        [TestMethod]
        public void TestEditRedirectsWhenValidAndSuccessful()
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
                fakeEmail[i].Pending = false;
                fakeEmail[i].Immediate = false;
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);
            var email = CreateValidEntities.EmailQueue(99);
            email.Immediate = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 3, email)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("EmailQueue Edited Successfully.", Controller.Message);
            Assert.IsNotNull(result);

            EmailQueueRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything));
            var args3 = (EmailQueue)EmailQueueRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailQueue>.Is.Anything))[0][0];
            Assert.IsNotNull(args3);
            Assert.AreEqual("Subject99", args3.Subject);
            Assert.IsTrue(args3.Immediate);
            Assert.AreEqual("Body99", args3.Body);

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
            #endregion Assert	
        }

        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
