using Gramps.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace Gramps.Tests.ControllerTests.CallForProposalControllerTests
{
    public partial class CallForProposalControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/CallForProposal/Index/".ShouldMapTo<CallForProposalController>(a => a.Index(null, null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestLaunchMapping()
        {
            "~/CallForProposal/Launch/5".ShouldMapTo<CallForProposalController>(a => a.Launch(5));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/CallForProposal/Create".ShouldMapTo<CallForProposalController>(a => a.Create());
        }

        #endregion Mapping Tests
    }
}
