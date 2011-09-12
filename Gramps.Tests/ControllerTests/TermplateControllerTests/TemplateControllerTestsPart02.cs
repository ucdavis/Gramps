using Gramps.Controllers;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace Gramps.Tests.ControllerTests.TermplateControllerTests
{
    public partial class TemplateControllerTests
    {
        #region Edit Get Tests

        [TestMethod]
        public void TestEditGetRedirectsWhenTemplateNotFound()
        {
            #region Arrange
            //SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            #endregion Arrange

            #region Act
            Controller.Edit(4)
                .AssertActionRedirect()
                .ToAction<TemplateController>(a => a.Index());
            #endregion Act

            #region Assert
            #endregion Assert		
        }


        [TestMethod]
        public void TestEditGetRedirectsWhenNoAccess()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Edit(2)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "Me"));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditGetReturnsView()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2)
                .AssertViewRendered()
                .WithViewData<TemplateViewModel>();
            #endregion Act

            #region Assert        
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "Me"));
            Assert.IsNotNull(result);
            Assert.AreEqual("Name2", result.Template.Name);
            Assert.AreEqual(2, result.TemplateId);
            Assert.IsNull(result.CallForProposalId);
            #endregion Assert
        }
        #endregion Edit Get Tests

        #region Edit Post Tests
        [TestMethod]
        public void TestEditPostRedirectsWhenTemplateNotFound()
        {
            #region Arrange
            //SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            #endregion Arrange

            #region Act
            Controller.Edit(4, new Template())
                .AssertActionRedirect()
                .ToAction<TemplateController>(a => a.Index());
            #endregion Act

            #region Assert
            TemplateRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            #endregion Assert
        }


        [TestMethod]
        public void TestEditPostRedirectsWhenNoAccess()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(false);
            #endregion Arrange

            #region Act
            Controller.Edit(2, new Template())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
            #endregion Act

            #region Assert
            Assert.AreEqual("You do not have access to that.", Controller.Message);
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "Me"));
            TemplateRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenInvalid()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var templateToEdit = CreateValidEntities.Template(9);
            templateToEdit.Name = "x".RepeatTimes(101);
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, templateToEdit)
                .AssertViewRendered()
                .WithViewData<TemplateViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "Me"));
            Controller.ModelState.AssertErrorsAre("Name: length must be between 0 and 100");
            Assert.IsNotNull(result);
            Assert.AreEqual("x".RepeatTimes(101), result.Template.Name);
            Assert.AreEqual(2, result.TemplateId);
            Assert.IsNull(result.CallForProposalId);
            TemplateRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenValid1()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var templateToEdit = CreateValidEntities.Template(9);
            templateToEdit.IsActive = true;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, templateToEdit)
                .AssertViewRendered()
                .WithViewData<TemplateViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "Me"));
            Assert.AreEqual("Template Edited Successfully", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.Template.Name);
            Assert.AreEqual(true, result.Template.IsActive);
            Assert.AreEqual(2, result.TemplateId);
            Assert.IsNull(result.CallForProposalId);
            TemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            #endregion Assert
        }

        [TestMethod]
        public void TestEditPostReturnsViewWhenValid2()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            AccessService.Expect(a => a.HasAccess(Arg<int?>.Is.Anything, Arg<int?>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            var templateToEdit = CreateValidEntities.Template(9);
            templateToEdit.IsActive = false;
            #endregion Arrange

            #region Act
            var result = Controller.Edit(2, templateToEdit)
                .AssertViewRendered()
                .WithViewData<TemplateViewModel>();
            #endregion Act

            #region Assert
            AccessService.AssertWasCalled(a => a.HasAccess(2, null, "Me"));
            Assert.AreEqual("Template Edited Successfully", Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual("Name9", result.Template.Name);
            Assert.AreEqual(false, result.Template.IsActive);
            Assert.AreEqual(2, result.TemplateId);
            Assert.IsNull(result.CallForProposalId);
            TemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            #endregion Assert
        }

        #endregion Edit Post Tests
    }
}
