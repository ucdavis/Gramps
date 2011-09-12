using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FluentNHibernate.MappingModel;
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


        [TestMethod]
        public void TestAnswerService1()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = string.Empty;

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
        public void TestAnswerService2()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = string.Empty;

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
        public void TestAnswerService3()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = string.Empty;

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
        public void TestAnswerService4()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = "My Answer";

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
        public void TestAnswerService5()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            //proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = null;

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
        public void TestAnswerService6()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Email").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = "My Answer";

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
        public void TestAnswerService7()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Email").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = "Test@Testy.COM";

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
        public void TestAnswerService8()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Url").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = "My Answer";

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
        public void TestAnswerService9()
        {
            #region Arrange
            const int questionId = 8;
            var validators = new List<Validator>();
            var questionTypes = new List<QuestionType>();
            SetupData(validators, questionTypes);

            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CreateValidEntities.CallForProposal(1);
            proposal.CallForProposal.Questions.Add(new Question());
            proposal.CallForProposal.Questions[0].SetIdTo(questionId);
            proposal.CallForProposal.Questions[0].Name = "What Test Question?";
            proposal.CallForProposal.Questions[0].QuestionType = questionTypes.Where(a => a.Name == "Text Box").Single();
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Required").Single());
            proposal.CallForProposal.Questions[0].Validators.Add(validators.Where(a => a.Name == "Url").Single());

            var proposalAnswer = new QuestionAnswerParameter[1];
            proposalAnswer[0] = new QuestionAnswerParameter();
            proposalAnswer[0].QuestionId = questionId;
            proposalAnswer[0].Answer = "http://www.ucdavis.edu";

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
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("continue these tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }

        #region Helpers
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
