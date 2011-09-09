using System.Linq;
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
        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            SetupData();
            #endregion Arrange

            #region Act
            var result = Controller.Index()
                .AssertViewRendered()
                .WithViewData<TemplateListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Templates.Count());
            Assert.AreEqual("Name2", result.Templates.ElementAt(0).Name);
            #endregion Assert		
        }

        #endregion Index Tests

        #region Create Get Tests

        [TestMethod]
        public void TestCreateGetReturnView()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            var result = Controller.Create()
                .AssertViewRendered()
                .WithViewData<TemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Template);
            Assert.IsTrue(result.IsTemplate);
            Assert.IsFalse(result.IsCallForProposal);
            #endregion Assert		
        } 
        #endregion Create Get Tests

        #region Create Post Tests

        [TestMethod]
        public void TestCreatePostWithInvalidDataReturnsView()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] {""}, "Me");
            var templateToCreate = CreateValidEntities.Template(9);
            templateToCreate.Name = null;
            #endregion Arrange

            #region Act
            var result = Controller.Create(templateToCreate)
                .AssertViewRendered()
                .WithViewData<TemplateViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.Template.Name);
            Controller.ModelState.AssertErrorsAre("Name: may not be null or empty");
            TemplateRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            #endregion Assert		
        }


        [TestMethod]
        public void TestCreatePostRedirectsAndSaves()
        {
            #region Arrange
            SetupData();
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            var templateToCreate = CreateValidEntities.Template(9);
            #endregion Arrange

            #region Act
            Controller.Create(templateToCreate)
                .AssertActionRedirect()
                .ToAction<TemplateController>(a => a.Index());
            #endregion Act

            #region Assert
            TemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Template>.Is.Anything));
            var argsTemplate = (Template) TemplateRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Template>.Is.Anything))[0][0];
            Assert.IsNotNull(argsTemplate);
            Assert.AreEqual(false, argsTemplate.IsActive);
            Assert.AreEqual("Name9", argsTemplate.Name);

            EditorRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<Editor>.Is.Anything));
            var argsEditor = (Editor) EditorRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<Editor>.Is.Anything))[0][0];
            Assert.IsNotNull(argsEditor);
            Assert.AreEqual("Me", argsEditor.User.LoginId);
            Assert.AreEqual("Name9", argsEditor.Template.Name);

            EmailTemplateRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<EmailTemplate>.Is.Anything), x => x.Repeat.Times(7));
            var argsEmailTemplate = EmailTemplateRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<EmailTemplate>.Is.Anything)); 
            Assert.IsNotNull(argsEmailTemplate);
            Assert.AreEqual(7, argsEmailTemplate.Count);
            Assert.AreEqual(EmailTemplateType.InitialCall, ((EmailTemplate)argsEmailTemplate[0][0]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ProposalApproved, ((EmailTemplate)argsEmailTemplate[1][0]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ProposalConfirmation, ((EmailTemplate)argsEmailTemplate[2][0]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ProposalDenied, ((EmailTemplate)argsEmailTemplate[3][0]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ReadyForReview, ((EmailTemplate)argsEmailTemplate[4][0]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ReminderCallIsAboutToClose, ((EmailTemplate)argsEmailTemplate[5][0]).TemplateType);
            Assert.AreEqual(EmailTemplateType.ProposalUnsubmitted, ((EmailTemplate)argsEmailTemplate[6][0]).TemplateType);            
            #endregion Assert		
        }
        #endregion Create Post Tests
    }
}
