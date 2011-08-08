using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Services;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region Edit Get Tests
        /// <summary>
        /// Tests the FieldToTest with A value of TestValue does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestEditGetThrowsAnExceptionIfGuidIsNotUnique()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var proposals = new List<Proposal>();
                for (int i = 0; i < 2; i++)
                {
                    proposals.Add(CreateValidEntities.Proposal(i+1));
                    proposals[i].Guid = SpecificGuid.GetGuid(1);
                }
                var fakeProposals = new FakeProposals();
                fakeProposals.Records(3, ProposalRepository, proposals);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Edit(SpecificGuid.GetGuid(1));
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains more than one element", ex.Message);
                throw;
            }	
        }


        [TestMethod]
        public void TestEditGetRedirectsWhenProposalNotFound()
        {
            #region Arrange
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Edit(SpecificGuid.GetGuid(7))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "email3@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Edit(SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetRedirectsWhenSubmitted()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email2@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot edit proposal once submitted.", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual(SpecificGuid.GetGuid(2), results.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWhenCallEndDatePast()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email3@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(3))
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Call for proposal has closed, you will not be able to save changes.", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(3), results.Proposal.Guid);
            Assert.AreEqual("Name1", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWhenCallNotActive()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email4@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(4))
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Call for proposal has been deactivated, you will not be able to save changes.", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(4), results.Proposal.Guid);
            Assert.AreEqual("Name2", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsView()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5))
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual(null, Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }
        #endregion Edit Get Tests

        #region Edit Post Tests

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestEditPostThrowsAnExceptionIfGuidIsNotUnique()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var proposals = new List<Proposal>();
                for (int i = 0; i < 2; i++)
                {
                    proposals.Add(CreateValidEntities.Proposal(i + 1));
                    proposals[i].Guid = SpecificGuid.GetGuid(1);
                }
                var fakeProposals = new FakeProposals();
                fakeProposals.Records(3, ProposalRepository, proposals);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Edit(SpecificGuid.GetGuid(1), null, new QuestionAnswerParameter[0], null, StaticValues.RB_SaveOptions_SaveWithValidation );
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains more than one element", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestEditPostRedirectsWhenProposalNotFound()
        {
            #region Arrange
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Edit(SpecificGuid.GetGuid(7), null, new QuestionAnswerParameter[0], null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsWhenNoAccess()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email3@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            Controller.Edit(SpecificGuid.GetGuid(2), null, new QuestionAnswerParameter[0], null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostRedirectsWhenSubmitted()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email2@testy.com");
            SetupData6();
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(2), null, new QuestionAnswerParameter[0], null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot edit proposal once submitted.", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual(SpecificGuid.GetGuid(2), results.RouteValues["id"]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenCallEndDatePast()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email3@testy.com");
            SetupData6();
            var proposalToEdit = CreateValidEntities.Proposal(9);
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(3), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Call for proposal has closed");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(3), results.Proposal.Guid);
            Assert.AreEqual("Name1", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenCallNotActive()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email4@testy.com");
            SetupData6();
            var proposalToEdit = CreateValidEntities.Proposal(9);
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(4), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Call for proposal has been deactivated");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(4), results.Proposal.Guid);
            Assert.AreEqual("Name2", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostReturnsViewWhenUploadedFileIsNotPdf()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            var proposalToEdit = CreateValidEntities.Proposal(9);
            var qaParm = new QuestionAnswerParameter[0];
            var fakedFile = new FakeHttpPostedFileBase("fileName", "notpdf", new byte[] {1, 2, 4});
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, fakedFile, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Can only upload PDF files.");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenUploadedFileIsPdf()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 12.31m;
            var qaParm = new QuestionAnswerParameter[0];
            var fakedFile = new FakeHttpPostedFileBase("fileName", "application/PDF", new byte[] { 1, 2, 4 });
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, fakedFile, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Edited Successfully", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            var proposalArgs = (Proposal) ProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything))[0][0]; 
            Assert.IsNotNull(proposalArgs);
            Assert.AreEqual(12.31m, proposalArgs.RequestedAmount);
            Assert.IsFalse(proposalArgs.IsSubmitted);
            Assert.IsNotNull(proposalArgs.File);
            Assert.AreEqual("application/pdf", proposalArgs.File.ContentType.ToLower());
            Assert.AreEqual("124", proposalArgs.File.Contents.ByteArrayToString());
            Assert.AreEqual("fileName", proposalArgs.File.Name);
            Assert.AreEqual(DateTime.Now.Date, proposalArgs.File.DateAdded.Date);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostInvestigator1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            //SetupData7();
            var fakeInvestigators = new FakeInvestigators();
            fakeInvestigators.Records(3, InvestigatorRepository);
            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0.01m;
            var qaParm = new QuestionAnswerParameter[0];            
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Must have one primary investigator");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostInvestigator2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            //SetupData7();
            var investigators = new List<Investigator>();
            for (int i = 0; i < 3; i++)
            {
                investigators.Add(CreateValidEntities.Investigator(i + 1));
                investigators[i].Proposal = ProposalRepository.GetNullableById(5);
                investigators[i].IsPrimary = true;
            }
            var fakeInvestigators = new FakeInvestigators();
            fakeInvestigators.Records(0, InvestigatorRepository, investigators);
            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Must have one primary investigator");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostInvestigator2A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            //SetupData7();
            var investigators = new List<Investigator>();
            for (int i = 0; i < 3; i++)
            {
                investigators.Add(CreateValidEntities.Investigator(i + 1));
                investigators[i].Proposal = ProposalRepository.GetNullableById(5);
                investigators[i].IsPrimary = true;
            }
            var fakeInvestigators = new FakeInvestigators();
            fakeInvestigators.Records(0, InvestigatorRepository, investigators);
            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SubmitFinal)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Must have one primary investigator");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SubmitFinal, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostInvestigator2B()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            //SetupData7();
            var investigators = new List<Investigator>();
            for (int i = 0; i < 3; i++)
            {
                investigators.Add(CreateValidEntities.Investigator(i + 1));
                investigators[i].Proposal = ProposalRepository.GetNullableById(5);
                investigators[i].IsPrimary = true;
            }
            var fakeInvestigators = new FakeInvestigators();
            fakeInvestigators.Records(0, InvestigatorRepository, investigators);
            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            //Controller.ModelState.AssertErrorsAre("Must have one primary investigator");
            Assert.AreEqual("Proposal Edited Successfully", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostProposalMaximum1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 10.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Requested Amount must be $0.01 or less");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert		
        }

        [TestMethod]
        public void TestEditPostProposalMaximum1A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 10.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SubmitFinal)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Requested Amount must be $0.01 or less");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SubmitFinal, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum1B()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 10.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            //Controller.ModelState.AssertErrorsAre("Requested Amount must be $0.01 or less");
            Assert.AreEqual("Proposal Edited Successfully", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum1C()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email6@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 10.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(6), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            //Controller.ModelState.AssertErrorsAre("Requested Amount must be $0.01 or less");
            Assert.AreEqual("Proposal Edited Successfully", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(6), results.Proposal.Guid);
            Assert.AreEqual("Name4", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum1D()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email6@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 10.01m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(6), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SubmitFinal)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            //Controller.ModelState.AssertErrorsAre("Requested Amount must be $0.01 or less");
            Assert.AreEqual("Proposal Submitted Successfully", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual(SpecificGuid.GetGuid(6), results.RouteValues["id"]);
            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            var proposalArgs = (Proposal)ProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything))[0][0];
            Assert.IsNotNull(proposalArgs);
            Assert.AreEqual(10.01m, proposalArgs.RequestedAmount);
            Assert.IsTrue(proposalArgs.IsSubmitted);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostProposalMaximum2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Requested Amount must be entered");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum2A()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SubmitFinal)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Requested Amount must be entered");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SubmitFinal, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum2B()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email5@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(5), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            //Controller.ModelState.AssertErrorsAre("Requested Amount must be entered");
            Assert.AreEqual("Proposal Edited Successfully", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(5), results.Proposal.Guid);
            Assert.AreEqual("Name3", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum2C()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email6@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(6), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveWithValidation)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Requested Amount must be entered");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(6), results.Proposal.Guid);
            Assert.AreEqual("Name4", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveWithValidation, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum2D()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email6@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(6), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SaveNoValidate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            //Controller.ModelState.AssertErrorsAre("Requested Amount must be entered");
            Assert.AreEqual("Proposal Edited Successfully", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(6), results.Proposal.Guid);
            Assert.AreEqual("Name4", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SaveNoValidate, results.SaveOptionChoice);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostProposalMaximum2E()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "email6@testy.com");
            SetupData6();
            SetupData7();

            var proposalToEdit = CreateValidEntities.Proposal(9);
            proposalToEdit.RequestedAmount = 0m;
            var qaParm = new QuestionAnswerParameter[0];
            #endregion Arrange

            #region Act
            var results = Controller.Edit(SpecificGuid.GetGuid(6), proposalToEdit, qaParm, null, StaticValues.RB_SaveOptions_SubmitFinal)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Requested Amount must be entered");
            Assert.AreEqual("Unable to save. Please Correct Errors", Controller.Message);
            Assert.IsNotNull(results);
            Assert.AreEqual("test1@testy.com", results.ContactEmail);
            Assert.AreEqual(SpecificGuid.GetGuid(6), results.Proposal.Guid);
            Assert.AreEqual("Name4", results.CallForProposal.Name);
            Assert.AreEqual(StaticValues.RB_SaveOptions_SubmitFinal, results.SaveOptionChoice);
            ProposalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));

            #endregion Assert
        }

        //Test passed answers

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Write these tests");
            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert
        }
        #endregion Edit Post Tests
    }
}
