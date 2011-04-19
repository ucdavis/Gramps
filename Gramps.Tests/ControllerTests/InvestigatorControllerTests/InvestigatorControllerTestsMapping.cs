using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gramps.Tests.ControllerTests.InvestigatorControllerTests
{
    public partial class InvestigatorControllerTests 
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/Investigator/Create/730ce6ea-d76d-482f-84da-915f1d3b7562".ShouldMapTo<InvestigatorController>(a => a.Create(SpecificGuid.GetGuid(1)), true);
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/Investigator/Create/730ce6ea-d76d-482f-84da-915f1d3b7562".ShouldMapTo<InvestigatorController>(a => a.Create(SpecificGuid.GetGuid(1), new Investigator()), true);
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/Investigator/Edit/5 propsalId?730ce6ea-d76d-482f-84da-915f1d3b7562".ShouldMapTo<InvestigatorController>(a => a.Edit(5, SpecificGuid.GetGuid(1)), true);
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/Investigator/Edit/5 propsalId?730ce6ea-d76d-482f-84da-915f1d3b7562".ShouldMapTo<InvestigatorController>(a => a.Edit(5, SpecificGuid.GetGuid(1), new Investigator()), true);
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/Investigator/Delete/5 propsalId?730ce6ea-d76d-482f-84da-915f1d3b7562".ShouldMapTo<InvestigatorController>(a => a.Delete(5, SpecificGuid.GetGuid(1)), true);
        }
        #endregion Mapping Tests

    }
}
