using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;
using System.Linq;
using System.Linq.Expressions;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the EmailsForCall class
    /// </summary>
    [UserOnly]
    public class EmailsForCallController : ApplicationController
    {
	    private readonly IRepository<EmailsForCall> _emailsforcallRepository;

        public EmailsForCallController(IRepository<EmailsForCall> emailsforcallRepository)
        {
            _emailsforcallRepository = emailsforcallRepository;
        }
    
        //
        // GET: /EmailsForCall/
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            var viewModel = EmailsForCallListViewModel.Create(Repository, templateId, callForProposalId);

            return View(viewModel);
        }


        public ActionResult BulkCreate(int? templateId, int? callForProposalId)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }
            var viewModel = EmailsForCallViewModel.Create(Repository, template, callforProposal);

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult BulkCreate(int? templateId, int? callForProposalId, string bulkLoadEmails)
        {
            var emailsCreatedCount = 0;
            var notAddedCount = 0;
            Template template = null;
            CallForProposal callforProposal = null;
            var existingList = new List<string>();
            var notAddedSb = new StringBuilder();

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                existingList = _emailsforcallRepository.Queryable.Where(a => a.Template == template).Select(a => a.Email).ToList();
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                existingList = _emailsforcallRepository.Queryable.Where(a => a.CallForProposal == callforProposal).Select(a => a.Email).ToList();
            }
            
            var RegexPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";

            // Find matches
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(bulkLoadEmails, RegexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            // add each match
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (existingList.Contains(match.ToString().ToLower()))
                {
                    notAddedCount++;
                    notAddedSb.AppendLine(match.ToString().ToLower() + "  == Email already exists in list");
                }
                else
                {
                    ModelState.Clear();
                    var emailsforcallToCreate = new EmailsForCall(match.ToString().ToLower());
                    emailsforcallToCreate.Template = template;
                    emailsforcallToCreate.CallForProposal = callforProposal;
                    emailsforcallToCreate.TransferValidationMessagesTo(ModelState);
                    if (ModelState.IsValid)
                    {
                        _emailsforcallRepository.EnsurePersistent(emailsforcallToCreate);
                        emailsCreatedCount++;
                        existingList.Add(emailsforcallToCreate.Email);
                    }
                    else
                    {
                        notAddedCount++;
                        var sbErr = new StringBuilder();
                        foreach (var result in ModelState.Values)
                        {
                            foreach (var errs in result.Errors)
                            {
                                sbErr.Append(errs.ErrorMessage);
                            }
                        }

                        notAddedSb.AppendLine(string.Format("{0}  == {1} \n", match.ToString().ToLower(), sbErr));
                    }                   
                }
            }

            if (notAddedCount > 0)
            {
                ModelState.Clear();
                Message = string.Format("{0} EmailsForCall Created Successfully == {1} EmailsForCall Not Created", emailsCreatedCount, notAddedCount);
                var viewModel = EmailsForCallViewModel.Create(Repository, template, callforProposal);
                viewModel.BulkLoadEmails = notAddedSb.ToString();
                return View(viewModel);

            }
            else
            {
                Message = string.Format("{0} EmailsForCall Created Successfully", emailsCreatedCount);
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));

            }
        }

        //
        // GET: /EmailsForCall/Create
        public ActionResult Create(int? templateId, int? callForProposalId)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }
            var viewModel = EmailsForCallViewModel.Create(Repository, template, callforProposal);
            
            return View(viewModel);
        } 

        //
        // POST: /EmailsForCall/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(int? templateId, int? callForProposalId, EmailsForCall emailsforcall)
        {
            Template template = null;
            CallForProposal callforProposal = null;
            IQueryable<string> existingList;

            if (templateId.HasValue)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                if(_emailsforcallRepository.Queryable.Where(a => a.Template == template && a.Email == emailsforcall.Email).Any())
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
            }
            else if (callForProposalId.HasValue)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                if (_emailsforcallRepository.Queryable.Where(a => a.CallForProposal == callforProposal && a.Email == emailsforcall.Email).Any())
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
            }

            var emailsforcallToCreate = new EmailsForCall(emailsforcall.Email.ToLower());
  

            //TransferValues(emailsforcall, emailsforcallToCreate);
            emailsforcallToCreate.Template = template;
            emailsforcallToCreate.CallForProposal = callforProposal;

            emailsforcallToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _emailsforcallRepository.EnsurePersistent(emailsforcallToCreate);

                Message = "EmailsForCall Created Successfully";

                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else
            {
                var viewModel = EmailsForCallViewModel.Create(Repository, template, callforProposal);
                viewModel.EmailsForCall = emailsforcall;
                Message = "EmailsForCall not created";

                return View(viewModel);
            }
        }

        //
        // GET: /EmailsForCall/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var emailsforcall = _emailsforcallRepository.GetNullableById(id);

        //    if (emailsforcall == null) return this.RedirectToAction(a => a.Index(null, null));

        //    var viewModel = EmailsForCallViewModel.Create(Repository);
        //    viewModel.EmailsForCall = emailsforcall;

        //    return View(viewModel);
        //}
        
        //
        // POST: /EmailsForCall/Edit/5
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Edit(int id, EmailsForCall emailsforcall)
        //{
        //    var emailsforcallToEdit = _emailsforcallRepository.GetNullableById(id);

        //    if (emailsforcallToEdit == null) return this.RedirectToAction(a => a.Index(null, null));

        //    TransferValues(emailsforcall, emailsforcallToEdit);

        //    emailsforcallToEdit.TransferValidationMessagesTo(ModelState);

        //    if (ModelState.IsValid)
        //    {
        //        _emailsforcallRepository.EnsurePersistent(emailsforcallToEdit);

        //        Message = "EmailsForCall Edited Successfully";

        //        return this.RedirectToAction(a => a.Index(null, null));
        //    }
        //    else
        //    {
        //        var viewModel = EmailsForCallViewModel.Create(Repository);
        //        viewModel.EmailsForCall = emailsforcall;

        //        return View(viewModel);
        //    }
        //}
        

        //
        // POST: /EmailsForCall/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, int? templateId, int? callForProposalId)
        {
			var emailsforcallToDelete = _emailsforcallRepository.GetNullableById(id);

            if (emailsforcallToDelete == null)
            {
                Message = "Email not removed";
                this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }

            _emailsforcallRepository.Remove(emailsforcallToDelete);

            Message = "Email Removed Successfully";

            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(EmailsForCall source, EmailsForCall destination)
        {
            throw new NotImplementedException();
        }

    }


}
