using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gramps.Core.Domain;

namespace Gramps.Controllers.ViewModels
{

    public class NavigationViewModel
    {
        public bool IsTemplate = false;
        public bool IsCallForProposal = false;
        public int? TemplateId = 0;
        public int? CallForProposalId = 0;

        public static NavigationViewModel Create(bool isTemplate, bool isCallForProposal, int? templateId, int? callForProposalId)
        {
            return new NavigationViewModel
                       {
                           IsTemplate = isTemplate, 
                           IsCallForProposal = isCallForProposal, 
                           TemplateId = templateId, 
                           CallForProposalId = callForProposalId
                       };
        }
    }

    public class CallNavigationViewModel
    {
        public CallForProposal CallForProposal;
        public static CallNavigationViewModel Create(CallForProposal callForProposal)
        {
            return new CallNavigationViewModel{CallForProposal = callForProposal};
        }
    }
}