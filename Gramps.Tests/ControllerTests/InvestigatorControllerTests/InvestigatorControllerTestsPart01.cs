using System.Linq;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.InvestigatorControllerTests
{
    public partial class InvestigatorControllerTests
    {
        #region Create Tests
        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetRedirectsToErrorIfProposalNotFound()
        {
            #region Arrange
            SetupTestData1();
            #endregion Arrange

            #region Act
            Controller.Create(SpecificGuid.GetGuid(15))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsToErrorIfNoAccess()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] {""}, "me@notme.com");
            #endregion Arrange

            #region Act
            Controller.Create(SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetRedirectsToDetailsIfSubmitted()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email3@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(3))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(3)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot add investigator to proposal once proposal is submitted.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(3), result.RouteValues.ElementAt(2).Value);
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreateGetRedirectsToEditIfCallNotActive()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email1@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(1))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot add investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(1), result.RouteValues.ElementAt(2).Value);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsToEditIfCallEndDatePast()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email2@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(2))
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot add investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(2), result.RouteValues.ElementAt(2).Value);
            #endregion Assert
        }


        [TestMethod]
        public void TestCreateGetReturnsViewWithExpectedData()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email4@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(4))
                .AssertViewRendered()
                .WithViewData<InvestigatorViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Proposal.Id);
            Assert.AreEqual(0, result.Investigator.Id);
            #endregion Assert		
        }
        #endregion Create Get Tests
        #region Create Post Tests
        [TestMethod]
        public void TestCreatePostRedirectsToErrorIfProposalNotFound()
        {
            #region Arrange
            SetupTestData1();
            #endregion Arrange

            #region Act
            Controller.Create(SpecificGuid.GetGuid(15), new Investigator())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("Your proposal was not found.", Controller.Message);
            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsToErrorIfNoAccess()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "me@notme.com");
            #endregion Arrange

            #region Act
            Controller.Create(SpecificGuid.GetGuid(1), new Investigator())
                .AssertActionRedirect()
                .ToAction<ErrorController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostRedirectsToDetailsIfSubmitted()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email3@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(3), new Investigator())
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Details(SpecificGuid.GetGuid(3)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Cannot add investigator to proposal once proposal is submitted.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(3), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostRedirectsToEditIfCallNotActive()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email1@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(1), new Investigator())
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot add investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(1), result.RouteValues.ElementAt(2).Value);
            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostRedirectsToEditIfCallEndDatePast()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email2@testy.com");
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(2), new Investigator())
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(2)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Call For Proposal is not active or end date has passed. Cannot add investigator.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(2), result.RouteValues.ElementAt(2).Value);

            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestCreatePostDoesNotSaveIfPrimaryIsTrueTwiceForProposal()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            var newInvestigator = CreateValidEntities.Investigator(9, true);
            newInvestigator.IsPrimary = true;
            newInvestigator.State = "XX";
            newInvestigator.Zip = "Zip9";
            newInvestigator.Phone = "999-999-9999";
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(6), newInvestigator)
                .AssertViewRendered()
                .WithViewData<InvestigatorViewModel>();
            #endregion Act

            #region Assert
            //Assert.AreEqual("", Controller.Message);
            Controller.ModelState.AssertErrorsAre("There can only be one primary investigator per proposal.");

            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Proposal.Id);
            Assert.AreEqual(0, result.Investigator.Id);
            Assert.IsTrue(result.Investigator.IsPrimary);
            Assert.AreEqual("Address19", result.Investigator.Address1);
            Assert.AreEqual("Address29", result.Investigator.Address2);
            Assert.AreEqual("Address39", result.Investigator.Address3);
            Assert.AreEqual("City9", result.Investigator.City);
            Assert.AreEqual("test9@testy.com", result.Investigator.Email);
            Assert.AreEqual("Institution9", result.Investigator.Institution);
            Assert.AreEqual("Name9", result.Investigator.Name);
            Assert.AreEqual("999-999-9999", result.Investigator.Phone);
            Assert.AreEqual("Position9", result.Investigator.Position);
            Assert.AreEqual("XX", result.Investigator.State);
            Assert.AreEqual("Zip9", result.Investigator.Zip);
            Assert.AreEqual(6, result.Investigator.Proposal.Id);

            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreatePostDoesNotSaveIfNotValid()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            var newInvestigator = CreateValidEntities.Investigator(9, true);
            newInvestigator.IsPrimary = false;
            newInvestigator.State = "xxx";
            newInvestigator.Zip = "Zip9";
            newInvestigator.Phone = "999-999-9999";
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(6), newInvestigator)
                .AssertViewRendered()
                .WithViewData<InvestigatorViewModel>();
            #endregion Act

            #region Assert
            //Assert.AreEqual("", Controller.Message);
            Controller.ModelState.AssertErrorsAre("State: length must be between 0 and 2");

            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Proposal.Id);
            Assert.AreEqual(0, result.Investigator.Id);
            Assert.IsFalse(result.Investigator.IsPrimary);
            Assert.AreEqual("Address19", result.Investigator.Address1);
            Assert.AreEqual("Address29", result.Investigator.Address2);
            Assert.AreEqual("Address39", result.Investigator.Address3);
            Assert.AreEqual("City9", result.Investigator.City);
            Assert.AreEqual("test9@testy.com", result.Investigator.Email);
            Assert.AreEqual("Institution9", result.Investigator.Institution);
            Assert.AreEqual("Name9", result.Investigator.Name);
            Assert.AreEqual("999-999-9999", result.Investigator.Phone);
            Assert.AreEqual("Position9", result.Investigator.Position);
            Assert.AreEqual("XXX", result.Investigator.State);
            Assert.AreEqual("Zip9", result.Investigator.Zip);
            Assert.AreEqual(6, result.Investigator.Proposal.Id);

            InvestigatorRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostSavesIfValid1()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email6@testy.com");
            var newInvestigator = CreateValidEntities.Investigator(9, true);
            newInvestigator.IsPrimary = false;
            newInvestigator.State = "xx";
            newInvestigator.Zip = "Zip9";
            newInvestigator.Phone = "999-999-9999";
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(6), newInvestigator)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(6)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Investigator Created Successfully.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(6), result.RouteValues.ElementAt(2).Value);

            InvestigatorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            var args = (Investigator) InvestigatorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything))[0][0]; 
            Assert.AreEqual(6, args.Proposal.Id);
            Assert.IsFalse(args.IsPrimary);
            Assert.AreEqual("Address19", args.Address1);
            Assert.AreEqual("Address29", args.Address2);
            Assert.AreEqual("Address39", args.Address3);
            Assert.AreEqual("City9", args.City);
            Assert.AreEqual("test9@testy.com", args.Email);
            Assert.AreEqual("Institution9", args.Institution);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual("999-999-9999", args.Phone);
            Assert.AreEqual("Position9", args.Position);
            Assert.AreEqual("XX", args.State);
            Assert.AreEqual("Zip9", args.Zip);

            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostSavesIfValid2()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email7@testy.com");
            var newInvestigator = CreateValidEntities.Investigator(9, true);
            newInvestigator.IsPrimary = true;
            newInvestigator.State = "xx";
            newInvestigator.Zip = "Zip9";
            newInvestigator.Phone = "999-999-9999";
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(7), newInvestigator)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(7)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Investigator Created Successfully.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(7), result.RouteValues.ElementAt(2).Value);

            InvestigatorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            var args = (Investigator)InvestigatorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything))[0][0];
            Assert.AreEqual(7, args.Proposal.Id);
            Assert.IsTrue(args.IsPrimary);
            Assert.AreEqual("Address19", args.Address1);
            Assert.AreEqual("Address29", args.Address2);
            Assert.AreEqual("Address39", args.Address3);
            Assert.AreEqual("City9", args.City);
            Assert.AreEqual("test9@testy.com", args.Email);
            Assert.AreEqual("Institution9", args.Institution);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual("999-999-9999", args.Phone);
            Assert.AreEqual("Position9", args.Position);
            Assert.AreEqual("XX", args.State);
            Assert.AreEqual("Zip9", args.Zip);

            #endregion Assert
        }

        [TestMethod]
        public void TestCreatePostSavesIfValid3()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(1, new[] { "" }, "email5@testy.com");
            var newInvestigator = CreateValidEntities.Investigator(9, true);
            newInvestigator.IsPrimary = true;
            newInvestigator.State = "xx";
            newInvestigator.Zip = "Zip9";
            newInvestigator.Phone = "999-999-9999";
            #endregion Arrange

            #region Act
            var result = Controller.Create(SpecificGuid.GetGuid(5), newInvestigator)
                .AssertActionRedirect()
                .ToAction<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(5)));
            #endregion Act

            #region Assert
            Assert.AreEqual("Investigator Created Successfully.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("id", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(SpecificGuid.GetGuid(5), result.RouteValues.ElementAt(2).Value);

            InvestigatorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything));
            var args = (Investigator)InvestigatorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Investigator>.Is.Anything))[0][0];
            Assert.AreEqual(5, args.Proposal.Id);
            Assert.IsTrue(args.IsPrimary);
            Assert.AreEqual("Address19", args.Address1);
            Assert.AreEqual("Address29", args.Address2);
            Assert.AreEqual("Address39", args.Address3);
            Assert.AreEqual("City9", args.City);
            Assert.AreEqual("test9@testy.com", args.Email);
            Assert.AreEqual("Institution9", args.Institution);
            Assert.AreEqual("Name9", args.Name);
            Assert.AreEqual("999-999-9999", args.Phone);
            Assert.AreEqual("Position9", args.Position);
            Assert.AreEqual("XX", args.State);
            Assert.AreEqual("Zip9", args.Zip);

            #endregion Assert
        }

        #endregion Create Post Tests
        #endregion Create Tests
    }
}
