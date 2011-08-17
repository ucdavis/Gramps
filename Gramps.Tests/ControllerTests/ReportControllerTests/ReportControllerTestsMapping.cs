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

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestCreateForCallGetMapping()
        {
            "~/Report/CreateForCall/".ShouldMapTo<ReportController>(a => a.CreateForCall(null, null));
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestCreateForCallPostMapping()
        {
            "~/Report/CreateForCall/".ShouldMapTo<ReportController>(a => a.CreateForCall(new Report(), null, null, null, null), true);
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestEditForTemplateGetMapping()
        {
            "~/Report/EditForTemplate/5".ShouldMapTo<ReportController>(a => a.EditForTemplate(5, null, null));
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestEditForTemplatePostMapping()
        {
            "~/Report/EditForTemplate/5".ShouldMapTo<ReportController>(a => a.EditForTemplate(5, null, null, null, null, null));
        }
        #endregion Mapping Tests
    }
}
