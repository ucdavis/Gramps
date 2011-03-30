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
    public class CallForProposalControllerTests : ControllerTestBase<CallForProposalController>
    {
        private readonly Type _controllerClass = typeof(CallForProposalController);
        public IRepository<CallForProposal> CallforproposalRepository;
        public IAccessService AccessService;

        public IRepository<Editor> EditorRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            CallforproposalRepository = FakeRepository<CallForProposal>();
            AccessService = MockRepository.GenerateStub<IAccessService>();   
            Controller = new TestControllerBuilder().CreateController<CallForProposalController>(CallforproposalRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public CallForProposalControllerTests()
        {
            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            //CallforproposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallforproposalRepository).Repeat.Any();
        }
        #endregion Init

        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/CallForProposal/Index/".ShouldMapTo<CallForProposalController>(a => a.Index(null, null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestLaunchMapping()
        {
            "~/CallForProposal/Launch/5".ShouldMapTo<CallForProposalController>(a => a.Launch(5));
        }

        #endregion Mapping Tests

        #region Method Tests

        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();            
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.CallForProposals.Count());
            Assert.IsNull(result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name1", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(2).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(3).Name);
            #endregion Assert				
        }

        [TestMethod]
        public void TestIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposals.Count());
            Assert.IsNull(result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name3", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotAnyone");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.CallForProposals.Count());
            Assert.IsNull(result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index("Active", null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposals.Count());
            Assert.AreEqual("Active", result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name1", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index("InActive", null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CallForProposals.Count());
            Assert.AreEqual("InActive", result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(0).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, new DateTime(2011, 03, 29).Date, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposals.Count());
            Assert.AreEqual(null, result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.AreEqual(new DateTime(2011, 03, 29).Date, result.FilterStartCreate);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index("Active", new DateTime(2011, 03, 29).Date, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.CallForProposals.Count());
            Assert.AreEqual("Active", result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.AreEqual(new DateTime(2011, 03, 29).Date, result.FilterStartCreate);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(1).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, new DateTime(2011, 03, 29).Date, new DateTime(2011, 03, 31).Date)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CallForProposals.Count());
            Assert.AreEqual(null, result.FilterActive);
            Assert.AreEqual(new DateTime(2011, 03, 31).Date, result.FilterEndCreate);
            Assert.AreEqual(new DateTime(2011, 03, 29).Date, result.FilterStartCreate);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(0).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView9()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, new DateTime(2011, 03, 31).Date)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.CallForProposals.Count());
            Assert.AreEqual(null, result.FilterActive);
            Assert.AreEqual(new DateTime(2011, 03, 31).Date, result.FilterEndCreate);
            Assert.AreEqual(null, result.FilterStartCreate);
            Assert.AreEqual("Name1", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(1).Name);
            #endregion Assert
        }
        #endregion Index Tests

        #region Launch Tests


        [TestMethod]
        public void TestLaunchRedirectsToIdexIfCallNotFound()
        {
            #region Arrange
            SetupDataForTests();
            Assert.IsNull(CallforproposalRepository.GetNullableById(8));
            #endregion Arrange

            #region Act
            Controller.Launch(8)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestLaunchRedirectsToHomeIndexIfNotAnEditor()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            SetupDataForTests();
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(false).Repeat.Any();
            Assert.IsNotNull(CallforproposalRepository.GetNullableById(5));
            #endregion Arrange

            #region Act
            Controller.Launch(5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0]; 
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }


        [TestMethod]
        public void TestLaunchReturnsViewWhenCallFoundWithAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            AccessService.Expect(a => a.HasAccess(null, 5, "Me")).Return(true).Repeat.Any();
            Assert.IsNotNull(CallforproposalRepository.GetNullableById(5));
            #endregion Arrange

            #region Act
            var result = Controller.Launch(5)
                .AssertViewRendered()
                .WithViewData<CallNavigationViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name5", result.CallForProposal.Name);

            Assert.IsNull(Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(5, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert			
        }
        #endregion Launch Tests


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
            Assert.AreEqual(2, result.Count(), "It looks like a method was added or removed from the controller.");
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

        [TestMethod]
        public void TestControllerMethodLaunchContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Launch");
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

        #region Helper Methods
        private void SetupDataForTests()
        {           
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].LoginId = "Me";

            users.Add(CreateValidEntities.User(2));
            users[1].LoginId = "NotMe";

            var calls = new List<CallForProposal>();
            for (int i = 0; i < 7; i++)
            {
                calls.Add(CreateValidEntities.CallForProposal(i+1));
                calls[i].IsActive = true;
                calls[i].CreatedDate = new DateTime(2011, 03, 29).AddDays(i);
            }

            calls[1].IsActive = false;
            calls[4].CreatedDate = new DateTime(2011, 12, 25);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallforproposalRepository, calls);

            var editors = new List<Editor>();
            for (int i = 0; i < 7; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].CallForProposal = CallforproposalRepository.GetNullableById(i + 1);
                editors[i].User = users[0];
            }
            editors[2].User = users[1];

            editors[5].CallForProposal = CallforproposalRepository.GetNullableById(4);
            editors[5].User = users[1];
            editors[6].CallForProposal = CallforproposalRepository.GetNullableById(5);
            editors[6].User = users[1];

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
        }
        

        #endregion Helper Methods
    }
}
