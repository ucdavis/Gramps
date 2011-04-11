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


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/EmailsForCall/Index/".ShouldMapTo<EmailsForCallController>(a => a.Index(null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestBulkCreateGetMapping()
        {
            "~/EmailsForCall/BulkCreate/".ShouldMapTo<EmailsForCallController>(a => a.BulkCreate(null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestBulkCreatePostMapping()
        {
            "~/EmailsForCall/BulkCreate/".ShouldMapTo<EmailsForCallController>(a => a.BulkCreate(null, null, "test"), true);
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/EmailsForCall/Create/".ShouldMapTo<EmailsForCallController>(a => a.Create(null, null));
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/EmailsForCall/Create/".ShouldMapTo<EmailsForCallController>(a => a.Create(null, null, new EmailsForCall()), true);
        }
        #endregion Mapping Tests
    }
}
