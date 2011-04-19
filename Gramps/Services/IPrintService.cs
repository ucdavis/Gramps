using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace Gramps.Services
{
    public interface IPrintService
    {
        FileContentResult Print(int callForProposalId, int? proposalId, bool showComments = false);
    }

    public class PrintService : IPrintService
    {
        public virtual FileContentResult Print(int callForProposalId, int? proposalId, bool showComments = false)
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

            return new FileContentResult(bytes, "application/pdf");// File(bytes, "application/pdf");
        }
    }

}