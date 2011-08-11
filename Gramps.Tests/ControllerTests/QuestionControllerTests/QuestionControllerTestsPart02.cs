using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Linq.Expressions;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region Create Get Tests
        [TestMethod]
        public void TestCreateGetRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Create(2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Create(null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, null)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.QuestionTypes.Count());
            Assert.AreEqual(4, result.Validators.Count());
            Assert.IsTrue(result.IsTemplate);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(1, result.TemplateId);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(0, 3)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.QuestionTypes.Count());
            Assert.AreEqual(4, result.Validators.Count());
            Assert.IsFalse(result.IsTemplate);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            #endregion Assert
        }


        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCreateGetThrowsExceptionIfTemplateIdAndCallForProposalIdNotSpecified1()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
                AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
                SetupData2();
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(null, null);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Must have either a template or a call for proposal", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCreateGetThrowsExceptionIfTemplateIdAndCallForProposalIdNotSpecified2()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
                AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
                SetupData2();
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(0, 0);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Must have either a template or a call for proposal", ex.Message);
                throw;
            }
        }
        #endregion Create Get Tests
    }
}
