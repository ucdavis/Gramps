using System;
using System.Linq;
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

namespace Gramps.Tests.ControllerTests.TermplateControllerTests
{
    [TestClass]
    public partial class TemplateControllerTests : ControllerTestBase<TemplateController>
    {
        private readonly Type _controllerClass = typeof(TemplateController);
        public IRepository<Template> TemplateRepository;
        public IAccessService AccessService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            TemplateRepository = FakeRepository<Template>();
            AccessService = MockRepository.GenerateStub<IAccessService>();

            Controller = new TestControllerBuilder().CreateController<TemplateController>(TemplateRepository, AccessService);
        }

        protected override void RegisterRoutes()
        {
            //new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public TemplateControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<Template>()).Return(TemplateRepository).Repeat.Any();	
        }
        #endregion Init
    }
}
