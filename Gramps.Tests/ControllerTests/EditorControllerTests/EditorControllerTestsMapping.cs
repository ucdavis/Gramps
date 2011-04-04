﻿using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
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
        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestAddEditorPostMapping()
        {
            "~/Editor/AddEditor/5".ShouldMapTo<EditorController>(a => a.AddEditor(null, null, 5), true);
        }
        #endregion Mapping Tests
    }
}
