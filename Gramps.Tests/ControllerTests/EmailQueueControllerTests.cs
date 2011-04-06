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
using Gramps.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;



namespace Gramps.Tests.ControllerTests
{
    [TestClass]
    public class EmailQueueControllerTests : ControllerTestBase<EmailQueueController>
    {
        private readonly Type _controllerClass = typeof(EmailQueueController);
        public IRepository<EmailQueue> EmailQueueRepository;
        public IAccessService AccessService;
        public IRepository<CallForProposal> CallForProposalRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            EmailQueueRepository = FakeRepository<EmailQueue>();
            AccessService = MockRepository.GenerateStub<IAccessService>();  
            Controller = new TestControllerBuilder().CreateController<EmailQueueController>(EmailQueueRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public EmailQueueControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<EmailQueue>()).Return(EmailQueueRepository).Repeat.Any();
        }
        #endregion Init

        #region Mapping Tests

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/EmailQueue/Index/5".ShouldMapTo<EmailQueueController>(a => a.Index(5));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/EmailQueue/Details/5".ShouldMapTo<EmailQueueController>(a => a.Details(5, 3), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestEditMapping()
        {
            "~/EmailQueue/Edit/5".ShouldMapTo<EmailQueueController>(a => a.Edit(5, 3), true);
        }
        #endregion Mapping Tests

        #region Method Tests
        #region Index Tests
                
        [TestMethod]
        public void TestIndexRedirectsToCallForProposalIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Index(4)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert		
        }



        [TestMethod]
        public void TestIndexRedirectsToHomeIfNoAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnsViewWithExpectedValuesIfHasAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(true).Repeat.Any();

            var fakeEmails = new List<EmailQueue>();
            for (int i = 0; i < 4; i++)
            {
                fakeEmails.Add(CreateValidEntities.EmailQueue(i+1));
                fakeEmails[i].CallForProposal = CallForProposalRepository.GetNullableById(2);
            }
            fakeEmails[1].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmailQueue = new FakeEmailQueues();
            fakeEmailQueue.Records(0, EmailQueueRepository, fakeEmails);
            #endregion Arrange

            #region Act
            var result = Controller.Index(2)
                .AssertViewRendered()
                .WithViewData<EmailQueueListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.EmailQueues.Count());
            #endregion Assert		
        }

        #endregion Index Tests
        #region Details Tests
        [TestMethod]
        public void TestDetailsRedirectsToCallForProposalIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Details(4, 5)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsRedirectsToHomeIfNoAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Details(4, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsRedirectsToIndexIfEmailNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(3, EmailQueueRepository);

            #endregion Arrange

            #region Act
            var result = Controller.Details(4, 3)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Email not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);

            #endregion Assert
        }

        [TestMethod]
        public void TestDetailsRedirectsToHomeIndexIfIdDifferent()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a =>a.HasSameId(
                Arg<Template>.Is.Anything, 
                Arg<CallForProposal>.Is.Anything, 
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything)).Return(false).Repeat.Any();

            var fakeEmail = new List<EmailQueue>();
            for (int i = 0; i < 3; i++)
            {
                fakeEmail.Add(CreateValidEntities.EmailQueue(i+1));
                fakeEmail[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);

            #endregion Arrange

            #region Act
            Controller.Details(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(3, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(1, args2[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestDetailsReturnsViewWithExpectedResults()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(
                Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything)).Return(true).Repeat.Any();

            var fakeEmail = new List<EmailQueue>();
            for (int i = 0; i < 3; i++)
            {
                fakeEmail.Add(CreateValidEntities.EmailQueue(i + 1));
                fakeEmail[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);
            #endregion Arrange

            #region Act
            var result = Controller.Details(2, 3)
                .AssertViewRendered()
                .WithViewData<EmailQueueViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(3, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(3, args2[3]);

            Assert.IsNotNull(result);
            Assert.AreEqual("Subject2", result.EmailQueue.Subject);
            Assert.AreEqual(3, result.CallForProposal.Id);
            #endregion Assert		
        }
        #endregion Details Tests
        #region Edit Tests                
        #region Edit Get Tests
        [TestMethod]
        public void TestEditRedirectsToCallForProposalIndexIfCallNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            #endregion Arrange

            #region Act
            Controller.Edit(4, 5)
                .AssertActionRedirect()
                .ToAction<CallForProposalController>(a => a.Index(null, null, null));
            #endregion Act

            #region Assert
            #endregion Assert
        }

        [TestMethod]
        public void TestEditRedirectsToHomeIfNoAccess()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(4, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditRedirectsToIndexIfEmailNotFound()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(3, EmailQueueRepository);

            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 3)
                .AssertActionRedirect()
                .ToAction<EmailQueueController>(a => a.Index(3));
            #endregion Act

            #region Assert
            Assert.AreEqual("Email not found.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.RouteValues.ElementAt(2).Value);

            #endregion Assert
        }

        [TestMethod]
        public void TestEditRedirectsToHomeIndexIfIdDifferent()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(
                Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything)).Return(false).Repeat.Any();

            var fakeEmail = new List<EmailQueue>();
            for (int i = 0; i < 3; i++)
            {
                fakeEmail.Add(CreateValidEntities.EmailQueue(i + 1));
                fakeEmail[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);

            #endregion Arrange

            #region Act
            Controller.Edit(1, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(3, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(1, args2[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditReturnsViewWithExpectedResults()
        {
            #region Arrange
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(a => a.HasSameId(
                Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything)).Return(true).Repeat.Any();

            var fakeEmail = new List<EmailQueue>();
            for (int i = 0; i < 3; i++)
            {
                fakeEmail.Add(CreateValidEntities.EmailQueue(i + 1));
                fakeEmail[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            fakeEmail[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            var fakeEmails = new FakeEmailQueues();
            fakeEmails.Records(0, EmailQueueRepository, fakeEmail);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 3)
                .AssertViewRendered()
                .WithViewData<EmailQueueViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.IsNull(args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything,
                Arg<CallForProposal>.Is.Anything,
                Arg<int?>.Is.Anything,
                Arg<int?>.Is.Anything));
            var args2 = AccessService.GetArgumentsForCallsMadeOn(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, args2[0]);
            Assert.AreEqual(3, ((CallForProposal)args2[1]).Id);
            Assert.AreEqual(null, args2[2]);
            Assert.AreEqual(3, args2[3]);

            Assert.IsNotNull(result);
            Assert.AreEqual("Subject2", result.EmailQueue.Subject);
            Assert.AreEqual(3, result.CallForProposal.Id);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        
        #endregion Edit Post Tests
        #endregion Edit Tests
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
        public void TestControllerMethodDetailsContainsExpectedAttributes()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethod("Details");
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
        public void TestControllerMethodCreateContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Edit");
            #endregion Arrange

            #region Act
            //var expectedAttribute = controllerMethod.ElementAt(0).GetCustomAttributes(true).OfType<UserOnlyAttribute>();
            var allAttributes = controllerMethod.ElementAt(0).GetCustomAttributes(true);
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
    }
}
