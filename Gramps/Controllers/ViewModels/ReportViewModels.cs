using System.Linq;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Gramps.Controllers.ViewModels
{

    /// <summary>
    /// ViewModel for the Report class
    /// </summary>
    public class ReportViewModel
    {
        public Report Report { get; set; }

        public static ReportViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new ReportViewModel { Report = new Report() };

            return viewModel;
        }
    }

    public class TemplateReportListViewModel : NavigationViewModel
    {
        public IQueryable<Report> ReportList;

        public static TemplateReportListViewModel Create(IRepository repository, int? templateId, int? callForProposalId)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new TemplateReportListViewModel();

            if (templateId != null && templateId != 0)
            {
                viewModel.IsTemplate = true;                
                viewModel.ReportList = repository.OfType<Report>().Queryable.Where(a => a.Template.Id == templateId);
                viewModel.TemplateId = templateId;
            }

            return viewModel;
        }
    }

    public class CallReportListViewModel : CallNavigationViewModel
    {
        public IQueryable<Report> ReportList;

        public static CallReportListViewModel Create(IRepository repository, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");
            var viewModel = new CallReportListViewModel{CallForProposal = callForProposal};
            viewModel.ReportList = repository.OfType<Report>().Queryable.Where(a => a.CallForProposal.Id == callForProposal.Id);

            return viewModel;
        }
    }
}