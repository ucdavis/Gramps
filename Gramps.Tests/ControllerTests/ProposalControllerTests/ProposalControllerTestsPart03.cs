using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region AdminDownload Tests
        [TestMethod]
        public void TestAdminDownloadRedirectsWhenCallForProposalNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.AdminDownload(1, 4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(2).Value);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminDownloadRedirectsWhenNoAccess()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminDownload(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminDownloadRedirectsWhenNotSameId()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                            Arg<int?>.Is.Anything)).Return(false).Repeat.Any();
            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals[0].CallForProposal = CallForProposalRepository.GetNullableById(2); //different
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(2, ProposalRepository, proposals);

            #endregion Arrange

            #region Act
            Controller.AdminDownload(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminDownloadRedirectsWhenProposalNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                            Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var proposals = new FakeProposals();
            proposals.Records(3, ProposalRepository);
            #endregion Arrange

            #region Act
            Controller.AdminDownload(4, 3)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminDownloadRedirectsWhenProposalPdfNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                            Arg<int?>.Is.Anything)).Return(true).Repeat.Any();

            var proposalList = new List<Proposal>();
            proposalList.Add(CreateValidEntities.Proposal(1));
            proposalList[0].File = null;
            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);
            #endregion Arrange

            #region Act
            Controller.AdminDownload(1, 3)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal PDF Not Found", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestAdminDownloadReturnsFileWhenProposalPdfFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                            Arg<int?>.Is.Anything)).Return(true).Repeat.Any();

            var proposalList = new List<Proposal>();
            proposalList.Add(CreateValidEntities.Proposal(1));
            proposalList[0].File = CreateValidEntities.File(1);
            proposalList[0].File.Contents = new byte[]{1,2,3,5};
            proposalList[0].File.ContentType = "somePdf";
            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);
            #endregion Arrange

            #region Act
            var result = Controller.AdminDownload(1, 3)
                .AssertResultIs<FileContentResult>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Name1", result.FileDownloadName);
            Assert.AreEqual("somePdf", result.ContentType);
            Assert.AreEqual("1235", result.FileContents.ByteArrayToString());
            #endregion Assert
        }
        #endregion AdminDownload Tests

        #region SendCall Tests
        [TestMethod]
        public void TestSendCallRedirectsWhenCallForProposalNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(4, true)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(2).Value);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestSendCallRedirectsWhenNoAccess()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.SendCall(3, true)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestSendCallRedirectsWhenNotActive()
        {
            #region Arrange
            var callForProposals = new List<CallForProposal>();
            callForProposals.Add(CreateValidEntities.CallForProposal(1));
            callForProposals[0].IsActive = false;
            callForProposals[0].EndDate = DateTime.Now.AddDays(+1);
            var calls = new FakeCallForProposals();
            calls.Records(2, CallForProposalRepository, callForProposals);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, true)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Is not active or end date is passed", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            Assert.IsNull(result.RouteValues.ElementAt(5).Value);
            Assert.IsNull(result.RouteValues.ElementAt(6).Value);
            Assert.IsNull(result.RouteValues.ElementAt(7).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestSendCallRedirectsWhenEndDateHasPassed()
        {
            #region Arrange
            var callForProposals = new List<CallForProposal>();
            callForProposals.Add(CreateValidEntities.CallForProposal(1));
            callForProposals[0].IsActive = true;
            callForProposals[0].EndDate = DateTime.Now.AddDays(-1);
            var calls = new FakeCallForProposals();
            calls.Records(2, CallForProposalRepository, callForProposals);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, true)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Is not active or end date is passed", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            Assert.IsNull(result.RouteValues.ElementAt(5).Value);
            Assert.IsNull(result.RouteValues.ElementAt(6).Value);
            Assert.IsNull(result.RouteValues.ElementAt(7).Value);
            #endregion Assert
        }


        [TestMethod]
        public void TestSendCallRedirectsWhenSuccessful()
        {
            #region Arrange
            var proposals = new List<Proposal>();
            for (int i = 0; i < 5; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i+1));
                proposals[i].IsSubmitted = false;
                proposals[i].WasWarned = false;
            }
            proposals[1].IsSubmitted = true;
            proposals[2].WasWarned = true;
            proposals[3].IsSubmitted = true;
            proposals[3].WasWarned = true;

            var callForProposals = new List<CallForProposal>();
            callForProposals.Add(CreateValidEntities.CallForProposal(1));
            callForProposals[0].IsActive = true;
            callForProposals[0].EndDate = DateTime.Now.AddDays(+1);
            callForProposals[0].Proposals = proposals;
            callForProposals[0].EmailTemplates = new List<EmailTemplate>();
            callForProposals[0].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            callForProposals[0].EmailTemplates[0].TemplateType = EmailTemplateType.ReminderCallIsAboutToClose;

            var calls = new FakeCallForProposals();
            calls.Records(2, CallForProposalRepository, callForProposals);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();

            EmailService.Expect(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, true)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            EmailService.AssertWasCalled(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything), x => x.Repeat.Times(2));
            Assert.AreEqual("2 Emails Generated", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            Assert.IsNull(result.RouteValues.ElementAt(5).Value);
            Assert.IsNull(result.RouteValues.ElementAt(6).Value);
            Assert.IsNull(result.RouteValues.ElementAt(7).Value);

            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything), x=> x.Repeat.Times(2));
            var args = ProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything)); 
            Assert.IsTrue(((Proposal)args[0][0]).WasWarned);
            Assert.IsTrue(((Proposal)args[1][0]).WasWarned);
            #endregion Assert
        }
        #endregion SendCall Tests
    }
}
