using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.EmailTemplateControllerTests
{
    public partial class EmailTemplateControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexWithoutAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(null, 1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexWithoutAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(2, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsViewWithExpectValues1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(7, result.DescriptionDict.Count);
            Assert.AreEqual("Email sent out to list for proposals", result.DescriptionDict[EmailTemplateType.InitialCall]);
            Assert.AreEqual("Email sent out to applicant if proposal is approved", result.DescriptionDict[EmailTemplateType.ProposalApproved]);
            Assert.AreEqual("Email sent out to applicant to confirm proposal has been created", result.DescriptionDict[EmailTemplateType.ProposalConfirmation]);
            Assert.AreEqual("Email sent out to applicant to notify them that their proposal has not been accepted", result.DescriptionDict[EmailTemplateType.ProposalDenied]);
            Assert.AreEqual("Email sent to applicant to inform them their proposal has been set back to edit", result.DescriptionDict[EmailTemplateType.ProposalUnsubmitted]);
            Assert.AreEqual("Email sent out to list of reviewers to notify them that the proposals are ready to be reviewed", result.DescriptionDict[EmailTemplateType.ReadyForReview]);
            Assert.AreEqual("Email sent out to list applicants that have not finilized their proposals", result.DescriptionDict[EmailTemplateType.ReminderCallIsAboutToClose]);
            Assert.AreEqual(7, result.EmailTemplateList.Count());
            Assert.AreEqual(8, result.EmailTemplateList.ElementAt(0).Id);
            Assert.AreEqual(9, result.EmailTemplateList.ElementAt(1).Id);
            Assert.AreEqual(10, result.EmailTemplateList.ElementAt(2).Id);
            Assert.AreEqual(11, result.EmailTemplateList.ElementAt(3).Id);
            Assert.AreEqual(12, result.EmailTemplateList.ElementAt(4).Id);
            Assert.AreEqual(13, result.EmailTemplateList.ElementAt(5).Id);
            Assert.AreEqual(14, result.EmailTemplateList.ElementAt(6).Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexReturnsViewWithExpectValues2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 2, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, 2)
                .AssertViewRendered()
                .WithViewData<EmailTemplateListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(7, result.DescriptionDict.Count);
            Assert.AreEqual("Email sent out to list for proposals", result.DescriptionDict[EmailTemplateType.InitialCall]);
            Assert.AreEqual("Email sent out to applicant if proposal is approved", result.DescriptionDict[EmailTemplateType.ProposalApproved]);
            Assert.AreEqual("Email sent out to applicant to confirm proposal has been created", result.DescriptionDict[EmailTemplateType.ProposalConfirmation]);
            Assert.AreEqual("Email sent out to applicant to notify them that their proposal has not been accepted", result.DescriptionDict[EmailTemplateType.ProposalDenied]);
            Assert.AreEqual("Email sent to applicant to inform them their proposal has been set back to edit", result.DescriptionDict[EmailTemplateType.ProposalUnsubmitted]);
            Assert.AreEqual("Email sent out to list of reviewers to notify them that the proposals are ready to be reviewed", result.DescriptionDict[EmailTemplateType.ReadyForReview]);
            Assert.AreEqual("Email sent out to list applicants that have not finilized their proposals", result.DescriptionDict[EmailTemplateType.ReminderCallIsAboutToClose]);
            Assert.AreEqual(0, result.EmailTemplateList.Count());

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(2, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsViewWithExpectValues3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(7, result.DescriptionDict.Count);
            Assert.AreEqual("Email sent out to list for proposals", result.DescriptionDict[EmailTemplateType.InitialCall]);
            Assert.AreEqual("Email sent out to applicant if proposal is approved", result.DescriptionDict[EmailTemplateType.ProposalApproved]);
            Assert.AreEqual("Email sent out to applicant to confirm proposal has been created", result.DescriptionDict[EmailTemplateType.ProposalConfirmation]);
            Assert.AreEqual("Email sent out to applicant to notify them that their proposal has not been accepted", result.DescriptionDict[EmailTemplateType.ProposalDenied]);
            Assert.AreEqual("Email sent to applicant to inform them their proposal has been set back to edit", result.DescriptionDict[EmailTemplateType.ProposalUnsubmitted]);
            Assert.AreEqual("Email sent out to list of reviewers to notify them that the proposals are ready to be reviewed", result.DescriptionDict[EmailTemplateType.ReadyForReview]);
            Assert.AreEqual("Email sent out to list applicants that have not finilized their proposals", result.DescriptionDict[EmailTemplateType.ReminderCallIsAboutToClose]);
            Assert.AreEqual(7, result.EmailTemplateList.Count());
            Assert.AreEqual(1, result.EmailTemplateList.ElementAt(0).Id);
            Assert.AreEqual(2, result.EmailTemplateList.ElementAt(1).Id);
            Assert.AreEqual(3, result.EmailTemplateList.ElementAt(2).Id);
            Assert.AreEqual(4, result.EmailTemplateList.ElementAt(3).Id);
            Assert.AreEqual(5, result.EmailTemplateList.ElementAt(4).Id);
            Assert.AreEqual(6, result.EmailTemplateList.ElementAt(5).Id);
            Assert.AreEqual(7, result.EmailTemplateList.ElementAt(6).Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsViewWithExpectValues4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(1, null, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(1, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(7, result.DescriptionDict.Count);
            Assert.AreEqual("Email sent out to list for proposals", result.DescriptionDict[EmailTemplateType.InitialCall]);
            Assert.AreEqual("Email sent out to applicant if proposal is approved", result.DescriptionDict[EmailTemplateType.ProposalApproved]);
            Assert.AreEqual("Email sent out to applicant to confirm proposal has been created", result.DescriptionDict[EmailTemplateType.ProposalConfirmation]);
            Assert.AreEqual("Email sent out to applicant to notify them that their proposal has not been accepted", result.DescriptionDict[EmailTemplateType.ProposalDenied]);
            Assert.AreEqual("Email sent to applicant to inform them their proposal has been set back to edit", result.DescriptionDict[EmailTemplateType.ProposalUnsubmitted]);
            Assert.AreEqual("Email sent out to list of reviewers to notify them that the proposals are ready to be reviewed", result.DescriptionDict[EmailTemplateType.ReadyForReview]);
            Assert.AreEqual("Email sent out to list applicants that have not finilized their proposals", result.DescriptionDict[EmailTemplateType.ReminderCallIsAboutToClose]);
            Assert.AreEqual(7, result.EmailTemplateList.Count());
            Assert.AreEqual(15, result.EmailTemplateList.ElementAt(0).Id);
            Assert.AreEqual(16, result.EmailTemplateList.ElementAt(1).Id);
            Assert.AreEqual(17, result.EmailTemplateList.ElementAt(2).Id);
            Assert.AreEqual(18, result.EmailTemplateList.ElementAt(3).Id);
            Assert.AreEqual(19, result.EmailTemplateList.ElementAt(4).Id);
            Assert.AreEqual(20, result.EmailTemplateList.ElementAt(5).Id);
            Assert.AreEqual(21, result.EmailTemplateList.ElementAt(6).Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(1, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);
            #endregion Assert
        }
        #endregion Index Tests
    }
}
