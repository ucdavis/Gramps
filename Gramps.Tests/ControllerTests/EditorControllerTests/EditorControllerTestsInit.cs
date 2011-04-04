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


namespace Gramps.Tests.ControllerTests.EditorControllerTests
{
    [TestClass]
    public partial class EditorControllerTests : ControllerTestBase<EditorController>
    {
        private readonly Type _controllerClass = typeof(EditorController);
        public IRepository<Editor> EditorRepository;
        public IAccessService AccessService;
        public IEmailService EmailService;
        public IRepository<Template> TemplateRepository;
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<User> UserRepository;

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

            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();
        }
        #endregion Init

        #region Helpers
        private void SetupDataForTests()
        {
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(3, UserRepository);

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

        private void SetupDataForTests2()
        {
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(5, TemplateRepository);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(5, CallForProposalRepository);

            var editors = new List<Editor>();
            for (int i = 0; i < 4; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].Template = null;
                editors[i].CallForProposal = CallForProposalRepository.GetNullableById(5);
            }
            editors[2].CallForProposal = CallForProposalRepository.GetNullableById(1);
            editors[2].User = UserRepository.GetNullableById(2);
            editors[1].User = UserRepository.GetNullableById(1);

            for (int i = 4; i < 8; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].Template = TemplateRepository.GetNullableById(3);
                editors[i].CallForProposal = null;
            }
            editors[5].Template = TemplateRepository.GetNullableById(1);
            editors[5].User = UserRepository.GetNullableById(2);
            editors[7].Template = TemplateRepository.GetNullableById(2);
            editors[6].User = UserRepository.GetNullableById(1);

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
        }
        #endregion Helpers
    }
}
