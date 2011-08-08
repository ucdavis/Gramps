using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Models;
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
        public IRepository<Investigator> InvestigatorRepository;
        public IRepository<Comment> CommentRepository;
        public IAccessService AccessService;
        public IEmailService EmailService;
        public IMembershipService MembershipService;
 
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
            MembershipService = MockRepository.GenerateStub<IMembershipService>();

            Controller = new TestControllerBuilder().CreateController<ProposalController>(ProposalRepository, AccessService, EmailService, MembershipService);
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

            InvestigatorRepository = FakeRepository<Investigator>();
            Controller.Repository.Expect(a => a.OfType<Investigator>()).Return(InvestigatorRepository).Repeat.Any();

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
            calls[3].EmailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            calls[3].EmailTemplates[0].TemplateType = EmailTemplateType.ProposalConfirmation;
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
        }
        public void SetupData4()
        {
            var proposals = new List<Proposal>();
            for (int i = 0; i < 4; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i + 1));
                proposals[i].CallForProposal = CallForProposalRepository.GetNullableById(4);
            }
            proposals[1].CallForProposal = CallForProposalRepository.GetNullableById(2);
            proposals[1].Sequence = 1;
            proposals[0].Sequence = 1;
            proposals[2].Sequence = 2;
            proposals[3].Sequence = 3;
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(0, ProposalRepository, proposals);
        }


        public void SetupData5()
        {
            var calls = new List<CallForProposal>();
            for (int i = 0; i < 7; i++)
            {
                calls.Add(CreateValidEntities.CallForProposal(i+1));
                calls[i].IsActive = true;
            }
            calls[2].IsActive = false;
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);

            var editors = new List<Editor>();
            for (int i = 0; i < 7; i++)
            {
                editors.Add(CreateValidEntities.Editor(i+1));
                editors[i].User = null;
                editors[i].ReviewerEmail = "is@reviewer.com";
                editors[i].CallForProposal = CallForProposalRepository.GetNullableById(i + 1);
            }

            editors[1].User = CreateValidEntities.User(1);
            editors[3].ReviewerEmail = "no@one.com";
            editors[6].ReviewerEmail = "also@reviewer.com";

            var fakeEditors = new FakeEditors();
            fakeEditors.Records(0, EditorRepository, editors);

            var proposals = new List<Proposal>();
            for (int i = 0; i < 7; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i + 1));
                proposals[i].Email = "email2@testy.com";
                proposals[i].CreatedDate = DateTime.Now.AddDays(i);
            }
            proposals[1].Email = "notme@test.com";
            proposals[5].Email = "also@reviewer.com";
            proposals[6].Email = "also@reviewer.com";
            var fakeProposals = new FakeProposals();
            fakeProposals.Records(0, ProposalRepository, proposals);
        }

        public void SetupData6()
        {
            var editor = CreateValidEntities.Editor(1);
            editor.IsOwner = true;
            editor.User = CreateValidEntities.User(1);
            var calls = new List<CallForProposal>();
            for (int i = 0; i < 4; i++)
            {
                calls.Add(CreateValidEntities.CallForProposal(i+1));
                calls[i].AddEditor(editor);
                calls[i].IsActive = true;
                calls[i].EndDate = DateTime.Now;
            }
            calls[0].EndDate = DateTime.Now.AddDays(-1);
            calls[1].IsActive = false;
            calls[3].ProposalMaximum = 0;

            var proposals = new List<Proposal>();
            for (int i = 0; i < 6; i++)
            {
                proposals.Add(CreateValidEntities.Proposal(i + 1));
                proposals[i].Guid = SpecificGuid.GetGuid(i + 1);
            }
            proposals[1].IsSubmitted = true;
            proposals[2].CallForProposal = calls[0];
            proposals[3].CallForProposal = calls[1];

            proposals[4].CallForProposal = calls[2];
            proposals[5].CallForProposal = calls[3];


            var fakeProposals = new FakeProposals();
            fakeProposals.Records(0, ProposalRepository, proposals);
        }

        public void SetupData7()
        {
            var investigators = new List<Investigator>();
            for (int i = 0; i < 4; i++)
            {
                investigators.Add(CreateValidEntities.Investigator(i + 1));
                investigators[i].Proposal = ProposalRepository.GetNullableById(5);
            }
            investigators[0].IsPrimary = true;
            investigators[3].IsPrimary = true;
            investigators[3].Proposal = ProposalRepository.GetNullableById(6);

            var fakeInvestigators = new FakeInvestigators();
            fakeInvestigators.Records(0, InvestigatorRepository, investigators);
        }
        #endregion Helpers
    }
}
