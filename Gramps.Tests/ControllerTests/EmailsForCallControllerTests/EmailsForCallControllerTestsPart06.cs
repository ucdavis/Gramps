using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region SendCall Tests
        #region SendCall Get Tests

        [TestMethod]
        public void TestSendCallGetWHereCallNotFoundRegirectsToIndex()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("filterActive", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);

            Assert.AreEqual("filterStartCreate", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            Assert.AreEqual("filterEndCreate", result.RouteValues.ElementAt(4).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(4).Value);
            #endregion Assert		
        }

        [TestMethod]
        public void TestSendCallGetWHereNoAccessRegirectsToHome()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.SendCall(3)
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
        public void TestSendCallReturnsViewWithExpectedValues()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(3)
                .AssertViewRendered()
                .WithViewData<EmailsForCallSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.EmailsForCallList.Count());
            Assert.AreEqual(3, result.EmailsForCallList.ElementAt(0).Id);
            Assert.AreEqual(4, result.EmailsForCallList.ElementAt(1).Id);
            Assert.AreEqual(5, result.EmailsForCallList.ElementAt(2).Id);
            Assert.IsFalse(result.Immediate);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }
        #endregion SendCall Get Tests
        #region SendCall Post Tests

        [TestMethod]
        public void TestSendCallPostWHereCallNotFoundRegirectsToIndex()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.SendCall(4, true)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("filterActive", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);

            Assert.AreEqual("filterStartCreate", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            Assert.AreEqual("filterEndCreate", result.RouteValues.ElementAt(4).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(4).Value);
            #endregion Assert
        }

        [TestMethod]
        public void TestSendCallPostWHereNoAccessRegirectsToHome()
        {
            #region Arrange
            var fakeCall = new FakeCallForProposals();
            fakeCall.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.SendCall(3, false)
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
        public void TestSendCallPostReturnsViewIfCallNotActive()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].IsActive = false;
            calls[0].EndDate = DateTime.Now.AddDays(1);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var emailsForCall = new List<EmailsForCall>();
            for (int i = 0; i < 5; i++)
            {
                emailsForCall.Add(CreateValidEntities.EmailsForCall(i+1));
                emailsForCall[i].CallForProposal = CallForProposalRepository.GetNullableById(1);
            }
            emailsForCall[1].HasBeenEmailed = true;
            var fakeEmails = new FakeEmailsForCall();
            fakeEmails.Records(0, EmailsForCallRepository, emailsForCall);

            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, false)
                .AssertViewRendered()
                .WithViewData<EmailsForCallSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.EmailsForCallList.Count());
            Assert.AreEqual(1, result.CallForProposal.Id);
            Assert.IsFalse(result.Immediate);

            Assert.AreEqual("Is not active or end date is passed", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            EmailService.AssertWasNotCalled(a => a.SendEmail(Arg<HttpRequestBase>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<EmailTemplate>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything));
            EmailsForCallRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestSendCallPostReturnsViewIfEndDatePast()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].IsActive = true;
            calls[0].EndDate = DateTime.Now.AddDays(-1);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var emailsForCall = new List<EmailsForCall>();
            for (int i = 0; i < 5; i++)
            {
                emailsForCall.Add(CreateValidEntities.EmailsForCall(i + 1));
                emailsForCall[i].CallForProposal = CallForProposalRepository.GetNullableById(1);
            }
            emailsForCall[1].HasBeenEmailed = true;
            var fakeEmails = new FakeEmailsForCall();
            fakeEmails.Records(0, EmailsForCallRepository, emailsForCall);

            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, false)
                .AssertViewRendered()
                .WithViewData<EmailsForCallSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.EmailsForCallList.Count());
            Assert.AreEqual(1, result.CallForProposal.Id);
            Assert.IsFalse(result.Immediate);

            Assert.AreEqual("Is not active or end date is passed", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            EmailService.AssertWasNotCalled(a => a.SendEmail(Arg<HttpRequestBase>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<EmailTemplate>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything));
            EmailsForCallRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestSendCallPostReturnsViewIfCallNotActiveAndPast()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].IsActive = false;
            calls[0].EndDate = DateTime.Now.AddDays(-1);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var emailsForCall = new List<EmailsForCall>();
            for (int i = 0; i < 5; i++)
            {
                emailsForCall.Add(CreateValidEntities.EmailsForCall(i + 1));
                emailsForCall[i].CallForProposal = CallForProposalRepository.GetNullableById(1);
            }
            emailsForCall[1].HasBeenEmailed = true;
            var fakeEmails = new FakeEmailsForCall();
            fakeEmails.Records(0, EmailsForCallRepository, emailsForCall);

            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, false)
                .AssertViewRendered()
                .WithViewData<EmailsForCallSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.EmailsForCallList.Count());
            Assert.AreEqual(1, result.CallForProposal.Id);
            Assert.IsFalse(result.Immediate);

            Assert.AreEqual("Is not active or end date is passed", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            EmailService.AssertWasNotCalled(a => a.SendEmail(Arg<HttpRequestBase>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<EmailTemplate>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything));
            EmailsForCallRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestSendCallPostReturnsViewWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls[0].IsActive = true;
            calls[0].EndDate = DateTime.Now.AddDays(1);
            calls[0].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            calls[0].EmailTemplates[0].TemplateType = EmailTemplateType.InitialCall;
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var emailsForCall = new List<EmailsForCall>();
            for (int i = 0; i < 5; i++)
            {
                emailsForCall.Add(CreateValidEntities.EmailsForCall(i + 1));
                emailsForCall[i].CallForProposal = CallForProposalRepository.GetNullableById(1);
                emailsForCall[i].HasBeenEmailed = true;
            }
            emailsForCall[1].HasBeenEmailed = false;
            emailsForCall[3].HasBeenEmailed = false;
            var fakeEmails = new FakeEmailsForCall();
            fakeEmails.Records(0, EmailsForCallRepository, emailsForCall);

            #endregion Arrange

            #region Act
            var result = Controller.SendCall(1, false)
                .AssertViewRendered()
                .WithViewData<EmailsForCallSendViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.EmailsForCallList.Count());
            Assert.AreEqual(1, result.CallForProposal.Id);
            Assert.IsFalse(result.Immediate);

            Assert.AreEqual("2 Emails Generated", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(1, args[1]);
            Assert.AreEqual("Me", args[2]);

            EmailService.AssertWasCalled(a => a.SendEmail(Arg<HttpRequestBase>.Is.Anything, Arg<UrlHelper>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<EmailTemplate>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything), x=> x.Repeat.Times(2));
            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything), x=> x.Repeat.Times(2));
            CallForProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything));

            var args1 =
                EmailService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.SendEmail(Arg<HttpRequestBase>.Is.Anything, Arg<UrlHelper>.Is.Anything,
                                Arg<CallForProposal>.Is.Anything, Arg<EmailTemplate>.Is.Anything,
                                Arg<string>.Is.Anything, Arg<bool>.Is.Anything, Arg<string>.Is.Anything));
            Assert.AreEqual(1, ((CallForProposal)args1[0][2]).Id);
            Assert.AreEqual(EmailTemplateType.InitialCall, ((EmailTemplate)args1[0][3]).TemplateType);
            Assert.AreEqual("test2@testy.com", args1[0][4]);
            Assert.IsFalse(((bool)args1[0][5]));
            Assert.IsNull(args1[0][6]);

            Assert.AreEqual(1, ((CallForProposal)args1[1][2]).Id);
            Assert.AreEqual(EmailTemplateType.InitialCall, ((EmailTemplate)args1[1][3]).TemplateType);
            Assert.AreEqual("test4@testy.com", args1[1][4]);
            Assert.IsFalse(((bool)args1[1][5]));
            Assert.IsNull(args1[1][6]);

            var args2 =
                EmailsForCallRepository.GetArgumentsForCallsMadeOn(
                    a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            Assert.IsNotNull(((EmailsForCall)args2[0][0]).EmailedOnDate);
            Assert.AreEqual(DateTime.Now.Date, ((EmailsForCall)args2[0][0]).EmailedOnDate.Value.Date);
            Assert.AreEqual(true, ((EmailsForCall)args2[0][0]).HasBeenEmailed);

            Assert.IsNotNull(((EmailsForCall)args2[1][0]).EmailedOnDate);
            Assert.AreEqual(DateTime.Now.Date, ((EmailsForCall)args2[1][0]).EmailedOnDate.Value.Date);
            Assert.AreEqual(true, ((EmailsForCall)args2[1][0]).HasBeenEmailed);

            var args3 =
                CallForProposalRepository.GetArgumentsForCallsMadeOn(
                    a => a.EnsurePersistent(Arg<CallForProposal>.Is.Anything))[0][0];
            Assert.IsNotNull(((CallForProposal)args3).CallsSentDate);
            Assert.AreEqual(DateTime.Now.Date, ((CallForProposal)args3).CallsSentDate.Value.Date);

            #endregion Assert
        }
        #endregion SendCall Post Tests
        #endregion SendCall Tests
    }
}
