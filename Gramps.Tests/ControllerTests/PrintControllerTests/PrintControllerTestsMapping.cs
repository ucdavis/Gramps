using Gramps.Controllers;
using Gramps.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Gramps.Tests.ControllerTests.PrintControllerTests
{
    public partial class PrintControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestProposalAdminMapping()
        {
            "~/Print/ProposalAdmin/5".ShouldMapTo<PrintController>(a => a.ProposalAdmin(5, null, false), true);
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestProposalReviewerMapping()
        {
            "~/Print/ProposalReviewer/5".ShouldMapTo<PrintController>(a => a.ProposalReviewer(5, null), true);
        }        
        #endregion Mapping Tests
    }
}
