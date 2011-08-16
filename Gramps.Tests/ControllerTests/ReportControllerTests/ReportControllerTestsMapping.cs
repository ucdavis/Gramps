using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gramps.Controllers;
using Gramps.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Gramps.Tests.Core.Extensions;

namespace Gramps.Tests.ControllerTests.ReportControllerTests
{
    public partial class ReportControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestTemplateIndexMapping()
        {
            "~/Report/TemplateIndex/".ShouldMapTo<ReportController>(a => a.TemplateIndex(null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestCallIndexMapping()
        {
            "~/Report/CallIndex/5".ShouldMapTo<ReportController>(a => a.CallIndex(5));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreateForTemplateGetMapping()
        {
            "~/Report/CreateForTemplate/".ShouldMapTo<ReportController>(a => a.CreateForTemplate(null, null));
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreateForTemplatePostMapping()
        {
            "~/Report/CreateForTemplate/".ShouldMapTo<ReportController>(a => a.CreateForTemplate(new Report(), null, null, null, null), true);
        }
        #endregion Mapping Tests
    }
}
