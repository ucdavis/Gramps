using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region SendDecision Tests

        [TestMethod]
        public void TestSendDecisionRedirectsWhenCallForProposalNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendDecision(4, true)
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
        public void TestSendDecisionRedirectsWhenNoAccess()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.SendDecision(3, true)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestSendDecisionRedirectsWhenNotActive()
        {
            #region Arrange
            var callForProposals = new List<CallForProposal>();
            callForProposals.Add(CreateValidEntities.CallForProposal(1));
            callForProposals[0].IsActive = false;
            callForProposals[0].EndDate = DateTime.Now.AddDays(-1);
            var calls = new FakeCallForProposals();
            calls.Records(2, CallForProposalRepository, callForProposals);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SendDecision(1, true)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Is not active or end date is not passed", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            Assert.IsNull(result.RouteValues.ElementAt(5).Value);
            Assert.IsNull(result.RouteValues.ElementAt(6).Value);
            Assert.IsNull(result.RouteValues.ElementAt(7).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestSendDecisionRedirectsWhenEndDateHasNotPassed()
        {
            #region Arrange
            var callForProposals = new List<CallForProposal>();
            callForProposals.Add(CreateValidEntities.CallForProposal(1));
            callForProposals[0].IsActive = true;
            callForProposals[0].EndDate = DateTime.Now.AddDays(+1);
            var calls = new FakeCallForProposals();
            calls.Records(2, CallForProposalRepository, callForProposals);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.SendDecision(1, true)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Is not active or end date is not passed", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            Assert.IsNull(result.RouteValues.ElementAt(5).Value);
            Assert.IsNull(result.RouteValues.ElementAt(6).Value);
            Assert.IsNull(result.RouteValues.ElementAt(7).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestSendDecisionRedirectsWhenSuccessful()
        {
            #region Arrange
            var proposals = new List<Proposal>();
            for (int i = 0; i < 7; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i + 1));
                proposals[i].IsNotified = false;
                proposals[i].IsApproved = true;
                proposals[i].IsDenied = false;
            }

            //These ones should not be notified
            proposals[1].IsNotified = true;
            proposals[1].IsApproved = false;
            proposals[1].IsDenied = false;
            proposals[2].IsNotified = true;
            proposals[2].IsApproved = true;
            proposals[2].IsDenied = false;
            proposals[3].IsNotified = true;
            proposals[3].IsApproved = false;
            proposals[3].IsDenied = true;
            proposals[4].IsNotified = false;
            proposals[4].IsApproved = false;
            proposals[4].IsDenied = false;

            proposals[5].IsNotified = false;
            proposals[5].IsApproved = false;
            proposals[5].IsDenied = true;

            var callForProposals = new List<CallForProposal>();
            callForProposals.Add(CreateValidEntities.CallForProposal(1));
            callForProposals[0].IsActive = true;
            callForProposals[0].EndDate = DateTime.Now.AddDays(-1);
            callForProposals[0].Proposals = proposals;
            callForProposals[0].EmailTemplates = new List<EmailTemplate>();
            callForProposals[0].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            callForProposals[0].EmailTemplates.Add(CreateValidEntities.EmailTemplate(2));
            callForProposals[0].EmailTemplates[0].TemplateType = EmailTemplateType.ProposalApproved;
            callForProposals[0].EmailTemplates[1].TemplateType = EmailTemplateType.ProposalDenied;

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
            var result = Controller.SendDecision(1, true)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            EmailService.AssertWasCalled(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything), x => x.Repeat.Times(3));
            Assert.AreEqual("3 Emails Generated. 2 Approved 1 Denied", Controller.Message);
            Assert.IsNotNull(result);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            Assert.IsNull(result.RouteValues.ElementAt(5).Value);
            Assert.IsNull(result.RouteValues.ElementAt(6).Value);
            Assert.IsNull(result.RouteValues.ElementAt(7).Value);

            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything), x => x.Repeat.Times(3));
            var args = ProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            Assert.IsTrue(((Proposal)args[0][0]).IsNotified);
            Assert.IsNotNull(((Proposal)args[0][0]).NotifiedDate);
            Assert.AreEqual(DateTime.Now.Date, ((Proposal)args[0][0]).NotifiedDate.Value.Date);
            Assert.IsTrue(((Proposal)args[1][0]).IsNotified);
            Assert.IsNotNull(((Proposal)args[1][0]).NotifiedDate);
            Assert.AreEqual(DateTime.Now.Date, ((Proposal)args[1][0]).NotifiedDate.Value.Date);
            Assert.IsTrue(((Proposal)args[2][0]).IsNotified);
            Assert.IsNotNull(((Proposal)args[2][0]).NotifiedDate);
            Assert.AreEqual(DateTime.Now.Date, ((Proposal)args[2][0]).NotifiedDate.Value.Date);
            #endregion Assert
        }

        #endregion SendDecision Tests

        #region AdminEdit Get Tests
        [TestMethod]
        public void TestAdminEditGetRedirectsWhenCallForProposalNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 4)
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
        public void TestAdminEditGetRedirectsWhenNoAccess()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminEdit(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditGetRedirectsWhenNotSameId()
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
            Controller.AdminEdit(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditGetRedirectsWhenProposalNotFound()
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
            Controller.AdminEdit(4, 3)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Found", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditGetReturnsView1()
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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);
            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(2, 3)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            ReviewedProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything));
            var args = (ReviewedProposal)ReviewedProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<ReviewedProposal>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(DateTime.Now.Date, args.LastViewedDate.Date);
            Assert.AreEqual(new DateTime(2011, 01, 01), args.FirstViewedDate);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposal.Id);
            Assert.AreEqual(2, result.Proposal.Id);
            Assert.AreEqual("Text1", result.Comment.Text);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditGetReturnsView2()
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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);
            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 3)
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
            Assert.IsNull(result.Comment);
            #endregion Assert
        }
        #endregion AdminEdit Get Tests

        #region AdminEdit Post Tests
        [TestMethod]
        public void TestAdminEditPostRedirectsWhenCallForProposalNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 4, null, null, null)
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
        public void TestAdminEditPostRedirectsWhenNoAccess()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminEdit(1, 3, null, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostRedirectsWhenNotSameId()
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
            Controller.AdminEdit(1, 3, null, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostRedirectsWhenProposalNotFound()
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
            Controller.AdminEdit(4, 3, null, null, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Not Found", Controller.Message);
            #endregion Assert
        }



        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestAdminEditPostWithInvalidChoiceThrowsException()
        {
            var thisFar = false;
            try
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

                var comments = new List<Comment>();
                comments.Add(CreateValidEntities.Comment(1));
                comments[0].Editor = EditorRepository.GetNullableById(1);
                comments[0].Proposal = ProposalRepository.GetNullableById(2);
                var fakedComments = new FakeComments();
                fakedComments.Records(0, CommentRepository, comments);
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.AdminEdit(1, 3, null, null, "BadChoice");
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar, "Some Other exception happened first.");
                Assert.IsNotNull(ex);
                Assert.AreEqual("Error with parameter", ex.Message);

                throw;
            }	
        }


        [TestMethod]
        public void TestAdminEditPostWhenAlreadyNotifiedDoesNotSave1()
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
            proposalList[0].IsNotified = true;
            proposalList[0].IsApproved = true;
            proposalList[0].IsDenied = false;
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);
            
            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsNotified = true;
            proposalToEdit.IsApproved = false;
            proposalToEdit.IsDenied = false;

            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 3, proposalToEdit, CreateValidEntities.Comment(1), StaticValues.RB_Decission_NotDecided)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("You should not change the Decission if they have been notified.");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Proposal);
            Assert.IsNotNull(result.Comment);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAdminEditPostWhenAlreadyNotifiedDoesNotSave2()
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
            proposalList[0].IsNotified = true;
            proposalList[0].IsApproved = false;
            proposalList[0].IsDenied = true;
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);

            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsNotified = true;
            proposalToEdit.IsApproved = false;
            proposalToEdit.IsDenied = false;

            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 3, proposalToEdit, CreateValidEntities.Comment(1), StaticValues.RB_Decission_NotDecided)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("You should not change the Decission if they have been notified.");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Proposal);
            Assert.IsNotNull(result.Comment);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostWhenAlreadyNotifiedDoesNotSave3()
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
            proposalList[0].IsNotified = true;
            proposalList[0].IsApproved = false;
            proposalList[0].IsDenied = false;
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);

            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsNotified = true;
            proposalToEdit.IsApproved = true;
            proposalToEdit.IsDenied = false;

            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 3, proposalToEdit, CreateValidEntities.Comment(1), StaticValues.RB_Decission_Approved)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("You should not change the Decission if they have been notified.", "Can not approve an unsubmitted proposal");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Proposal);
            Assert.IsNotNull(result.Comment);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostWhenApprovedAndUnsubmitted()
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
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);

            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsSubmitted = false;
            //proposalToEdit.IsApproved = true;


            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 3, proposalToEdit, CreateValidEntities.Comment(1), StaticValues.RB_Decission_Approved)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Can not approve an unsubmitted proposal");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Proposal);
            Assert.IsNotNull(result.Comment);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostWhenWasSubmittedAndUnsubmittingAndApproved()
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
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);
            proposalList[0].IsSubmitted = true;

            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsSubmitted = false;
            //proposalToEdit.IsApproved = true;


            #endregion Arrange

            #region Act
            var result = Controller.AdminEdit(1, 3, proposalToEdit, CreateValidEntities.Comment(1), StaticValues.RB_Decission_Approved)
                .AssertViewRendered()
                .WithViewData<ProposalAdminViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Can not approve an unsubmitted proposal", "Can not unsubmit unless the decission is undecided");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Proposal);
            Assert.IsNotNull(result.Comment);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostSaves1()
        {
            #region Arrange
            var callsFor = new List<CallForProposal>();
            callsFor.Add(CreateValidEntities.CallForProposal(1));
            callsFor[0].EmailTemplates = new List<EmailTemplate>();
            callsFor[0].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            callsFor[0].EmailTemplates[0].TemplateType = EmailTemplateType.ProposalUnsubmitted;
 
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository, callsFor);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                            Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var proposalList = new List<Proposal>();
            proposalList.Add(CreateValidEntities.Proposal(1));
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);
            proposalList[0].IsSubmitted = true;

            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsSubmitted = false;
            //proposalToEdit.IsApproved = true;

            EmailService.Expect(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything)).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminEdit(1, 1, proposalToEdit, CreateValidEntities.Comment(1),StaticValues.RB_Decission_NotDecided)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert            
            Assert.AreEqual("Proposal successfully edited", Controller.Message);            
            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            CommentRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Comment>.Is.Anything));
            EmailService.AssertWasCalled(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostSaves2()
        {
            #region Arrange
            var callsFor = new List<CallForProposal>();
            callsFor.Add(CreateValidEntities.CallForProposal(1));
            callsFor[0].EmailTemplates = new List<EmailTemplate>();
            callsFor[0].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            callsFor[0].EmailTemplates[0].TemplateType = EmailTemplateType.ProposalUnsubmitted;

            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository, callsFor);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                            Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var proposalList = new List<Proposal>();
            proposalList.Add(CreateValidEntities.Proposal(1));
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);
            proposalList[0].IsSubmitted = true;

            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsSubmitted = false;
            //proposalToEdit.IsApproved = true;

            EmailService.Expect(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything)).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminEdit(1, 1, proposalToEdit, new Comment(proposalList[0], editors[0], null), StaticValues.RB_Decission_NotDecided)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal successfully edited", Controller.Message);
            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            CommentRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Comment>.Is.Anything));
            EmailService.AssertWasCalled(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminEditPostSaves3()
        {
            #region Arrange
            var callsFor = new List<CallForProposal>();
            callsFor.Add(CreateValidEntities.CallForProposal(1));
            callsFor[0].EmailTemplates = new List<EmailTemplate>();
            callsFor[0].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            callsFor[0].EmailTemplates[0].TemplateType = EmailTemplateType.ProposalUnsubmitted;

            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository, callsFor);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "me");
            AccessService.Expect(a => a.HasAccess(null, 1, "me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                            Arg<int?>.Is.Anything)).Return(true).Repeat.Any();
            var proposalList = new List<Proposal>();
            proposalList.Add(CreateValidEntities.Proposal(1));
            proposalList[0].CallForProposal = CreateValidEntities.CallForProposal(1);
            proposalList[0].IsSubmitted = true;

            var proposals = new FakeProposals();
            proposals.Records(2, ProposalRepository, proposalList);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
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

            var comments = new List<Comment>();
            comments.Add(CreateValidEntities.Comment(1));
            comments[0].Editor = EditorRepository.GetNullableById(1);
            comments[0].Proposal = ProposalRepository.GetNullableById(2);
            var fakedComments = new FakeComments();
            fakedComments.Records(0, CommentRepository, comments);

            var proposalToEdit = CreateValidEntities.Proposal(1);
            proposalToEdit.IsSubmitted = true;
            //proposalToEdit.IsApproved = true;

            EmailService.Expect(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything)).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminEdit(1, 1, proposalToEdit, new Comment(proposalList[0], editors[0], null), StaticValues.RB_Decission_NotDecided)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.AdminIndex(1, null, null, null, null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal successfully edited", Controller.Message);
            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            CommentRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Comment>.Is.Anything));
            EmailService.AssertWasNotCalled(a => a.SendProposalEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything));
            #endregion Assert
        }
        #endregion AdminEdit Post Tests
    }
}
