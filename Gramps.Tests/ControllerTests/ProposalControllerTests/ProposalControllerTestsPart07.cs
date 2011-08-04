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
        #region Create Get Tests


        [TestMethod]
        public void TestCreateGetRedirectsWhenCallNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenCallNotActive()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenCallHasEnded1()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenCallHasEnded2()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name4", result.CallForProposal.Name);
            Assert.AreEqual("test1@testy.com", result.ContactEmail);
            Assert.IsNotNull(result.Proposal);
            Assert.AreEqual(0, result.Proposal.Id);
            #endregion Assert		
        }
     
        #endregion Create Get Tests
        #region Create Post Tests


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
        #endregion Create Post Tests
    }
}
