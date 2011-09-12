using System;
using System.Collections.Generic;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace Gramps.Tests.ServiceTests
{
    [TestClass]
    public class AccessServiceTests
    {
        #region Init         
        public IRepository Repository { get; set; }
        public IAccessService AccessService { get; set; }
        public IRepository<Template> TemplateRepository { get; set; }
        public IRepository<CallForProposal> CallForProposalRepository { get; set; } 

        public AccessServiceTests()
        {
            Repository = MockRepository.GenerateStub<IRepository>();


            CallForProposalRepository = MockRepository.GenerateStub<IRepository<CallForProposal>>();
            Repository.Expect(a => a.OfType<CallForProposal>()).Return(CallForProposalRepository);

            TemplateRepository = MockRepository.GenerateStub<IRepository<Template>>();
            Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository);

            AccessService = new AccessService(Repository);
        }

        #endregion Init

        #region HasAccess Tests

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestHasActionThrowsExceptionIfTemplateNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                SetupData();               
                #endregion Arrange

                #region Act
                thisFar = true;
                AccessService.HasAccess(4, null, "Me");
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Template is required", ex.Message);
                throw;
            }	
        }


        [TestMethod]
        public void TestHasAccessReturnsTrueIfTemplateHasMatchingEditor()
        {
            #region Arrange
            SetupData();
            #endregion Arrange

            #region Act
            var result = AccessService.HasAccess(2, null, "LoginId2");
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestHasAccessReturnsFalseIfTemplateHasNoMatchingEditor()
        {
            #region Arrange
            SetupData();
            #endregion Arrange

            #region Act
            var result = AccessService.HasAccess(2, null, "LoginId4");
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestHasActionThrowsExceptionIfCallNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                SetupData();
                #endregion Arrange

                #region Act
                thisFar = true;
                AccessService.HasAccess(null, 4, "Me");
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("CallForProposal is required", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestHasAccessReturnsTrueIfCallHasMatchingEditor()
        {
            #region Arrange
            SetupData();
            #endregion Arrange

            #region Act
            var result = AccessService.HasAccess(null, 2, "LoginId2");
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestHasAccessReturnsFalseIfCallHasNoMatchingEditor()
        {
            #region Arrange
            SetupData();
            #endregion Arrange

            #region Act
            var result = AccessService.HasAccess(null, 2, "LoginId4");
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }


        [TestMethod]
        public void TestHasAccessReturnsFalseIfBothNull()
        {
            #region Arrange
            SetupData();
            #endregion Arrange

            #region Act
            var result = AccessService.HasAccess(null, null, "LoginId4");
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestHasAccessReturnsFalseIfBothZero()
        {
            #region Arrange
            SetupData();
            #endregion Arrange

            #region Act
            var result = AccessService.HasAccess(0, 0, "LoginId4");
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }
        #endregion HasAccess Tests

        #region HasSameId Tests

        [TestMethod]
        public void TestHasSameIdReturnsFalseIfTemplateDoesNotMatch1()
        {
            #region Arrange
            var template = CreateValidEntities.Template(1);
            template.SetIdTo(1);
            #endregion Arrange

            #region Act
            var result = AccessService.HasSameId(template, null, 2, null);
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestHasSameIdReturnsFalseIfTemplateDoesNotMatch2()
        {
            #region Arrange
            var template = CreateValidEntities.Template(1);
            template.SetIdTo(1);
            #endregion Arrange

            #region Act
            var result = AccessService.HasSameId(template, null, null, null);
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestHasSameIdReturnsFalseIfCallDoesNotMatch1()
        {
            #region Arrange
            var call = CreateValidEntities.CallForProposal(1);
            call.SetIdTo(1);
            #endregion Arrange

            #region Act
            var result = AccessService.HasSameId(null, call, null, 2);
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestHasSameIdReturnsFalseIfCallDoesNotMatch2()
        {
            #region Arrange
            var call = CreateValidEntities.CallForProposal(1);
            call.SetIdTo(1);
            #endregion Arrange

            #region Act
            var result = AccessService.HasSameId(null, call, null, null);
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestHasSameIdReturnsFalseIfCallAndTemplateAreNull()
        {
            #region Arrange
            #endregion Arrange

            #region Act
            var result = AccessService.HasSameId(null, null, 1, 1);
            #endregion Act

            #region Assert
            Assert.IsFalse(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestHasSameIdReturnsTrueIfTemplateMatches()
        {
            #region Arrange
            var template = CreateValidEntities.Template(1);
            template.SetIdTo(1);
            #endregion Arrange

            #region Act
            var result = AccessService.HasSameId(template, null, 1, null);
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            #endregion Assert
        }

        [TestMethod]
        public void TestHasSameIdReturnsTrueIfCallMatches()
        {
            #region Arrange
            var call = CreateValidEntities.CallForProposal(1);
            call.SetIdTo(1);
            #endregion Arrange

            #region Act
            var result = AccessService.HasSameId(null, call, null, 1);
            #endregion Act

            #region Assert
            Assert.IsTrue(result);
            #endregion Assert
        }
        #endregion HasSameId Tests

        #region Helpers
        public void SetupData()
        {
            var users = new List<User>();
            var editors = new List<Editor>();
            for (int i = 0; i < 3; i++)
            {
                users.Add(CreateValidEntities.User(i+1));
                editors.Add(CreateValidEntities.Editor(i+1));
                editors[i].User = users[i];
            }
            
            var templates = new List<Template>();
            for (int i = 0; i < 3; i++)
            {
                templates.Add( CreateValidEntities.Template(i+1));
            }
            templates[1].Editors = editors;

            var fakeTemplates = new FakeTemplates();
            fakeTemplates.Records(0, TemplateRepository, templates);

            var calls = new List<CallForProposal>();
            for(int i = 0; i < 3; i++)
            {
                calls.Add(CreateValidEntities.CallForProposal(i + 1));
            }
            calls[1].Editors = editors;

            var fakeCalls = new FakeCallForProposals();
            fakeCalls.Records(0, CallForProposalRepository, calls);
        }
        #endregion Helpers
    }
}
