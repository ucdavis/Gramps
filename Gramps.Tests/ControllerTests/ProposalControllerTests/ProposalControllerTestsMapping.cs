using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Gramps.Tests.Core.Extensions;

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
        public void TestReviewerIndexPostMapping()
        {
            "~/Proposal/ReviewerIndex/5".ShouldMapTo<ProposalController>(a => a.ReviewerIndex(5, null, null));
        }
        #endregion Mapping Tests
    }
}
