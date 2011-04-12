using System.Linq;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region Delete Tests
        [TestMethod]
        public void TestDeleteWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(5, null, 3)
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

            EmailsForCallRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Delete(5, 4, null)
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

            EmailsForCallRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenNotFoundRedirectsToIndex1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            //SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(11, null, 3)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Email not removed.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            EmailsForCallRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWhenNotFoundRedirectsToIndex2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(11, 4, null)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual("Email not removed.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);

            EmailsForCallRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWithNoDiffIdRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(null,
                CallForProposalRepository.GetNullableById(3), null, 3)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Delete(2, null, 3)
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

            EmailsForCallRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWithNoDiffIdRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(TemplateRepository.GetNullableById(4), null, 4, null)).Return(false).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            Controller.Delete(2, 4, null)
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

            EmailsForCallRepository.AssertWasNotCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestDeleteWithValidDataDoesRemoveRecord1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(null,
                EmailsForCallRepository.GetNullableById(5).CallForProposal, null, 3)).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(5, null, 3)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);


            Assert.AreEqual("Email Removed Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            var args2 = (EmailsForCall)EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<EmailsForCall>.Is.Anything))[0][0];
            Assert.IsNotNull(args2);
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
        public void TestDeleteWithValidDataDoesRemoveRecord2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            AccessService.Expect(a => a.HasSameId(EmailsForCallRepository.GetNullableById(10).Template,
                null, 4, null)).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Delete(10, 4, null)
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);


            Assert.AreEqual("Email Removed Successfully.", Controller.Message);

            EmailsForCallRepository.AssertWasCalled(a => a.Remove(Arg<EmailsForCall>.Is.Anything));
            var args2 = (EmailsForCall)EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.Remove(Arg<EmailsForCall>.Is.Anything))[0][0];
            Assert.IsNotNull(args2);
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
        #endregion Delete Tests
    }
}
