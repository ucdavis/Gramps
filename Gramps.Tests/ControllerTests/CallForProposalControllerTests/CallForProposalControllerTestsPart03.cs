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
using Gramps.Tests.Core.Extensions;
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
        public void TestCreatePostThrowsExceptionWhenUserNotFound()
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
        public void TestCreatePostRedirectsWhenNotAnEditorOfTemplate()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            SetupDataForTests3();
            #endregion Arrange

            #region Act
            Controller.Create(3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that template.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreatePostReturnsViewWhenTemplateHasInvalidData()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(1, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests3();
            SetupDataForTests2(); 
            #endregion Arrange

            #region Act
            var result = Controller.Create(1)
                .AssertViewRendered()
                .WithViewData<CallForProposalCreateViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("There was a problem selecting that template.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(1, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CallForProposal);
            Assert.AreEqual(new DateTime().Date, result.CallForProposal.EndDate.Date);
            Assert.AreEqual(2, result.Templates.Count());
            Assert.AreEqual("Name4", result.Templates.ElementAtOrDefault(0).Name);
            Assert.AreEqual("Name6", result.Templates.ElementAtOrDefault(1).Name);


            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            CallForProposalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreatePostRedirectsToEditWhenCallIsCreatedSuccesfully1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests3();
            #endregion Arrange

            #region Act
            Controller.Create(2)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal Created Successfully.", Controller.Message);
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            CallForProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything));
            var args2 = (CallForProposal) CallForProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything))[0][0]; 

            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.IsActive);
            Assert.AreEqual(DateTime.Now.AddMonths(1).Date, args2.EndDate);
            Assert.AreEqual(DateTime.Now.Date, args2.CreatedDate.Date);
            Assert.AreEqual(1, args2.Editors.Count);
            Assert.AreEqual("Me", args2.Editors[0].User.LoginId);
            Assert.IsTrue(args2.Editors[0].IsOwner);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostRedirectsToEditWhenCallIsCreatedSuccesfully2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests3();
            #endregion Arrange

            #region Act
            Controller.Create(3)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Edit(1));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal Created Successfully.", Controller.Message);
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            CallForProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything));
            var args2 = (CallForProposal)CallForProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything))[0][0];

            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.IsActive);
            Assert.AreEqual(DateTime.Now.AddMonths(1).Date, args2.EndDate);
            Assert.AreEqual(DateTime.Now.Date, args2.CreatedDate.Date);
            Assert.AreEqual(4, args2.Editors.Count);
            Assert.AreEqual("Me", args2.Editors[0].User.LoginId);
            Assert.IsTrue(args2.Editors[0].IsOwner);

            Assert.AreEqual(3, args2.Emails.Count);            
            Assert.AreEqual(3, args2.Questions.Count);
            Assert.AreEqual(3, args2.Reports.Count);
            Assert.AreEqual(7, args2.EmailTemplates.Count);
            Assert.AreEqual("Subject1", args2.EmailTemplates[0].Subject);
            Assert.AreEqual(EmailTemplateType.InitialCall, args2.EmailTemplates[0].TemplateType);
            Assert.AreEqual("Subject2", args2.EmailTemplates[1].Subject);
            Assert.AreEqual(EmailTemplateType.ProposalDenied, args2.EmailTemplates[1].TemplateType);
            Assert.AreEqual(null, args2.EmailTemplates[2].Subject);

            #endregion Assert
        }



        #endregion Create Post Tests
        #endregion Create Tests
    }
}
