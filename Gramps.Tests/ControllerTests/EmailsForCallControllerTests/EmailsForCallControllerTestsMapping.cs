﻿using Gramps.Controllers;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;


namespace Gramps.Tests.ControllerTests.EmailsForCallControllerTests
{
    public partial class EmailsForCallControllerTests
    {
        #region Mapping Tests
        /// <summary>
        /// #1
        /// </summary>
        [TestMethod]
        public void TestIndexMapping()
        {
            "~/EmailsForCall/Index/".ShouldMapTo<EmailsForCallController>(a => a.Index(null, null));
        }

        /// <summary>
        /// #2
        /// </summary>
        [TestMethod]
        public void TestBulkCreateGetMapping()
        {
            "~/EmailsForCall/BulkCreate/".ShouldMapTo<EmailsForCallController>(a => a.BulkCreate(null, null));
        }

        /// <summary>
        /// #3
        /// </summary>
        [TestMethod]
        public void TestBulkCreatePostMapping()
        {
            "~/EmailsForCall/BulkCreate/".ShouldMapTo<EmailsForCallController>(a => a.BulkCreate(null, null, "test"), true);
        }

        /// <summary>
        /// #4
        /// </summary>
        [TestMethod]
        public void TestCreateGetMapping()
        {
            "~/EmailsForCall/Create/".ShouldMapTo<EmailsForCallController>(a => a.Create(null, null));
        }

        /// <summary>
        /// #5
        /// </summary>
        [TestMethod]
        public void TestCreatePostMapping()
        {
            "~/EmailsForCall/Create/".ShouldMapTo<EmailsForCallController>(a => a.Create(null, null, new EmailsForCall()), true);
        }

        /// <summary>
        /// #6
        /// </summary>
        [TestMethod]
        public void TestEditGetMapping()
        {
            "~/EmailsForCall/Edit/5".ShouldMapTo<EmailsForCallController>(a => a.Edit(5, null, null));
        }

        /// <summary>
        /// #7
        /// </summary>
        [TestMethod]
        public void TestEditPostMapping()
        {
            "~/EmailsForCall/Edit/5".ShouldMapTo<EmailsForCallController>(a => a.Edit(5, null, null, new EmailsForCall()), true);
        }

        /// <summary>
        /// #8
        /// </summary>
        [TestMethod]
        public void TestDeleteMapping()
        {
            "~/EmailsForCall/Delete/5".ShouldMapTo<EmailsForCallController>(a => a.Delete(5, null, null));
        }

        /// <summary>
        /// #9
        /// </summary>
        [TestMethod]
        public void TestSendCallGetMapping()
        {
            "~/EmailsForCall/SendCall/5".ShouldMapTo<EmailsForCallController>(a => a.SendCall(5));
        }

        /// <summary>
        /// #10
        /// </summary>
        [TestMethod]
        public void TestSendCallPostMapping()
        {
            "~/EmailsForCall/SendCall/5".ShouldMapTo<EmailsForCallController>(a => a.SendCall(5, false), true);
        }
        #endregion Mapping Tests
    }
}
