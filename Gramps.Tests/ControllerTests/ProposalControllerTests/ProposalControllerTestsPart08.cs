using Gramps.Controllers.ViewModels;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region Confirmation Tests

        [TestMethod]
        public void TestConfirmationReturnsViewWithExpectedValues()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            var result = Controller.Confirmation("some@email.com")
                .AssertViewRendered()
                .WithViewData<ProposalConfirmationViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Format("Thank you. An email with a link to complete your proposal will be sent to shortly. It will be sent to {0}", "some@email.com"), result.Message1);
            Assert.AreEqual("Note: This email will be sent from the following address so if you do not receive it please check your email filters. automatedemail@caes.ucdavis.edu", result.Message2);
            #endregion Assert		
        }
        #endregion Confirmation Tests

        #region Home Tests

        [TestMethod]
        public void TestHomeReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "email2@testy.com");
            SetupData5();
            #endregion Arrange

            #region Act
            var result = Controller.Home()
                .AssertViewRendered()
                .WithViewData<ProposalPublicListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsReviewer);
            Assert.AreEqual(null, result.CallForProposals);
            Assert.AreEqual(4, result.UsersProposals.Count);
            Assert.AreEqual(5, result.UsersProposals[0].Id);
            Assert.AreEqual(4, result.UsersProposals[1].Id);
            Assert.AreEqual(3, result.UsersProposals[2].Id);
            Assert.AreEqual(1, result.UsersProposals[3].Id);
            #endregion Assert		
        }

        [TestMethod]
        public void TestHomeReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "blah@testy.com");
            SetupData5();
            #endregion Arrange

            #region Act
            var result = Controller.Home()
                .AssertViewRendered()
                .WithViewData<ProposalPublicListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsReviewer);
            Assert.AreEqual(null, result.CallForProposals);
            Assert.AreEqual(0, result.UsersProposals.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestHomeReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "notme@test.com");
            SetupData5();
            #endregion Arrange

            #region Act
            var result = Controller.Home()
                .AssertViewRendered()
                .WithViewData<ProposalPublicListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsReviewer);
            Assert.AreEqual(null, result.CallForProposals);
            Assert.AreEqual(1, result.UsersProposals.Count);
            Assert.AreEqual(2, result.UsersProposals[0].Id);
            #endregion Assert
        }

        [TestMethod]
        public void TestHomeReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "is@reviewer.com");
            SetupData5();
            #endregion Arrange

            #region Act
            var result = Controller.Home()
                .AssertViewRendered()
                .WithViewData<ProposalPublicListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsReviewer);
            Assert.AreEqual(3, result.CallForProposals.Count);
            Assert.AreEqual(0, result.UsersProposals.Count);
            #endregion Assert		
        }

        [TestMethod]
        public void TestHomeReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "also@reviewer.com");
            SetupData5();
            #endregion Arrange

            #region Act
            var result = Controller.Home()
                .AssertViewRendered()
                .WithViewData<ProposalPublicListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsReviewer);
            Assert.AreEqual(1, result.CallForProposals.Count);
            Assert.AreEqual(2, result.UsersProposals.Count);
            Assert.AreEqual(7, result.UsersProposals[0].Id);
            Assert.AreEqual(6, result.UsersProposals[1].Id);
            #endregion Assert
        }
        #endregion Home Tests
    }
}
