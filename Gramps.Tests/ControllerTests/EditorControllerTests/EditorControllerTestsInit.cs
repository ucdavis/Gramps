using System;
using System.Collections.Generic;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Models;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;


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
        public IMembershipService MembershipService;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            EditorRepository = FakeRepository<Editor>();
            AccessService = MockRepository.GenerateStub<IAccessService>();
            EmailService = MockRepository.GenerateStub<IEmailService>();
            MembershipService = MockRepository.GenerateStub<IMembershipService>();
            Controller = new TestControllerBuilder().CreateController<EditorController>(EditorRepository, AccessService, EmailService, MembershipService);  
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

        private void SetupDataForTests3()
        {
            var fakeUsers = new FakeUsers();
            fakeUsers.Records(4, UserRepository);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);

            var editors = new List<Editor>();
            for (int i = 0; i < 10; i++)
            {
                editors.Add(CreateValidEntities.Editor(i+1));
                editors[i].CallForProposal = CallForProposalRepository.GetNullableById(i % 2 == 0 ? 1 : 2);
                editors[i].Template = null;
            }
            editors[0].User = CreateValidEntities.User(1);
            editors[1].User = CreateValidEntities.User(2);
            editors[2].CallForProposal = null;
            editors[4].CallForProposal = CallForProposalRepository.GetNullableById(3);

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
        }

        private void SetupDataForTests4()
        {
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls.Add(CreateValidEntities.CallForProposal(2));
            calls[0].IsActive = false;
            calls[1].IsActive = true;
            calls[1].EmailTemplates = new List<EmailTemplate>();
            calls[1].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            calls[1].EmailTemplates[0].TemplateType = EmailTemplateType.ReadyForReview;

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var editors = new List<Editor>();
            for (int i = 0; i < 10; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
                if (i < 5)
                {
                    editors[i].CallForProposal = CallForProposalRepository.GetNullableById(1);
                }
                else
                {
                    editors[i].CallForProposal = CallForProposalRepository.GetNullableById(2);
                }
                editors[i].Template = null;
            }
            editors[0].User = CreateValidEntities.User(1);
            editors[5].User = CreateValidEntities.User(2);
            editors[6].HasBeenNotified = true;

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);
        }

        #endregion Helpers
    }
}
