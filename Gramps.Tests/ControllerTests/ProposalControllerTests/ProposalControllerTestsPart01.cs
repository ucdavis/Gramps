using System;
using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Resources;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsToHomeController()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            Controller.Index()
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion Index Tests

        #region AdminIndex Tests

        [TestMethod]
        public void TestAdminIndexRedirectsWhenCallNotFound()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(4, null, null, null, null, null)
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
        public void TestAdminIndexRedirectsWhenNoAccess()
        {
            #region Arrange
            var calls = new FakeCallForProposals();
            calls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "me");
            AccessService.Expect(a => a.HasAccess(null, 3, "me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.AdminIndex(3, null, null, null, null, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestAdminIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.Proposals.Count);
            Assert.AreEqual(new DateTime(2011,01,20), result.Proposals[0].LastViewedDate);
            Assert.IsNull(result.Proposals[1].LastViewedDate);
            Assert.AreEqual("LoginId1", result.Editor.User.LoginId);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual("Name3", result.CallForProposal.Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAdminIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, StaticValues.RB_Decission_Approved, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Proposals.Count);
            Assert.AreEqual(2, result.Proposals[0].Id);
            Assert.AreEqual(5, result.Proposals[1].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, StaticValues.RB_Decission_Denied, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Proposals.Count);
            Assert.AreEqual(3, result.Proposals[0].Id);
            Assert.AreEqual(6, result.Proposals[1].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, StaticValues.RB_Decission_NotDecided, null, null, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(16, result.Proposals.Count);

            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, StaticValues.RB_Notified_Notified, null, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Proposals.Count);
            Assert.AreEqual(2, result.Proposals[0].Id);
            Assert.AreEqual(7, result.Proposals[1].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, StaticValues.RB_Notified_NotNotified, null, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(18, result.Proposals.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, StaticValues.RB_Decission_Approved, StaticValues.RB_Notified_NotNotified, null, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Proposals.Count);
            Assert.AreEqual(5, result.Proposals[0].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, null, StaticValues.RB_Submitted_Submitted, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Proposals.Count);
            Assert.AreEqual(4, result.Proposals[0].Id);
            Assert.AreEqual(5, result.Proposals[1].Id);
            Assert.AreEqual(6, result.Proposals[2].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView9()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, StaticValues.RB_Decission_Approved, null, StaticValues.RB_Submitted_Submitted, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Proposals.Count);
            Assert.AreEqual(5, result.Proposals[0].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView10()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, null, StaticValues.RB_Submitted_NotSubmitted, null, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(17, result.Proposals.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView11()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, null, null, StaticValues.RB_Warned_Warned, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Proposals.Count);
            Assert.AreEqual(8, result.Proposals[0].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView12()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, null, null, StaticValues.RB_Warned_NotWarned, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(19, result.Proposals.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAdminIndexReturnsView13()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "LoginId1");
            AccessService.Expect(a => a.HasAccess(null, 3, "LoginId1")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.AdminIndex(3, null, null, StaticValues.RB_Submitted_NotSubmitted, StaticValues.RB_Warned_NotWarned, null)
                .AssertViewRendered()
                .WithViewData<ProposalAdminListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(16, result.Proposals.Count);
            #endregion Assert
        }
        #endregion AdminIndex Tests
    }
}
