using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;


namespace Gramps.Tests.RepositoryTests.CallForProposalRepositoryTests
{
    public partial class CallForProposalRepositoryTests
    {
        #region Constructor Tests

        [TestMethod]
        public void TestToDoConstructorTests()
        {
            #region Arrange
            Assert.Inconclusive("Once Template tests are done, add the constructor tests for Call For Proposals");
            

            #endregion Arrange

            #region Act

            #endregion Act

            #region Assert

            #endregion Assert		
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapCallForProposal1()
        {
            #region Arrange
            var id = CallForProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var compareDate1 = new DateTime(2010, 12, 14);
            var compareDate2 = new DateTime(2010, 12, 15);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<CallForProposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.IsActive, true)
                .CheckProperty(c => c.CreatedDate, compareDate1)
                .CheckProperty(c => c.EndDate, compareDate2)
                .CheckProperty(c => c.CallsSentDate, null)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapCallForProposal2()
        {
            #region Arrange
            var id = CallForProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var compareDate1 = new DateTime(2010, 12, 14);
            var compareDate2 = new DateTime(2010, 12, 15);
            var compareDate3 = new DateTime(2010, 12, 16);
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<CallForProposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.IsActive, false)
                .CheckProperty(c => c.CreatedDate, compareDate1)
                .CheckProperty(c => c.EndDate, compareDate2)
                .CheckProperty(c => c.CallsSentDate, compareDate3)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapCallForProposal3()
        {
            #region Arrange
            var id = CallForProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<CallForProposal>(session)
                .CheckProperty(c => c.Id, id)

                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapCallForProposal4()
        {
            #region Arrange
            var id = CallForProposalRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            Repository.OfType<Template>().DbContext.BeginTransaction();
            var template = CreateValidEntities.Template(1);
            Repository.OfType<Template>().EnsurePersistent(template);
            Repository.OfType<Template>().DbContext.CommitTransaction();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<CallForProposal>(session)
                .CheckProperty(c => c.Id, id)
                .CheckReference(c => c.TemplateGeneratedFrom, template)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapCallForProposal5()
        {
            #region Arrange
            var id = CallForProposalRepository.Queryable.Max(x => x.Id) + 1;
            var dummyCall = CreateValidEntities.CallForProposal(1);
            var session = NHibernateSessionManager.Instance.GetSession();

            #region Emails
            var emails = new List<EmailsForCall>();
            emails.Add(new EmailsForCall("test1@testy.com"));
            emails.Add(new EmailsForCall("test2@testy.com"));
            
            dummyCall.SetIdTo(id);
            emails[0].CallForProposal = dummyCall;
            emails[1].CallForProposal = dummyCall;
            #endregion Emails
            #region Email Templates
            var emailTemplates = new List<EmailTemplate>();
            emailTemplates.Add(CreateValidEntities.EmailTemplate(1));
            emailTemplates.Add(CreateValidEntities.EmailTemplate(2));
            emailTemplates[0].CallForProposal = dummyCall;
            emailTemplates[1].CallForProposal = dummyCall;
            #endregion Email Templates
            #region Editors
            var editors = new List<Editor>();
            editors.Add(CreateValidEntities.Editor(1));
            editors.Add(CreateValidEntities.Editor(2));
            editors[0].CallForProposal = dummyCall;
            editors[1].CallForProposal = dummyCall;
            #endregion Editors
            #region Questions
            Repository.OfType<QuestionType>().DbContext.BeginTransaction();
            LoadQuestionTypes();
            Repository.OfType<QuestionType>().DbContext.CommitTransaction();
            var questions = new List<Question>();
            questions.Add(new Question { Name = "Name1", CallForProposal = dummyCall });
            questions.Add(new Question { Name = "Name2", CallForProposal = dummyCall });
            #endregion Questions
            #region Proposals
            Repository.OfType<Proposal>().DbContext.BeginTransaction();
            var proposals = new List<Proposal>();
            proposals.Add(CreateValidEntities.Proposal(1));
            proposals.Add(CreateValidEntities.Proposal(2));
            proposals.Add(CreateValidEntities.Proposal(3));
            proposals[0].CallForProposal = dummyCall;
            proposals[1].CallForProposal = dummyCall;
            proposals[2].CallForProposal = dummyCall;
            foreach (var proposal in proposals)
            {
                Repository.OfType<Proposal>().EnsurePersistent(proposal);
            }
            Repository.OfType<Proposal>().DbContext.CommitTransaction();

            #endregion Proposals

            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<CallForProposal>(session, new CallForProposalEqualityComparer())
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Emails, emails)
                .CheckProperty(c => c.EmailTemplates, emailTemplates)
                .CheckProperty(c => c.Editors, editors)
                .CheckProperty(c => c.Questions, questions)
                .CheckProperty(c => c.Proposals, proposals)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        #endregion Fluent Mapping Tests

        public class CallForProposalEqualityComparer : IEqualityComparer
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param><exception cref="T:System.ArgumentException"><paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.</exception>
            bool IEqualityComparer.Equals(object x, object y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                if (x is IList<EmailsForCall> && y is IList<EmailsForCall>)
                {
                    var xVal = (IList<EmailsForCall>)x;
                    var yVal = (IList<EmailsForCall>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Email, yVal[i].Email);
                    }
                    return true;
                }
                if (x is IList<EmailTemplate> && y is IList<EmailTemplate>)
                {
                    var xVal = (IList<EmailTemplate>)x;
                    var yVal = (IList<EmailTemplate>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Subject, yVal[i].Subject);
                    }
                    return true;
                }
                if (x is IList<Editor> && y is IList<Editor>)
                {
                    var xVal = (IList<Editor>)x;
                    var yVal = (IList<Editor>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].ReviewerName, yVal[i].ReviewerName);
                    }
                    return true;
                }
                if (x is IList<Question> && y is IList<Question>)
                {
                    var xVal = (IList<Question>)x;
                    var yVal = (IList<Question>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Name, yVal[i].Name);
                    }
                    return true;
                }

                if (x is IList<Proposal> && y is IList<Proposal>)
                {
                    var xVal = (IList<Proposal>)x;
                    var yVal = (IList<Proposal>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Email, yVal[i].Email);
                    }
                    return true;
                }
                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}