using System.Linq;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.InvestigatorControllerTests
{
    public partial class InvestigatorControllerTests
    {
        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsToErrorIfProposalNotFound()
        {
            #region Arrange
            SetupTestData1();
            #endregion Arrange

            #region Act
            Controller.Edit(99, SpecificGuid.GetGuid(15))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToErrorIfNoAccess()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@notme.com");
            #endregion Arrange

            #region Act
            Controller.Edit(99, SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToDetailsIfSubmitted()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email3@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(99,SpecificGuid.GetGuid(3))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(3)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot edit investigator for proposal once proposal is submitted.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(3), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToEditIfCallNotActive()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email1@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(99, SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot edit investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(1), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToEditIfCallEndDatePast()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email2@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(99, SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot edit investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(2), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetRedirectsToEditInvestigatorNotFound()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(6, SpecificGuid.GetGuid(6))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Investigator not found.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(6), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsToEditInvestigatorNotSameId()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            Controller.Edit(4, SpecificGuid.GetGuid(6))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWhenValid()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, SpecificGuid.GetGuid(6))
                .AssertViewRendered()
                .WithViewData<InvestigatorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Proposal.Id);
            Assert.AreEqual(2, result.Investigator.Id);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        [TestMethod]
        public void TestEditPosttRedirectsToErrorIfProposalNotFound()
        {
            #region Arrange
            SetupTestData1();
            #endregion Arrange

            #region Act
            Controller.Edit(99, SpecificGuid.GetGuid(15), new Investigator())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToErrorIfNoAccess()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@notme.com");
            #endregion Arrange

            #region Act
            Controller.Edit(99, SpecificGuid.GetGuid(1), new Investigator())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToDetailsIfSubmitted()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email3@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(99, SpecificGuid.GetGuid(3), new Investigator())
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(3)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot edit investigator for proposal once proposal is submitted.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(3), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToEditIfCallNotActive()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email1@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(99, SpecificGuid.GetGuid(1), new Investigator())
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot edit investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(1), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToEditIfCallEndDatePast()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email2@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(99, SpecificGuid.GetGuid(2), new Investigator())
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot edit investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(2), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsToEditInvestigatorNotFound()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Edit(6, SpecificGuid.GetGuid(6), new Investigator())
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Investigator not found.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(6), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsToEditInvestigatorNotSameId()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            Controller.Edit(4, SpecificGuid.GetGuid(6), new Investigator())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostReturnsViewWhenMoreThanOneInvestigatorIsPrimary()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            var investigator = CreateValidEntities.Investigator(9);
            investigator.IsPrimary = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, SpecificGuid.GetGuid(6), investigator)
                .AssertViewRendered()
                .WithViewData<InvestigatorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.Investigator.Name);

            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("There can only be one primary investigator per proposal.");

            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            var investigator = CreateValidEntities.Investigator(9);
            investigator.IsPrimary = false;
            investigator.Name = null;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, SpecificGuid.GetGuid(6), investigator)
                .AssertViewRendered()
                .WithViewData<InvestigatorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Investigator.Name);

            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");

            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRediectsWhenSuccessuful()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            var investigator = CreateValidEntities.Investigator(9, true);
            investigator.IsPrimary = false;
            investigator.State = "xx";
            investigator.Zip = "Zip9";
            investigator.Phone = "999-999-9999";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, SpecificGuid.GetGuid(6), investigator)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(6), result.RouteValues.ElementAt(2).Value);

            Assert.AreEqual("Investigator Edited Successfully.", Controller.Message);
            InvestigatorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            var args = (Investigator) InvestigatorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsPrimary);
            Assert.AreEqual("Address19", args.Address1);
            Assert.AreEqual("Address29", args.Address2);
            Assert.AreEqual("Address39", args.Address3);
            Assert.AreEqual("City9", args.City);
            Assert.AreEqual("test9@testy.com", args.Email);
            Assert.AreEqual("Institution9", args.Institution);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual("999-999-9999", args.Phone);
            Assert.AreEqual("Position9", args.Position);
            Assert.AreEqual("XX", args.State);
            Assert.AreEqual("Zip9", args.Zip);
            #endregion Assert	
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
