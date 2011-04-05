using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
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


namespace Gramps.Tests.ControllerTests.EditorControllerTests
{
    public partial class EditorControllerTests
    {
        #region SendCall Tests                   
        #region SendCall Get Tests

        [TestMethod]
        public void TestSendCallGetRedirectsToCallIndexIfCallForProposalNotFound()
        {
            #region Arrange
            var callFake = new FakeCallForProposals();
            callFake.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.RouteValues.Count);
            Assert.IsNull(result.RouteValues.ElementAt(2).Value);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            #endregion Assert		
        }


        [TestMethod]
        public void TestSendCallGetRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(false).Repeat.Any();
            var callFake = new FakeCallForProposals();
            callFake.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.SendCall(2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(2, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }


        [TestMethod]
        public void TestSendCallGetReturnsViewWithExpectedValues1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(true).Repeat.Any();
            SetupDataForTests3();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(2)
                .AssertViewRendered()
                .WithViewData<ReviewersSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(2, result.CallForProposal.Id);
            Assert.AreEqual(4, result.EditorsToNotify.Count());
            Assert.AreEqual("ReviewerName4", result.EditorsToNotify.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName6", result.EditorsToNotify.ElementAt(1).ReviewerName);
            Assert.AreEqual("ReviewerName8", result.EditorsToNotify.ElementAt(2).ReviewerName);
            Assert.AreEqual("ReviewerName10", result.EditorsToNotify.ElementAt(3).ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(2, args[1]);
            Assert.AreEqual("Me", args[2]);

            #endregion Assert		
        }

        [TestMethod]
        public void TestSendCallGetReturnsViewWithExpectedValues2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            SetupDataForTests3();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1)
                .AssertViewRendered()
                .WithViewData<ReviewersSendViewModel>();
            #endregion Act

            #region Assert           
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Immediate);
            Assert.AreEqual(1, result.CallForProposal.Id);
            Assert.AreEqual(2, result.EditorsToNotify.Count());
            Assert.AreEqual("ReviewerName7", result.EditorsToNotify.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName9", result.EditorsToNotify.ElementAt(1).ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            #endregion Assert
        }
        #endregion SendCall Get Tests
        #region SendCall Post Tests

