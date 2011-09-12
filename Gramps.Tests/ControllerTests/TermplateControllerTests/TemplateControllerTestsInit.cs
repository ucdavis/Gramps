using System;
using System.Collections.Generic;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace Gramps.Tests.ControllerTests.TermplateControllerTests
{
    [TestClass]
    public partial class TemplateControllerTests : ControllerTestBase<TemplateController>
    {
        private readonly Type _controllerClass = typeof(TemplateController);
        public IRepository<Template> TemplateRepository;
        public IAccessService AccessService;
        public IRepository<Editor> EditorRepository;
        public IRepository<User> UserRepository;
        public IRepository<EmailTemplate> EmailTemplateRepository; 

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            TemplateRepository = FakeRepository<Template>();
            AccessService = MockRepository.GenerateStub<IAccessService>();

            Controller = new TestControllerBuilder().CreateController<TemplateController>(TemplateRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            //new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public TemplateControllerTests()
        {
            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            EmailTemplateRepository = FakeRepository<EmailTemplate>();
            Controller.Repository.Expect(a => a.OfType<EmailTemplate>()).Return(EmailTemplateRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();	
        }
        #endregion Init

        #region Helpers
        public void SetupData()
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

            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors.Add(CreateValidEntities.Editor(2));
            editors.Add(CreateValidEntities.Editor(3));
            editors.Add(CreateValidEntities.Editor(4));
            editors[3].User = users[0];
            editors[2].User = users[1];
            foreach (var editor in editors)
            {
                editor.Template = new Template();
            }
            editors[0].Template.SetIdTo(2);
            editors[1].Template.SetIdTo(2);
            editors[2].Template.SetIdTo(2);
            editors[3].Template.SetIdTo(2);

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);

            #region Template 3 Populate Values
            templates[2].Editors.Add(EditorRepository.GetNullableById(1));
            templates[2].Editors.Add(EditorRepository.GetNullableById(2));
            templates[2].Editors.Add(EditorRepository.GetNullableById(3));
            templates[2].Editors.Add(EditorRepository.GetNullableById(4));

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
        #endregion Helpers
    }
}
