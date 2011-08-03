using System;
using System.Collections.Generic;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region ReviewerIndex Tests

        [TestMethod]
        public void TestReviewerIndexRedirectsToProposalHomeIfCallNotFound1()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.ReviewerIndex(4, null, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        [TestMethod]
        public void TestReviewerIndexRedirectsToProposalHomeIfCallNotFound2()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.ReviewerIndex(4, StaticValues.RB_Decission_NotDecided, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerIndexRedirectsToProposalHomeIfCallNotFound3()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.ReviewerIndex(4, StaticValues.RB_Decission_NotDecided, "test@testy.com")
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            #endregion Assert
        }


        [TestMethod]
        public void TestReviewerIndexRedirectsInNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "notme@test.com");
            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors.Add(CreateValidEntities.Editor(2));
            editors.Add(CreateValidEntities.Editor(3));
            editors[1].ReviewerEmail = "me@test.com";
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].Editors = editors;
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(2, CallForProposalRepository, calls);
            #endregion Arrange

            #region Act
            Controller.ReviewerIndex(1, null, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestReviewerIndexRedirectsInNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "notme@test.com");
            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors.Add(CreateValidEntities.Editor(2));
            editors.Add(CreateValidEntities.Editor(3));
            editors[1].ReviewerEmail = "me@test.com";
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].Editors = editors;
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(2, CallForProposalRepository, calls);
            #endregion Arrange

            #region Act
            Controller.ReviewerIndex(1, null, "me@test.com")
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerIndexRedirectsInNoAccess3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "notme@test.com");
            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors.Add(CreateValidEntities.Editor(2));
            editors.Add(CreateValidEntities.Editor(3));
            editors[1].ReviewerEmail = "me@test.com";
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].Editors = editors;
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(2, CallForProposalRepository, calls);
            #endregion Arrange

            #region Act
            Controller.ReviewerIndex(1, StaticValues.RB_Decission_Denied, "notme@test.com")
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.ReviewerIndex(1, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalReviewerListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.Editor.Id);
            Assert.IsNull(result.FilterDecission);
            Assert.IsNull(result.FilterEmail);
            Assert.AreEqual(3, result.Proposals.Count);
            Assert.AreEqual(false, result.Proposals[0].Approved);
            Assert.AreEqual(DateTime.Now.Date, result.Proposals[0].CreatedDate.Date);
            Assert.AreEqual(false, result.Proposals[0].Denied);
            Assert.AreEqual("email4@testy.com", result.Proposals[0].Email);
            Assert.AreEqual(4, result.Proposals[0].Id);
            Assert.AreEqual(new DateTime(2011, 01, 20), result.Proposals[0].LastViewedDate);
            Assert.AreEqual(false, result.Proposals[0].NotifiedOfDecission);
            Assert.AreEqual(4, result.Proposals[0].Seq);
            Assert.AreEqual(true, result.Proposals[0].Submitted);
            Assert.AreEqual(new DateTime(2011, 02, 3), result.Proposals[0].SubmittedDate);
            Assert.AreEqual(false, result.Proposals[0].WarnedOfClosing);

            Assert.AreEqual(true, result.Proposals[1].Approved);
            Assert.AreEqual(DateTime.Now.Date, result.Proposals[1].CreatedDate.Date);
            Assert.AreEqual(false, result.Proposals[1].Denied);
            Assert.AreEqual("email5@testy.com", result.Proposals[1].Email);
            Assert.AreEqual(5, result.Proposals[1].Id);
            Assert.AreEqual(null, result.Proposals[1].LastViewedDate);
            Assert.AreEqual(false, result.Proposals[1].NotifiedOfDecission);
            Assert.AreEqual(5, result.Proposals[1].Seq);
            Assert.AreEqual(true, result.Proposals[1].Submitted);
            Assert.AreEqual(new DateTime(2011, 02, 4), result.Proposals[1].SubmittedDate);
            Assert.AreEqual(false, result.Proposals[1].WarnedOfClosing);

            Assert.AreEqual(false, result.Proposals[2].Approved);
            Assert.AreEqual(DateTime.Now.Date, result.Proposals[2].CreatedDate.Date);
            Assert.AreEqual(true, result.Proposals[2].Denied);
            Assert.AreEqual("email6@testy.com", result.Proposals[2].Email);
            Assert.AreEqual(6, result.Proposals[2].Id);
            Assert.AreEqual(null, result.Proposals[2].LastViewedDate);
            Assert.AreEqual(true, result.Proposals[2].NotifiedOfDecission);
            Assert.AreEqual(6, result.Proposals[2].Seq);
            Assert.AreEqual(true, result.Proposals[2].Submitted);
            Assert.AreEqual(new DateTime(2011, 02, 5), result.Proposals[2].SubmittedDate);
            Assert.AreEqual(true, result.Proposals[2].WarnedOfClosing);
            #endregion Assert		
        }

        [TestMethod]
        public void TestReviewerIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.ReviewerIndex(1, StaticValues.RB_Decission_Approved, null)
                .AssertViewRendered()
                .WithViewData<ProposalReviewerListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.Editor.Id);
            Assert.AreEqual(StaticValues.RB_Decission_Approved, result.FilterDecission);
            Assert.IsNull(result.FilterEmail);
            Assert.AreEqual(1, result.Proposals.Count);

            Assert.AreEqual(5, result.Proposals[0].Id);

            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerIndexReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.ReviewerIndex(1, StaticValues.RB_Decission_Denied, null)
                .AssertViewRendered()
                .WithViewData<ProposalReviewerListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.Editor.Id);
            Assert.AreEqual(StaticValues.RB_Decission_Denied, result.FilterDecission);
            Assert.IsNull(result.FilterEmail);
            Assert.AreEqual(1, result.Proposals.Count);

            Assert.AreEqual(6, result.Proposals[0].Id);

            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerIndexReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.ReviewerIndex(1, StaticValues.RB_Decission_NotDecided, null)
                .AssertViewRendered()
                .WithViewData<ProposalReviewerListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.Editor.Id);
            Assert.AreEqual(StaticValues.RB_Decission_NotDecided, result.FilterDecission);
            Assert.IsNull(result.FilterEmail);
            Assert.AreEqual(1, result.Proposals.Count);

            Assert.AreEqual(4, result.Proposals[0].Id);

            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerIndexReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.ReviewerIndex(1, StaticValues.RB_Decission_Approved, "tester")
                .AssertViewRendered()
                .WithViewData<ProposalReviewerListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.Editor.Id);
            Assert.AreEqual(StaticValues.RB_Decission_Approved, result.FilterDecission);
            Assert.AreEqual("tester", result.FilterEmail);
            Assert.AreEqual(0, result.Proposals.Count);


            #endregion Assert
        }

        [TestMethod]
        public void TestReviewerIndexReturnsView6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "test2@testy.com");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.ReviewerIndex(1, StaticValues.RB_Decission_NotDecided, "Email4")
                .AssertViewRendered()
                .WithViewData<ProposalReviewerListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.Editor.Id);
            Assert.AreEqual(StaticValues.RB_Decission_NotDecided, result.FilterDecission);
            Assert.AreEqual("Email4", result.FilterEmail);
            Assert.AreEqual(1, result.Proposals.Count);
            Assert.AreEqual(4, result.Proposals[0].Id);

            #endregion Assert
        }
        #endregion ReviewerIndex Tests
    }
}
