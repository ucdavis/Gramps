using System;
using System.Web.Mvc;
using Gramps.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using UCDArch.Web.Helpers;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Gramps.Controllers
{
    /// <summary>
    /// Controller for the Investigator class
    /// </summary>
    public class InvestigatorController : ApplicationController
    {
	    private readonly IRepository<Investigator> _investigatorRepository;

        public InvestigatorController(IRepository<Investigator> investigatorRepository)
        {
            _investigatorRepository = investigatorRepository;
        }
    
        //
        // GET: /Investigator/
        public ActionResult Index()
        {
            var investigatorList = _investigatorRepository.Queryable;

            return View(investigatorList);
        }

        //
        // GET: /Investigator/Details/5
        public ActionResult Details(int id)
        {
            var investigator = _investigatorRepository.GetNullableById(id);

            if (investigator == null) return this.RedirectToAction(a => a.Index());

            return View(investigator);
        }

        //
        // GET: /Investigator/Create
        public ActionResult Create()
        {
			var viewModel = InvestigatorViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Investigator/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Investigator investigator)
        {
            var investigatorToCreate = new Investigator();

            TransferValues(investigator, investigatorToCreate);

            investigatorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _investigatorRepository.EnsurePersistent(investigatorToCreate);

                Message = "Investigator Created Successfully";

                return this.RedirectToAction(a => a.Index());
            }
            else
            {
				var viewModel = InvestigatorViewModel.Create(Repository);
                viewModel.Investigator = investigator;

                return View(viewModel);
            }
        }

        //
        // GET: /Investigator/Edit/5
        public ActionResult Edit(int id)
        {
            var investigator = _investigatorRepository.GetNullableById(id);

            if (investigator == null) return this.RedirectToAction(a => a.Index());

			var viewModel = InvestigatorViewModel.Create(Repository);
			viewModel.Investigator = investigator;

			return View(viewModel);
        }
        
        //
        // POST: /Investigator/Edit/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(int id, Investigator investigator)
        {
            var investigatorToEdit = _investigatorRepository.GetNullableById(id);

            if (investigatorToEdit == null) return this.RedirectToAction(a => a.Index());

            TransferValues(investigator, investigatorToEdit);

            investigatorToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _investigatorRepository.EnsurePersistent(investigatorToEdit);

                Message = "Investigator Edited Successfully";

                return this.RedirectToAction(a => a.Index());
            }
            else
            {
				var viewModel = InvestigatorViewModel.Create(Repository);
                viewModel.Investigator = investigator;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Investigator/Delete/5 
        public ActionResult Delete(int id)
        {
			var investigator = _investigatorRepository.GetNullableById(id);

            if (investigator == null) return this.RedirectToAction(a => a.Index());

            return View(investigator);
        }

        //
        // POST: /Investigator/Delete/5
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, Investigator investigator)
        {
			var investigatorToDelete = _investigatorRepository.GetNullableById(id);

            if (investigatorToDelete == null) this.RedirectToAction(a => a.Index());

            _investigatorRepository.Remove(investigatorToDelete);

            Message = "Investigator Removed Successfully";

            return this.RedirectToAction(a => a.Index());
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Investigator source, Investigator destination)
        {
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the Investigator class
    /// </summary>
    public class InvestigatorViewModel
	{
		public Investigator Investigator { get; set; }
 
		public static InvestigatorViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new InvestigatorViewModel {Investigator = new Investigator()};
 
			return viewModel;
		}
	}
}
