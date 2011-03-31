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
