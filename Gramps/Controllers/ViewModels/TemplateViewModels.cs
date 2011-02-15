using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Gramps.Controllers.ViewModels
{
    /// <summary>
    /// ViewModel for the Template class
    /// </summary>
    public class TemplateViewModel : NavigationViewModel
    {
        public Template Template { get; set; }

        public static TemplateViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository must be supplied");


            var viewModel = new TemplateViewModel { Template = new Template() };
            viewModel.IsTemplate = true;
            viewModel.IsCallForProposal = false;

            return viewModel;
        }
    }

    public class TemplateListViewModel
    {
        public IQueryable<Template> Templates { get; set; }


        public static TemplateListViewModel Create(IRepository repository, string loginId)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new TemplateListViewModel();
            var templateIds = repository.OfType<Editor>().Queryable.Where(a => a.Template != null && a.User != null && a.User.LoginId == loginId).Select(x => x.Template.Id).ToList();
            viewModel.Templates = repository.OfType<Template>().Queryable.Where(a => templateIds.Contains(a.Id));
            return viewModel;
        }
    }
}