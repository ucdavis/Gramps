using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace Gramps.Tests.ControllerTests.CallForProposalControllerTests
{
    public partial class CallForProposalControllerTests
    {
        #region Create Tests
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");            
            SetupDataForTests2();            
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<CallForProposalCreateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CallForProposal);
            Assert.AreEqual(2, result.Templates.Count());
            Assert.AreEqual("Name4", result.Templates.ElementAtOrDefault(0).Name);
            Assert.AreEqual("Name6", result.Templates.ElementAtOrDefault(1).Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupDataForTests2();
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<CallForProposalCreateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CallForProposal);
            Assert.AreEqual(4, result.Templates.Count());
            Assert.AreEqual("Name7", result.Templates.ElementAtOrDefault(0).Name);
            Assert.AreEqual("Name8", result.Templates.ElementAtOrDefault(1).Name);
            #endregion Assert
        }
        
        #endregion Create Get Tests
        #region Create Post Tests

        /// <summary>
        /// Tests the FieldToTest with A value of TestValue does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestFieldToTestWithAValueOfTestValueDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotFound");
                var fakeUsers = new FakeUsers();
                fakeUsers.Records(3, UserRepository);
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(99);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no elements", ex.Message);
                throw;
            }	
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange

            Assert.Inconclusive("Continue these tests");

            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert		
        }
        

        #endregion Create Post Tests
        #endregion Create Tests
    }
}
