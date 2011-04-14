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


namespace Gramps.Tests.ControllerTests.InvestigatorControllerTests
{
    [TestClass]
    public partial class InvestigatorControllerTests : ControllerTestBase<InvestigatorController>
    {
        private readonly Type _controllerClass = typeof(InvestigatorController);
        public IRepository<Investigator> InvestigatorRepository;
        public IRepository<Proposal> ProposalRepository;
        public IRepository<CallForProposal> CallForProposalRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            InvestigatorRepository = FakeRepository<Investigator>();
            ProposalRepository = FakeRepository<Proposal>();

            Controller = new TestControllerBuilder().CreateController<InvestigatorController>(InvestigatorRepository, ProposalRepository);            
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public InvestigatorControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Proposal>()).Return(ProposalRepository).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<Investigator>()).Return(InvestigatorRepository).Repeat.Any();	
        }
        #endregion Init

        #region Helpers
        private void SetupTestData1()
        {
            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));
            calls.Add(CreateValidEntities.CallForProposal(2));
            calls.Add(CreateValidEntities.CallForProposal(3));
            calls[0].IsActive = false;
            calls[1].EndDate = DateTime.Now.AddDays(-1);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var proposals = new List<Proposal>();
            for (int i = 0; i < 5; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i+1));
                proposals[i].Guid = SpecificGuid.GetGuid(i + 1);
                proposals[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }

            proposals[0].CallForProposal = CallForProposalRepository.GetNullableById(1);
            proposals[1].CallForProposal = CallForProposalRepository.GetNullableById(2);
            proposals[2].IsSubmitted = true;

            var fakeProposals = new FakeProposals();
            fakeProposals.Records(0, ProposalRepository, proposals);
        }
        #endregion Helpers
    }
}
