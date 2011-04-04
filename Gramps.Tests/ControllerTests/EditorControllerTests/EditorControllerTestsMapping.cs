using Gramps.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;


namespace Gramps.Tests.ControllerTests.EditorControllerTests
{
    public partial class EditorControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/Editor/Index/".ShouldMapTo<EditorController>(a => a.Index(null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestAddEditorGetMapping()
        {
            "~/Editor/AddEditor/".ShouldMapTo<EditorController>(a => a.AddEditor(null, null));
        }
        #endregion Mapping Tests
    }
}
