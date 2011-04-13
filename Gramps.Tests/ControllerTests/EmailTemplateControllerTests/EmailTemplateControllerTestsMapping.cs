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
        #endregion Mapping Tests
    }
}
