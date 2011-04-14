﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Services;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using Gramps.Tests.Core.Extensions;

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
        #endregion Mapping Tests

    }
}
