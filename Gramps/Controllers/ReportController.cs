using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.Validator;

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
            var viewModel = ReportViewModel.Create(Repository, templateId, callForProposalId);

            return View(viewModel);
        }

        //
        // POST: /Report/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateForTemplate(Report report, int? templateId, int? callForProposalId, CreateReportParameter[] createReportParameters)
        {

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

        ////
        //// GET: /Report/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var report = _reportRepository.GetNullableById(id);

        //    if (report == null) return this.RedirectToAction(a => a.Index());

        //    var viewModel = ReportViewModel.Create(Repository);
        //    viewModel.Report = report;

        //    return View(viewModel);
        //}
        
        ////
        //// POST: /Report/Edit/5
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Edit(int id, Report report)
        //{
        //    var reportToEdit = _reportRepository.GetNullableById(id);

        //    if (reportToEdit == null) return this.RedirectToAction(a => a.Index());

        //    TransferValues(report, reportToEdit);

        //    reportToEdit.TransferValidationMessagesTo(ModelState);

        //    if (ModelState.IsValid)
        //    {
        //        _reportRepository.EnsurePersistent(reportToEdit);

        //        Message = "Report Edited Successfully";

        //        return this.RedirectToAction(a => a.Index());
        //    }
        //    else
        //    {
        //        var viewModel = ReportViewModel.Create(Repository);
        //        viewModel.Report = report;

        //        return View(viewModel);
        //    }
        //}
        
        ////
        //// GET: /Report/Delete/5 
        //public ActionResult Delete(int id)
        //{
        //    var report = _reportRepository.GetNullableById(id);

        //    if (report == null) return this.RedirectToAction(a => a.Index());

        //    return View(report);
        //}

        ////
        //// POST: /Report/Delete/5
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Delete(int id, Report report)
        //{
        //    var reportToDelete = _reportRepository.GetNullableById(id);

        //    if (reportToDelete == null) this.RedirectToAction(a => a.Index());

        //    _reportRepository.Remove(reportToDelete);

        //    Message = "Report Removed Successfully";

        //    return this.RedirectToAction(a => a.Index());
        //}
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Report source, Report destination)
        {
            throw new NotImplementedException();
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
