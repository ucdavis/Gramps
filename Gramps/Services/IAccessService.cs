using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;

using UCDArch.Core.Utils;

namespace Gramps.Services
{
    public interface IAccessService
    {
        bool HasAccess(int? templateId, int? callForProposalId, string userId);
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
            if (templateId !=null)
            {
                var template = _repository.OfType<Template>().GetNullableById(templateId.Value);
                Check.Require(template != null, "Template is required");
                return template.IsEditor(userId);
            }
            if (callForProposalId != null)
            {
                var callForProposal = _repository.OfType<CallForProposal>().GetNullableById(callForProposalId.Value);
                Check.Require(callForProposal != null, "CallForProposal is required");
                return callForProposal.IsEditor(userId); 
            }
            return false;
        }
    }
}