using System;
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
    /// Controller for the Report class
    /// </summary>
    public class ReportController : ApplicationController
    {
        private readonly IAccessService _accessService;
	    private readonly IRepository<Report> _reportRepository;

        public ReportController(IRepository<Report> reportRepository, IAccessService accessService)
        {
            _reportRepository = reportRepository;
            _accessService = accessService;
        }
    
        //
        // GET: /Report/
        public ActionResult TemplateIndex(int? templateId, int? callForProposalId)
        {
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

        //
        // GET: /Report/Create
        public ActionResult CreateForTemplate(int? templateId, int? callForProposalId)
        {
            var viewModel = ReportViewModel.Create(Repository);

            return View(viewModel);
        }

        //
        // POST: /Report/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateForTemplate(Report report)
        {
            var reportToCreate = new Report();

            TransferValues(report, reportToCreate);

            reportToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _reportRepository.EnsurePersistent(reportToCreate);

                Message = "Report Created Successfully";

                return this.RedirectToAction(a => a.TemplateIndex(null, null)); //TODO: Fix
            }
            else
            {
                var viewModel = ReportViewModel.Create(Repository);
                viewModel.Report = report;

                return View(viewModel);
            }
        }    



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

}
