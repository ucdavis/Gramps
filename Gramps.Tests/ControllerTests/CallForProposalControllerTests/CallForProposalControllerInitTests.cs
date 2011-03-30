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
using Gramps.Core.Helpers;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace Gramps.Tests.ControllerTests.CallForProposalControllerTests
{
    [TestClass]
    public partial class CallForProposalControllerTests : ControllerTestBase<CallForProposalController>
    {
        private readonly Type _controllerClass = typeof(CallForProposalController);
        public IRepository<CallForProposal> CallForProposalRepository;
        public IAccessService AccessService;

        public IRepository<Editor> EditorRepository;
        public IRepository<User> UserRepository;
        public IRepository<Template> TemplateRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            AccessService = MockRepository.GenerateStub<IAccessService>();   
            Controller = new TestControllerBuilder().CreateController<CallForProposalController>(CallForProposalRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public CallForProposalControllerTests()
        {
            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            TemplateRepository = FakeRepository<Template>();
            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();

            //CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();
        }
        #endregion Init

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
            Assert.AreEqual(3, result.Count(), "It looks like a method was added or removed from the controller.");
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

        [TestMethod]
        public void TestControllerMethodCreateContainsExpectedAttributes1()
        {
            #region Arrange
            var controllerClass = _controllerClass;
            var controllerMethod = controllerClass.GetMethods().Where(a => a.Name == "Create");
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
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var editors = new List<Editor>();
            for (int i = 0; i < 7; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].CallForProposal = CallForProposalRepository.GetNullableById(i + 1);
                editors[i].User = users[0];
            }
            editors[2].User = users[1];

            editors[5].CallForProposal = CallForProposalRepository.GetNullableById(4);
            editors[5].User = users[1];
            editors[6].CallForProposal = CallForProposalRepository.GetNullableById(5);
            editors[6].User = users[1];

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
        }
        
        /// <summary>
        /// Setup data to return a list of templates that the user has access to.
        /// </summary>
        private void SetupDataForTests2()
        {
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users[0].LoginId = "Me";

            users.Add(CreateValidEntities.User(2));
            users[1].LoginId = "NotMe";

            var editors = new List<Editor>();
            for (int i = 0; i < 10; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].User = i < 6 ? users[0] : users[1];
                editors[i].Template = CreateValidEntities.Template(i + 1);
                editors[i].Template.SetIdTo(i + 1);
            }
            editors[0].Template = null;
            editors[1].Template.IsActive = false;
            editors[2].User = null;
            editors[4].Template = editors[3].Template;

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
        }

        private void SetupDataForTests3()
        {
            var users = new List<User>();
            users.Add(CreateValidEntities.User(1));
            users.Add(CreateValidEntities.User(1));
            users[0].LoginId = "Me";
            users[1].LoginId = "NotMe";
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(0, UserRepository, users);
            

            var templates = new List<Template>();
            templates.Add(CreateValidEntities.Template(1));
            templates.Add(CreateValidEntities.Template(2));
            templates.Add(CreateValidEntities.Template(3));

            templates[0].Name = null; //Invalid

            #region Template 3 Populate Values
            templates[2].Editors.Add(CreateValidEntities.Editor(1));
            templates[2].Editors.Add(CreateValidEntities.Editor(2));
            templates[2].Editors.Add(CreateValidEntities.Editor(3));
            templates[2].Editors.Add(CreateValidEntities.Editor(4));
            templates[2].Editors[3].User = users[0];
            templates[2].Editors[2].User = users[1];

            templates[2].Emails.Add(CreateValidEntities.EmailsForCall(1));
            templates[2].Emails.Add(CreateValidEntities.EmailsForCall(2));
            templates[2].Emails.Add(CreateValidEntities.EmailsForCall(3));

            templates[2].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            templates[2].EmailTemplates.Add(CreateValidEntities.EmailTemplate(2));
            templates[2].EmailTemplates[0].TemplateType = EmailTemplateType.InitialCall;
            templates[2].EmailTemplates[1].TemplateType = EmailTemplateType.ProposalDenied;

            templates[2].Questions.Add(CreateValidEntities.Question(1));
            templates[2].Questions.Add(CreateValidEntities.Question(2));
            templates[2].Questions.Add(CreateValidEntities.Question(3));

            templates[2].Reports.Add(CreateValidEntities.Report(1));
            templates[2].Reports.Add(CreateValidEntities.Report(2));
            templates[2].Reports.Add(CreateValidEntities.Report(3));
            #endregion Template 3 Populate Values


            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(0, TemplateRepository, templates);
        }

        #endregion Helper Methods
    }
}
