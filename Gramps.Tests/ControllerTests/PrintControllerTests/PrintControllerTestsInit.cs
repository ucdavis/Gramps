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


namespace Gramps.Tests.ControllerTests.PrintControllerTests
{
    [TestClass]
    public partial class PrintControllerTests : ControllerTestBase<PrintController>
    {
        private readonly Type _controllerClass = typeof(PrintController);
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<Proposal> ProposalRepository;
        public IAccessService AccessService;
        public IPrintService PrintService;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            ProposalRepository = FakeRepository<Proposal>();
            AccessService = MockRepository.GenerateStub<IAccessService>();
            PrintService = MockRepository.GenerateStub<IPrintService>();

            Controller = new TestControllerBuilder().CreateController<PrintController>(CallForProposalRepository, ProposalRepository, AccessService, PrintService);
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public PrintControllerTests()
        {
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<Proposal>()).Return(ProposalRepository).Repeat.Any();
        }
        #endregion Init

        #region Helpers
        public void SetupDataForTests1()
        {
            var fakeCallsForProposal = new List<CallForProposal>();
            fakeCallsForProposal.Add(CreateValidEntities.CallForProposal(1));
            fakeCallsForProposal.Add(CreateValidEntities.CallForProposal(2));
            fakeCallsForProposal.Add(CreateValidEntities.CallForProposal(3));
   

            var fakeProposals = new List<Proposal>();
            for (int i = 0; i < 5; i++)
            {
                fakeProposals.Add(CreateValidEntities.Proposal(i+1));
                fakeProposals[i].CallForProposal = fakeCallsForProposal[0];
            }

            fakeProposals[4].CallForProposal = fakeCallsForProposal[1];

            var fakeProps = new FakeProposals();
            fakeProps.Records(0, ProposalRepository, fakeProposals);
            for (int i = 0; i < 4; i++)
            {
                fakeCallsForProposal[0].Proposals.Add(fakeProposals[i]);
            }
            fakeCallsForProposal[1].Proposals.Add(fakeProposals[4]);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, fakeCallsForProposal);
        }

        public void SetupDataForTests2()
        {
            var editors = new List<Editor>();
            for (int i = 0; i < 4; i++)
            {
                editors.Add(CreateValidEntities.Editor(i+1));
            }

            var fakeCallsForProposal = new List<CallForProposal>();
            fakeCallsForProposal.Add(CreateValidEntities.CallForProposal(1));
            fakeCallsForProposal.Add(CreateValidEntities.CallForProposal(2));
            fakeCallsForProposal.Add(CreateValidEntities.CallForProposal(3));

            fakeCallsForProposal[0].Editors.Add(editors[0]);
            fakeCallsForProposal[0].Editors.Add(editors[3]);
            fakeCallsForProposal[1].Editors.Add(editors[1]);
            fakeCallsForProposal[2].Editors.Add(editors[2]);


            var fakeProposals = new List<Proposal>();
            for (int i = 0; i < 5; i++)
            {
                fakeProposals.Add(CreateValidEntities.Proposal(i + 1));
                fakeProposals[i].CallForProposal = fakeCallsForProposal[0];
            }

            fakeProposals[4].CallForProposal = fakeCallsForProposal[1];

            var fakeProps = new FakeProposals();
            fakeProps.Records(0, ProposalRepository, fakeProposals);
            for (int i = 0; i < 4; i++)
            {
                fakeCallsForProposal[0].Proposals.Add(fakeProposals[i]);
            }
            fakeCallsForProposal[1].Proposals.Add(fakeProposals[4]);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, fakeCallsForProposal);
        }
        #endregion Helpers
    }
}
