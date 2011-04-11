using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region Create Tests
        #region Create Get Tests
        [TestMethod]
        public void TestCreateGetWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(4, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetWithAccessReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(0, result.TemplateId);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetWithAccessReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, null)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(4, result.TemplateId);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }
        #endregion Create Get Tests
        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, new EmailsForCall())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Create(4, null, new EmailsForCall())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostWithDuplicateEmailReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, new EmailsForCall("TeSt3@TESTY.com"))
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TeSt3@TESTY.com".ToLower(), result.EmailsForCall.Email);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.AreEqual(3, result.EmailsForCall.CallForProposal.Id);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(0, result.TemplateId);
            Assert.AreEqual(null, result.EmailsForCall.Template);
            Assert.IsFalse(result.IsTemplate);

            Assert.AreEqual("Emails For Call not created.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email already exists");

            EmailsForCallRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, args1[1]);
            Assert.AreEqual("Me", args1[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostWithDuplicateEmailReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, null, new EmailsForCall("TeSt8@TESTY.com"))
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TeSt8@TESTY.com".ToLower(), result.EmailsForCall.Email);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.AreEqual(null, result.EmailsForCall.CallForProposal);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(4, result.TemplateId);
            Assert.AreEqual(4, result.EmailsForCall.Template.Id);
            Assert.IsTrue(result.IsTemplate);

            Assert.AreEqual("Emails For Call not created.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email already exists");

            EmailsForCallRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args1[0]);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual("Me", args1[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithInvalidEmailReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, new EmailsForCall("TeSt3@TE@STY.com"))
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TeSt3@TE@STY.com".ToLower(), result.EmailsForCall.Email);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.AreEqual(3, result.EmailsForCall.CallForProposal.Id);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(0, result.TemplateId);
            Assert.AreEqual(null, result.EmailsForCall.Template);
            Assert.IsFalse(result.IsTemplate);

            Assert.AreEqual("Emails For Call not created.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email: not a well-formed email address");

            EmailsForCallRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, args1[1]);
            Assert.AreEqual("Me", args1[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostWithInvalidEmailReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, null, new EmailsForCall("TeSt3@TE@STY.com"))
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TeSt3@TE@STY.com".ToLower(), result.EmailsForCall.Email);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.AreEqual(null, result.EmailsForCall.CallForProposal);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(4, result.TemplateId);
            Assert.AreEqual(4, result.EmailsForCall.Template.Id);
            Assert.IsTrue(result.IsTemplate);

            Assert.AreEqual("Emails For Call not created.", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email: not a well-formed email address");

            EmailsForCallRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args1[0]);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual("Me", args1[2]);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostRedirectsWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, new EmailsForCall("TeSt99@TESTY.com"))
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);


            Assert.AreEqual("Emails For Call Created Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args = (EmailsForCall) EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("TeSt99@TESTY.com".ToLower(), args.Email);
            Assert.AreEqual(3, args.CallForProposal.Id);
            Assert.AreEqual(null, args.Template);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, args1[1]);
            Assert.AreEqual("Me", args1[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, null, new EmailsForCall("TeSt99@TESTY.com"))
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);


            Assert.AreEqual("Emails For Call Created Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args = (EmailsForCall)EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("TeSt99@TESTY.com".ToLower(), args.Email);
            Assert.AreEqual(null, args.CallForProposal);
            Assert.AreEqual(4, args.Template.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args1[0]);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual("Me", args1[2]);
            #endregion Assert
        }

        #endregion Create Post Tests
        #endregion Create Tests
    }
}
