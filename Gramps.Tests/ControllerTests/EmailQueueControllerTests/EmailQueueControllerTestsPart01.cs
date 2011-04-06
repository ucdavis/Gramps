using System.Collections.Generic;
using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;



namespace Gramps.Tests.ControllerTests.EmailQueueControllerTests
{
    public partial class EmailQueueControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsToCallForProposalIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Index(4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }



        [TestMethod]
        public void TestIndexRedirectsToHomeIfNoAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(3)
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
        public void TestIndexReturnsViewWithExpectedValuesIfHasAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(true).Repeat.Any();

            var fakeEmails = new List<EmailQueue>();
            for (int i = 0; i < 4; i++)
            {
                fakeEmails.Add(CreateValidEntities.EmailQueue(i + 1));
                fakeEmails[i].CallForProposal = CallForProposalRepository.GetNullableById(2);
            }
            fakeEmails[1].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmailQueue = new FakeEmailQueues();
            fakeEmailQueue.Records(0, EmailQueueRepository, fakeEmails);
            #endregion Arrange

            #region Act
            var result = Controller.Index(2)
                .AssertViewRendered()
                .WithViewData<EmailQueueListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.EmailQueues.Count());
            #endregion Assert
        }

        #endregion Index Tests

        #region Details Tests
        [TestMethod]
        public void TestDetailsRedirectsToCallForProposalIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4, 5)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsRedirectsToHomeIfNoAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Details(4, 3)
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
        public void TestDetailsRedirectsToIndexIfEmailNotFound()
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
            var result = Controller.Details(4, 3)
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
        public void TestDetailsRedirectsToHomeIndexIfIdDifferent()
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
            Controller.Details(1, 3)
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
        public void TestDetailsReturnsViewWithExpectedResults()
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
            var result = Controller.Details(2, 3)
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
        #endregion Details Tests
    }
}
