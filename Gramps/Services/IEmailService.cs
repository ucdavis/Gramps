using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace Gramps.Services
{
    public interface IEmailService
    {
        void SendEmail(CallForProposal callForProposal, EmailTemplate emailTemplate, string email, bool immediate);
        void SendConfirmation(HttpRequestBase request, UrlHelper url, Proposal proposal, EmailTemplate emailTemplate, bool immediate);
    }

    public class EmailService : IEmailService
    {
        private readonly IRepository _repository;

        public EmailService(IRepository repository)
        {
            _repository = repository;
        }

        public virtual void SendEmail(CallForProposal callForProposal,EmailTemplate emailTemplate, string email, bool immediate)
        {
            var emailQueue = new EmailQueue(callForProposal, email, emailTemplate.Subject, emailTemplate.Text);
            //Need to replace parameters.

            emailQueue.Immediate = immediate;

            _repository.OfType<EmailQueue>().EnsurePersistent(emailQueue);

            
        }

        public virtual void SendConfirmation(HttpRequestBase request, UrlHelper url, Proposal proposal, EmailTemplate emailTemplate, bool immediate)
        {
            var emailQueue = new EmailQueue(proposal.CallForProposal, proposal.Email, emailTemplate.Subject,
                                            emailTemplate.Text);
            emailQueue.Immediate = immediate;
            emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body, GetAbsoluteUrl(request, url, "~/Proposal/Edit/" + proposal.Guid));
            _repository.OfType<EmailQueue>().EnsurePersistent(emailQueue);
        }

        private string GetAbsoluteUrl(HttpRequestBase request, UrlHelper url, string relative)
        {
            return string.Format("{0}://{1}:{2}{3}", request.Url.Scheme, request.Url.Host, request.Url.Port, url.Content(relative));
        }
    }
}