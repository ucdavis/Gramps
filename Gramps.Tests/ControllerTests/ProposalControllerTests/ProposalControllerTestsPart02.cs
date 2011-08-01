using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region AdminDetails Tests

        [TestMethod]
        public void TestAdminDetailsRedirectsWhenCallForProposalNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.AdminDetails(1, 4)
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
        public void TestAdminDetailsRedirectsWhenNoAccess()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminDetails(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminDetailsRedirectsWhenNotSameId()
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
            #endregion Arrange

            #region Act
            Controller.AdminDetails(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminDetailsRedirectsWhenProposalNotFound()
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
            Controller.AdminDetails(4, 3)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Found", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestAdminDetailsReturnsView1()
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

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(3);
            editors[0].User = CreateValidEntities.User(1);
            editors[0].User.LoginId = "me";
            var fakedEditors = new FakeEditors();
            fakedEditors.Records(3, EditorRepository, editors);
            var reviewedProposals = new List<ReviewedProposal>();
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(1));
            reviewedProposals[0].Editor = EditorRepository.GetNullableById(1);
            reviewedProposals[0].Proposal = ProposalRepository.GetNullableById(2);
            reviewedProposals[0].FirstViewedDate = new DateTime(2011,01,01);
            var fakedReviewedProposal = new FakeReviewedProposals();
            fakedReviewedProposal.Records(0, ReviewedProposalRepository, reviewedProposals);

            #endregion Arrange

            #region Act
            var result = Controller.AdminDetails(2, 3)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            ReviewedProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything));
            var args = (ReviewedProposal) ReviewedProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual(DateTime.Now.Date, args.LastViewedDate.Date);
            Assert.AreEqual(new DateTime(2011, 01, 01), args.FirstViewedDate);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposal.Id);
            Assert.AreEqual(2, result.Proposal.Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAdminDetailsReturnsView2()
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

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(3);
            editors[0].User = CreateValidEntities.User(1);
            editors[0].User.LoginId = "me";
            var fakedEditors = new FakeEditors();
            fakedEditors.Records(3, EditorRepository, editors);
            var reviewedProposals = new List<ReviewedProposal>();
            reviewedProposals.Add(CreateValidEntities.ReviewedProposal(1));
            reviewedProposals[0].Editor = EditorRepository.GetNullableById(1);
            reviewedProposals[0].Proposal = ProposalRepository.GetNullableById(2);
            reviewedProposals[0].FirstViewedDate = new DateTime(2011, 01, 01);
            var fakedReviewedProposal = new FakeReviewedProposals();
            fakedReviewedProposal.Records(0, ReviewedProposalRepository, reviewedProposals);

            #endregion Arrange

            #region Act
            var result = Controller.AdminDetails(1, 3)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            ReviewedProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything));
            var args = (ReviewedProposal)ReviewedProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(DateTime.Now.Date, args.LastViewedDate.Date);
            Assert.AreEqual(DateTime.Now.Date, args.FirstViewedDate.Date);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposal.Id);
            Assert.AreEqual(1, result.Proposal.Id);
            #endregion Assert
        }
        #endregion AdminDetails Tests

    }
}
