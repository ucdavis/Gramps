using Gramps.Controllers;
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

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreateReviewerGetMapping()
        {
            "~/Editor/CreateReviewer/".ShouldMapTo<EditorController>(a => a.CreateReviewer(null, null));
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestCreateReviewerPostMapping()
        {
            "~/Editor/CreateReviewer/".ShouldMapTo<EditorController>(a => a.CreateReviewer(null, null, new Editor()), true);
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestEditReviewerGetMapping()
        {
            "~/Editor/EditReviewer/5".ShouldMapTo<EditorController>(a => a.EditReviewer(5, null, null));
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestEditReviewerPostMapping()
        {
            "~/Editor/EditReviewer/5".ShouldMapTo<EditorController>(a => a.EditReviewer(5, null, null, new Editor()), true);
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/Editor/Delete/5".ShouldMapTo<EditorController>(a => a.Delete(5, null, null));
        }

        /// <summary>
        /// #9
        /// </summary>
        [TestMethod]
        public void TestSendCallGetMapping()
        {
            "~/Editor/SendCall/5".ShouldMapTo<EditorController>(a => a.SendCall(5));
        }

        /// <summary>
        /// #10
        /// </summary>
        [TestMethod]
        public void TestSendCallPostMapping()
        {
            "~/Editor/SendCall/5".ShouldMapTo<EditorController>(a => a.SendCall(5,true), true);
        }
        #endregion Mapping Tests
    }
}
