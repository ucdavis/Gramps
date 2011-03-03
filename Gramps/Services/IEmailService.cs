using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Gramps.Core.Domain;
using Gramps.Core.Resources;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Gramps.Services
{
    public interface IEmailService
    {
        void SendEmail(HttpRequestBase request, UrlHelper url, CallForProposal callForProposal, EmailTemplate emailTemplate, string email, bool immediate, string tempPass = null);
        void SendConfirmation(HttpRequestBase request, UrlHelper url, Proposal proposal, EmailTemplate emailTemplate, bool immediate, string userName, string tempPass);
        void SendPasswordReset(CallForProposal callForProposal, string email, string tempPassword);
        void SendProposalEmail(HttpRequestBase request, UrlHelper url, Proposal proposal, EmailTemplate emailTemplate, bool immediate);
    }

    public class EmailService : IEmailService
    {
        private readonly IRepository _repository;

        public EmailService(IRepository repository)
        {
            _repository = repository;
        }

        public virtual void SendPasswordReset(CallForProposal callForProposal, string email, string tempPassword)
        {
            var emailQueue = new EmailQueue(callForProposal, email, "Password Reset", "");
            emailQueue.Immediate = true;
            emailQueue.Body =
                "<p>Your password has been reset to the following. We recommend changing it once you login.</p>";
            emailQueue.Body = string.Format("{0}<p>UserName {1}</p><p>Password {2}</p>", emailQueue.Body, email, tempPassword);
            _repository.OfType<EmailQueue>().EnsurePersistent(emailQueue);
        }

        public virtual void SendEmail(HttpRequestBase request, UrlHelper url, CallForProposal callForProposal, EmailTemplate emailTemplate, string email, bool immediate, string tempPass = null)
        {
            var emailQueue = new EmailQueue(callForProposal, email, emailTemplate.Subject, emailTemplate.Text);
            //Need to replace parameters.

            emailQueue.Body = emailQueue.Body + "<br />" + StaticValues.EmailAutomatedDisclaimer;
            if (emailTemplate.TemplateType == EmailTemplateType.InitialCall)
            {
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenProposalMaximum),
                                                          String.Format("{0:C}", callForProposal.ProposalMaximum));

                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenCloseDate),
                                                          String.Format("{0:D}", callForProposal.EndDate));


                emailQueue.Body = emailQueue.Body + "<br />" + StaticValues.EmailCreateProposal + "<br />";
                emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body, GetAbsoluteUrl(request, url, "~/Proposal/Create/" + callForProposal.Id));
            }
            if (emailTemplate.TemplateType == EmailTemplateType.ReadyForReview)
            {
                var reviewerName = callForProposal.Editors.Where(a => a.ReviewerEmail == email).FirstOrDefault();
                if (reviewerName != null)
                {
                    emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenReviewerName),
                                                          reviewerName.ReviewerName);
                }
                else
                {
                    emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenReviewerName),
                                                          email);
                }
                

                if (string.IsNullOrEmpty(tempPass))
                {
                    emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body, "You have an existing account. Use your email as the userName to login");
                }
                else
                {
                    emailQueue.Body = string.Format("{0}<p>An account has been created for you.</p><p>UserName {1}</p><p>Password {2}</p><p>You may change your password (recommended) after logging in.</p>", emailQueue.Body, email, tempPass);
                }
                emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body, "After you have logged in, you may use this link to review submitted proposals for this Grant Request:");
                emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body, GetAbsoluteUrl(request, url, "~/Proposal/ReviewerIndex/" + callForProposal.Id));
                emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body,
                                                "Or to view all active Call For Proposals you can use this link(Home):");
                emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body, GetAbsoluteUrl(request, url, "~/Proposal/Home"));
            }


            emailQueue.Immediate = immediate;

            _repository.OfType<EmailQueue>().EnsurePersistent(emailQueue);

            
        }

        private static string Token(string token)
        {
            return string.Format("{{{0}}}", token);
        }

        public virtual void SendConfirmation(HttpRequestBase request, UrlHelper url, Proposal proposal, EmailTemplate emailTemplate, bool immediate, string userName, string tempPass)
        {
            Check.Require(emailTemplate.TemplateType == EmailTemplateType.ProposalConfirmation, "Email must be Proposal Conformation Template Type");
            
            var emailQueue = new EmailQueue(proposal.CallForProposal, proposal.Email, emailTemplate.Subject,
                                            emailTemplate.Text);
            emailQueue.Immediate = immediate;

            emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenCloseDate),
                                                          String.Format("{0:D}", proposal.CallForProposal.EndDate));

            emailQueue.Body = string.Format("{0}<br /><p>{1}</p>", emailQueue.Body, StaticValues.EmailAutomatedDisclaimer);
            if (string.IsNullOrEmpty(tempPass))
            {
                emailQueue.Body = string.Format("{0}<p>{1}</p>", emailQueue.Body, "You have an existing account. Use your email as the userName to login");
            }
            else
            {
                emailQueue.Body = string.Format("{0}<p>{1}</p><p>{2} {3}</p><p>{4} {5}</p><p>{6}</p>"
                    , emailQueue.Body
                    , "An account has been created for you."
                    , "UserName"
                    , userName
                    , "Password"
                    , tempPass
                    , "You may change your password (recommended) after logging in.");
            }
            emailQueue.Body = string.Format("{0}<p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p>"
                , emailQueue.Body
                , "After you have logged in, you may use this link to edit your proposal:"
                , GetAbsoluteUrl(request, url, "~/Proposal/Edit/" + proposal.Guid)
                , "Or you may access a list of your proposal(s) here:"
                , GetAbsoluteUrl(request, url, "~/Proposal/Home"));

            _repository.OfType<EmailQueue>().EnsurePersistent(emailQueue);

        }

        public virtual void SendProposalEmail(HttpRequestBase request, UrlHelper url, Proposal proposal, EmailTemplate emailTemplate, bool immediate)
        {
            var emailQueue = new EmailQueue(proposal.CallForProposal, proposal.Email, emailTemplate.Subject,
                                            emailTemplate.Text);
            emailQueue.Immediate = immediate;
            emailQueue.Body = string.Format("{0}<br /><p>{1}</p>", emailQueue.Body, StaticValues.EmailAutomatedDisclaimer);

            if (emailTemplate.TemplateType == EmailTemplateType.ProposalApproved)
            {
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenApprovedAmount),
                                                          String.Format("{0:C}", proposal.ApprovedAmount));
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenProposalLink),
                                                          GetAbsoluteUrl(request, url, "~/Proposal/Details/" + proposal.Guid));
            }
            else if(emailTemplate.TemplateType == EmailTemplateType.ProposalDenied)
            {
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenProposalLink),
                                                          GetAbsoluteUrl(request, url, "~/Proposal/Details/" + proposal.Guid));
            }
            else if(emailTemplate.TemplateType == EmailTemplateType.ReminderCallIsAboutToClose)
            {
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenCloseDate),
                                                          String.Format("{0:D}", proposal.CallForProposal.EndDate));
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenProposalLink),
                                                          GetAbsoluteUrl(request, url, "~/Proposal/Edit/" + proposal.Guid));
            }
            else if(emailTemplate.TemplateType == EmailTemplateType.ProposalUnsubmitted)
            {
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenCloseDate),
                                          String.Format("{0:D}", proposal.CallForProposal.EndDate));
                emailQueue.Body = emailQueue.Body.Replace(Token(StaticValues.TokenProposalLink),
                                                          GetAbsoluteUrl(request, url, "~/Proposal/Edit/" + proposal.Guid));
            }

            _repository.OfType<EmailQueue>().EnsurePersistent(emailQueue);

        }

        private string GetAbsoluteUrl(HttpRequestBase request, UrlHelper url, string relative)
        {
            return string.Format("{0}://{1}:{2}{3}", request.Url.Scheme, request.Url.Host, request.Url.Port, url.Content(relative));
        }
    }
}