using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Mapping;
using Gramps.Core.Helpers;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;
using UCDArch.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace Gramps.Core.Domain
{
    public class CallForProposal : DomainObject
    {
        #region Constructor
        public CallForProposal(Template template, User user)
        {            
            SetDefaults();
            TemplateGeneratedFrom = template;
            Name = template.Name;

            #region Copy/Add Editors
            var owner = new Editor();
            owner.IsOwner = true;
            owner.User = user;
            AddEditor(owner);
            foreach (var editor in template.Editors)
            {
                if (!editor.IsOwner)
                {
                    AddEditor(editor);
                }
            }
            #endregion Copy/Add Editors

            #region Added Emails to send out call for proposals to
            foreach (var emailsForCall in template.Emails)
            {
                AddEmailForCall(emailsForCall.Email); //Emails to send out call for proposals to.
            }
            #endregion Added Emails to send out call for proposals to

            #region Copy/Add Required Email templates
            var requiredEmailTemmplates = RequiredEmailTemplates.GetRequiredEmailTemplates();
            foreach (var emailTemplate in template.EmailTemplates)
            {
                Check.Require(emailTemplate.TemplateType != null);
                if (requiredEmailTemmplates.ContainsKey((EmailTemplateType)emailTemplate.TemplateType))
                {
                    AddEmailTemplate(emailTemplate);
                    requiredEmailTemmplates[(EmailTemplateType)emailTemplate.TemplateType] = true;
                }
            }
            foreach (var requiredEmailTemmplate in requiredEmailTemmplates)
            {
                if (requiredEmailTemmplate.Value == false)
                {
                    AddEmailTemplate(new EmailTemplate() { TemplateType = requiredEmailTemmplate.Key });
                }
            }
            #endregion Copy/Add Required Email templates

            #region Copy Questions

            foreach (var question in template.Questions)
            {
                AddQuestion(question);
            }
    

            #endregion Copy Questions
                      
        }
        public CallForProposal(string name)
        {
            SetDefaults();
            Name = name;
        }

        public CallForProposal()
        {
            SetDefaults();
        }

        protected void SetDefaults()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            Emails = new List<EmailsForCall>();
            EmailTemplates = new List<EmailTemplate>();
            Editors = new List<Editor>();
            Questions = new List<Question>();
            Proposals = new List<Proposal>();
        }

        #endregion Constructor

        #region Mapped Fields
        [UCDArch.Core.NHibernateValidator.Extensions.Required]
        [Length(100)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }

        [NotNull]
        public virtual DateTime CreatedDate { get; set; }
        
        [NotNull]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public virtual DateTime EndDate { get; set; }
        public virtual DateTime? CallsSentDate { get; set; }
        public virtual Template TemplateGeneratedFrom { get; set; }
        [RangeDouble(Min = 0.00, Message = "Minimum amount is one cent")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public virtual Decimal ProposalMaximum { get; set; }
        
        [NotNull]
        public virtual IList<EmailsForCall> Emails { get; set; }
        
        [NotNull]
        public virtual IList<EmailTemplate> EmailTemplates { get; set; }
        
        [NotNull]        
        public virtual IList<Editor> Editors { get; set; }
        
        [NotNull]
        public virtual IList<Question> Questions { get; set; }
        
        [NotNull]
        public virtual IList<Proposal> Proposals { get; set; }
        #endregion Mapped Fields

        #region Methods

        public virtual bool IsEditor(string userId)
        {
            return Editors.Where(a => a.User != null && a.User.LoginId == userId).Any();
        }

        public virtual bool IsReviewer(string userId)
        {
            return Editors.Where(a => a.ReviewerEmail == userId).Any();
        }
        public virtual void AddEmailForCall(string email)
        {
            var emailForCall = new EmailsForCall(email);
            emailForCall.CallForProposal = this;
            //if (emailForCall.IsValid())
            //{ 
            //    Emails.Add(emailForCall);
            //}
            Emails.Add(emailForCall);
        }
        public virtual void RemoveEmailForCall(EmailsForCall emailsForCall)
        {
            if (Emails != null && Emails.Contains(emailsForCall) && !emailsForCall.HasBeenEmailed)
            {
                Emails.Remove(emailsForCall);
            }
        }

        public virtual void AddEmailTemplate(EmailTemplate emailTemplate)
        {
            var newEmailTemplate = new EmailTemplate();
            newEmailTemplate.CallForProposal = this;
            newEmailTemplate.Template = null;
            newEmailTemplate.Subject = emailTemplate.Subject;
            newEmailTemplate.TemplateType = emailTemplate.TemplateType;
            newEmailTemplate.Text = emailTemplate.Text;

            EmailTemplates.Add(newEmailTemplate);
        }

        public virtual void AddEditor(Editor editor)
        {
            var newEditor = new Editor();
            newEditor.CallForProposal = this;
            //newEditor.Comments = 
            newEditor.IsOwner = editor.IsOwner;
            newEditor.ReviewerEmail = editor.ReviewerEmail;
            newEditor.ReviewerId = editor.ReviewerId; //Copy the same one unless they reset
            newEditor.ReviewerName = editor.ReviewerName;
            newEditor.Template = null;
            newEditor.User = editor.User;
            
            Editors.Add(newEditor);
        }

        public virtual void RemoveEditor(Editor editor)
        {
            if (Editors != null && Editors.Contains(editor) && !editor.IsOwner)
            {
                Editors.Remove(editor);
            }
        }

        public virtual void AddQuestion(Question question)
        {
            var newQuestion = new Question();
            newQuestion.CallForProposal = this;
            newQuestion.QuestionType = question.QuestionType;
            newQuestion.Name = question.Name;
            newQuestion.Order = question.Order;
            newQuestion.Template = null;
            if (question.Options != null)
            {
                foreach (var questionOption in question.Options)
                {
                    newQuestion.AddQuestionOption(questionOption);
                }
            }
            if (question.Validators != null)
            {
                foreach (var validator in question.Validators)
                {
                    newQuestion.Addvalidators(validator);
                }
            }

            Questions.Add(newQuestion);
        }

        public virtual void RemoveQuestion(Question question)
        {
            if (Questions != null && Questions.Contains(question))
            {
                //What happens if there is an answer?
                Questions.Remove(question);
            }
        }

        #endregion Methods

        #region ValidationOnlyFields

        [AssertTrue(Message = "One or more invalid emails for call detected")]
        private bool EmailsForCallList
        {
            get
            {
                if (Emails != null && IsActive)
                {
                    foreach (var emailsForCall in Emails)
                    {
                        if (!emailsForCall.IsValid())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [AssertTrue(Message = "One or more invalid email templates detected")]
        private bool EmailTemplatesList
        {
            get
            {
                if (EmailTemplates != null && IsActive)
                {
                    foreach (var emailTemplates in EmailTemplates)
                    {
                        if (!emailTemplates.IsValid())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [AssertTrue(Message = "One or more invalid editors or reviewers detected")]
        private bool EditorsList
        {
            get
            {
                if (Editors != null && IsActive)
                {
                    foreach (var editor in Editors)
                    {
                        if (!editor.IsValid())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [AssertTrue(Message = "One or more invalid questions detected")]
        private bool QuestionsList
        {
            get
            {
                if (Questions != null && IsActive)
                {
                    foreach (var question in Questions)
                    {
                        if (!question.IsValid())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [AssertTrue(Message = "Owner is required")]
        private bool Owner
        {
            get
            {
                if (Editors == null)
                {
                    return true; //Other check gets this.
                }
                foreach (var editor in Editors)
                {
                    if (editor.IsOwner)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion ValidationOnlyFields

    }

    public class CallForProposalMap : ClassMap<CallForProposal>
    {
        public CallForProposalMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);
            Map(x => x.CreatedDate);
            Map(x => x.EndDate);
            Map(x => x.CallsSentDate);
            Map(x => x.ProposalMaximum);
            References(x => x.TemplateGeneratedFrom);
            HasMany(x => x.Emails).Inverse().Cascade.AllDeleteOrphan();
            HasMany(x => x.EmailTemplates).Inverse().Cascade.AllDeleteOrphan();
            HasMany(x => x.Editors).Inverse().Cascade.AllDeleteOrphan();
            HasMany(x => x.Questions).Inverse().Cascade.AllDeleteOrphan();
            HasMany(x => x.Proposals).Cascade.None();
        }
    }
}
