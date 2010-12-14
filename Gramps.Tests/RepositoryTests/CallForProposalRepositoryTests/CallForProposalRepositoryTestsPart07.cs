using System;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region Question Tests
        #region Invalid Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionsWithAValueOfNullDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                callForProposal = GetValid(9);
                callForProposal.Questions = null;
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(callForProposal);
                Assert.AreEqual(callForProposal.Questions, null);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Questions: may not be null");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestQuestionsWithAnInvalidValueDoesNotSave()
        {
            CallForProposal callForProposal = null;
            try
            {
                #region Arrange
                var questionTypeRepository = new Repository<QuestionType>();
                questionTypeRepository.DbContext.BeginTransaction();
                LoadQuestionTypes();
                questionTypeRepository.DbContext.CommitTransaction();
                callForProposal = GetValid(9);
                callForProposal.IsActive = true;
                var question = new Question();
                question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Text Box").Single();
                question.Name = "";
                question.Order = 1;
                callForProposal.AddQuestion(question);
                #endregion Arrange

                #region Act
                CallForProposalRepository.DbContext.BeginTransaction();
                CallForProposalRepository.EnsurePersistent(callForProposal);
                CallForProposalRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(callForProposal);
                Assert.AreEqual(1, callForProposal.Questions.Count);
                var results = callForProposal.ValidationResults().AsMessageList();
                results.AssertErrorsAre("QuestionsList: One or more invalid questions detected");
                Assert.IsTrue(callForProposal.IsTransient());
                Assert.IsFalse(callForProposal.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests
        #region Valid Tests

        [TestMethod]
        public void TestQuestionWithEmptyListSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Questions = new List<Question>();
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(0, record.Questions.Count);
            #endregion Assert	
        }

        [TestMethod]
        public void TestQuestionWithOneValueNoOptionsOrValidatorsSaves()
        {
            #region Arrange
            var questionTypeRepository = new Repository<QuestionType>();
            questionTypeRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            questionTypeRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            var question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Text Box").Single();
            question.Name = "Who Me?";
            question.Order = 1;
            record.AddQuestion(question);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(1, record.Questions.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionWithOneValueOptionsAndNoValidatorsSaves()
        {
            #region Arrange
            var questionTypeRepository = new Repository<QuestionType>();
            questionTypeRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            questionTypeRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            var question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Radio Buttons").Single();
            question.Name = "Who Me?";
            question.Order = 1;
            question.AddQuestionOption(new QuestionOption() {Name = "Blue"});
            question.AddQuestionOption(new QuestionOption() { Name = "Green" });
            question.AddQuestionOption(new QuestionOption() { Name = "Yellow" });
            record.AddQuestion(question);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(1, record.Questions.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestQuestionWithValueOptionsAndValidatorsSaves()
        {
            #region Arrange
            var questionTypeRepository = new Repository<QuestionType>();
            var validatorRepository = new Repository<Validator>();
            questionTypeRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            questionTypeRepository.DbContext.CommitTransaction();
            validatorRepository.DbContext.BeginTransaction();
            LoadValidators();
            validatorRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            var question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Radio Buttons").Single();
            question.Name = "Who Me?";
            question.Order = 1;
            question.AddQuestionOption(new QuestionOption() { Name = "Blue" });
            question.AddQuestionOption(new QuestionOption() { Name = "Green" });
            question.AddQuestionOption(new QuestionOption() { Name = "Yellow" });
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            record.AddQuestion(question);

            question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Text Box").Single();
            question.Name = "When?";
            question.Order = 2;
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Date").Single());
            record.AddQuestion(question);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            Assert.IsNotNull(record.Questions);
            Assert.AreEqual(2, record.Questions.Count);
            #endregion Assert
        }


        [TestMethod]
        public void TestQuestionWithInvalidDataSavesifNotActive()
        {
            #region Arrange
            var questionTypeRepository = new Repository<QuestionType>();
            questionTypeRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            questionTypeRepository.DbContext.CommitTransaction();
            var callForProposal = GetValid(9);
            callForProposal.IsActive = false;
            var question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Text Box").Single();
            question.Name = "";
            question.Order = 1;
            callForProposal.AddQuestion(question);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(callForProposal.IsTransient());
            Assert.IsTrue(callForProposal.IsValid());
            Assert.IsNotNull(callForProposal.Questions);
            Assert.AreEqual(1, callForProposal.Questions.Count);
            #endregion Assert		
        }


        #endregion Valid Tests
        #region Cascade Tests


        [TestMethod]
        public void TestQuestionsAndQuestionOptionsAndValidatorCrossTablesCascadeSaves()
        {
            #region Arrange
            var questionTypeRepository = new Repository<QuestionType>();
            var validatorRepository = new Repository<Validator>();
            questionTypeRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            questionTypeRepository.DbContext.CommitTransaction();
            validatorRepository.DbContext.BeginTransaction();
            LoadValidators();
            validatorRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            var question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Radio Buttons").Single();
            question.Name = "Who Me?";
            question.Order = 1;
            question.AddQuestionOption(new QuestionOption() { Name = "Blue" });
            question.AddQuestionOption(new QuestionOption() { Name = "Green" });
            question.AddQuestionOption(new QuestionOption() { Name = "Yellow" });
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            record.AddQuestion(question);

            question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Text Box").Single();
            question.Name = "When?";
            question.Order = 2;
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Date").Single());
            record.AddQuestion(question);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            var saveQuestion1Id = record.Questions[0].Id;
            var saveQuestion2Id = record.Questions[1].Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[0]);
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            var questionRepository = new Repository<Question>();
            var question1 = questionRepository.Queryable.Where(a => a.Id == saveQuestion1Id).Single();
            var question2 = questionRepository.Queryable.Where(a => a.Id == saveQuestion2Id).Single();
            Assert.AreEqual(3, question1.Options.Count);
            Assert.AreEqual("Green", question1.Options[1].Name);
            Assert.AreEqual(1, question1.Validators.Count);

            Assert.AreEqual(2, question2.Validators.Count);
            Assert.AreEqual("Date", question2.Validators[1].Name);
            #endregion Assert		
        }

        [TestMethod]
        public void TestQuestionsAndQuestionOptionsAndValidatorCrossTablesCascadeUpdates()
        {
            #region Arrange
            var questionTypeRepository = new Repository<QuestionType>();
            var validatorRepository = new Repository<Validator>();
            questionTypeRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            questionTypeRepository.DbContext.CommitTransaction();
            validatorRepository.DbContext.BeginTransaction();
            LoadValidators();
            validatorRepository.DbContext.CommitTransaction();
            var record = GetValid(9);
            var question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Radio Buttons").Single();
            question.Name = "Who Me?";
            question.Order = 1;
            question.AddQuestionOption(new QuestionOption() { Name = "Blue" });
            question.AddQuestionOption(new QuestionOption() { Name = "Green" });
            question.AddQuestionOption(new QuestionOption() { Name = "Yellow" });
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            record.AddQuestion(question);

            question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Text Box").Single();
            question.Name = "When?";
            question.Order = 2;
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Date").Single());
            record.AddQuestion(question);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            var saveId = record.Id;
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[0]);
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            record = CallForProposalRepository.Queryable.Where(a => a.Id == saveId).Single();
            record.Questions[0].Options[1].Name = "UpGreen";

            record.Questions[1].Validators.RemoveAt(1);
            record.Questions[1].Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Url").Single());

            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();

            var saveQuestion1Id = record.Questions[0].Id;
            var saveQuestion2Id = record.Questions[1].Id;

            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[0]);
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            var questionRepository = new Repository<Question>();
            var question1 = questionRepository.Queryable.Where(a => a.Id == saveQuestion1Id).Single();
            var question2 = questionRepository.Queryable.Where(a => a.Id == saveQuestion2Id).Single();
            Assert.AreEqual(3, question1.Options.Count);
            Assert.AreEqual("UpGreen", question1.Options[1].Name);
            Assert.AreEqual(1, question1.Validators.Count);

            Assert.AreEqual(2, question2.Validators.Count);
            Assert.AreEqual("Url", question2.Validators[1].Name);
            #endregion Assert
        }


        [TestMethod]
        public void TestQuestionsCascadeDeletesToQuestionQuestionOptionsAndQuestionValidatorCrossTableOnly()
        {
            #region Arrange
            var questionRepository = new Repository<Question>();
            var questionTypeRepository = new Repository<QuestionType>();
            var validatorRepository = new Repository<Validator>();
            var questionOptionRepository = new Repository<QuestionOption>();

            questionTypeRepository.DbContext.BeginTransaction();
            LoadQuestionTypes();
            questionTypeRepository.DbContext.CommitTransaction();
            validatorRepository.DbContext.BeginTransaction();
            LoadValidators();
            validatorRepository.DbContext.CommitTransaction();

            var questionCount = questionRepository.Queryable.Count();
            var questionOptionCount = questionOptionRepository.Queryable.Count();
            var questionTypeCount = questionTypeRepository.Queryable.Count();
            var validatorCount = validatorRepository.Queryable.Count();

            var record = GetValid(9);
            var question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Radio Buttons").Single();
            question.Name = "Who Me?";
            question.Order = 1;
            question.AddQuestionOption(new QuestionOption() { Name = "Blue" });
            question.AddQuestionOption(new QuestionOption() { Name = "Green" });
            question.AddQuestionOption(new QuestionOption() { Name = "Yellow" });
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            record.AddQuestion(question);

            question = new Question();
            question.QuestionType = questionTypeRepository.Queryable.Where(a => a.Name == "Text Box").Single();
            question.Name = "When?";
            question.Order = 2;
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Required").Single());
            question.Addvalidators(validatorRepository.Queryable.Where(a => a.Name == "Date").Single());
            record.AddQuestion(question);
            #endregion Arrange

            #region Act
            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.EnsurePersistent(record);
            CallForProposalRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[0]);
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            CallForProposalRepository.DbContext.BeginTransaction();
            CallForProposalRepository.Remove(record);
            CallForProposalRepository.DbContext.CommitTransaction();

            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[0]);
            NHibernateSessionManager.Instance.GetSession().Evict(record.Questions[1]);
            NHibernateSessionManager.Instance.GetSession().Evict(record);

            Assert.AreEqual(validatorCount, validatorRepository.Queryable.Count());
            Assert.AreEqual(questionCount, questionRepository.Queryable.Count());
            Assert.AreEqual(questionOptionCount, questionOptionRepository.Queryable.Count());
            Assert.AreEqual(questionTypeCount, questionTypeRepository.Queryable.Count());
            

            #endregion Assert		
        }

        #endregion Cascade Tests
        #endregion Question Tests
    }
}
