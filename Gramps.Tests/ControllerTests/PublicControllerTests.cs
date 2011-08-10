using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Web.Attributes;



namespace Gramps.Tests.ControllerTests
{
    [TestClass]
    public class PublicControllerTests : ControllerTestBase<PublicController>
    {
        private readonly Type _controllerClass = typeof(PublicController);

        public  IFormsAuthenticationService FormsService;
        public  IMembershipService MembershipService;
        public  IEmailService EmailService;
        public IRepository<Proposal> ProposalRepository; 


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
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(3, ProposalRepository);
            #endregion Arrange

            #region Act
            var result = Controller.ForgotPassword("Jason", false)
                .AssertViewRendered()
                .WithViewData<ForgotPasswordModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("");
            Assert.IsNotNull(result);
            Assert.AreEqual("jason", result.UserName);
            #endregion Assert		
        }
        #endregion ForgotPassword Post Tests

        [TestMethod]
        public void TestWriteMethodTests()
        {
            #region Arrange
            Assert.Inconclusive("Need to write these tests");          
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert		
        }      
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
            Assert.Inconclusive("Tests are still being written. When done, remove this line.");
            Assert.AreEqual(3, result.Count(), "It looks like a method was added or removed from the controller.");
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

        //Examples

        //[TestMethod]
        //public void TestControllerMethodLogOnContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("LogOn");
        //    #endregion Arrange

        //    #region Act
        //    //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(0, allAttributes.Count());
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodLogOutContainsExpectedAttributes()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethod("LogOut");
        //    #endregion Arrange

        //    #region Act
        //    //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(0, allAttributes.Count());
        //    #endregion Assert
        //}


        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes1()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(1, allAttributes.Count());
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes2()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<HttpPostAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "HttpPostAttribute not found");
        //    Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestControllerMethodCreateContainsExpectedAttributes3()
        //{
        //    #region Arrange
        //    var controllerClass = _controllerClass;
        //    var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
        //    #endregion Arrange

        //    #region Act
        //    var expectedAttribute = controllerMethod.ElementAt(1).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
        //    var allAttributes = controllerMethod.ElementAt(1).GetCustomAttributes(true);
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
        //    Assert.AreEqual(2, allAttributes.Count(), "More than expected custom attributes found.");
        //    #endregion Assert
        //}


        #endregion Controller Method Tests

        #endregion Reflection Tests
    }
}
