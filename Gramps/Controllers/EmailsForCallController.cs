using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Gramps.Controllers.Filters;
using Gramps.Controllers.ViewModels;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using Gramps.Services;
using MvcContrib;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Helpers;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the EmailsForCall class
    /// </summary>
    [UserOnly]
    public class EmailsForCallController : ApplicationController
    {
	    private readonly IRepository<EmailsForCall> _emailsforcallRepository;
        private readonly IAccessService _accessService;
        private readonly IEmailService _emailService;

        public EmailsForCallController(IRepository<EmailsForCall> emailsforcallRepository, IAccessService accessService, IEmailService emailService)
        {
            _emailsforcallRepository = emailsforcallRepository;
            _accessService = accessService;
            _emailService = emailService;
        }
    
        /// <summary>
        /// #1
        /// GET: /EmailsForCall/
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult Index(int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = EmailsForCallListViewModel.Create(Repository, templateId, callForProposalId);

            return View(viewModel);
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult BulkCreate(int? templateId, int? callForProposalId)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }

            var viewModel = EmailsForCallViewModel.Create(Repository, template, callforProposal);

            return View(viewModel);
        }

        /// <summary>
        /// #3
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <param name="bulkLoadEmails"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult BulkCreate(int? templateId, int? callForProposalId, string bulkLoadEmails)
        {
            var emailsCreatedCount = 0;
            var notAddedCount = 0;
            Template template = null;
            CallForProposal callforProposal = null;
            var existingList = new List<string>();
            var notAddedSb = new StringBuilder();

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                existingList = _emailsforcallRepository.Queryable.Where(a => a.Template != null && a.Template == template).Select(a => a.Email.ToLower()).ToList();
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                existingList = _emailsforcallRepository.Queryable.Where(a => a.CallForProposal != null && a.CallForProposal == callforProposal).Select(a => a.Email.ToLower()).ToList();
            }

            
            const string regexPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";

            // Find matches
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(bulkLoadEmails, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);


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

        /// <summary>
        /// #4
        /// GET: /EmailsForCall/Create
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult Create(int? templateId, int? callForProposalId)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
            }

            var viewModel = EmailsForCallViewModel.Create(Repository, template, callforProposal);
            
            return View(viewModel);
        } 

        /// <summary>
        /// #5
        /// POST: /EmailsForCall/Create
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <param name="emailsforcall"></param>
        /// <returns></returns> 
        [HttpPost]
        public ActionResult Create(int? templateId, int? callForProposalId, EmailsForCall emailsforcall)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                if(_emailsforcallRepository.Queryable.Where(a => a.Template != null && a.Template == template && a.Email == emailsforcall.Email).Any())
                {
                    ModelState.AddModelError("Email", string.Format(StaticValues.ModelError_AlreadyExists, "Email"));
                }
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                if (_emailsforcallRepository.Queryable.Where(a => a.CallForProposal != null && a.CallForProposal == callforProposal && a.Email == emailsforcall.Email).Any())
                {
                    ModelState.AddModelError("Email", string.Format(StaticValues.ModelError_AlreadyExists, "Email"));
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

                Message = string.Format(StaticValues.Message_CreatedSuccessfully, "Emails For Call");

                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else
            {
                var viewModel = EmailsForCallViewModel.Create(Repository, template, callforProposal);
                viewModel.EmailsForCall = emailsforcallToCreate;
                Message = string.Format(StaticValues.Message_NotCreated, "Emails For Call");

                return View(viewModel);
            }
        }

        
        /// <summary>
        /// #6
        /// GET: /EmailsForCall/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateId"></param>
        /// <param name="callForProposalId"></param>
        /// <returns></returns>
        public ActionResult Edit(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var emailsforcall = _emailsforcallRepository.GetNullableById(id);

            if (emailsforcall == null)
            {
                Message = string.Format(StaticValues.Message_NotFound, "Emails For Call");
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (!_accessService.HasSameId(emailsforcall.Template, emailsforcall.CallForProposal, templateId, callForProposalId))
            {
                Message = string.Format(StaticValues.Message_NoAccess, "that");
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var viewModel = EmailsForCallViewModel.Create(Repository, emailsforcall.Template, emailsforcall.CallForProposal);
            viewModel.EmailsForCall = emailsforcall;

            return View(viewModel);
        }
        
        
        // POST: /EmailsForCall/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, int? templateId, int? callForProposalId, EmailsForCall emailsforcall)
        {
            Template template = null;
            CallForProposal callforProposal = null;

            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var emailsforcallToEdit = _emailsforcallRepository.GetNullableById(id);

            if (emailsforcallToEdit == null)
            {
                Message = "EmailsForCall not found.";
                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (!_accessService.HasSameId(emailsforcallToEdit.Template, emailsforcallToEdit.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            //TransferValues(emailsforcall, emailsforcallToEdit);
            emailsforcallToEdit.Email = emailsforcall.Email.ToLower();
            if (callForProposalId.HasValue && callForProposalId != 0)
            {
                emailsforcallToEdit.HasBeenEmailed = emailsforcall.HasBeenEmailed;
            }

            emailsforcallToEdit.TransferValidationMessagesTo(ModelState);

            if (templateId.HasValue && templateId != 0)
            {
                template = Repository.OfType<Template>().GetNullableById(templateId.Value);
                if(_emailsforcallRepository.Queryable.Where(a => a.Template == template && a.Id != emailsforcallToEdit.Id && a.Email == emailsforcall.Email).Any())
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
            }
            else if (callForProposalId.HasValue && callForProposalId != 0)
            {
                callforProposal = Repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                if (_emailsforcallRepository.Queryable.Where(a => a.CallForProposal == callforProposal && a.Id != emailsforcallToEdit.Id && a.Email == emailsforcall.Email).Any())
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
            }

            if (ModelState.IsValid)
            {
                _emailsforcallRepository.EnsurePersistent(emailsforcallToEdit);

                Message = "EmailsForCall Edited Successfully";

                return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            else
            {
                var viewModel = EmailsForCallViewModel.Create(Repository, emailsforcallToEdit.Template, emailsforcallToEdit.CallForProposal);
                viewModel.EmailsForCall = emailsforcallToEdit;

                return View(viewModel);
            }
        }
        

        //
        // POST: /EmailsForCall/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, int? templateId, int? callForProposalId)
        {
            if (!_accessService.HasAccess(templateId, callForProposalId, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

			var emailsforcallToDelete = _emailsforcallRepository.GetNullableById(id);

            if (emailsforcallToDelete == null)
            {
                Message = "Email not removed";
                this.RedirectToAction(a => a.Index(templateId, callForProposalId));
            }
            if (!_accessService.HasSameId(emailsforcallToDelete.Template, emailsforcallToDelete.CallForProposal, templateId, callForProposalId))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            _emailsforcallRepository.Remove(emailsforcallToDelete);

            Message = "Email Removed Successfully";

            return this.RedirectToAction(a => a.Index(templateId, callForProposalId));
        }

        public ActionResult SendCall(int id)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {

                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }

            var viewModel = EmailsForCallSendViewModel.Create(Repository, callforproposal);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SendCall(int id, bool immediate)
        {
            var callforproposal = Repository.OfType<CallForProposal>().GetNullableById(id);

            if (callforproposal == null)
            {

                return this.RedirectToAction<CallForProposalController>(a => a.Index(null, null, null));
            }

            if (!_accessService.HasAccess(null, callforproposal.Id, CurrentUser.Identity.Name))
            {
                Message = "You do not have access to that.";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }



            var viewModel = EmailsForCallSendViewModel.Create(Repository, callforproposal);
            if (!callforproposal.IsActive || callforproposal.EndDate.Date <= DateTime.Now.Date)
            {
                Message = "Is not active or end date is passed";
                return View(viewModel);
            }


            var count = 0;
            foreach (var emailsForCall in viewModel.EmailsForCallList.Where(a => !a.HasBeenEmailed))
            {
                _emailService.SendEmail(Request, Url, callforproposal, callforproposal.EmailTemplates.Where(a => a.TemplateType == EmailTemplateType.InitialCall).Single(), emailsForCall.Email, immediate);
                emailsForCall.EmailedOnDate = DateTime.Now;
                emailsForCall.HasBeenEmailed = true;
                _emailsforcallRepository.EnsurePersistent(emailsForCall);
                count++;
            }

            callforproposal.CallsSentDate = DateTime.Now;
            Repository.OfType<CallForProposal>().EnsurePersistent(callforproposal);

            viewModel = EmailsForCallSendViewModel.Create(Repository, callforproposal);
            Message = string.Format("{0} Emails Generated", count);
            return View(viewModel);
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
