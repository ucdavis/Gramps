using System;
using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        #region Create Get Tests
        [TestMethod]
        public void TestCreateGetRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Create(2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Create(null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(1, null)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.QuestionTypes.Count());
            Assert.AreEqual(4, result.Validators.Count());
            Assert.IsTrue(result.IsTemplate);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(1, result.TemplateId);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            SetupData2();
            #endregion Arrange

            #region Act
            var result = Controller.Create(0, 3)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.QuestionTypes.Count());
            Assert.AreEqual(4, result.Validators.Count());
            Assert.IsFalse(result.IsTemplate);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            #endregion Assert
        }


        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCreateGetThrowsExceptionIfTemplateIdAndCallForProposalIdNotSpecified1()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
                AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
                SetupData2();
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(null, null);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Must have either a template or a call for proposal", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCreateGetThrowsExceptionIfTemplateIdAndCallForProposalIdNotSpecified2()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
                AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
                SetupData2();
                #endregion Arrange

                #region Act
                thisFar = true;
                Controller.Create(0, 0);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Must have either a template or a call for proposal", ex.Message);
                throw;
            }
        }
        #endregion Create Get Tests

        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsIfNoAccess1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Create(2, null, new Question(), new string[0])
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "tester@testy.com"));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsIfNoAccess2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, new Question(), new string[0])
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(null, 3, "tester@testy.com"));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenNoCallOrTemplate1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, null, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenNoCallOrTemplate2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 4, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("RelatedTable: Must be related to Template or CallForProposal not both.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Text Box";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators.Add(CreateValidEntities.Validator(2));
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("Cannot have Email, Url, Date, Phone Number, or zip validators selected together.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesSaveWhenWrongValidators1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Text Box";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Boolean";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Boolean is Required.");
            #endregion Assert
        }
        [TestMethod]
        public void TestCreatePostDoesSaveWhenWrongValidators2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Boolean";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Radio Buttons";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Radio Buttons is Required.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesSaveWhenWrongValidators3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Radio Buttons";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Checkbox List";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Checkbox List is Required.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesSaveWhenWrongValidators4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Checkbox List";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Drop Down";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Drop Down is Required.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesSaveWhenWrongValidators5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Drop Down";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Text Area";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("The only validator allowed for a Question Type of Text Area is Required.");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesSaveWhenWrongValidators6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Text Area";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Date";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators.Add(CreateValidEntities.Validator(2));
            questionToCreate.Validators.Add(CreateValidEntities.Validator(3));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            questionToCreate.Validators[1].Class = "date";
            questionToCreate.Validators[1].Name = "Date";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("Name3 is not a valid validator for a Question Type of Date");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesSaveWhenWrongValidators7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "Date";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators.Add(CreateValidEntities.Validator(2));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            questionToCreate.Validators[1].Class = "date";
            questionToCreate.Validators[1].Name = "Date";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(null, 3));
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveWhenWrongValidators8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            questionToCreate.QuestionType = CreateValidEntities.QuestionType(1);
            questionToCreate.QuestionType.Name = "No Answer";
            questionToCreate.Validators.Add(CreateValidEntities.Validator(1));
            questionToCreate.Validators.Add(CreateValidEntities.Validator(2));
            questionToCreate.Validators.Add(CreateValidEntities.Validator(3));
            questionToCreate.Validators[0].Class = "required";
            questionToCreate.Validators[0].Name = "Required";
            questionToCreate.Validators[1].Class = "date";
            questionToCreate.Validators[1].Name = "Date";
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(null, 3, questionToCreate, questionOptions)
                .AssertViewRendered()
                .WithViewData<QuestionViewModel>();
            #endregion Act

            #region Assert
            QuestionRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            Assert.IsNotNull(result);
            Controller.ModelState.AssertErrorsAre("Required is not a valid validator for a Question Type of No Answer"
                , "Date is not a valid validator for a Question Type of No Answer"
                , "Name3 is not a valid validator for a Question Type of No Answer");
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostSavesAndRedirectsWhenValid1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(2, null, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["templateId"]);
            Assert.AreEqual(null, result.RouteValues["callForProposalId"]);
            Assert.AreEqual("Question added successfully", Controller.Message);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question) QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0]; 
            Assert.IsNotNull(args);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual(13, args.Order);
            #endregion Assert			
        }

        [TestMethod]
        public void TestCreatePostSavesAndRedirectsWhenValid2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "tester@testy.com");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var questionToCreate = CreateValidEntities.Question(9);
            var questionOptions = new string[0];
            SetupData1();
            #endregion Arrange

            #region Act
            var result = Controller.Create(0, 3, questionToCreate, questionOptions)
                .AssertActionRedirect()
                .ToAction<QuestionController>(a => a.Index(2, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.RouteValues["templateId"]);
            Assert.AreEqual(3, result.RouteValues["callForProposalId"]);
            Assert.AreEqual("Question added successfully", Controller.Message);
            QuestionRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Question>.Is.Anything));
            var args = (Question)QuestionRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Question>.Is.Anything))[0][0];
            Assert.IsNotNull(args);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual(7, args.Order);
            #endregion Assert
        }
        #endregion Create Post Tests
    }
}
