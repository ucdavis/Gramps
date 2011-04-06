using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;



namespace Gramps.Tests.ControllerTests.EmailQueueControllerTests
{
    public partial class EmailQueueControllerTests
    {
        #region Mapping Tests

        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/EmailQueue/Index/5".ShouldMapTo<EmailQueueController>(a => a.Index(5));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestDetailsMapping()
        {
            "~/EmailQueue/Details/5".ShouldMapTo<EmailQueueController>(a => a.Details(5, 3), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/EmailQueue/Edit/5".ShouldMapTo<EmailQueueController>(a => a.Edit(5, 3), true);
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/EmailQueue/Edit/5".ShouldMapTo<EmailQueueController>(a => a.Edit(5, 3, new EmailQueue()), true);
        }
        #endregion Mapping Tests
    }
}
