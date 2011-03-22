using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
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
    /// Controller for the Print class
    /// </summary>
    public class PrintController : ApplicationController
    {
        private readonly IRepository<CallForProposal> _callForProposalRepository;
        private readonly IRepository<Proposal> _proposalRepository;
        private readonly IAccessService _accessService;

        public PrintController(IRepository<CallForProposal> callForProposalRepository, IRepository<Proposal> proposalRepository, IAccessService accessService)
        {
            _callForProposalRepository = callForProposalRepository;
            _proposalRepository = proposalRepository;
            _accessService = accessService;
        }

        [UserOnly]
        public ActionResult ProposalAdmin(int callForProposalId, int? proposalId, bool showComments = false)
        {
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callForProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            if (proposalId.HasValue && proposalId.Value > 0)
            {
                var proposal = Repository.OfType<Proposal>().GetNullableById(proposalId.Value);
                if(proposal == null)
                {
                    Message = "Proposal Not found";
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
                if (!callForProposal.Proposals.Contains(proposal))
                {
                    Message = "Proposal Not found with call";
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
            }

            return CommonPrint(callForProposal.Id, proposalId, showComments);
        }

        [PublicAuthorize]
        public ActionResult ProposalReviewer(int callForProposalId, int? proposalId)
        {
            var callForProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId);

            if (callForProposal == null)
            {
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }
            if (!callForProposal.IsReviewer(CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (!_accessService.HasSameId(null, callForProposal, null, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<ProposalController>(a => a.Home());
            }

            if (proposalId.HasValue && proposalId.Value > 0)
            {
                var proposal = Repository.OfType<Proposal>().GetNullableById(proposalId.Value);
                if (proposal == null)
                {
                    Message = "Proposal Not found";
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
                if (!callForProposal.Proposals.Contains(proposal))
                {
                    Message = "Proposal Not found with call";
                    return this.RedirectToAction<ErrorController>(a => a.Index());
                }
            }

            return CommonPrint(callForProposal.Id, proposalId, false);
        }

        private ActionResult CommonPrint(int callForProposalId, int? proposalId, bool showComments = false)
        {
            var rview = new Microsoft.Reporting.WebForms.ReportViewer();
            rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

            rview.ServerReport.ReportPath = @"/Gramps.Report/CallReport";

            var paramList = new List<Microsoft.Reporting.WebForms.ReportParameter>();

            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("CallId", callForProposalId.ToString()));
            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ProposalId", proposalId != null && proposalId.Value > 0 ? proposalId.Value.ToString() : string.Empty));
            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("ShowComments", showComments.ToString().ToLower()));

            rview.ServerReport.SetParameters(paramList);

            string mimeType, encoding, extension;
            string[] streamids;
            Microsoft.Reporting.WebForms.Warning[] warnings;

            string format = "PDF";

            string deviceInfo = "<DeviceInfo>" +
                                "<SimplePageHeaders>True</SimplePageHeaders>" +
                                "<HumanReadablePDF>True</HumanReadablePDF>" +   // this line disables the compression done by SSRS 2008 so that it can be merged.
                                "</DeviceInfo>";

            byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

            return File(bytes, "application/pdf");
        }

    }

}
