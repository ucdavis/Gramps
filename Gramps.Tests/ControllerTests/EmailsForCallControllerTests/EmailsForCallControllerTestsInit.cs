using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    [TestClass]
    public partial class EmailsForCallControllerTests : ControllerTestBase<EmailsForCallController>
    {
        private readonly Type _controllerClass = typeof(EmailsForCallController);
        public IRepository<EmailsForCall> EmailsForCallRepository;
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<Template> TemplateRepository;
        public IAccessService AccessService;
        public IEmailService EmailService;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            EmailsForCallRepository = FakeRepository<EmailsForCall>();
            AccessService = MockRepository.GenerateStub<IAccessService>();
            EmailService = MockRepository.GenerateStub<IEmailService>();  
            Controller = new TestControllerBuilder().CreateController<EmailsForCallController>(EmailsForCallRepository, AccessService, EmailService);            
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public EmailsForCallControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            TemplateRepository = FakeRepository<Template>();
            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<EmailsForCall>()).Return(EmailsForCallRepository).Repeat.Any();
        }
        #endregion Init

        #region Helpers
        private void SetupDataForTests1()
        {
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(4, TemplateRepository);

            var emailsForCall = new List<EmailsForCall>();
            for (var i = 0; i < 10; i++)
            {
                emailsForCall.Add(CreateValidEntities.EmailsForCall(i+1));
                if (i < 5)
                {
                    emailsForCall[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
                    emailsForCall[i].Template = null;
                }
                else
                {
                    emailsForCall[i].Template = TemplateRepository.GetNullableById(4);
                    emailsForCall[i].CallForProposal = null;
                }
            }
            emailsForCall[0].CallForProposal = null;
            emailsForCall[1].CallForProposal = CallForProposalRepository.GetNullableById(2);
            emailsForCall[5].Template = null;
            emailsForCall[6].Template = TemplateRepository.GetNullableById(1);
            
            var fakeEmailsForCall = new FakeEmailsForCall();
            fakeEmailsForCall.Records(0, EmailsForCallRepository, emailsForCall);

        }
        #endregion Helpers
    }
}
