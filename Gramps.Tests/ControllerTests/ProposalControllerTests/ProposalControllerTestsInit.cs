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


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    [TestClass]
    public partial class ProposalControllerTests : ControllerTestBase<ProposalController>
    {
        private readonly Type _controllerClass = typeof(ProposalController);
        public IRepository<Proposal> ProposalRepository;
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<Editor> EditorRepository;
        public IRepository<ReviewedProposal> ReviewedProposalRepository;
        public IRepository<Comment> CommentRepository;
        public IAccessService AccessService;
        public IEmailService EmailService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            ProposalRepository = FakeRepository<Proposal>();
            AccessService = MockRepository.GenerateStub<IAccessService>();
            EmailService = MockRepository.GenerateStub<IEmailService>();  

            Controller = new TestControllerBuilder().CreateController<ProposalController>(ProposalRepository, AccessService, EmailService);
        }

        protected override void RegisterRoutes()
        {
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public ProposalControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            EditorRepository = FakeRepository<Editor>();
            Controller.Repository.Expect(a => a.OfType<Editor>()).Return(EditorRepository).Repeat.Any();

            ReviewedProposalRepository = FakeRepository<ReviewedProposal>();
            Controller.Repository.Expect(a => a.OfType<ReviewedProposal>()).Return(ReviewedProposalRepository).Repeat.Any();

            CommentRepository = FakeRepository<Comment>();
            Controller.Repository.Expect(a => a.OfType<Comment>()).Return(CommentRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Proposal>()).Return(ProposalRepository).Repeat.Any();	
        }
        #endregion Init

        #region Helpers
        public void SetupDataForTests1()
        {
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);

            var users = new List<User>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i+1));
            }

            var fakeEditors = new List<Editor>();
            for (int i = 0; i < 3; i++)
            {
                fakeEditors.Add(CreateValidEntities.Editor(i+1));
                fakeEditors[i].User = users[i];
                fakeEditors[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            var fakeEditor = new FakeEditors();
            fakeEditor.Records(0, EditorRepository, fakeEditors);

            var proposals = new List<Proposal>();
            for (int i = 0; i < 20; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i+1));
                proposals[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
            }
            proposals[0].ReviewedByEditors.Add(new ReviewedProposal(proposals[0], EditorRepository.GetNullableById(1)));
            proposals[0].ReviewedByEditors[0].LastViewedDate = new DateTime(2011,01,20);

            proposals[1].IsApproved = true;
            proposals[1].IsNotified = true;
            proposals[2].IsDenied = true;
            proposals[3].IsSubmitted = true;
            proposals[4].IsSubmitted = true;
            proposals[4].IsApproved = true;
            proposals[5].IsSubmitted = true;
            proposals[5].IsDenied = true;

            proposals[6].IsNotified = true;
            proposals[7].WasWarned = true;


            var fakeProposals = new FakeProposals();
            fakeProposals.Records(3, ProposalRepository, proposals);

        }
        public void SetupDataForTests2()
        {

            var editors = new List<Editor>();
            for (int i = 0; i < 3; i++)
            {
                editors.Add(CreateValidEntities.Editor(i + 1));
            }

            var calls = new List<CallForProposal>();
            calls.Add(CreateValidEntities.CallForProposal(1));            
            calls[0].Editors = editors;
            calls.Add(CreateValidEntities.CallForProposal(2));
            calls[1].Editors = editors;
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(1, CallForProposalRepository, calls);
            
            foreach (var editor in editors)
            {
                editor.CallForProposal = CallForProposalRepository.GetNullableById(1);
            }
            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);

            var proposals = new List<Proposal>();
            for (int i = 0; i < 10; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i + 1));
                proposals[i].CallForProposal = CallForProposalRepository.GetNullableById(1);
                proposals[i].Sequence = i + 1;
            }

            proposals[9].CallForProposal = CallForProposalRepository.GetNullableById(2);

            var reviewedProposals = new List<ReviewedProposal>();
            reviewedProposals.Add(new ReviewedProposal(proposals[3], EditorRepository.GetNullableById(2)));
            reviewedProposals[0].LastViewedDate = new DateTime(2011, 01, 20);
            reviewedProposals[0].Proposal.SetIdTo(4);
            var fakeReviewedProposals = new FakeReviewedProposals();
            fakeReviewedProposals.Records(3, ReviewedProposalRepository, reviewedProposals);

            proposals[3].ReviewedByEditors.Add(ReviewedProposalRepository.GetNullableById(1));
        

            proposals[1].IsApproved = true;
            proposals[1].IsNotified = true;
            proposals[2].IsDenied = true;
            proposals[3].IsSubmitted = true;
            proposals[3].SubmittedDate = new DateTime(2011,02,3);
            proposals[4].IsSubmitted = true;
            proposals[4].SubmittedDate = new DateTime(2011, 02, 4);
            proposals[4].IsApproved = true;
            proposals[4].File = CreateValidEntities.File(1);
            proposals[4].File.Contents = null;
            proposals[5].IsSubmitted = true;
            proposals[5].SubmittedDate = new DateTime(2011, 02, 5);
            proposals[5].IsDenied = true;
            proposals[5].IsNotified = true;
            proposals[5].WasWarned = true;
            proposals[5].File = CreateValidEntities.File(44);
            proposals[5].File.Contents = new byte[]{2,3,6};
            proposals[5].File.ContentType = "FakeType";

            var fakeProposals = new FakeProposals();
            fakeProposals.Records(0, ProposalRepository, proposals);
        }

        /// <summary>
        /// 4 calls
        /// </summary>
        public void SetupData3()
        {
            var users = new List<User>();
            var editors = new List<Editor>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
                editors.Add(CreateValidEntities.Editor(i + 1));
                editors[i].User = users[i];
            }
            editors[0].IsOwner = true;
            var calls = new List<CallForProposal>();
            for (int i = 0; i < 4; i++)
            {
                calls.Add(CreateValidEntities.CallForProposal(i + 1));
                calls[i].Editors = editors;
                calls[i].IsActive = true;
                calls[i].EndDate = DateTime.Now.AddDays(5);
            }
            calls[0].IsActive = false;
            calls[1].EndDate = DateTime.Now.AddDays(-1);
            calls[2].EndDate = DateTime.Now.Date;
            calls[3].EndDate = DateTime.Now.AddDays(1);
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
        }
        #endregion Helpers
    }
}
