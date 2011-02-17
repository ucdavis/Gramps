using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Gramps.Controllers.ViewModels
{
    /// <summary>
    /// ViewModel for the EmailQueue class
    /// </summary>
    public class EmailQueueViewModel : CallNavigationViewModel
    {
        public EmailQueue EmailQueue { get; set; }

        public static EmailQueueViewModel Create(IRepository repository, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new EmailQueueViewModel { EmailQueue = new EmailQueue(), CallForProposal = callForProposal};

            return viewModel;
        }
    }

    /// <summary>
    /// ViewModel for the EmailQueue class
    /// </summary>
    public class EmailQueueListViewModel : CallNavigationViewModel
    {
        public IEnumerable<EmailQueue> EmailQueues { get; set; }

        public static EmailQueueListViewModel Create(IRepository repository, CallForProposal callForProposal)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new EmailQueueListViewModel{CallForProposal = callForProposal};
            viewModel.EmailQueues = repository.OfType<EmailQueue>().Queryable.Where(a => a.CallForProposal == callForProposal);

            return viewModel;
        }
    }
}