using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Investigator
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class InvestigatorRepositoryTests : AbstractRepositoryTests<Investigator, int, InvestigatorMap>
    {
        /// <summary>
        /// Gets or sets the Investigator repository.
        /// </summary>
        /// <value>The Investigator repository.</value>
        public IRepository<Investigator> InvestigatorRepository { get; set; }
        public IRepository<CallForProposal> CallForProposalRepository { get; set; }
        public IRepository<Proposal> ProposalRepository { get; set; }
        public IRepository<User> UserRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="InvestigatorRepositoryTests"/> class.
        /// </summary>
        public InvestigatorRepositoryTests()
        {
            InvestigatorRepository = new Repository<Investigator>();
            CallForProposalRepository = new Repository<CallForProposal>();
            ProposalRepository = new Repository<Proposal>();
            UserRepository = new Repository<User>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Investigator GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Investigator(counter);
            rtValue.Proposal = ProposalRepository.Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Investigator> GetQuery(int numberAtEnd)
        {
            return InvestigatorRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Investigator entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Investigator entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            ProposalRepository.DbContext.BeginTransaction();
            LoadUsers(1);
            var callForProposal = CreateValidEntities.CallForProposal(1);
            var owner = new Editor();
            owner.IsOwner = true;
            owner.User = UserRepository.Queryable.Where(a => a.Id == 1).Single();
            callForProposal.AddEditor(owner);
            for (int i = 0; i < 9; i++)
            {
                var editor = new Editor(string.Format("jason{0}@test.com", (i + 1)));
                callForProposal.AddEditor(editor);
            }
            CallForProposalRepository.EnsurePersistent(callForProposal);
            CallForProposalRepository.DbContext.CommitTransaction();

            ProposalRepository.DbContext.BeginTransaction();
            var proposal = CreateValidEntities.Proposal(1);
            proposal.CallForProposal = CallForProposalRepository.Queryable.Where(a => a.Id == 1).Single();
            ProposalRepository.EnsurePersistent(proposal);
            ProposalRepository.DbContext.CommitTransaction();
            InvestigatorRepository.DbContext.BeginTransaction();            
            LoadRecords(5);
            InvestigatorRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        
        
        
        
        
        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();

            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));

            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Investigator));

        }

        #endregion Reflection of Database.	
		
		
    }
}