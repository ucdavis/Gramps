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

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            ReportRepository = FakeRepository<Report>();
            AccessService = MockRepository.GenerateStub<IAccessService>();

            Controller = new TestControllerBuilder().CreateController<ReportController>(ReportRepository, AccessService);
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

            Controller.Repository.Expect(a => a.OfType<Report>()).Return(ReportRepository).Repeat.Any();	
        }
        #endregion Init



        #region Helper Methods
        public void SetupData1()
        {
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(3, TemplateRepository);

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository);

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
        #endregion Helper Methods

       
    }
}
