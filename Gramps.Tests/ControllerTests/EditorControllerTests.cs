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


namespace Gramps.Tests.ControllerTests
{
    [TestClass]
    public class EditorControllerTests : ControllerTestBase<EditorController>
    {
        private readonly Type _controllerClass = typeof(EditorController);
        public IRepository<Editor> EditorRepository;
        public IAccessService AccessService;
        public IEmailService EmailService;
        public IRepository<Template> TemplateRepository;
        public IRepository<CallForProposal> CallForProposalRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            EditorRepository = FakeRepository<Editor>();
            AccessService = MockRepository.GenerateStub<IAccessService>();
            EmailService = MockRepository.GenerateStub<IEmailService>();  
            Controller = new TestControllerBuilder().CreateController<EditorController>(EditorRepository, AccessService, EmailService);            
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public EditorControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            TemplateRepository = FakeRepository<Template>();
            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();
        }
        #endregion Init

        #region Mapping Tests
        [TestMethod]
        public void TestExampleMapping()
        {
            "~/Editor/Index/".ShouldMapTo<EditorController>(a => a.Index(null, null));
        }
        #endregion Mapping Tests

        #region Method Tests

        #region Index Tests

        [TestMethod]
        public void TestIndexRedirectsToHomeControllerIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(null, 5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexRedirectsToHomeControllerIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(3, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.IsNull(args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        [TestMethod]
        public void TestIndexReturnsViewWithExpectedData1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, 5)
                .AssertViewRendered()
                .WithViewData<EditorListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(3, result.EditorList.Count());
            Assert.AreEqual("ReviewerName1", result.EditorList.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName2", result.EditorList.ElementAt(1).ReviewerName);
            Assert.AreEqual("ReviewerName4", result.EditorList.ElementAt(2).ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnsViewWithExpectedData2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(3, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(3, null)
                .AssertViewRendered()
                .WithViewData<EditorListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(2, result.EditorList.Count());
            Assert.AreEqual("ReviewerName5", result.EditorList.ElementAt(0).ReviewerName);
            Assert.AreEqual("ReviewerName7", result.EditorList.ElementAt(1).ReviewerName);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(3, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }


        #endregion Index Tests  
        
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
        /// Tests the controller has four attributes.
        /// </summary>
        [TestMethod]
        public void TestControllerHasFourAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            Assert.AreEqual(4, result.Count());
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
        public void TestControllerHasUserOnlyAttribute()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            #endregion Arrange

            #region Act
            var result = controllerClass.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            #endregion Act

            #region Assert
            Assert.IsTrue(result.Count() > 0, "UserOnlyAttribute not found.");
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
            Assert.AreEqual(1, result.Count(), "It looks like a method was added or removed from the controller.");
            #endregion Assert
        }

        [TestMethod]
        public void TestControllerMethodIndexContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Index");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.GetCustomAttributes(true);
            #endregion Act

            #region Assert
            //Assert.AreEqual(1, expectedAttribute.Count(), "UserOnlyAttribute not found");
            Assert.AreEqual(0, allAttributes.Count());
            #endregion Assert
        }

        //Examples
        
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

        #region Helpers
        private void SetupDataForTests()
        {
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            for (int i = 0; i < 4; i++)
            {
                editors.Add(CreateValidEntities.Editor(i+1));
                editors[i].Template = null;
                editors[i].CallForProposal = CallForProposalRepository.GetNullableById(5);
            }
            editors[2].CallForProposal = CallForProposalRepository.GetNullableById(1);

            for (int i = 4; i < 8; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].Template = TemplateRepository.GetNullableById(3);
                editors[i].CallForProposal = null;
            }
            editors[5].Template = TemplateRepository.GetNullableById(1);
            editors[7].Template = TemplateRepository.GetNullableById(2);

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
        }
        #endregion Helpers
    }
}
