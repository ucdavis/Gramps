using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;

using UCDArch.Core.Utils;

namespace Gramps.Services
{
    public interface IAccessService
    {
        bool HasAccess(int? templateId, int? callForProposalId, string userId);
        bool HasSameId(Template template, CallForProposal callForProposal, int? templateId, int? callForProposalId);
    }

    public class AccessService : IAccessService
    {
        private readonly IRepository _repository;

        public AccessService(IRepository repository)
        {
            _repository = repository;
        }

        public virtual bool HasAccess(int? templateId, int? callForProposalId, string userId)
        {
            if (templateId !=null && templateId != 0)
            {
                var template = _repository.OfType<Template>().GetNullableById(templateId.Value);
                Check.Require(template != null, "Template is required");
                return template.IsEditor(userId);
            }
            if (callForProposalId != null && callForProposalId != 0)
            {
                var callForProposal = _repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                Check.Require(callForProposal != null, "CallForProposal is required");
                return callForProposal.IsEditor(userId); 
            }
            return false;
        }

        public virtual bool HasSameId(Template template, CallForProposal callForProposal, int? templateId, int? callForProposalId)
        {
            if (template != null)
            {
                if (templateId == null || templateId != template.Id)
                {
                    return false;
                }
            }
            if (callForProposal != null)
            {
                if (callForProposalId == null || callForProposalId != callForProposal.Id)
                {
                    return false;
                }
            }
            if (template == null && callForProposal == null)
            {
                return false;
            }

            return true;
        }
    }
}