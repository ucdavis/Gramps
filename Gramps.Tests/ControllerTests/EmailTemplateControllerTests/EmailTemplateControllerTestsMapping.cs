using Gramps.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Gramps.Tests.Core.Extensions;


namespace Gramps.Tests.ControllerTests.EmailTemplateControllerTests
{
    public partial class EmailTemplateControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/EmailTemplate/Index/".ShouldMapTo<EmailTemplateController>(a => a.Index(null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestSendTestEmailMapping()
        {
            "~/EmailTemplate/SendTestEmail/".ShouldMapTo<EmailTemplateController>(a => a.SendTestEmail(null, null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/EmailTemplate/Edit/5".ShouldMapTo<EmailTemplateController>(a => a.Edit(5, null, null));
        }
        #endregion Mapping Tests
    }
}
