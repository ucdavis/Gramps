using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Services;
using MvcContrib;
using UCDArch.Core.PersistanceSupport;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Print class
    /// </summary>
    public class PrintController : ApplicationController
    {
        private readonly IRepository<CallForProposal> _callForProposalRepository;
        private readonly IRepository<Proposal> _proposalRepository;
        private readonly IAccessService _accessService;
        private readonly IPrintService _printService;

        public PrintController(IRepository<CallForProposal> callForProposalRepository, IRepository<Proposal> proposalRepository, IAccessService accessService, IPrintService printService)
        {
            _callForProposalRepository = callForProposalRepository;
            _proposalRepository = proposalRepository;
            _accessService = accessService;
            _printService = printService;
        }

        /// <summary>
        /// #1
        /// </summary>
        /// <param name="callForProposalId"></param>
        /// <param name="proposalId"></param>
        /// <param name="showComments"></param>
        /// <returns></returns>
        [UserOnly]
        public ActionResult ProposalAdmin(int callForProposalId, int? proposalId, bool showComments = false)
        {
            var callForProposal = _callForProposalRepository.GetNullableById(callForProposalId);

            if (callForProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            if (proposalId.HasValue && proposalId.Value > 0)
            {
                var proposal = _proposalRepository.GetNullableById(proposalId.Value);
                if(proposal == null)
                {
                    Message = string.Format(StaticValues.Message_NotFound, "Proposal");
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
                if (!callForProposal.Proposals.Contains(proposal))
                {
                    Message = "Proposal Not found with call.";
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
            }

            return _printService.Print(callForProposal.Id, proposalId, showComments); //CommonPrint(callForProposal.Id, proposalId, showComments);
        }

        [PublicAuthorize]
        public ActionResult ProposalReviewer(int callForProposalId, int? proposalId)
        {
            var callForProposal = _callForProposalRepository.GetNullableById(callForProposalId);

            if (callForProposal == null)
            {
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }
            if (!callForProposal.IsReviewer(CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (proposalId.HasValue && proposalId.Value > 0)
            {
                var proposal = _proposalRepository.GetNullableById(proposalId.Value);
                if (proposal == null)
                {
                    Message = string.Format(StaticValues.Message_NotFound, "Proposal");
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
                if (!callForProposal.Proposals.Contains(proposal))
                {
                    Message = "Proposal Not found with call.";
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
            }

            return _printService.Print(callForProposal.Id, proposalId, false); //CommonPrint(callForProposal.Id, proposalId, false);
        }

        //private ActionResult CommonPrint(int callForProposalId, int? proposalId, bool showComments = false)
        //{
        //    var rview = new Microsoft.Reporting.WebForms.ReportViewer();
        //    rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

        //    rview.ServerReport.ReportPath = @"/Gramps.Report/CallReport";

        //    var paramList = new List<Microsoft.Reporting.WebForms.ReportParameter>();

        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CallId", callForProposalId.ToString()));
        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ProposalId", proposalId != null && proposalId.Value > 0 ? proposalId.Value.ToString() : string.Empty));
        //    paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ShowComments", showComments.ToString().ToLower()));

        //    rview.ServerReport.SetParameters(paramList);

        //    string mimeType, encoding, extension;
        //    string[] streamids;
        //    Microsoft.Reporting.WebForms.Warning[] warnings;

        //    string format = "PDF";

        //    string deviceInfo = "<DeviceInfo>" +
        //                        "<SimplePageHeaders>True</SimplePageHeaders>" +
        //                        "<HumanReadablePDF>True</HumanReadablePDF>" +   // this line disables the compression done by SSRS 2008 so that it can be merged.
        //                        "</DeviceInfo>";

        //    byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

        //    return File(bytes, "application/pdf");
        //}

    }

}
