using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region Edit Get Tests
        /// <summary>
        /// Tests the FieldToTest with A value of TestValue does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestEditGetThrowsAnExceptionIfGuidIsNotUnique()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var proposals = new List<Proposal>();
                for (int i = 0; i < 2; i++)
                {
                    proposals.Add(CreateValidEntities.Proposal(i+1));
                    proposals[i].Guid = SpecificGuid.GetGuid(1);
                }
                var fakeProposals = new FakeProposals();
                fakeProposals.Records(3, ProposalRepository, proposals);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Edit(SpecificGuid.GetGuid(1));
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains more than one element", ex.Message);
                throw;
            }	
        }


        [TestMethod]
        public void TestEditGetRedirectsWhenProposalNotFound()
        {
            #region Arrange
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Edit(SpecificGuid.GetGuid(7))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "email3@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Edit(SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenSubmitted()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email2@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot edit proposal once submitted.", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual(SpecificGuid.GetGuid(2), results.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWhenCallEndDatePast()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email3@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(3))
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Call for proposal has closed, you will not be able to save changes.", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(3), results.Proposal.Guid);
            Assert.AreEqual("Name1", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Write these tests");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert
            
            #endregion Assert
        }
        #endregion Edit Get Tests
    }
}
