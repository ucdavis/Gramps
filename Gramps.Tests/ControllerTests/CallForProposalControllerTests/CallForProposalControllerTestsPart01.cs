using System;
using System.Linq;
using Gramps.Controllers.ViewModels;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace Gramps.Tests.ControllerTests.CallForProposalControllerTests
{
    public partial class CallForProposalControllerTests
    {
        #region Index Tests

        [TestMethod]
        public void TestIndexReturnsView1()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.CallForProposals.Count());
            Assert.IsNull(result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name1", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(2).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(3).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView2()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotMe");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposals.Count());
            Assert.IsNull(result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name3", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView3()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "NotAnyone");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.CallForProposals.Count());
            Assert.IsNull(result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView4()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index("Active", null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposals.Count());
            Assert.AreEqual("Active", result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name1", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView5()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index("InActive", null, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CallForProposals.Count());
            Assert.AreEqual("InActive", result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.IsNull(result.FilterStartCreate);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(0).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView6()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, new DateTime(2011, 03, 29).Date, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CallForProposals.Count());
            Assert.AreEqual(null, result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.AreEqual(new DateTime(2011, 03, 29).Date, result.FilterStartCreate);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(1).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(2).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView7()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index("Active", new DateTime(2011, 03, 29).Date, null)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.CallForProposals.Count());
            Assert.AreEqual("Active", result.FilterActive);
            Assert.IsNull(result.FilterEndCreate);
            Assert.AreEqual(new DateTime(2011, 03, 29).Date, result.FilterStartCreate);
            Assert.AreEqual("Name4", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name5", result.CallForProposals.ElementAt(1).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView8()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, new DateTime(2011, 03, 29).Date, new DateTime(2011, 03, 31).Date)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CallForProposals.Count());
            Assert.AreEqual(null, result.FilterActive);
            Assert.AreEqual(new DateTime(2011, 03, 31).Date, result.FilterEndCreate);
            Assert.AreEqual(new DateTime(2011, 03, 29).Date, result.FilterStartCreate);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(0).Name);
            #endregion Assert
        }

        [TestMethod]
        public void TestIndexReturnsView9()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "Me");
            SetupDataForTests();
            #endregion Arrange

            #region Act
            var result = Controller.Index(null, null, new DateTime(2011, 03, 31).Date)
                .AssertViewRendered()
                .WithViewData<CallForProposalListViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNull(Controller.Message);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.CallForProposals.Count());
            Assert.AreEqual(null, result.FilterActive);
            Assert.AreEqual(new DateTime(2011, 03, 31).Date, result.FilterEndCreate);
            Assert.AreEqual(null, result.FilterStartCreate);
            Assert.AreEqual("Name1", result.CallForProposals.ElementAt(0).Name);
            Assert.AreEqual("Name2", result.CallForProposals.ElementAt(1).Name);
            #endregion Assert
        }
        #endregion Index Tests
    }
}
