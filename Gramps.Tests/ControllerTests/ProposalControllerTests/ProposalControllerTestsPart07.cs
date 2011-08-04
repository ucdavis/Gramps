using System.Web;
using System.Web.Mvc;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region Create Get Tests


        [TestMethod]
        public void TestCreateGetRedirectsWhenCallNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(5)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenCallNotActive()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenCallHasEnded1()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenCallHasEnded2()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsView()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name4", result.CallForProposal.Name);
            Assert.AreEqual("test1@testy.com", result.ContactEmail);
            Assert.IsNotNull(result.Proposal);
            Assert.AreEqual(0, result.Proposal.Id);
            #endregion Assert		
        }
     
        #endregion Create Get Tests
        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsWhenCallNotFound()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(5, true, CreateValidEntities.Proposal(1))
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenCallNotActive()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(1, true, CreateValidEntities.Proposal(1))
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenCallHasEnded1()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(2, true, CreateValidEntities.Proposal(1))
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsWhenCallHasEnded2()
        {
            #region Arrange
            SetupData3();
            #endregion Arrange

            #region Act
            Controller.Create(3, true, CreateValidEntities.Proposal(1))
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.About());
            #endregion Act

            #region Assert
            Assert.AreEqual("Grant No longer Available", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostReturnsViewWhenNotValid1()
        {
            #region Arrange
            SetupData3();
            SetupData4();

            var proposalToCreate = CreateValidEntities.Proposal(9);
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, false, proposalToCreate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Recaptcha value not valid");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Proposal);
            Assert.AreEqual("Name4", result.CallForProposal.Name);
            Assert.AreEqual(4, result.Proposal.Sequence);
            Assert.AreEqual("email9@testy.com", result.Proposal.Email);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostReturnsViewWhenNotValid2()
        {
            #region Arrange
            SetupData3();
            SetupData4();

            var proposalToCreate = CreateValidEntities.Proposal(9);
            proposalToCreate.Email = null; //invalid
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, true, proposalToCreate)
                .AssertViewRendered()
                .WithViewData<ProposalViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Email: may not be null or empty");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Proposal);
            Assert.AreEqual("Name4", result.CallForProposal.Name);
            Assert.AreEqual(4, result.Proposal.Sequence);
            Assert.AreEqual(null, result.Proposal.Email);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostWhenValidAndUserDoesNotExist()
        {
            #region Arrange
            SetupData3();
            SetupData4();

            var proposalToCreate = CreateValidEntities.Proposal(9);
            MembershipService.Expect(a => a.DoesUserExist("email9@testy.com")).Return(false).Repeat.Any();
            MembershipService.Expect(a => a.ResetPassword("email9@testy.com")).Return("passReset").Repeat.Any();
            EmailService.Expect(a => a.SendConfirmation(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, true, proposalToCreate)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Confirmation("email9@testy.com"));
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Created Successfully", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("email9@testy.com", result.RouteValues["proposalEmail"]);
            
            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            var proposalArgs = (Proposal) ProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything))[0][0];
            Assert.AreEqual("email9@testy.com", proposalArgs.Email);
            Assert.AreEqual(4, proposalArgs.Sequence);

            EmailService.AssertWasCalled(a => a.SendConfirmation(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything));
            var emailArgs = EmailService.GetArgumentsForCallsMadeOn(a => a.SendConfirmation(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))[0];
            Assert.IsNotNull(emailArgs);
            Assert.AreEqual("email9@testy.com", ((Proposal)emailArgs[2]).Email);
            Assert.AreEqual(EmailTemplateType.ProposalConfirmation, ((EmailTemplate)emailArgs[3]).TemplateType);
            Assert.AreEqual(true, emailArgs[4]);
            Assert.AreEqual("email9@testy.com", emailArgs[5]);
            Assert.AreEqual("passReset", emailArgs[6]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostWhenValidAndUserDoesExist()
        {
            #region Arrange
            SetupData3();
            SetupData4();

            var proposalToCreate = CreateValidEntities.Proposal(9);
            MembershipService.Expect(a => a.DoesUserExist("email9@testy.com")).Return(true).Repeat.Any();
            //MembershipService.Expect(a => a.ResetPassword("email9@testy.com")).Return("passReset").Repeat.Any();
            EmailService.Expect(a => a.SendConfirmation(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything)).Repeat.Any();
            #endregion Arrange

            #region Act
            var result = Controller.Create(4, true, proposalToCreate)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Confirmation("email9@testy.com"));
            #endregion Act

            #region Assert
            Assert.AreEqual("Proposal Created Successfully", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("email9@testy.com", result.RouteValues["proposalEmail"]);

            ProposalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything));
            var proposalArgs = (Proposal)ProposalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Proposal>.Is.Anything))[0][0];
            Assert.AreEqual("email9@testy.com", proposalArgs.Email);
            Assert.AreEqual(4, proposalArgs.Sequence);
            MembershipService.AssertWasNotCalled(a => a.ResetPassword(Arg<string>.Is.Anything));

            EmailService.AssertWasCalled(a => a.SendConfirmation(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything));
            var emailArgs = EmailService.GetArgumentsForCallsMadeOn(a => a.SendConfirmation(
                Arg<HttpRequestBase>.Is.Anything,
                Arg<UrlHelper>.Is.Anything,
                Arg<Proposal>.Is.Anything,
                Arg<EmailTemplate>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))[0];
            Assert.IsNotNull(emailArgs);
            Assert.AreEqual("email9@testy.com", ((Proposal)emailArgs[2]).Email);
            Assert.AreEqual(EmailTemplateType.ProposalConfirmation, ((EmailTemplate)emailArgs[3]).TemplateType);
            Assert.AreEqual(true, emailArgs[4]);
            Assert.AreEqual("email9@testy.com", emailArgs[5]);
            Assert.AreEqual(null, emailArgs[6]);
            #endregion Assert
        }

        #endregion Create Post Tests
    }
}
