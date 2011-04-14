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


namespace Gramps.Tests.ControllerTests.EmailTemplateControllerTests
{
    [TestClass]
    public partial class EmailTemplateControllerTests : ControllerTestBase<EmailTemplateController>
    {
        private readonly Type _controllerClass = typeof(EmailTemplateController);
        public IRepository<EmailTemplate> EmailTemplateRepository;
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<Template> TemplateRepository;
        public IAccessService AccessService;
 

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            EmailTemplateRepository = FakeRepository<EmailTemplate>();
            AccessService = MockRepository.GenerateStub<IAccessService>();  

            Controller = new TestControllerBuilder().CreateController<EmailTemplateController>(EmailTemplateRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public EmailTemplateControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            TemplateRepository = FakeRepository<Template>();
            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<EmailTemplate>()).Return(EmailTemplateRepository).Repeat.Any();
        }
        #endregion Init  

        #region Helpers
        public void SetupTestData1()
        {
            //var callForProposals = new List<CallForProposal>();
            //for (int i = 0; i < 3; i++)
            //{
            //    callForProposals.Add(CreateValidEntities.CallForProposal(i+1));
            //}
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);

            //var templates = new List<Template>();
            //for (int i = 0; i < 2; i++)
            //{
            //    templates.Add(CreateValidEntities.Template(i+1));
            //}
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(2, TemplateRepository);

            var emailTemplates = new List<EmailTemplate>();
            for (int i = 0; i < 21; i++)
            {
                emailTemplates.Add(CreateValidEntities.EmailTemplate(i+1));
                switch (i % 7)
                {
                    case 0:
                        emailTemplates[i].TemplateType = EmailTemplateType.InitialCall;
                        break;
                    case 1:
                        emailTemplates[i].TemplateType = EmailTemplateType.ProposalApproved;
                        break;
                    case 2:
                        emailTemplates[i].TemplateType = EmailTemplateType.ProposalConfirmation;
                        break;
                    case 3:
                        emailTemplates[i].TemplateType = EmailTemplateType.ProposalDenied;
                        break;
                    case 4:
                        emailTemplates[i].TemplateType = EmailTemplateType.ProposalUnsubmitted;
                        break;
                    case 5:
                        emailTemplates[i].TemplateType = EmailTemplateType.ReadyForReview;
                        break;
                    case 6:
                        emailTemplates[i].TemplateType = EmailTemplateType.ReminderCallIsAboutToClose;
                        break;
                    default:
                        throw new ApplicationException("Oops, programing error");
                }
                if (i > 13)
                {
                    emailTemplates[i].Template = TemplateRepository.GetNullableById(1);
                    emailTemplates[i].CallForProposal = null;
                }
                else if (i > 6)
                {
                    emailTemplates[i].Template = null;
                    emailTemplates[i].CallForProposal = CallForProposalRepository.GetNullableById(1);
                }
                else
                {
                    emailTemplates[i].Template = TemplateRepository.GetNullableById(2);
                    emailTemplates[i].CallForProposal = null;
                }
            }

            var fakeEmailTemplates = new FakeEmailTemplates();
            fakeEmailTemplates.Records(0, EmailTemplateRepository, emailTemplates);

        }
        #endregion Helpers   
    }
}
