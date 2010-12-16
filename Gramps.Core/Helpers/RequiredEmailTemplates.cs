using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gramps.Core.Domain;

namespace Gramps.Core.Helpers
{
    public class RequiredEmailTemplates
    {
        public static Dictionary<EmailTemplateType, bool> GetRequiredEmailTemplates()
        {
            var emailTemplates = new Dictionary<EmailTemplateType ,bool>
                                     {
                                         {EmailTemplateType.InitialCall, false},
                                         {EmailTemplateType.ReadyForReview, false},
                                         {EmailTemplateType.ReminderCallIsAboutToClose, false},
                                         {EmailTemplateType.ProposalConfirmation, false},
                                         {EmailTemplateType.ProposalApproved, false},
                                         {EmailTemplateType.ProposalDenied, false}
                                     };

            return emailTemplates;
        }
    }
}
