using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace Gramps.Services
{
    public interface IEmailService
    {
        void SendEmail(CallForProposal callForProposal, EmailTemplate emailTemplate, string email, bool immediate);
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
    }
}