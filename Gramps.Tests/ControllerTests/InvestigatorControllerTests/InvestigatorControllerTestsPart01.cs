using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.InvestigatorControllerTests
{
    public partial class InvestigatorControllerTests
    {
        #region Create Tests
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetRedirectsToErrorIfProposalNotFound()
        {
            #region Arrange
            SetupTestData1();
            #endregion Arrange

            #region Act
            Controller.Create(SpecificGuid.GetGuid(6))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsToErrorIfNoAccess()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "me@notme.com");
            #endregion Arrange

            #region Act
            Controller.Create(SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetRedirectsToDetailsIfSubmitted()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email3@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(3))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(3)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot edit proposal once submitted.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(3), result.RouteValues.ElementAt(2).Value);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreateGetRedirectsToEditIfCallNotActive()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email1@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot add investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(1), result.RouteValues.ElementAt(2).Value);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsToEditIfCallEndDatePast()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email2@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot add investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(2), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsViewWithExpectedData()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email4@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(4))
                .AssertViewRendered()
                .WithViewData<InvestigatorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Proposal.Id);
            Assert.AreEqual(0, result.Investigator.Id);
            #endregion Assert		
        }
        #endregion Create Get Tests
        #endregion Create Tests
    }
}
