using Gramps.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace Gramps.Tests.ControllerTests.QuestionControllerTests
{
    public partial class QuestionControllerTests
    {
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping1()
        {
            "~/Question/Index/".ShouldMapTo<QuestionController>(a => a.Index(null, null));
        }

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping2()
        {
            "~/Question/".ShouldMapTo<QuestionController>(a => a.Index(null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Question/Create/".ShouldMapTo<QuestionController>(a => a.Create(null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Question/Create/".ShouldMapTo<QuestionController>(a => a.Create(null, null, null, null));
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Question/Edit/5".ShouldMapTo<QuestionController>(a => a.Edit(5, null, null));
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Question/Edit/5".ShouldMapTo<QuestionController>(a => a.Edit(5, null, null, null, null));
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/Question/Delete/5".ShouldMapTo<QuestionController>(a => a.Delete(5, null, null));
        }
    }
}
