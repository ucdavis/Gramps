using System;
using System.Linq;
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

namespace Gramps.Tests.ControllerTests.TermplateControllerTests
{
    public partial class TemplateControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/Template/Index/".ShouldMapTo<TemplateController>(a => a.Index());
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Template/".ShouldMapTo<TemplateController>(a => a.Index());
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Template/Create/".ShouldMapTo<TemplateController>(a => a.Create());
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Template/Create/".ShouldMapTo<TemplateController>(a => a.Create(null));
        }
        #endregion Mapping Tests

    }
}
