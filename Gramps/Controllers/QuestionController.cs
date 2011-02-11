using System;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Question class
    /// </summary>
    public class QuestionController : ApplicationController
    {
        private readonly IAccessService _accessService;
        private readonly IRepository<Question> _questionRepository;

        public QuestionController(IRepository<Question> questionRepository, IAccessService accessService)
        {
            _questionRepository = questionRepository;
            _accessService = accessService;
        }

        //
        // GET: /Question/
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = QuestionListViewModel.Create(Repository, templateId, callForProposalId);
            return View(viewModel);
        }

    }


}
