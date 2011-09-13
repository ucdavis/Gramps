using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing;

namespace Gramps.Tests.ServiceTests
{
    [TestClass]
    public class AnswerServiceTests
    {
        public IAnswerService AnswerService { get; set; }

        public AnswerServiceTests()
        {
            AnswerService = new AnswerService();
        }

        #region Text Box Tests
                   
        [TestMethod]
        public void TestAnswerServiceTextBox1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert		
        }

        [TestMethod]
        public void TestAnswerServiceTextBox2()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = true; //This is different from TestAnswerService1

            const bool saveWithValidate = false; //This is different from TestAnswerService1
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox3()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = false; //This is different from TestAnswerService1
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid); //This is different from TestAnswerService1
            Assert.AreEqual(0, proposal.Answers.Count); 
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox4()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox5()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new string[0], questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, proposal.Answers.Count);
            //Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox6()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[] { "Required", "Email" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is not a valid email.");
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("my answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox7()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("Test@Testy.COM", new[] { "Required", "Email" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("test@testy.com", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox8()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[] { "Required", "Url" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is not a valid url.");
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox9()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("http://www.ucdavis.edu", new[] { "Required", "Url" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("http://www.ucdavis.edu", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox10()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[] { "Required", "Date" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is not a valid date.");
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox11()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("12/25/2011", new[] { "Required", "Date" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("12/25/2011", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox12()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[]{ "Required", "Phone Number" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is not a valid phone number.");
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox13()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("(530) 555-5555", new[] { "Required", "Phone Number" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("(530) 555-5555", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox14()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[] { "Required", "Zip Code" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is not a valid zip code.");
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextBox15()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("95616", new[] { "Required", "Zip Code" }, questionId, validators, questionTypes, out proposalAnswer);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("95616", proposal.Answers[0].Answer);
            #endregion Assert
        }
        #endregion Text Box Tests

        #region Text Area Tests
        [TestMethod]
        public void TestAnswerServiceTextArea1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Text Area");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextArea2()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Text Area");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextArea3()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Text Area");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceTextArea4()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("My Answer", new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Text Area", 5);

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? must be less than 5 characters long.");
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("My Answer", proposal.Answers[0].Answer);
            #endregion Assert
        }

        #endregion Text Area Tests

        #region Boolean Tests
        [TestMethod]
        public void TestAnswerServiceBoolean1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new string[0], questionId, validators, questionTypes, out proposalAnswer, "Boolean");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("false", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceBoolean2()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new string[0], questionId, validators, questionTypes, out proposalAnswer, "Boolean");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("false", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceBoolean3()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("true", new string[0], questionId, validators, questionTypes, out proposalAnswer, "Boolean");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("true", proposal.Answers[0].Answer);
            #endregion Assert
        }
        #endregion Boolean Tests

        #region Radio Buttons Tests
        [TestMethod]
        public void TestAnswerServiceRadioButtons1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Radio Buttons");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceRadioButtons2()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Radio Buttons");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceRadioButtons3()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("Blue", new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Radio Buttons");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("Blue", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceRadioButtons4()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new string[0], questionId, validators, questionTypes, out proposalAnswer, "Radio Buttons");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }
        #endregion Radio Buttons Tests

        #region Checkbox List Test
        [TestMethod]
        public void TestAnswerServiceCheckboxList1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Checkbox List");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceCheckboxList2()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Checkbox List");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]        
        public void TestAnswerServiceCheckboxList3()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new string[0], questionId, validators, questionTypes, out proposalAnswer, "Checkbox List");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceCheckboxList4()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Checkbox List");
            proposalAnswer[0].CblAnswer = new[]{string.Empty};

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceCheckboxList5()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Checkbox List");
            proposalAnswer[0].CblAnswer = new []{"Blue"};

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("Blue", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceCheckboxList6()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Checkbox List");
            proposalAnswer[0].CblAnswer = new[] { "Blue", "Green" };

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("Blue,Green", proposal.Answers[0].Answer);
            #endregion Assert
        }
        #endregion Checkbox List Test

        #region Drop Down Tests
        [TestMethod]
        public void TestAnswerServiceDropDown1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Drop Down");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceDropDown2()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Drop Down");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceDropDown3()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal("Blue", new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Drop Down");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(1, proposal.Answers.Count);
            Assert.AreEqual("Blue", proposal.Answers[0].Answer);
            #endregion Assert
        }

        [TestMethod]
        public void TestAnswerServiceDropDown4()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new string[0], questionId, validators, questionTypes, out proposalAnswer, "Drop Down");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }
        #endregion Drop Down Tests

        #region Date Tests //Same as Text Box, just used to link jQuery Calendar.
        [TestMethod]
        public void TestAnswerServiceDate1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(string.Empty, new[] { "Required" }, questionId, validators, questionTypes, out proposalAnswer, "Date");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsFalse(modelState.IsValid);
            modelState.AssertErrorsAre("What Test Question? is a required field.");
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }

        #endregion Date Tests

        #region No Answer Tests
        [TestMethod]
        public void TestAnswerServiceNoAnswer1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            QuestionAnswerParameter[] proposalAnswer;
            var proposal = Proposal(null, new string[0], questionId, validators, questionTypes, out proposalAnswer, "No Answer");

            var modelState = new ModelStateDictionary();

            var propPassed = CreateValidEntities.Proposal(1);
            propPassed.IsSubmitted = false;

            const bool saveWithValidate = true;
            #endregion Arrange

            #region Act
            AnswerService.ProcessAnswers(propPassed, proposalAnswer, saveWithValidate, proposal, modelState);
            #endregion Act

            #region Assert
            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, proposal.Answers.Count);
            #endregion Assert
        }
        #endregion No Answer Tests

        #region Helpers

        private static Proposal Proposal(string answer, string[] validatorText, int questionId, List<Validator> validators, List<QuestionType> questionTypes, out QuestionAnswerParameter[] proposalAnswer, string questionType = "Text Box", int? maxLength = null)
        {
            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == questionType).Single();
            if(maxLength.HasValue)
            {
                proposal.CallForProposal.Questions[0].MaxCharacters = maxLength.Value;
            }
            foreach(var val in validatorText)
            {
                proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == val).Single());
            }


            proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = answer;
            return proposal;
        }

        public void SetupData(List<Validator> validators, List<QuestionType> questionTypes)
        {
            #region Validator
            var validator = new Validator();
            validator.Name = "Required";
            validator.Class = validator.Name.ToLower();
            validator.RegEx = @"^.+$";
            validator.ErrorMessage = "{0} is a required field.";
            validators.Add(validator);

            validator = new Validator();
            validator.Name = "Email";
            validator.Class = validator.Name.ToLower();
            validator.RegEx = @"(^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$){1}|^$";
            validator.ErrorMessage = "{0} is not a valid email.";
            validators.Add(validator);

            validator = new Validator();
            validator.Name = "Url";
            validator.Class = validator.Name.ToLower();
            validator.RegEx = @"(^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$){1}|^$";
            validator.ErrorMessage = "{0} is not a valid url.";
            validators.Add(validator);

            validator = new Validator();
            validator.Name = "Date";
            validator.Class = validator.Name.ToLower();
            validator.RegEx = @"(^(((0?[1-9]|1[012])[-\/](0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])[-\/](29|30)|(0?[13578]|1[02])[-\/]31)[-\/](19|[2-9]\d)\d{2}|0?2[-\/]29[-\/]((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$){1}|^$";
            validator.ErrorMessage = "{0} is not a valid date.";
            validators.Add(validator);

            validator = new Validator();
            validator.Name = "Phone Number";
            validator.Class = "phoneUS";
            validator.RegEx = @"(^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$){1}|^$";
            validator.ErrorMessage = "{0} is not a valid phone number.";
            validators.Add(validator);

            validator = new Validator();
            validator.Name = "Zip Code";
            validator.Class = "zipUS";
            validator.RegEx = @"^\d{5}(-\d{4})?$";
            validator.ErrorMessage = "{0} is not a valid zip code.";
            validators.Add(validator);
            #endregion Validator

            #region Question Type
            var questionType = new QuestionType();
            questionType.Name = "Text Box";
            questionType.HasOptions = false;
            questionType.ExtendedProperty = true;
            questionTypes.Add(questionType);

            questionType = new QuestionType();
            questionType.Name = "Text Area";
            questionType.HasOptions = false;
            questionType.ExtendedProperty = false;
            questionTypes.Add(questionType);

            questionType = new QuestionType();
            questionType.Name = "Boolean";
            questionType.HasOptions = false;
            questionType.ExtendedProperty = false;
            questionTypes.Add(questionType);

            questionType = new QuestionType();
            questionType.Name = "Radio Buttons";
            questionType.HasOptions = true;
            questionType.ExtendedProperty = false;
            questionTypes.Add(questionType);

            questionType = new QuestionType();
            questionType.Name = "Checkbox List";
            questionType.HasOptions = true;
            questionType.ExtendedProperty = false;
            questionTypes.Add(questionType);

            questionType = new QuestionType();
            questionType.Name = "Drop Down";
            questionType.HasOptions = true;
            questionType.ExtendedProperty = false;
            questionTypes.Add(questionType);

            questionType = new QuestionType();
            questionType.Name = "Date";
            questionType.HasOptions = false;
            questionType.ExtendedProperty = true;
            questionTypes.Add(questionType);

            questionType = new QuestionType();
            questionType.Name = "No Answer";
            questionType.HasOptions = false;
            questionType.ExtendedProperty = false;
            questionTypes.Add(questionType);
            #endregion Question Type
        }
        #endregion Helpers
    }
}
