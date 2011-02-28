using System.Collections.Generic;
using System.Linq;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    /// <summary>
    /// Entity Name:		CallForProposal
    /// LookupFieldName:	Name yrjuy
    /// </summary>
    [TestClass]
    public partial class CallForProposalRepositoryTests : AbstractRepositoryTests<CallForProposal, int, CallForProposalMap>
    {
        /// <summary>
        /// Gets or sets the CallForProposal repository.
        /// </summary>
        /// <value>The CallForProposal repository.</value>
        public IRepository<CallForProposal> CallForProposalRepository { get; set; }

        public IRepository<User> UserRepository { get; set; }
        
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="CallForProposalRepositoryTests"/> class.
        /// </summary>
        public CallForProposalRepositoryTests()
        {
            CallForProposalRepository = new Repository<CallForProposal>();
            UserRepository = new Repository<User>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override CallForProposal GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.CallForProposal(counter);
            var editor = new Editor();
            editor.User = UserRepository.GetNullableById(2);
            editor.IsOwner = true;
            rtValue.AddEditor(editor);

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<CallForProposal> GetQuery(int numberAtEnd)
        {
            return CallForProposalRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(CallForProposal entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(CallForProposal entity, ARTAction action)
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
            UserRepository.DbContext.BeginTransaction();
            LoadUsers(3);
            UserRepository.DbContext.CommitTransaction();
            CallForProposalRepository.DbContext.BeginTransaction();            
            LoadRecords(5);
            CallForProposalRepository.DbContext.CommitTransaction();
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

            expectedFields.Add(new NameAndType("CallsSentDate", "System.Nullable`1[System.DateTime]", new List<string>()));
            expectedFields.Add(new NameAndType("CreatedDate", "System.DateTime", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Editors", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Editor]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("EditorsList", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One or more invalid editors or reviewers detected\")]"
            }));
            expectedFields.Add(new NameAndType("Emails", "System.Collections.Generic.IList`1[Gramps.Core.Domain.EmailsForCall]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("EmailsForCallList", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One or more invalid emails for call detected\")]"
            }));
            expectedFields.Add(new NameAndType("EmailTemplates", "System.Collections.Generic.IList`1[Gramps.Core.Domain.EmailTemplate]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("EmailTemplatesList", "System.Boolean", new List<string>
            {
                "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One or more invalid email templates detected\")]"
            }));
            expectedFields.Add(new NameAndType("EndDate", "System.DateTime", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]",
                "[System.ComponentModel.DataAnnotations.DisplayFormatAttribute(DataFormatString = \"{0:MM/dd/yyyy}\", ApplyFormatInEditMode = True)]"
            }));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Owner", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"Owner is required\")]"
            }));
            expectedFields.Add(new NameAndType("ProposalMaximum", "System.Decimal", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.DisplayFormatAttribute(ApplyFormatInEditMode = True, DataFormatString = \"{0:C}\")]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RangeDoubleAttribute(Min = 0, Message = \"Minimum amount is one cent\")]"
            }));
            expectedFields.Add(new NameAndType("Proposals", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Proposal]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Questions", "System.Collections.Generic.IList`1[Gramps.Core.Domain.Question]", new List<string>
            {
                "[NHibernate.Validator.Constraints.NotNullAttribute()]"
            }));
            expectedFields.Add(new NameAndType("QuestionsList", "System.Boolean", new List<string>
            {
                 "[NHibernate.Validator.Constraints.AssertTrueAttribute(Message = \"One or more invalid questions detected\")]"
            }));
            expectedFields.Add(new NameAndType("TemplateGeneratedFrom", "Gramps.Core.Domain.Template", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(CallForProposal));

        }

        #endregion Reflection of Database.	
		
		
    }
}