        [TestMethod]
        public void TestSendCallPostRedirectsToCallIndexIfCallForProposalNotFound()
        {
            #region Arrange
            var callFake = new FakeCallForProposals();
            callFake.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(4, false)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.RouteValues.Count);
            Assert.IsNull(result.RouteValues.ElementAt(2).Value);
            Assert.IsNull(result.RouteValues.ElementAt(3).Value);
            Assert.IsNull(result.RouteValues.ElementAt(4).Value);
            #endregion Assert
        }


        [TestMethod]
        public void TestSendCallPostRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(false).Repeat.Any();
            var callFake = new FakeCallForProposals();
            callFake.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.SendCall(2, false)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(2, args[1]);
            Assert.AreEqual("Me", args[2]);

            #endregion Assert
        }


        [TestMethod]
        public void TestSendCallPostReturnsViewIfCallIsNotActive()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            SetupDataForTests4();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, false)
                .AssertViewRendered()
                .WithViewData<ReviewersSendViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Is not active, no emails sent.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.EditorsToNotify.Count());
            Assert.AreEqual("ReviewerName2", result.EditorsToNotify.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName3", result.EditorsToNotify.ElementAt(1).ReviewerName);
            Assert.AreEqual("ReviewerName4", result.EditorsToNotify.ElementAt(2).ReviewerName);
            Assert.AreEqual("ReviewerName5", result.EditorsToNotify.ElementAt(3).ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            MembershipService.AssertWasNotCalled(a => a.DoesUserExist(Arg<string>.Is.Anything));
            MembershipService.AssertWasNotCalled(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            MembershipService.AssertWasNotCalled(a => a.ResetPassword(Arg<string>.Is.Anything));

            EmailService.AssertWasNotCalled(a => a.SendEmail(
                Arg<HttpRequestBase>.Is.Anything, 
                Arg<UrlHelper>.Is.Anything, 
                Arg<CallForProposal>.Is.Anything, 
                Arg<EmailTemplate>.Is.Anything, 
                Arg<string>.Is.Anything, 
                Arg<bool>.Is.Anything, 
                Arg<string>.Is.Anything));
            EditorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestSendCallPostSendsEmailsAndCreatesUsers()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(true).Repeat.Any();
            MembershipService.Expect(a => a.DoesUserExist("test8@testy.com")).Return(true).Repeat.Any();
            MembershipService.Expect(a => a.DoesUserExist("test9@testy.com")).Return(false).Repeat.Any();
            MembershipService.Expect(a => a.DoesUserExist("test10@testy.com")).Return(false).Repeat.Any();

            MembershipService.Expect(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(MembershipCreateStatus.Success).Repeat.Any();

            MembershipService.Expect(a => a.ResetPassword("test9@testy.com")).Return("Reset1").Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("test10@testy.com")).Return("Reset2").Repeat.Any();

            EmailService.Expect(a => a.SendEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything)).Repeat.Any();

            EditorRepository.Expect(a => a.EnsurePersistent(Arg<Editor>.Is.Anything)).Repeat.Any();

            SetupDataForTests4();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(2, false)
                .AssertViewRendered()
                .WithViewData<ReviewersSendViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("3 Emails Generated.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.EditorsToNotify.Count());
            Assert.AreEqual("test7@testy.com", result.EditorsToNotify.ElementAt(0).ReviewerEmail);
            Assert.AreEqual("test8@testy.com", result.EditorsToNotify.ElementAt(1).ReviewerEmail);
            Assert.AreEqual("test9@testy.com", result.EditorsToNotify.ElementAt(2).ReviewerEmail);
            Assert.AreEqual("test10@testy.com", result.EditorsToNotify.ElementAt(3).ReviewerEmail);


            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args1 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args1[0]);
            Assert.AreEqual(2, args1[1]);
            Assert.AreEqual("Me", args1[2]);

            MembershipService.AssertWasCalled(a => a.DoesUserExist(Arg<string>.Is.Anything), x => x.Repeat.Times(3));
            var args2 = MembershipService.GetArgumentsForCallsMadeOn(a => a.DoesUserExist(Arg<string>.Is.Anything)); 
            Assert.IsNotNull(args2);
            Assert.AreEqual("test8@testy.com", args2[0][0]);
            Assert.AreEqual("test9@testy.com", args2[1][0]);
            Assert.AreEqual("test10@testy.com", args2[2][0]);

            MembershipService.AssertWasCalled(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything), x => x.Repeat.Times(2));
            var args3 = MembershipService.GetArgumentsForCallsMadeOn(a => a.CreateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            Assert.IsNotNull(args3);
            Assert.AreEqual("test9@testy.com", args3[0][0]);
            Assert.AreEqual("Ht548*%KjjY2#", args3[0][1]);
            Assert.AreEqual("test9@testy.com", args3[0][2]);
            Assert.AreEqual("test10@testy.com", args3[1][0]);
            Assert.AreEqual("Ht548*%KjjY2#", args3[1][1]);
            Assert.AreEqual("test10@testy.com", args3[1][2]);

            MembershipService.AssertWasCalled(a => a.ResetPassword(Arg<string>.Is.Anything), x => x.Repeat.Times(2));
            var args4 = MembershipService.GetArgumentsForCallsMadeOn(a => a.ResetPassword(Arg<string>.Is.Anything));            
            Assert.IsNotNull(args4);
            Assert.AreEqual("test9@testy.com", args4[0][0]);
            Assert.AreEqual("test10@testy.com", args4[1][0]);

            EmailService.AssertWasCalled(a => a.SendEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything), x => x.Repeat.Times(3));
            var args5 = EmailService.GetArgumentsForCallsMadeOn(a => a.SendEmail(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything)); 
            Assert.IsNotNull(args5);
            Assert.AreEqual(2, ((CallForProposal) args5[0][2]).Id);
            Assert.AreEqual(2, ((CallForProposal)args5[1][2]).Id);
            Assert.AreEqual(2, ((CallForProposal)args5[2][2]).Id);

            Assert.AreEqual(EmailTemplateType.ReadyForReview, ((EmailTemplate)args5[0][3]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ReadyForReview, ((EmailTemplate)args5[1][3]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ReadyForReview, ((EmailTemplate)args5[2][3]).TemplateType);

            Assert.AreEqual("test8@testy.com", args5[0][4]);
            Assert.AreEqual("test9@testy.com", args5[1][4]);
            Assert.AreEqual("test10@testy.com", args5[2][4]);

            Assert.IsFalse((bool)args5[0][5]);
            Assert.IsFalse((bool)args5[1][5]);
            Assert.IsFalse((bool)args5[2][5]);

            Assert.AreEqual(null, args5[0][6]);
            Assert.AreEqual("Reset1", args5[1][6]);
            Assert.AreEqual("Reset2", args5[2][6]);

            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything), x => x.Repeat.Times(3));
            var args6 = EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything)); 
            Assert.IsNotNull(args6);
            Assert.AreEqual("test8@testy.com", ((Editor)args6[0][0]).ReviewerEmail);
            Assert.AreEqual("test9@testy.com", ((Editor) args6[1][0]).ReviewerEmail);
            Assert.AreEqual("test10@testy.com", ((Editor)args6[2][0]).ReviewerEmail);
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(((Editor)args6[i][0]).HasBeenNotified);
                Assert.AreEqual(DateTime.Now.Date,((Editor)args6[i][0]).NotifiedDate.Value.Date);
            }
            #endregion Assert
        }


        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Write these tests");
            //Test exceptions
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        #endregion SendCall Post Tests
        #endregion SendCall Tests
    }
}
