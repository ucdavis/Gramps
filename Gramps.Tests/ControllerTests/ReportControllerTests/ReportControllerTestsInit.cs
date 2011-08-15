using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Gramps.Controllers;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace Gramps.Tests.ControllerTests.ReportControllerTests
{
    [TestClass]
    public partial class ReportControllerTests : ControllerTestBase<ReportController>
    {
        protected readonly Type ControllerClass = typeof(ReportController);
        public IRepository<Report> ReportRepository;
        public IAccessService AccessService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            ReportRepository = FakeRepository<Report>();
            AccessService = MockRepository.GenerateStub<IAccessService>();

            Controller = new TestControllerBuilder().CreateController<ReportController>(ReportRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            //new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public ReportControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Report>()).Return(ReportRepository).Repeat.Any();	
        }
        #endregion Init



        

       
    }
}
