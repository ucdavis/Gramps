using System;
using System.Linq;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.InvestigatorControllerTests
{
    public partial class InvestigatorControllerTests
    {
        #region Delete Tests
        [TestMethod]
        public void TestDeleteRedirectsToErrorIfProposalNotFound()
        {
            #region Arrange
            SetupTestData1();
            #endregion Arrange

            #region Act
            Controller.Delete(99, SpecificGuid.GetGuid(15))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            InvestigatorRepository.AssertWasNotCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsToErrorIfNoAccess()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@notme.com");
            #endregion Arrange

            #region Act
            Controller.Delete(99, SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            InvestigatorRepository.AssertWasNotCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsToDetailsIfSubmitted()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email3@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Delete(99, SpecificGuid.GetGuid(3))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(3)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot remove investigator for proposal once proposal is submitted.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(3), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasNotCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsToEditIfCallNotActive()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email1@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Delete(99, SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot remove investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(1), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasNotCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsToEditIfCallEndDatePast()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email2@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Delete(99, SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot remove investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(2), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasNotCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteRedirectsToEditInvestigatorNotFound()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Delete(6, SpecificGuid.GetGuid(6))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Investigator not found.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(6), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasNotCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsToEditInvestigatorNotSameId()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            Controller.Delete(4, SpecificGuid.GetGuid(6))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            InvestigatorRepository.AssertWasNotCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteRedirectsWhenValid()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Delete(2, SpecificGuid.GetGuid(6))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Investigator Removed Successfully", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(6), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            var args = (Investigator)InvestigatorRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<Investigator>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args.Id);
            #endregion Assert
        }


        [TestMethod]
        public void TestDeleteWhenRemoveThrowsException()
        {
            #region Arrange
            InvestigatorRepository.Expect(a => a.Remove(Arg<Investigator>.Is.Anything)).Throw(new ApplicationException("Forced Exception"));
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Delete(2, SpecificGuid.GetGuid(6))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Not Removed", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(6), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasCalled(a => a.Remove(Arg<Investigator>.Is.Anything));
            var args = (Investigator)InvestigatorRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<Investigator>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args.Id);
            #endregion Assert		
        }
        #endregion Delete Tests
    }
}
