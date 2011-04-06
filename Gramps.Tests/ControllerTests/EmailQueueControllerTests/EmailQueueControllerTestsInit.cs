using System;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;



namespace Gramps.Tests.ControllerTests.EmailQueueControllerTests
{
    [TestClass]
    public partial class EmailQueueControllerTests : ControllerTestBase<EmailQueueController>
    {
        private readonly Type _controllerClass = typeof(EmailQueueController);
        public IRepository<EmailQueue> EmailQueueRepository;
        public IAccessService AccessService;
        public IRepository<CallForProposal> CallForProposalRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            EmailQueueRepository = FakeRepository<EmailQueue>();
            AccessService = MockRepository.GenerateStub<IAccessService>();  
            Controller = new TestControllerBuilder().CreateController<EmailQueueController>(EmailQueueRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public EmailQueueControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<EmailQueue>()).Return(EmailQueueRepository).Repeat.Any();
        }
        #endregion Init

    }
}
