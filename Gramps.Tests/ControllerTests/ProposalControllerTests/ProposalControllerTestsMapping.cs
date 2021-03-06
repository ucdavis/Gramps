﻿using Gramps.Controllers;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace Gramps.Tests.ControllerTests.ProposalControllerTests
{
    public partial class ProposalControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Proposal/Index/".ShouldMapTo<ProposalController>(a => a.Index());
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Proposal/".ShouldMapTo<ProposalController>(a => a.Index());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestAdminIndexMapping()
        {
            "~/Proposal/AdminIndex/5".ShouldMapTo<ProposalController>(a => a.AdminIndex(5,null, null, null, null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestAdminDetailsMapping()
        {
            "~/Proposal/AdminDetails/5".ShouldMapTo<ProposalController>(a => a.AdminDetails(5, 3), true);
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestAdminDownloadMapping()
        {
            "~/Proposal/AdminDownload/5".ShouldMapTo<ProposalController>(a => a.AdminDownload(5, 3), true);
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestSendCallMapping()
        {
            "~/Proposal/SendCall/5".ShouldMapTo<ProposalController>(a => a.SendCall(5, true), true);
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestSendDecisionMapping()
        {
            "~/Proposal/SendDecision/5".ShouldMapTo<ProposalController>(a => a.SendDecision(5, true), true);
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestAdminEditGetMapping()
        {
            "~/Proposal/AdminEdit/5".ShouldMapTo<ProposalController>(a => a.AdminEdit(5, 5), true);
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestAdminEditPostMapping()
        {
            "~/Proposal/AdminEdit/5".ShouldMapTo<ProposalController>(a => a.AdminEdit(5, 5, null, null, string.Empty), true);
        }

        /// <summary>
        /// #9
        /// </summary>
        [TestMethod]
        public void TestReviewerIndexMapping()
        {
            "~/Proposal/ReviewerIndex/5".ShouldMapTo<ProposalController>(a => a.ReviewerIndex(5, null, null));
        }

        /// <summary>
        /// #10
        /// </summary>
        [TestMethod]
        public void TestReviewerDetailsMapping()
        {
            "~/Proposal/ReviewerDetails/5".ShouldMapTo<ProposalController>(a => a.ReviewerDetails(5, 6), true);
        }

        /// <summary>
        /// #11
        /// </summary>
        [TestMethod]
        public void TestReviewerDownloadMapping()
        {
            "~/Proposal/ReviewerDownload/5".ShouldMapTo<ProposalController>(a => a.ReviewerDownload(5, 6), true);
        }

        /// <summary>
        /// #12
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Proposal/Create/5".ShouldMapTo<ProposalController>(a => a.Create(5));
        }

        /// <summary>
        /// #13
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Proposal/Create/5".ShouldMapTo<ProposalController>(a => a.Create(5, true, null), true);
        }

        /// <summary>
        /// #14
        /// </summary>
        [TestMethod]
        public void TestConfirmationMapping()
        {
            "~/Proposal/Confirmation/".ShouldMapTo<ProposalController>(a => a.Confirmation(null));
        }

        /// <summary>
        /// #15
        /// </summary>
        [TestMethod]
        public void TestHomeMapping()
        {
            "~/Proposal/Home/".ShouldMapTo<ProposalController>(a => a.Home());
        }

        /// <summary>
        /// #16
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Proposal/Edit/".ShouldMapTo<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1)), true);
        }

        /// <summary>
        /// #17
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Proposal/Edit/".ShouldMapTo<ProposalController>(a => a.Edit(SpecificGuid.GetGuid(1), null, new QuestionAnswerParameter[0],null, string.Empty ), true);
        }

        /// <summary>
        /// #18
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/Proposal/Details/".ShouldMapTo<ProposalController>(a => a.Details(SpecificGuid.GetGuid(1)), true);
        }

        #endregion Mapping Tests
    }
}
