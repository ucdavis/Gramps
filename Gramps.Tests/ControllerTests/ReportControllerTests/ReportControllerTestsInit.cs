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


namespace Gramps.Tests.ControllerTests.ReportControllerTests
{
    [TestClass]
    public partial class ReportControllerTests : ControllerTestBase<ReportController>
    {
        protected readonly Type ControllerClass = typeof(ReportController);
        public IRepository<Report> ReportRepository;
        public IAccessService AccessService;
        public IRepository<Template> TemplateRepository;
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<Question> QuestionRepository;
        public IReportService ReportService;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            ReportRepository = FakeRepository<Report>();
            AccessService = MockRepository.GenerateStub<IAccessService>();
            ReportService = MockRepository.GenerateStub<IReportService>();

            Controller = new TestControllerBuilder().CreateController<ReportController>(ReportRepository, AccessService, ReportService);
        }

        protected override void RegisterRoutes()
        {
            //new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public ReportControllerTests()
        {
            TemplateRepository = FakeRepository<Template>();
            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();

            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            QuestionRepository = FakeRepository<Question>();
            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Report>()).Return(ReportRepository).Repeat.Any();	
        }
        #endregion Init



        #region Helper Methods
        public void SetupData1()
        {
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(3, TemplateRepository);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);

            var reports = new List<Report>();
            for (int i = 0; i < 5; i++)
            {
                reports.Add(CreateValidEntities.Report(i+1));
                reports[i].Template = TemplateRepository.GetNullableById(2);
                reports[i].CallForProposal = null;
            }

            reports[1].Template = TemplateRepository.GetNullableById(1);
            reports[2].Template = null;
            reports[2].CallForProposal = CallForProposalRepository.GetNullableById(2);

            var fakeReports = new FakeReports();
            fakeReports.Records(0, ReportRepository, reports);
        }

        public void SetupData2()
        {
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(3, TemplateRepository);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);

            var reports = new List<Report>();
            for (int i = 0; i < 5; i++)
            {
                reports.Add(CreateValidEntities.Report(i + 1));
                reports[i].Template = null;
                reports[i].CallForProposal = CallForProposalRepository.GetNullableById(2);
            }

            reports[1].CallForProposal = CallForProposalRepository.GetNullableById(1);
            reports[2].CallForProposal = null;
            reports[2].Template = TemplateRepository.GetNullableById(2);

            var fakeReports = new FakeReports();
            fakeReports.Records(0, ReportRepository, reports);
        }

        public void SetupData3()
        {
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(3, TemplateRepository);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);

            var questions = new List<Question>();
            for (int i = 0; i < 10; i++)
            {
                questions.Add(CreateValidEntities.Question(i+1));
                if (i < 5)
                {
                    questions[i].CallForProposal = null;
                    questions[i].Template = TemplateRepository.GetNullableById(2);
                }
                else
                {
                    questions[i].CallForProposal = CallForProposalRepository.GetNullableById(2);
                    questions[i].Template = null;
                }
                questions[i].QuestionType = CreateValidEntities.QuestionType(i + 1);

            }
            questions[1].CallForProposal = CallForProposalRepository.GetNullableById(1);
            questions[2].QuestionType.Name = "No Answer";

            questions[7].Template = TemplateRepository.GetNullableById(1);
            questions[8].QuestionType.Name = "No Answer";

            var fakeQuestions = new FakeQuestions();
            fakeQuestions.Records(0, QuestionRepository, questions);

            var reports = new List<Report>();
            for (int i = 0; i < 6; i++)
            {
                reports.Add(CreateValidEntities.Report(i + 1));
                if (i < 3)
                {
                    reports[i].Template = TemplateRepository.GetNullableById(2);
                    reports[i].CallForProposal = null;
                }
                else
                {
                    reports[i].Template = null;
                    reports[i].CallForProposal = CallForProposalRepository.GetNullableById(2);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                reports[1].ReportColumns.Add(CreateValidEntities.ReportColumn(i+1));
            }

            var fakeReports = new FakeReports();
            fakeReports.Records(0, ReportRepository, reports);
        }

        #endregion Helper Methods

       
    }
}
