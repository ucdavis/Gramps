using System.Collections.Generic;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;

namespace Gramps.Tests.ControllerTests.EmailQueueControllerTests
{
    public partial class EmailQueueControllerTests
    {
        #region Delete Tests

        [TestMethod]
        public void TestDeleteRedirectsToIndexIfNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(999, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            EmailQueueRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailQueue>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestDeleteRedirectsWithoutMessageWhenCanNotFind1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var emails = new List<EmailQueue>();
            emails.Add(CreateValidEntities.EmailQueue(1));
            emails.Add(CreateValidEntities.EmailQueue(2));
            emails.Add(CreateValidEntities.EmailQueue(3));
            emails[0].CallForProposal = null;
            emails[1].CallForProposal = CreateValidEntities.CallForProposal(1);
            emails[1].CallForProposal.SetIdTo(1);
            emails[2].CallForProposal = CreateValidEntities.CallForProposal(3);
            emails[2].CallForProposal.SetIdTo(3);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, emails);
            #endregion Arrange

            #region Act
            Controller.Delete(4, 3)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            EmailQueueRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailQueue>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestDeleteRedirectsWithoutMessageWhenCanNotFind2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var emails = new List<EmailQueue>();
            emails.Add(CreateValidEntities.EmailQueue(1));
            emails.Add(CreateValidEntities.EmailQueue(2));
            emails.Add(CreateValidEntities.EmailQueue(3));
            emails[0].CallForProposal = null;
            emails[1].CallForProposal = CreateValidEntities.CallForProposal(1);
            emails[1].CallForProposal.SetIdTo(1);
            emails[2].CallForProposal = CreateValidEntities.CallForProposal(3);
            emails[2].CallForProposal.SetIdTo(3);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, emails);
            #endregion Arrange

            #region Act
            Controller.Delete(1, 3)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            EmailQueueRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailQueue>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWithoutMessageWhenCanNotFind3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var emails = new List<EmailQueue>();
            emails.Add(CreateValidEntities.EmailQueue(1));
            emails.Add(CreateValidEntities.EmailQueue(2));
            emails.Add(CreateValidEntities.EmailQueue(3));
            emails[0].CallForProposal = null;
            emails[1].CallForProposal = CreateValidEntities.CallForProposal(1);
            emails[1].CallForProposal.SetIdTo(1);
            emails[2].CallForProposal = CreateValidEntities.CallForProposal(3);
            emails[2].CallForProposal.SetIdTo(3);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, emails);
            #endregion Arrange

            #region Act
            Controller.Delete(2, 3)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            EmailQueueRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailQueue>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteRedirectsToIndexWhenSuccessful()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var emails = new List<EmailQueue>();
            emails.Add(CreateValidEntities.EmailQueue(1));
            emails.Add(CreateValidEntities.EmailQueue(2));
            emails.Add(CreateValidEntities.EmailQueue(3));
            emails[0].CallForProposal = null;
            emails[1].CallForProposal = CreateValidEntities.CallForProposal(1);
            emails[1].CallForProposal.SetIdTo(1);
            emails[2].CallForProposal = CreateValidEntities.CallForProposal(3);
            emails[2].CallForProposal.SetIdTo(3);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, emails);
            #endregion Arrange

            #region Act
            Controller.Delete(3, 3)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("EmailQueue Removed Successfully.", Controller.Message);
            EmailQueueRepository.AssertWasCalled(a => a.Remove(emails[2]));
            #endregion Assert		
        }
        #endregion Delete Tests

    }
}
