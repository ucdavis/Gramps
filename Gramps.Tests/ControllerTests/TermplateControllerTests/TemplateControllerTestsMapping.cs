using Gramps.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

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

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Template/Edit/5".ShouldMapTo<TemplateController>(a => a.Edit(5));
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Template/Edit/5".ShouldMapTo<TemplateController>(a => a.Edit(5, null));
        }
        #endregion Mapping Tests

    }
}
