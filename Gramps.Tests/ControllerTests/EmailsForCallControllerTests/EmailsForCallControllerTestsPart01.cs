using System.Linq;
using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexWithNoAccessRedirectsToHome1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(null, 3)
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
        public void TestIndexWithNoAccessRedirectsToHome2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(false).Repeat.Any();
            #endregion Arrange

            #region Act
            Controller.Index(4, null)
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
        public void TestIndexWithAccessReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(null, 3, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, 3)
                .AssertViewRendered()
                .WithViewData<EmailsForCallListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCallForProposal);
            Assert.AreEqual(3, result.CallForProposalId);
            Assert.IsFalse(result.IsTemplate);
            Assert.AreEqual(0, result.TemplateId);
            Assert.AreEqual(3, result.EmailsForCallList.Count());
            Assert.AreEqual(3, result.EmailsForCallList.ElementAt(0).Id);
            Assert.AreEqual(4, result.EmailsForCallList.ElementAt(1).Id);
            Assert.AreEqual(5, result.EmailsForCallList.ElementAt(2).Id);


            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(null, args[0]);
            Assert.AreEqual(3, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert		
        }

        [TestMethod]
        public void TestIndexWithAccessReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(4, null, "Me")).Return(true).Repeat.Any();
            SetupDataForTests1();
            #endregion Arrange

            #region Act
            var result = Controller.Index(4, null)
                .AssertViewRendered()
                .WithViewData<EmailsForCallListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsCallForProposal);
            Assert.AreEqual(0, result.CallForProposalId);
            Assert.IsTrue(result.IsTemplate);
            Assert.AreEqual(4, result.TemplateId);
            Assert.AreEqual(3, result.EmailsForCallList.Count());
            Assert.AreEqual(8, result.EmailsForCallList.ElementAt(0).Id);
            Assert.AreEqual(9, result.EmailsForCallList.ElementAt(1).Id);
            Assert.AreEqual(10, result.EmailsForCallList.ElementAt(2).Id);


            AccessService.AssertWasCalled(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything));
            var args = AccessService.GetArgumentsForCallsMadeOn(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything))[0];
            Assert.AreEqual(4, args[0]);
            Assert.AreEqual(null, args[1]);
            Assert.AreEqual("Me", args[2]);
            #endregion Assert
        }

        #endregion Index Tests
    }
}
