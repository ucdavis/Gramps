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
        #region Edit Tests
        #region Edit Get Tests
        [TestMethod]
        public void TestEditGetWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, null, 3)
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
        public void TestEditGetWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, 4, null)
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
        public void TestEditGetWhenNotFoundRedirectsToIndex1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(11, null, 3)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Emails For Call not found.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWhenNotFoundRedirectsToIndex2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(11, 4, null)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Emails For Call not found.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithNoDiffIdRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(null, 
                CallForProposalRepository.GetNullableById(3), null, 3)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Edit(2, null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, 
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, 
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, 
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(2, ((CallForProposal)args1[1]).Id);            
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithNoDiffIdRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(TemplateRepository.GetNullableById(4), null, 4, null)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Edit(2, 4, null)
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];            
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(2, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithAccessReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(null,
                EmailsForCallRepository.GetNullableById(5).CallForProposal, null, 3)).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(5, null, 3)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(0, result.TemplateId);
            Assert.AreEqual(5, result.EmailsForCall.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWithAccessReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(EmailsForCallRepository.GetNullableById(10).Template,
                null, 4, null)).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, 4, null)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(4, result.TemplateId);
            Assert.AreEqual(10, result.EmailsForCall.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(4, ((Template)args1[0]).Id);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, null, 3, new EmailsForCall())
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
        public void TestEditPostWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, 4, null, new EmailsForCall())
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
        public void TestEditPostWhenNotFoundRedirectsToIndex1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(11, null, 3, new EmailsForCall())
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Emails For Call not found.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWhenNotFoundRedirectsToIndex2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(11, 4, null, new EmailsForCall())
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Emails For Call not found.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithNoDiffIdRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(null,
                CallForProposalRepository.GetNullableById(3), null, 3)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Edit(2, null, 3, new EmailsForCall())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(2, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithNoDiffIdRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(TemplateRepository.GetNullableById(4), null, 4, null)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Edit(2, 4, null, new EmailsForCall())
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(2, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostWithInvalidEmailDoesNotSave1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(null,
                EmailsForCallRepository.GetNullableById(5).CallForProposal, null, 3)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "blah@b@BLAH.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(5, null, 3, updatedEmail)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(0, result.TemplateId);
            Assert.AreEqual(5, result.EmailsForCall.Id);
            Assert.AreEqual("blah@b@BLAH.com".ToLower(), result.EmailsForCall.Email);
            Assert.IsTrue(result.EmailsForCall.HasBeenEmailed);

            //Assert.AreEqual("", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email: not a well-formed email address");

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostWithInvalidEmailDoesNotSave2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(EmailsForCallRepository.GetNullableById(10).Template,
                null, 4, null)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "blah@b@BLAH.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, 4, null, updatedEmail)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(4, result.TemplateId);
            Assert.AreEqual(10, result.EmailsForCall.Id);
            Assert.AreEqual("blah@b@BLAH.com".ToLower(), result.EmailsForCall.Email);
            Assert.IsFalse(result.EmailsForCall.HasBeenEmailed); //Template does not update this

            //Assert.AreEqual("", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email: not a well-formed email address");

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(4, ((Template)args1[0]).Id);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithDuplicateEmailDoesNotSave1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(null,
                EmailsForCallRepository.GetNullableById(5).CallForProposal, null, 3)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "TEST4@testy.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(5, null, 3, updatedEmail)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(0, result.TemplateId);
            Assert.AreEqual(5, result.EmailsForCall.Id);
            Assert.AreEqual("TEST4@testy.com".ToLower(), result.EmailsForCall.Email);
            Assert.IsTrue(result.EmailsForCall.HasBeenEmailed);

            //Assert.AreEqual("", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email already exists");

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithDuplicateEmailDoesNotSave2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(EmailsForCallRepository.GetNullableById(10).Template,
                null, 4, null)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "TEST9@testy.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, 4, null, updatedEmail)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(4, result.TemplateId);
            Assert.AreEqual(10, result.EmailsForCall.Id);
            Assert.AreEqual("TEST9@testy.com".ToLower(), result.EmailsForCall.Email);
            Assert.IsFalse(result.EmailsForCall.HasBeenEmailed); //Template does not update this

            //Assert.AreEqual("", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Email already exists");

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(4, ((Template)args1[0]).Id);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithSameEmailDoesSave1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(null,
                EmailsForCallRepository.GetNullableById(5).CallForProposal, null, 3)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "TEST5@testy.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(5, null, 3, updatedEmail)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);


            Assert.AreEqual("Emails For Call Edited Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args2 = (EmailsForCall) EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args2);
            Assert.IsTrue(args2.HasBeenEmailed);
            Assert.AreEqual("TEST5@testy.com".ToLower(), args2.Email);
            Assert.AreEqual(3, args2.CallForProposal.Id);
            Assert.IsNull(args2.Template);


            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithSameEmailDoesSave2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(EmailsForCallRepository.GetNullableById(10).Template,
                null, 4, null)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "TEST10@testy.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, 4, null, updatedEmail)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);


            Assert.AreEqual("Emails For Call Edited Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args2 = (EmailsForCall) EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.HasBeenEmailed);
            Assert.AreEqual("TEST10@testy.com".ToLower(), args2.Email);
            Assert.AreEqual(null, args2.CallForProposal);
            Assert.AreEqual(4, args2.Template.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(4, ((Template)args1[0]).Id);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithNewEmailDoesSave1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(null,
                EmailsForCallRepository.GetNullableById(5).CallForProposal, null, 3)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "TEST10@testy.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(5, null, 3, updatedEmail)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);


            Assert.AreEqual("Emails For Call Edited Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args2 = (EmailsForCall)EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0];
            Assert.IsNotNull(args2);
            Assert.IsTrue(args2.HasBeenEmailed);
            Assert.AreEqual("TEST10@testy.com".ToLower(), args2.Email);
            Assert.AreEqual(3, args2.CallForProposal.Id);
            Assert.IsNull(args2.Template);


            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args1[0]);
            Assert.AreEqual(3, ((CallForProposal)args1[1]).Id);
            Assert.AreEqual(null, args1[2]);
            Assert.AreEqual(3, args1[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithNewEmailDoesSave2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(EmailsForCallRepository.GetNullableById(10).Template,
                null, 4, null)).Return(true).Repeat.Any();

            var updatedEmail = new EmailsForCall();
            updatedEmail.Email = "TEST5@testy.com";
            updatedEmail.HasBeenEmailed = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, 4, null, updatedEmail)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);


            Assert.AreEqual("Emails For Call Edited Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args2 = (EmailsForCall)EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0];
            Assert.IsNotNull(args2);
            Assert.IsFalse(args2.HasBeenEmailed);
            Assert.AreEqual("TEST5@testy.com".ToLower(), args2.Email);
            Assert.AreEqual(null, args2.CallForProposal);
            Assert.AreEqual(4, args2.Template.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(
                Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(
                Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(4, ((Template)args1[0]).Id);
            Assert.AreEqual(null, args1[1]);
            Assert.AreEqual(4, args1[2]);
            Assert.AreEqual(null, args1[3]);
            #endregion Assert
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
