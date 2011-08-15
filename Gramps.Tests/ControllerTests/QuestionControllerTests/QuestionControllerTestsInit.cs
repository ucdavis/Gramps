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


namespace Gramps.Tests.ControllerTests.QuestionControllerTests
{
    [TestClass]
    public partial class QuestionControllerTests : ControllerTestBase<QuestionController>
    {
        protected readonly Type ControllerClass = typeof(QuestionController);
        public IRepository<Question> QuestionRepository;
        public IAccessService AccessService;
        public IRepository<CallForProposal> CallForProposalRepository;
        public IRepository<Template> TemplateRepository;
        public IRepository<QuestionType> QuestionTypeRepository;
        public IRepository<Validator> ValidatorRepository;
        public IRepository<QuestionAnswer> QuestionAnswerRepository; 


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            QuestionRepository = FakeRepository<Question>();
            AccessService = MockRepository.GenerateStub<IAccessService>();  
            Controller = new TestControllerBuilder().CreateController<QuestionController>(QuestionRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            //new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public QuestionControllerTests()
        {
            CallForProposalRepository = FakeRepository<CallForProposal>();
            Controller.Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository).Repeat.Any();

            TemplateRepository = FakeRepository<Template>();
            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();

            QuestionTypeRepository = FakeRepository<QuestionType>();
            Controller.Repository.Expect(a => a.OfType<QuestionType>()).Return(QuestionTypeRepository).Repeat.Any();

            ValidatorRepository = FakeRepository<Validator>();
            Controller.Repository.Expect(a => a.OfType<Validator>()).Return(ValidatorRepository).Repeat.Any();

            QuestionAnswerRepository = FakeRepository<QuestionAnswer>();
            Controller.Repository.Expect(a => a.OfType<QuestionAnswer>()).Return(QuestionAnswerRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Question>()).Return(QuestionRepository).Repeat.Any();	
        }
        #endregion Init

        
        public void SetupData1()
        {
            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(3, CallForProposalRepository);
            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(3, TemplateRepository);
            
            var questions = new List<Question>();
            for (int i = 0; i < 12; i++)
            {
                questions.Add(CreateValidEntities.Question(i+1));
                if (i < 6)
                {
                    questions[i].CallForProposal = null;
                    questions[i].Template = TemplateRepository.GetNullableById(2);
                }
                else
                {
                    questions[i].CallForProposal = CallForProposalRepository.GetNullableById(3);
                    questions[i].Template = null;
                }
                questions[i].Order = 12 - i;
            }

            questions[1].Template = TemplateRepository.GetNullableById(1);
            questions[10].CallForProposal = CallForProposalRepository.GetNullableById(2);
            questions[4].Options.Add(CreateValidEntities.QuestionOption(1));
            questions[4].Options.Add(CreateValidEntities.QuestionOption(2));
            questions[9].Options.Add(CreateValidEntities.QuestionOption(3));

            var fakeQuestions = new FakeQuestions();
            fakeQuestions.Records(0, QuestionRepository, questions);

            var questionAnswers = new List<QuestionAnswer>();
            questionAnswers.Add(CreateValidEntities.QuestionAnswer(1));
            questionAnswers[0].Question = QuestionRepository.GetNullableById(8);
            var fakeQuestionAnswers = new FakeQuestionAnswers();
            fakeQuestionAnswers.Records(3, QuestionAnswerRepository, questionAnswers);

        }

        public void SetupData2()
        {
            var fakeQuestionTypes = new FakeQuestionTypes();
            fakeQuestionTypes.Records(5, QuestionTypeRepository);
            var fakeValidators = new FakeValidators();
            fakeValidators.Records(4, ValidatorRepository);
        }
        
    }
}
