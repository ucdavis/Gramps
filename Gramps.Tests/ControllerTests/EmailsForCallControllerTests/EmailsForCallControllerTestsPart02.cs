using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region BulkCreate Tests
        #region BulkCreate Get Tests
        [TestMethod]
        public void TestBulkCreateGetWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.BulkCreate(null, 3)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreateGetWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.BulkCreate(4, null)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreateGetWithAccessReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(null, 3)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(0, result.TemplateId);
            Assert.AreEqual(null, result.BulkLoadEmails);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreateGetWithAccessReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(4, null)
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(4, result.TemplateId);
            Assert.AreEqual(null, result.BulkLoadEmails);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }
        #endregion BulkCreate Get Tests
        #region BulkCreate Post Tests

        [TestMethod]
        public void TestBulkCreatePostWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.BulkCreate(null, 3, "test@testy.com")
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreatePostWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.BulkCreate(4, null, "test@testy.com")
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreatePostReturnsViewWhenErrors1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(null, 3, "test@testy.com, test@testy.com")
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("1 EmailsForCall Created Successfully == 1 EmailsForCall Not Created", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("test@testy.com  == Email already exists in list\r\n", result.BulkLoadEmails);

            Controller.ModelState.AssertErrorsAre(); //They are cleared out.

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args1 = (EmailsForCall) EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0];
            Assert.AreEqual("test@testy.com", args1.Email);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreatePostReturnsViewWhenErrors2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(null, 3, "Lee, Bob; Mefirst, Move <test3@testy.com>; OU Bill, Thomas <test1@testy.com>; test@tester.notvalid; test4@TESTy.com; bill@bob.COM")
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("2 EmailsForCall Created Successfully == 2 EmailsForCall Not Created", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("test3@testy.com  == Email already exists in list\r\ntest4@testy.com  == Email already exists in list\r\n", result.BulkLoadEmails);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.AreEqual(0, result.TemplateId);

            Controller.ModelState.AssertErrorsAre(); //They are cleared out.

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            Assert.AreEqual(2, args1.Count());
            Assert.AreEqual("test1@testy.com", ((EmailsForCall)args1[0][0]).Email);
            Assert.AreEqual(3, ((EmailsForCall)args1[0][0]).CallForProposal.Id);
            Assert.AreEqual(null, ((EmailsForCall)args1[0][0]).Template);
            Assert.AreEqual("bill@bob.com", ((EmailsForCall)args1[1][0]).Email);
            Assert.AreEqual(3, ((EmailsForCall)args1[1][0]).CallForProposal.Id);
            Assert.AreEqual(null, ((EmailsForCall)args1[1][0]).Template);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreatePostReturnsViewWhenErrors3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(4, null, "test@testy.com, test@testy.com")
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("1 EmailsForCall Created Successfully == 1 EmailsForCall Not Created", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("test@testy.com  == Email already exists in list\r\n", result.BulkLoadEmails);

            Controller.ModelState.AssertErrorsAre(); //They are cleared out.

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            var args1 = (EmailsForCall)EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything))[0][0];
            Assert.AreEqual("test@testy.com", args1.Email);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreatePostReturnsViewWhenErrors4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(4, null, "Lee, Bob; Mefirst, Move <test7@testy.com>; OU Bill, Thomas <test1@testy.com>; test@tester.notvalid; test10@TESTy.com; bill@bob.COM")
                .AssertViewRendered()
                .WithViewData<EmailsForCallViewModel>();
            #endregion Act

            #region Assert
            Assert.AreEqual("3 EmailsForCall Created Successfully == 1 EmailsForCall Not Created", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("test10@testy.com  == Email already exists in list\r\n", result.BulkLoadEmails);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.AreEqual(4, result.TemplateId);

            Controller.ModelState.AssertErrorsAre(); //They are cleared out.

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything), x => x.Repeat.Times(3));
            var args1 = EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            Assert.AreEqual(3, args1.Count());
            Assert.AreEqual("test7@testy.com", ((EmailsForCall)args1[0][0]).Email);
            Assert.AreEqual(null, ((EmailsForCall)args1[0][0]).CallForProposal);
            Assert.AreEqual(4, ((EmailsForCall)args1[0][0]).Template.Id);

            Assert.AreEqual("test1@testy.com", ((EmailsForCall)args1[1][0]).Email);
            Assert.AreEqual(null, ((EmailsForCall)args1[1][0]).CallForProposal);
            Assert.AreEqual(4, ((EmailsForCall)args1[1][0]).Template.Id);

            Assert.AreEqual("bill@bob.com", ((EmailsForCall)args1[2][0]).Email);
            Assert.AreEqual(null, ((EmailsForCall)args1[2][0]).CallForProposal);
            Assert.AreEqual(4, ((EmailsForCall)args1[2][0]).Template.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreatePostRedirectsWhenNoErrors1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(null, 3, "test99@testy.com, test98@testy.com")
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("2 EmailsForCall Created Successfully", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(3, result.RouteValues.ElementAt(3).Value);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything), x=> x.Repeat.Times(2));
            var args1 = EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            Assert.AreEqual(2, args1.Count());
            Assert.AreEqual("test99@testy.com", ((EmailsForCall)args1[0][0]).Email);
            Assert.AreEqual(3, ((EmailsForCall)args1[0][0]).CallForProposal.Id);
            Assert.AreEqual(null, ((EmailsForCall)args1[0][0]).Template);

            Assert.AreEqual("test98@testy.com", ((EmailsForCall)args1[1][0]).Email);
            Assert.AreEqual(3, ((EmailsForCall)args1[1][0]).CallForProposal.Id);
            Assert.AreEqual(null, ((EmailsForCall)args1[1][0]).Template);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        [TestMethod]
        public void TestBulkCreatePostRedirectsWhenNoErrors2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.BulkCreate(4, null, "test99@testy.com, test98@testy.com")
                .AssertActionRedirect()
                .ToAction<EmailsForCallController>(a => a.Index(null, null));
            #endregion Act

            #region Assert
            Assert.AreEqual("2 EmailsForCall Created Successfully", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("templateId", result.RouteValues.ElementAt(2).Key);
            Assert.AreEqual(4, result.RouteValues.ElementAt(2).Value);
            Assert.AreEqual("callForProposalId", result.RouteValues.ElementAt(3).Key);
            Assert.AreEqual(null, result.RouteValues.ElementAt(3).Value);

            EmailsForCallRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything), x => x.Repeat.Times(2));
            var args1 = EmailsForCallRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailsForCall>.Is.Anything));
            Assert.AreEqual(2, args1.Count());
            Assert.AreEqual("test99@testy.com", ((EmailsForCall)args1[0][0]).Email);
            Assert.AreEqual(null, ((EmailsForCall)args1[0][0]).CallForProposal);
            Assert.AreEqual(4, ((EmailsForCall)args1[0][0]).Template.Id);

            Assert.AreEqual("test98@testy.com", ((EmailsForCall)args1[1][0]).Email);
            Assert.AreEqual(null, ((EmailsForCall)args1[1][0]).CallForProposal);
            Assert.AreEqual(4, ((EmailsForCall)args1[1][0]).Template.Id);

            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        #endregion BulkCreate Post Tests
        #endregion BulkCreate Tests
    }
}
