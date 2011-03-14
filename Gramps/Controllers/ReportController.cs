using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using MvcContrib;
using NPOI.HSSF.UserModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Helpers;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Report class
    /// </summary>
    [UserOnly]
    public class ReportController : ApplicationController
    {
        private readonly IAccessService _accessService;
	    private readonly IRepository<Report> _reportRepository;

        public ReportController(IRepository<Report> reportRepository, IAccessService accessService)
        {
            _reportRepository = reportRepository;
            _accessService = accessService;
        }

        #region Indexs

        //
        // GET: /Report/
        public ActionResult TemplateIndex(int? templateId, int? callForProposalId)
        {
            //Message = "Not Implemented yet";
            //return this.RedirectToAction<TemplateController>(a => a.Index());

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = TemplateReportListViewModel.Create(Repository, templateId, callForProposalId);

            return View(viewModel);
        }

        public ActionResult CallIndex(int id)
        {
            //Message = "Not Implemented yet";
            //return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));

            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null)); 
            }

            if (!_accessService.HasAccess(null, id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = CallReportListViewModel.Create(Repository, callforproposal);

            return View(viewModel);
        }

        #endregion Indexs

        #region Creates
        
        //
        // GET: /Report/Create


        public ActionResult CreateForTemplate(int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = ReportViewModel.Create(Repository, templateId, callForProposalId);

            return View(viewModel);
        }

        //
        // POST: /Report/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateForTemplate(Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var reportToCreate = CommonCreate(report, templateId, callForProposalId, createReportParameters);

            if (ModelState.IsValid)
            {
                _reportRepository.EnsurePersistent(reportToCreate);

                Message = "Report Created Successfully";

                return this.RedirectToAction(a => a.TemplateIndex(templateId, callForProposalId));
            }
            else
            {
                Message = "Unable to create report";
                var viewModel = ReportViewModel.Create(Repository, templateId, callForProposalId);
                viewModel.Report = reportToCreate;
                return View(viewModel);
            }
        }

        public ActionResult CreateForCall(int? templateId, int? callForProposalId)
        {
            if(!callForProposalId.HasValue || callForProposalId == 0)
            {
               return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null)); 
            }
            var callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);

            if (callforProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforProposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = CallReportViewModel.Create(Repository, callforProposal);

            return View(viewModel);
        }

        //
        // POST: /Report/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateForCall(Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters)
        {

            if (!callForProposalId.HasValue || callForProposalId == 0)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }
            var callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);

            if (callforProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforProposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }


            var reportToCreate = CommonCreate(report, templateId, callForProposalId, createReportParameters);
            reportToCreate.CallForProposal = callforProposal;

            if (ModelState.IsValid)
            {
                _reportRepository.EnsurePersistent(reportToCreate);

                Message = "Report Created Successfully";

                return this.RedirectToAction(a => a.CallIndex(callforProposal.Id));
            }
            else
            {
                Message = "Unable to create report";
                var viewModel = CallReportViewModel.Create(Repository, callforProposal);
                viewModel.Report = reportToCreate;
                return View(viewModel);
            }
        }    


        private Report CommonCreate(Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters)
        {
            Template template = null;
            CallForProposal callforProposal = null;
            var availableQuestionDict = new Dictionary<int, string>();
            if (templateId.HasValue && templateId.Value != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                availableQuestionDict = template.Questions.ToDictionary(question => question.Id, question => question.Name);
            }
            else if (callForProposalId.HasValue && callForProposalId.Value != 0)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                availableQuestionDict = callforProposal.Questions.ToDictionary(question => question.Id, question => question.Name);
            }

            if (createReportParameters == null)
            {
                createReportParameters = new CreateReportParameter[0];
            }


            var reportToCreate = new Report();
            reportToCreate.Template = template;
            reportToCreate.CallForProposal = callforProposal;
            reportToCreate.Name = report.Name;

            var count = 0;
            foreach (var createReportParameter in createReportParameters)
            {
                var reportColumn = new ReportColumn();
                reportColumn.ColumnOrder = count++;
                if (createReportParameter.Property)
                {
                    reportColumn.IsProperty = createReportParameter.Property;
                    reportColumn.Name = createReportParameter.PropertyName;
                }
                else
                {
                    if (availableQuestionDict.ContainsKey(createReportParameter.QuestionId))
                    {
                        reportColumn.Name = availableQuestionDict[createReportParameter.QuestionId];
                    }
                }
                reportToCreate.AddReportColumn(reportColumn);
            }

            if (reportToCreate.ReportColumns.Count == 0)
            {
                ModelState.AddModelError("ReportColumns", "Must select at least one column to report on");
            }


            reportToCreate.TransferValidationMessagesTo(ModelState);

            return reportToCreate;
        }
        #endregion Creates

        #region Edits
        //
        // GET: /Report/Edit/5
        public ActionResult EditForTemplate(int id, int? templateId, int? callForProposalId)
        {
            var report = _reportRepository.GetNullableById(id);

            if (report == null)
            {
                return this.RedirectToAction(a => a.TemplateIndex(templateId, callForProposalId));
            }

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = ReportViewModel.Create(Repository, templateId, callForProposalId);
            viewModel.Report = report;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditForTemplate(int id, Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters)
        {
            var reportToEdit = _reportRepository.GetNullableById(id);

            if (reportToEdit == null)
            {
                return this.RedirectToAction(a => a.TemplateIndex(templateId, callForProposalId));
            }
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var temp = CommonCreate(report, templateId, callForProposalId, createReportParameters);

            reportToEdit.ReportColumns.Clear();
            reportToEdit.Name = temp.Name;
            foreach (var reportColumn in temp.ReportColumns)
            {
                reportToEdit.AddReportColumn(reportColumn);
            }

            if (ModelState.IsValid)
            {
                _reportRepository.EnsurePersistent(reportToEdit);

                Message = "Report Edited Successfully";

                return this.RedirectToAction(a => a.TemplateIndex(templateId, callForProposalId));
            }
            else
            {
                Message = "Unable to edit report";
                var viewModel = ReportViewModel.Create(Repository, templateId, callForProposalId);
                viewModel.Report = reportToEdit;
                return View(viewModel);
            }
        }


        public ActionResult EditForCall(int id, int? templateId, int? callForProposalId)
        {
            if (!callForProposalId.HasValue || callForProposalId == 0)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }
            var callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);

            if (callforProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforProposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var report = _reportRepository.GetNullableById(id);
            if (report == null)
            {
                return this.RedirectToAction(a => a.CallIndex(callforProposal.Id));
            }

            var viewModel = CallReportViewModel.Create(Repository, callforProposal);
            viewModel.Report = report;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditForCall(int id, Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters)
        {

            if (!callForProposalId.HasValue || callForProposalId == 0)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }
            var callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);

            if (callforProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforProposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var reportToEdit = _reportRepository.GetNullableById(id);
            if (reportToEdit == null)
            {
                return this.RedirectToAction(a => a.CallIndex(callforProposal.Id));
            }

            var temp = CommonCreate(report, templateId, callForProposalId, createReportParameters);

            reportToEdit.ReportColumns.Clear();
            reportToEdit.Name = temp.Name;
            foreach (var reportColumn in temp.ReportColumns)
            {
                reportToEdit.AddReportColumn(reportColumn);
            }

            if (ModelState.IsValid)
            {
                _reportRepository.EnsurePersistent(reportToEdit);

                Message = "Report Edited Successfully";

                return this.RedirectToAction(a => a.CallIndex(callforProposal.Id));
            }
            else
            {
                Message = "Unable to edit report";
                var viewModel = CallReportViewModel.Create(Repository, callforProposal);
                viewModel.Report = reportToEdit;
                return View(viewModel);
            }
        }

        #endregion Edits


        [HttpPost]
        public ActionResult Delete(int reportId, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var reportToDelete = _reportRepository.GetNullableById(reportId);
            if (reportToDelete == null)
            {
                Message = "Report not found.";
                if (templateId.HasValue && templateId.Value != 0)
                {
                    return this.RedirectToAction(a => a.TemplateIndex(templateId, callForProposalId));
                }
                else
                {
                    return this.RedirectToAction(a => a.CallIndex(callForProposalId.Value));
                }
               
            }
            if (!_accessService.HasSameId(reportToDelete.Template, reportToDelete.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            _reportRepository.Remove(reportToDelete);
            Message = "Report removed";
            if (templateId.HasValue && templateId.Value != 0)
            {
                return this.RedirectToAction(a => a.TemplateIndex(templateId, callForProposalId));
            }
            else
            {
                return this.RedirectToAction(a => a.CallIndex(callForProposalId.Value));
            }

        }
        

        public ActionResult Launch(int id, int? callForProposalId)
        {
            if (!callForProposalId.HasValue || callForProposalId == 0)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }
            var callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);

            if (callforProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }
            if (!_accessService.HasAccess(null, callforProposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var report = _reportRepository.GetNullableById(id);
            if (report == null)
            {
                return this.RedirectToAction(a => a.CallIndex(callforProposal.Id));
            }

            var viewModel = ReportLaunchViewModel.Create(Repository, callforProposal, report);

            return View(viewModel);
        }

        public ActionResult ExportExcell(int id, int? callForProposalId)
        {
            if (!callForProposalId.HasValue || callForProposalId == 0)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }
            var callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);

            if (callforProposal == null)
            {
                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }
            if (!_accessService.HasAccess(null, callforProposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var report = _reportRepository.GetNullableById(id);
            if (report == null)
            {
                return this.RedirectToAction(a => a.CallIndex(callforProposal.Id));
            }

            var viewModel = ReportLaunchViewModel.Create(Repository, callforProposal, report, true);

            var fileName = string.Format("{0}-{1}.xls", callforProposal.Name.Replace(" ", string.Empty), DateTime.Now.Date.ToString("MMddyyyy"));

            try
            {
                // Opening the Excel template...
                var fs = new FileStream(Server.MapPath(@"~\Content\NPOITemplate.xls"), FileMode.Open, FileAccess.Read);

                // Getting the complete workbook...
                var templateWorkbook = new HSSFWorkbook(fs, true);

                // Getting the worksheet by its name...
                var sheet = templateWorkbook.GetSheetAt(0);// GetSheet("Sheet1");
                // Getting the row... 0 is the first row.
                var dataRow = sheet.CreateRow(0);
                for (int i = 0; i < viewModel.ColumnNames.Count; i++)
                {
                    dataRow.CreateCell(i).SetCellValue(viewModel.ColumnNames.ElementAt(i));
                    dataRow.GetCell(i).CellStyle.WrapText = true;    
                }

                var rowCount = 0;
                foreach (var rowValue in viewModel.RowValues)
                {
                    rowCount++;
                    dataRow = sheet.CreateRow(rowCount);
                    for (int i = 0; i < rowValue.Count(); i++)
                    {
                        dataRow.CreateCell(i).SetCellValue(rowValue.ElementAtOrDefault(i));
                    }
                }

                for (int i = 0; i < viewModel.ColumnNames.Count; i++)
                {
                    sheet.AutoSizeColumn(i);     
                }

                // Forcing formula recalculation...
                sheet.ForceFormulaRecalculation = true;

                var ms = new MemoryStream();

                // Writing the workbook content to the FileStream...
                templateWorkbook.Write(ms);

                Message = "Excel report created successfully!";

                // Sending the server processed data back to the user computer...
                return File(ms.ToArray(), "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                Message = "Error Creating Excel Report " + ex.Message;

                return this.RedirectToAction(a => a.CallIndex(callforProposal.Id));
            }
        }

    }

    public class CreateReportParameter
    {
        public CreateReportParameter()
        {
            Property = false;
        }

        public bool Property { get; set; }
        public int QuestionId { get; set; }
        public string PropertyName { get; set; }
    }

}
