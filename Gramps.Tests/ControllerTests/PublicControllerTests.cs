using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Helpers;
using Gramps.Models;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;



namespace Gramps.Tests.ControllerTests
{
    [TestClass]
    public class PublicControllerTests : ControllerTestBase<PublicController>
    {
        private readonly Type _controllerClass = typeof(PublicController);

        public IFormsAuthenticationService FormsService;
        public IMembershipService MembershipService;
        public IEmailService EmailService;
        public IRepository<Proposal> ProposalRepository;
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<Editor> EditorRepository;


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {

            FormsService = MockRepository.GenerateStub<IFormsAuthenticationService>();
            MembershipService = MockRepository.GenerateStub<IMembershipService>();
            EmailService = MockRepository.GenerateStub<IEmailService>();

            Controller = new TestControllerBuilder().CreateController<PublicController>(FormsService, MembershipService, EmailService);
            //Controller = new TestControllerBuilder().CreateController<PublicController>(PublicRepository, ExampleService);
        }

        

        protected override void RegisterRoutes()
        {
            //new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public PublicControllerTests()
        {
            ProposalRepository = FakeRepository<Proposal>();
            Controller.Repository.Expect(a => a.OfType<Proposal>()).Return(ProposalRepository).Repeat.Any();

            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();
        }
        #endregion Init

        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestLogOnGetMapping()
        {
            "~/Public/LogOn/".ShouldMapTo<PublicController>(a => a.LogOn());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestLogOnPostMapping()
        {
            "~/Public/LogOn/".ShouldMapTo<PublicController>(a => a.LogOn(null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestForgotPasswordGetMapping()
        {
            "~/Public/ForgotPassword/".ShouldMapTo<PublicController>(a => a.ForgotPassword());
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestForgotPasswordPostMapping()
        {
            "~/Public/ForgotPassword/".ShouldMapTo<PublicController>(a => a.ForgotPassword(string.Empty, false), true);
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestLogOffMapping()
        {
            "~/Public/LogOff/".ShouldMapTo<PublicController>(a => a.LogOff());
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestChangePasswordGetMapping()
        {
            "~/Public/ChangePassword/".ShouldMapTo<PublicController>(a => a.ChangePassword());
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestChangePasswordPostMapping()
        {
            "~/Public/ChangePassword/".ShouldMapTo<PublicController>(a => a.ChangePassword(null));
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestChangePasswordSuccessMapping()
        {
            "~/Public/ChangePasswordSuccess/".ShouldMapTo<PublicController>(a => a.ChangePasswordSuccess());
        }
        #endregion Mapping Tests

        #region Method Tests

        #region Logon Get Tests

        [TestMethod]
        public void TestLogonGetReturnView()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            Controller.LogOn()
                .AssertViewRendered();
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion Logon Get Tests

        #region LogOn Post Tests

        [TestMethod]
        public void TestLogOnPostWithErrorReturnsView1()
        {
            #region Arrange
            Controller.ModelState.AddModelError("Fake", "Fake Error");
            var logonModel = new LogOnModel();
            logonModel.UserName = "Jason";
            #endregion Arrange

            #region Act
            var result = Controller.LogOn(logonModel, null)
                .AssertViewRendered()
                .WithViewData<LogOnModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Fake Error");
            Assert.IsNotNull(result);
            Assert.AreEqual("jason", result.UserName);
            MembershipService.AssertWasNotCalled(a => a.ValidateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            FormsService.AssertWasNotCalled(a => a.SignIn(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestLogOnPostWithErrorReturnsView2()
        {
            #region Arrange
            var logonModel = new LogOnModel();
            logonModel.UserName = "Jason";
            logonModel.Password = "SomePassWord";
            MembershipService.Expect(a => a.ValidateUser("jason", "SomePassWord")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.LogOn(logonModel, null)
                .AssertViewRendered()
                .WithViewData<LogOnModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("The user name or password provided is incorrect.");
            Assert.IsNotNull(result);
            Assert.AreEqual("jason", result.UserName);
            MembershipService.AssertWasCalled(a => a.ValidateUser("jason", "SomePassWord"));
            FormsService.AssertWasNotCalled(a => a.SignIn(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestLogOnPostRedirects1()
        {
            #region Arrange
            var logonModel = new LogOnModel();
            logonModel.UserName = "Jason";
            logonModel.Password = "SomePassWord";
            logonModel.RememberMe = false;
            MembershipService.Expect(a => a.ValidateUser("jason", "SomePassWord")).Return(true).Repeat.Any();

            #endregion Arrange

            #region Act
            Controller.LogOn(logonModel, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            MembershipService.AssertWasCalled(a => a.ValidateUser("jason", "SomePassWord"));
            FormsService.AssertWasCalled(a => a.SignIn("jason", false));
            #endregion Assert
        }

        [TestMethod]
        public void TestLogOnPostRedirects2()
        {
            #region Arrange
            var logonModel = new LogOnModel();
            logonModel.UserName = "Jason";
            logonModel.Password = "SomePassWord";
            logonModel.RememberMe = true;
            MembershipService.Expect(a => a.ValidateUser("jason", "SomePassWord")).Return(true).Repeat.Any();

            #endregion Arrange

            #region Act
            Controller.LogOn(logonModel, null)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Home());
            #endregion Act

            #region Assert
            MembershipService.AssertWasCalled(a => a.ValidateUser("jason", "SomePassWord"));
            FormsService.AssertWasCalled(a => a.SignIn("jason", true));
            #endregion Assert
        }

        [TestMethod]
        public void TestLogOnPostRedirects3()
        {
            #region Arrange
            var logonModel = new LogOnModel();
            logonModel.UserName = "Jason";
            logonModel.Password = "SomePassWord";
            logonModel.RememberMe = false;
            MembershipService.Expect(a => a.ValidateUser("jason", "SomePassWord")).Return(true).Repeat.Any();

            #endregion Arrange

            #region Act
            var result = Controller.LogOn(logonModel, "/Error/Index")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("/Error/Index", result.Url);
            MembershipService.AssertWasCalled(a => a.ValidateUser("jason", "SomePassWord"));
            FormsService.AssertWasCalled(a => a.SignIn("jason", false));
            #endregion Assert
        }

        [TestMethod]
        public void TestLogOnPostRedirects4()
        {
            #region Arrange
            var logonModel = new LogOnModel();
            logonModel.UserName = "Jason";
            logonModel.Password = "SomePassWord";
            logonModel.RememberMe = true;
            MembershipService.Expect(a => a.ValidateUser("jason", "SomePassWord")).Return(true).Repeat.Any();

            #endregion Arrange

            #region Act
            var result = Controller.LogOn(logonModel, "/Error/Index")
                .AssertHttpRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("/Error/Index", result.Url);
            MembershipService.AssertWasCalled(a => a.ValidateUser("jason", "SomePassWord"));
            FormsService.AssertWasCalled(a => a.SignIn("jason", true));
            #endregion Assert
        }
        #endregion LogOn Post Tests

        #region ForgotPassword Get Tests

        [TestMethod]
        public void TestForgotPasswordGetReturnsView()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword()
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);            
            #endregion Assert		
        }
        #endregion ForgotPassword Get Tests

        #region ForgotPassword Post Tests

        [TestMethod]
        public void TestForgotPasswordPostReturnsViewWhenNotValid1()
        {
            #region Arrange
            //var fakeProposals = new FakeProposals();
            //fakeProposals.Records(3, ProposalRepository);
            //var fakeCalls = new FakeCallForProposals();
            //fakeCalls.Records(3, CallForProposalRepository);
            //var fakeEditors = new FakeEditors();
            //fakeEditors.Records(3, EditorRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword("Jason", false)
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Recaptcha value not valid");
            Assert.IsNotNull(result);
            Assert.AreEqual("jason", result.UserName);
            Assert.AreEqual("Unable to reset password", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestForgotPasswordPostReturnsViewWhenNotValid2()
        {
            #region Arrange
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(3, ProposalRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(3, EditorRepository);
            MembershipService.Expect(a => a.DoesUserExist("jason")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword("Jason", true)
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Email not found", "Linked Email not found");
            Assert.IsNotNull(result);
            Assert.AreEqual("jason", result.UserName);
            MembershipService.AssertWasCalled(a => a.DoesUserExist("jason"));
            Assert.AreEqual("Unable to reset password", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestForgotPasswordPostReturnsViewWhenNotValid3()
        {
            #region Arrange
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(3, ProposalRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(3, EditorRepository);
            MembershipService.Expect(a => a.DoesUserExist("jason")).Return(true).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword("Jason", true)
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Linked Email not found");
            Assert.IsNotNull(result);
            Assert.AreEqual("jason", result.UserName);
            MembershipService.AssertWasCalled(a => a.DoesUserExist("jason"));
            Assert.AreEqual("Unable to reset password", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestForgotPasswordPostRedirectsWhenValid1()
        {
            #region Arrange
            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals[0].Email = "jason";
            proposals[0].CallForProposal = CreateValidEntities.CallForProposal(77);
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(0, ProposalRepository, proposals);

            MembershipService.Expect(a => a.DoesUserExist("jason")).Return(true).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("jason")).Return("tdber12").Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ForgotPassword("Jason", true)
                .AssertActionRedirect()
                .ToAction<PublicController>(a => a.LogOn());
            #endregion Act

            #region Assert
            Assert.AreEqual("A new password has been sent to your email. It should arrive in a few minutes", Controller.Message);
            EmailService.AssertWasCalled(a => a.SendPasswordReset(Arg<CallForProposal>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            var args = EmailService.GetArgumentsForCallsMadeOn(a => a.SendPasswordReset(Arg<CallForProposal>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0]; 
            Assert.AreEqual("Name77", ((CallForProposal)args[0]).Name);
            Assert.AreEqual("jason", args[1]);
            Assert.AreEqual("tdber12", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestForgotPasswordPostRedirectsWhenValid2()
        {
            #region Arrange
            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals[0].Email = "jason";
            proposals[0].CallForProposal = CreateValidEntities.CallForProposal(77);
            proposals[0].CallForProposal.IsActive = false;
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals[1].Email = "jason";
            proposals[1].CallForProposal = CreateValidEntities.CallForProposal(88);
            proposals[1].CallForProposal.IsActive = false;
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(0, ProposalRepository, proposals);

            MembershipService.Expect(a => a.DoesUserExist("jason")).Return(true).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("jason")).Return("tdber12").Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ForgotPassword("Jason", true)
                .AssertActionRedirect()
                .ToAction<PublicController>(a => a.LogOn());
            #endregion Act

            #region Assert
            Assert.AreEqual("A new password has been sent to your email. It should arrive in a few minutes", Controller.Message);
            EmailService.AssertWasCalled(a => a.SendPasswordReset(Arg<CallForProposal>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            var args = EmailService.GetArgumentsForCallsMadeOn(a => a.SendPasswordReset(Arg<CallForProposal>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual("Name77", ((CallForProposal)args[0]).Name);
            Assert.AreEqual("jason", args[1]);
            Assert.AreEqual("tdber12", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestForgotPasswordPostRedirectsWhenValid3()
        {
            #region Arrange
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(3, ProposalRepository);

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors[0].ReviewerEmail = "jason";
            editors[0].CallForProposal = CreateValidEntities.CallForProposal(22);
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(11));
            calls[0].Editors.Add(EditorRepository.GetNullableById(1));
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            MembershipService.Expect(a => a.DoesUserExist("jason")).Return(true).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("jason")).Return("tdber12").Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.ForgotPassword("Jason", true)
                .AssertActionRedirect()
                .ToAction<PublicController>(a => a.LogOn());
            #endregion Act

            #region Assert
            Assert.AreEqual("A new password has been sent to your email. It should arrive in a few minutes", Controller.Message);
            EmailService.AssertWasCalled(a => a.SendPasswordReset(Arg<CallForProposal>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            var args = EmailService.GetArgumentsForCallsMadeOn(a => a.SendPasswordReset(Arg<CallForProposal>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual("Name11", ((CallForProposal)args[0]).Name);
            Assert.AreEqual("jason", args[1]);
            Assert.AreEqual("tdber12", args[2]);
            #endregion Assert
        }
        #endregion ForgotPassword Post Tests

        #region LogOff Tests

        [TestMethod]
        public void TestLogOffRedirects()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            Controller.LogOff()
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.LoggedOut());
            #endregion Act

            #region Assert
            FormsService.AssertWasCalled(a => a.SignOut());
            #endregion Assert		
        }
        #endregion LogOff Tests

        #region ChangePassword Get Tests

        [TestMethod]
        public void TestChangePasswordReturnsView()
        {
            #region Arrange
            MembershipService.Expect(a => a.MinPasswordLength).Return(56);
            #endregion Arrange

            #region Act
            var result = Controller.ChangePassword()
                .AssertViewRendered();
            #endregion Act

            #region Assert
            Assert.AreEqual(56, result.ViewData["PasswordLength"]);
            #endregion Assert		
        }
        #endregion ChangePassword Get Tests

        #region ChangePassword Post Tests

        [TestMethod]
        public void TestChangePasswordReturnsViewWhenNotValid1()
        {
            #region Arrange
            Controller.ModelState.AddModelError("Fake", "Fake Error");
            var model = new ChangePasswordModel();
            model.OldPassword = "oldie";
            MembershipService.Expect(a => a.MinPasswordLength).Return(23);
            #endregion Arrange

            #region Act
            var result = Controller.ChangePassword(model)
                .AssertViewRendered();
                
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Fake Error");
            Assert.IsNotNull(result);
            Assert.AreEqual(23, result.ViewData["PasswordLength"]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestChangePasswordReturnsViewWhenNotValid2()
        {
            #region Arrange
            Controller.ModelState.AddModelError("Fake", "Fake Error");
            var model = new ChangePasswordModel();
            model.OldPassword = "oldie";
            MembershipService.Expect(a => a.MinPasswordLength).Return(23);
            #endregion Arrange

            #region Act
            var result = Controller.ChangePassword(model)
                .AssertViewRendered()
                .WithViewData<ChangePasswordModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Fake Error");
            Assert.IsNotNull(result);
            Assert.AreEqual("oldie", result.OldPassword);
            #endregion Assert
        }

        [TestMethod]
        public void TestChangePasswordReturnsViewWhenNotValid3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "jason");
            var model = new ChangePasswordModel();
            model.OldPassword = "oldie";
            model.NewPassword = "newbie";            
            MembershipService.Expect(a => a.MinPasswordLength).Return(23);
            MembershipService.Expect(a => a.ChangePassword("jason", "oldie", "newbie")).Return(false);
            #endregion Arrange

            #region Act
            var result = Controller.ChangePassword(model)
                .AssertViewRendered()
                .WithViewData<ChangePasswordModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("The current password is incorrect or the new password is invalid.");
            Assert.IsNotNull(result);
            Assert.AreEqual("oldie", result.OldPassword);
            Assert.AreEqual("newbie", result.NewPassword);
            MembershipService.AssertWasCalled(a => a.ChangePassword("jason", "oldie", "newbie"));
            #endregion Assert
        }


        [TestMethod]
        public void TestChangePasswordRedirectsWhenValid()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "jason");
            var model = new ChangePasswordModel();
            model.OldPassword = "oldie";
            model.NewPassword = "newbie";
            MembershipService.Expect(a => a.MinPasswordLength).Return(23);
            MembershipService.Expect(a => a.ChangePassword("jason", "oldie", "newbie")).Return(true);
            #endregion Arrange

            #region Act
            Controller.ChangePassword(model)
                .AssertActionRedirect()
                .ToAction<PublicController>(a => a.ChangePasswordSuccess());
            #endregion Act

            #region Assert
            MembershipService.AssertWasCalled(a => a.ChangePassword("jason", "oldie", "newbie"));
            #endregion Assert		
        }
        #endregion ChangePassword Post Tests

        #region ChangePasswordSuccess Tests

        [TestMethod]
        public void TestChangePasswordSuccessReturnsView()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            Controller.ChangePasswordSuccess()
                .AssertViewRendered();
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion ChangePasswordSuccess Tests
        #endregion Method Tests

        #region Reflection Tests

        #region Controller Class Tests
        /// <summary>
        /// Tests the controller inherits from application controller.
        /// </summary>
        [TestMethod]
        public void TestControllerInheritsFromApplicationController()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            Assert.IsNotNull(controllerClass.BaseType);
            var result = controllerClass.BaseType.Name;
            #endregion Act

            #region Assert
            Assert.AreEqual("ApplicationController", result);
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has only three attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasOnlyThreeAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result.Count());
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has transaction attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasTransactionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseTransactionsByDefaultAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseTransactionsByDefaultAttribute not found.");
            #endregion Assert
        }

        /// <summary>
        /// Tests the controller has anti forgery token attribute.
        /// </summary>
        [TestMethod]
        public void TestControllerHasAntiForgeryTokenAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UseAntiForgeryTokenOnPostByDefault>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UseAntiForgeryTokenOnPostByDefault not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasVersionAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<VersionAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "VersionAttribute not found.");
            #endregion Assert
        }
        
        [TestMethod]
        public void TestControllerHasLocServiceMessageAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<LocServiceMessageAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "LocServiceMessageAttribute not found.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerHasHandleErrorAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<HandleErrorAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "HandleErrorAttribute not found.");
            #endregion Assert
        }
        #endregion Controller Class Tests

        #region Controller Method Tests

        [TestMethod]
        public void TestControllerContainsExpectedNumberOfPublicMethods()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetMethods().Where(a => a.DeclaringType == controllerClass);
            #endregion Act

            #region Assert
            Assert.AreEqual(8, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLogOnGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LogOn");
            #endregion Arrange

            #region Act            
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestControllerMethodLogOnPostContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "LogOn");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestControllerMethodForgotPasswordGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ForgotPassword");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodForgotPasswordPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ForgotPassword");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestControllerMethodForgotPasswordPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ForgotPassword");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<CaptchaValidatorAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "CaptchaValidatorAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestControllerMethodlogOffContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("LogOff");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestControllerMethodChangePasswordGetContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ChangePassword");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<PublicAuthorize>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "PublicAuthorize not found");
            Assert.AreEqual(1, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestControllerMethodChangePasswordPostContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ChangePassword");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<PublicAuthorize>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "PublicAuthorize not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestControllerMethodChangePasswordPostContainsExpectedAttributes2()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "ChangePassword");
            #endregion Arrange

            #region Act
            var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
            var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
            Assert.AreEqual(2, allAttributes.Count());
            #endregion Assert
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestControllerMethodChangePasswordSuccessContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("ChangePasswordSuccess");
            #endregion Arrange

            #region Act
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
