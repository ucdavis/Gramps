using Gramps.Controllers;
using Gramps.Tests.Core.Extensions;
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

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/CallForProposal/Create/?templateId=3".ShouldMapTo<CallForProposalController>(a => a.Create(3), true);
        }

        #endregion Mapping Tests
    }
}
