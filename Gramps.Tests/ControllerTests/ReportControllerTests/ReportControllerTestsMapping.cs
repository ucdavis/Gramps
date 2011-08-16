using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gramps.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

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
        #endregion Mapping Tests
    }
}
