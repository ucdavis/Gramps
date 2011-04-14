using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EmailTemplateControllerTests
{
    public partial class EmailTemplateControllerTests
    {
        #region Edit Tests
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetWithoutAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, null, 1)
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
        public void TestEditGetWithoutAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, 2, null)
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
        public void TestEditGetRedirectsToIndexWhenEmailTemplateNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(22, null, 1)
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(1, result.RouteValues.ElementAt(3).Value);

            //Assert.AreEqual("You do not have access to that.", Controller.Message);

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
        public void TestEditGetRedirectsToIndexWhenEmailTemplateNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(22, 2, null)
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(2, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            //Assert.AreEqual("You do not have access to that.", Controller.Message);

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
        public void TestEditGetWithDiffRedirectsToHome1()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(1).Template,
                            EmailTemplateRepository.GetNullableById(1).CallForProposal, null, 1)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(1, null, 1)
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWitDiffRedirectsToHome2A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(9).Template,
                            EmailTemplateRepository.GetNullableById(9).CallForProposal, null, 1)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(9, 2, null)
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);            
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetWitDiffRedirectsToHome2B()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(20).Template,
                            EmailTemplateRepository.GetNullableById(20).CallForProposal, 2, null)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(20, 2, null)
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(1, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);            
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData1()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(8).Template,
                            EmailTemplateRepository.GetNullableById(8).CallForProposal, null, 1)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(8, null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.InitialCall, result.EmailTemplate.TemplateType);
            Assert.AreEqual(3, result.Tokens.Count);
            Assert.AreEqual("ProposalMaximum", result.Tokens[0]);
            Assert.AreEqual("CloseDate", result.Tokens[1]);
            Assert.AreEqual("CreateProposalLink", result.Tokens[2]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert	
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData2()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(9).Template,
                            EmailTemplateRepository.GetNullableById(9).CallForProposal, null, 1)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(9, null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalApproved, result.EmailTemplate.TemplateType);
            Assert.AreEqual(2, result.Tokens.Count);
            Assert.AreEqual("ApprovedAmount", result.Tokens[0]);
            Assert.AreEqual("ProposalLink", result.Tokens[1]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData3()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(10).Template,
                            EmailTemplateRepository.GetNullableById(10).CallForProposal, null, 1)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(10, null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalConfirmation, result.EmailTemplate.TemplateType);
            Assert.AreEqual(1, result.Tokens.Count);
            Assert.AreEqual("CloseDate", result.Tokens[0]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData4()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(11).Template,
                            EmailTemplateRepository.GetNullableById(11).CallForProposal, null, 1)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(11, null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(11, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalDenied, result.EmailTemplate.TemplateType);
            Assert.AreEqual(1, result.Tokens.Count);
            Assert.AreEqual("ProposalLink", result.Tokens[0]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData5()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(12).Template,
                            EmailTemplateRepository.GetNullableById(12).CallForProposal, null, 1)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(12, null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(12, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalUnsubmitted, result.EmailTemplate.TemplateType);
            Assert.AreEqual(2, result.Tokens.Count);
            Assert.AreEqual("CloseDate", result.Tokens[0]);
            Assert.AreEqual("ProposalLink", result.Tokens[1]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData6()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(13).Template,
                            EmailTemplateRepository.GetNullableById(13).CallForProposal, null, 1)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(13, null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(13, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ReadyForReview, result.EmailTemplate.TemplateType);
            Assert.AreEqual(1, result.Tokens.Count);
            Assert.AreEqual("ReviewerName", result.Tokens[0]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData7()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(14).Template,
                            EmailTemplateRepository.GetNullableById(14).CallForProposal, null, 1)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(14, null, 1)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(14, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ReminderCallIsAboutToClose, result.EmailTemplate.TemplateType);
            Assert.AreEqual(2, result.Tokens.Count);
            Assert.AreEqual("CloseDate", result.Tokens[0]);
            Assert.AreEqual("ProposalLink", result.Tokens[1]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData1A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(1).Template,
                            EmailTemplateRepository.GetNullableById(1).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, 2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.InitialCall, result.EmailTemplate.TemplateType);
            Assert.AreEqual(3, result.Tokens.Count);
            Assert.AreEqual("ProposalMaximum", result.Tokens[0]);
            Assert.AreEqual("CloseDate", result.Tokens[1]);
            Assert.AreEqual("CreateProposalLink", result.Tokens[2]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert	
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData2A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(2).Template,
                            EmailTemplateRepository.GetNullableById(2).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, 2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalApproved, result.EmailTemplate.TemplateType);
            Assert.AreEqual(2, result.Tokens.Count);
            Assert.AreEqual("ApprovedAmount", result.Tokens[0]);
            Assert.AreEqual("ProposalLink", result.Tokens[1]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData3A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(3).Template,
                            EmailTemplateRepository.GetNullableById(3).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, 2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalConfirmation, result.EmailTemplate.TemplateType);
            Assert.AreEqual(1, result.Tokens.Count);
            Assert.AreEqual("CloseDate", result.Tokens[0]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData4A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(4).Template,
                            EmailTemplateRepository.GetNullableById(4).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(4, 2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalDenied, result.EmailTemplate.TemplateType);
            Assert.AreEqual(1, result.Tokens.Count);
            Assert.AreEqual("ProposalLink", result.Tokens[0]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData5A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(5).Template,
                            EmailTemplateRepository.GetNullableById(5).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(5, 2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ProposalUnsubmitted, result.EmailTemplate.TemplateType);
            Assert.AreEqual(2, result.Tokens.Count);
            Assert.AreEqual("CloseDate", result.Tokens[0]);
            Assert.AreEqual("ProposalLink", result.Tokens[1]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData6A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(6).Template,
                            EmailTemplateRepository.GetNullableById(6).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(6, 2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ReadyForReview, result.EmailTemplate.TemplateType);
            Assert.AreEqual(1, result.Tokens.Count);
            Assert.AreEqual("ReviewerName", result.Tokens[0]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditGetReturnsViewWithExpectedData7A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(7).Template,
                            EmailTemplateRepository.GetNullableById(7).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(7, 2, null)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.ReminderCallIsAboutToClose, result.EmailTemplate.TemplateType);
            Assert.AreEqual(2, result.Tokens.Count);
            Assert.AreEqual("CloseDate", result.Tokens[0]);
            Assert.AreEqual("ProposalLink", result.Tokens[1]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }
        #endregion Edit Get Tests
        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostWithoutAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, null, 1, new EmailTemplate())
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
        public void TestEditPostWithoutAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Edit(5, 2, null, new EmailTemplate())
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
        public void TestEditPostRedirectsToIndexWhenEmailTemplateNotFound1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(22, null, 1, new EmailTemplate())
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(1, result.RouteValues.ElementAt(3).Value);

            //Assert.AreEqual("You do not have access to that.", Controller.Message);

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
        public void TestEditPostRedirectsToIndexWhenEmailTemplateNotFound2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            SetupTestData1();
            #endregion Arrange

            #region Act
            var result = Controller.Edit(22, 2, null, new EmailTemplate())
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(2, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            //Assert.AreEqual("You do not have access to that.", Controller.Message);

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
        public void TestEditPostWithDiffRedirectsToHome1()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(1).Template,
                            EmailTemplateRepository.GetNullableById(1).CallForProposal, null, 1)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(1, null, 1, new EmailTemplate())
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWitDiffRedirectsToHome2A()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(9).Template,
                            EmailTemplateRepository.GetNullableById(9).CallForProposal, null, 1)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(9, 2, null, new EmailTemplate())
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWitDiffRedirectsToHome2B()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(20).Template,
                            EmailTemplateRepository.GetNullableById(20).CallForProposal, 2, null)).Return(false).Repeat.
                Any();
            #endregion Arrange

            #region Act
            Controller.Edit(20, 2, null, new EmailTemplate())
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

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(1, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);


            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView1()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(8).Template,
                            EmailTemplateRepository.GetNullableById(8).CallForProposal, null, 1)).Return(true).Repeat.
                Any();

            var newTemplate = CreateValidEntities.EmailTemplate(1);
            newTemplate.Subject = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(8, null, 1, newTemplate)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            //Assert.AreEqual("", Controller.Message);
            Controller.ModelState.AssertErrorsAre("Subject: may not be null or empty");

            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.InitialCall, result.EmailTemplate.TemplateType);
            Assert.AreEqual(3, result.Tokens.Count);
            Assert.AreEqual("ProposalMaximum", result.Tokens[0]);
            Assert.AreEqual("CloseDate", result.Tokens[1]);
            Assert.AreEqual("CreateProposalLink", result.Tokens[2]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithInvalidDataReturnsView2()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(1).Template,
                            EmailTemplateRepository.GetNullableById(1).CallForProposal, 2, null)).Return(true).Repeat.
                Any();
            var newTemplate = CreateValidEntities.EmailTemplate(1);
            newTemplate.Subject = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(1, 2, null, newTemplate)
                .AssertViewRendered()
                .WithViewData<EmailTemplateViewModel>();
            #endregion Act

            #region Assert
            Controller.ModelState.AssertErrorsAre("Subject: may not be null or empty");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.EmailTemplate.Id);
            Assert.AreEqual(EmailTemplateType.InitialCall, result.EmailTemplate.TemplateType);
            Assert.AreEqual(3, result.Tokens.Count);
            Assert.AreEqual("ProposalMaximum", result.Tokens[0]);
            Assert.AreEqual("CloseDate", result.Tokens[1]);
            Assert.AreEqual("CreateProposalLink", result.Tokens[2]);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithValidDataRedirects1()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 1, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(8).Template,
                            EmailTemplateRepository.GetNullableById(8).CallForProposal, null, 1)).Return(true).Repeat.
                Any();

            var newTemplate = new EmailTemplate();
            newTemplate.Subject = "New Subject";
            newTemplate.Text = "New Text";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(8, null, 1, newTemplate)
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Email Template Edited Successfully.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(1, result.RouteValues.ElementAt(3).Value);

            EmailTemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailTemplate>.Is.Anything));
            var args = (EmailTemplate) EmailTemplateRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailTemplate>.Is.Anything))[0][0];
            Assert.AreEqual("New Subject", args.Subject);
            Assert.AreEqual("New Text", args.Text);
            Assert.AreEqual(EmailTemplateType.InitialCall, args.TemplateType);
            Assert.AreEqual(null, args.Template);
            Assert.AreEqual(1, args.CallForProposal.Id);
            Assert.AreEqual(8, args.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, hasAccessArgs[0]);
            Assert.AreEqual(1, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(null, hasSameIdArgs[0]);
            Assert.AreEqual(1, ((CallForProposal)hasSameIdArgs[1]).Id);
            Assert.AreEqual(null, hasSameIdArgs[2]);
            Assert.AreEqual(1, hasSameIdArgs[3]);
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostWithValidDataRedirects2()
        {
            #region Arrange
            SetupTestData1();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(2, null, "Me")).Return(true).Repeat.Any();
            AccessService.Expect(
                a =>
                a.HasSameId(EmailTemplateRepository.GetNullableById(3).Template,
                            EmailTemplateRepository.GetNullableById(3).CallForProposal, 2, null)).Return(true).Repeat.
                Any();

            var newTemplate = new EmailTemplate();
            newTemplate.Subject = "New Subject";
            newTemplate.Text = "New Text";
            #endregion Arrange

            #region Act
            var result = Controller.Edit(3, 2, null, newTemplate)
                .AssertActionRedirect()
                .ToAction<EmailTemplateController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("Email Template Edited Successfully.", Controller.Message);

            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(2, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            EmailTemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailTemplate>.Is.Anything));
            var args = (EmailTemplate)EmailTemplateRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailTemplate>.Is.Anything))[0][0];
            Assert.AreEqual("New Subject", args.Subject);
            Assert.AreEqual("New Text", args.Text);
            Assert.AreEqual(EmailTemplateType.ProposalConfirmation, args.TemplateType);
            Assert.AreEqual(2, args.Template.Id);
            Assert.AreEqual(null, args.CallForProposal);
            Assert.AreEqual(3, args.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var hasAccessArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(2, hasAccessArgs[0]);
            Assert.AreEqual(null, hasAccessArgs[1]);
            Assert.AreEqual("Me", hasAccessArgs[2]);

            AccessService.AssertWasCalled(a => a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything, Arg<int?>.Is.Anything));
            var hasSameIdArgs =
                AccessService.GetArgumentsForCallsMadeOn(
                    a =>
                    a.HasSameId(Arg<Template>.Is.Anything, Arg<CallForProposal>.Is.Anything, Arg<int?>.Is.Anything,
                                Arg<int?>.Is.Anything))[0];
            Assert.AreEqual(2, ((Template)hasSameIdArgs[0]).Id);
            Assert.AreEqual(null, hasSameIdArgs[1]);
            Assert.AreEqual(2, hasSameIdArgs[2]);
            Assert.AreEqual(null, hasSameIdArgs[3]);
            #endregion Assert
        }
        #endregion Edit Post Tests
        #endregion Edit Tests
    }
}
