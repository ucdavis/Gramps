using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using Gramps.Core.Domain;
using Gramps.Tests.Core;
using Gramps.Tests.Core.Extensions;
using Gramps.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests.QuestionRepositoryTests
{

    public partial class QuestionRepositoryTests
    {

        #region ValidationClasses

        [TestMethod]
        public void TestValidationClassesReturnsExpectedResult1()
        {
            #region Arrange
            var record = GetValid(9);
            record.Addvalidators(Repository.OfType<Validator>().GetNullableById(1));
            record.Addvalidators(Repository.OfType<Validator>().GetNullableById(2));
            record.Addvalidators(Repository.OfType<Validator>().GetNullableById(3));
            #endregion Arrange

            #region Act
            var result = record.ValidationClasses;
            #endregion Act

            #region Assert
            Assert.AreEqual("required email url", result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestValidationClassesReturnsExpectedResult2()
        {
            #region Arrange
            var record = GetValid(9);
            record.Addvalidators(Repository.OfType<Validator>().GetNullableById(1));
            //record.Addvalidators(Repository.OfType<Validator>().GetNullableById(2));
            record.Addvalidators(Repository.OfType<Validator>().GetNullableById(3));
            #endregion Arrange

            #region Act
            var result = record.ValidationClasses;
            #endregion Act

            #region Assert
            Assert.AreEqual("required url", result);
            #endregion Assert
        }

        [TestMethod]
        public void TestValidationClassesReturnsExpectedResult3()
        {
            #region Arrange
            var record = GetValid(9);
            record.Addvalidators(Repository.OfType<Validator>().GetNullableById(1));
            //record.Addvalidators(Repository.OfType<Validator>().GetNullableById(2));
            //record.Addvalidators(Repository.OfType<Validator>().GetNullableById(3));
            #endregion Arrange

            #region Act
            var result = record.ValidationClasses;
            #endregion Act

            #region Assert
            Assert.AreEqual("required", result);
            #endregion Assert
        }

        [TestMethod]
        public void TestValidationClassesReturnsExpectedResult4()
        {
            #region Arrange
            var record = GetValid(9);
            //record.Addvalidators(Repository.OfType<Validator>().GetNullableById(1));
            //record.Addvalidators(Repository.OfType<Validator>().GetNullableById(2));
            //record.Addvalidators(Repository.OfType<Validator>().GetNullableById(3));
            #endregion Arrange

            #region Act
            var result = record.ValidationClasses;
            #endregion Act

            #region Assert
            Assert.AreEqual("", result);
            #endregion Assert
        }
        #endregion ValidationClasses

        #region OptionChoices

        [TestMethod]
        public void TestOptionChoicesReturnsExpectedResults1()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddQuestionOption(CreateValidEntities.QuestionOption(3));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(5));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(7));
            #endregion Arrange

            #region Act
            var result = record.OptionChoices;
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 Name5 Name7", result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestOptionChoicesReturnsExpectedResults2()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddQuestionOption(CreateValidEntities.QuestionOption(3));
            //record.AddQuestionOption(CreateValidEntities.QuestionOption(5));
            record.AddQuestionOption(CreateValidEntities.QuestionOption(7));
            #endregion Arrange

            #region Act
            var result = record.OptionChoices;
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3 Name7", result);
            #endregion Assert
        }

        [TestMethod]
        public void TestOptionChoicesReturnsExpectedResults3()
        {
            #region Arrange
            var record = GetValid(9);
            record.AddQuestionOption(CreateValidEntities.QuestionOption(3));
            //record.AddQuestionOption(CreateValidEntities.QuestionOption(5));
            //record.AddQuestionOption(CreateValidEntities.QuestionOption(7));
            #endregion Arrange

            #region Act
            var result = record.OptionChoices;
            #endregion Act

            #region Assert
            Assert.AreEqual("Name3", result);
            #endregion Assert
        }

        [TestMethod]
        public void TestOptionChoicesReturnsExpectedResults4()
        {
            #region Arrange
            var record = GetValid(9);
            //record.AddQuestionOption(CreateValidEntities.QuestionOption(3));
            //record.AddQuestionOption(CreateValidEntities.QuestionOption(5));
            //record.AddQuestionOption(CreateValidEntities.QuestionOption(7));
            #endregion Arrange

            #region Act
            var result = record.OptionChoices;
            #endregion Act

            #region Assert
            Assert.AreEqual("", result);
            #endregion Assert
        }

        #endregion OptionChoices

        #region Constructor Tests

        [TestMethod]
        public void TestConstructorSetsExpectedValues()
        {
            #region Arrange
            var record = new Question();
            #endregion Arrange

            #region Assert
            Assert.IsNotNull(record.Validators);
            Assert.AreEqual(0, record.Validators.Count);
            Assert.IsNotNull(record.Options);
            Assert.AreEqual(0, record.Options.Count);
            Assert.IsNull(record.QuestionType);
            #endregion Assert		
        }
        #endregion Constructor Tests

        #region Fluent Mapping Tests
        [TestMethod]
        public void TestCanCorrectlyMapQuestion1()
        {
            #region Arrange
            var id = QuestionRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var questionType = Repository.OfType<QuestionType>().GetNullableById(2);
            var template = Repository.OfType<Template>().Queryable.First();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Question>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.CallForProposal, null)
                .CheckProperty(c => c.QuestionType, questionType)
                .CheckProperty(c => c.Order, 1)
                .CheckProperty(c => c.Template, template)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapQuestion2()
        {
            #region Arrange
            var id = QuestionRepository.Queryable.Max(x => x.Id) + 1;
            var session = NHibernateSessionManager.Instance.GetSession();
            var questionType = Repository.OfType<QuestionType>().GetNullableById(2);
            var template = Repository.OfType<Template>().Queryable.First();
            var call = Repository.OfType<CallForProposal>().Queryable.First();
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Question>(session)
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.CallForProposal, call)
                .CheckProperty(c => c.QuestionType, questionType)
                .CheckProperty(c => c.Order, 12)
                .CheckProperty(c => c.Template, null)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapQuestion3()
        {
            #region Arrange
            var id = QuestionRepository.Queryable.Max(x => x.Id) + 1;
            var question = CreateValidEntities.Question(1);
            question.SetIdTo(id);
            var session = NHibernateSessionManager.Instance.GetSession();
            var questionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name=="Radio Buttons").Single();
            var template = Repository.OfType<Template>().Queryable.First();
            var call = Repository.OfType<CallForProposal>().Queryable.First();
            var options = new List<QuestionOption>();
            for (int i = 0; i < 3; i++)
            {
                options.Add(CreateValidEntities.QuestionOption(i+1));
                options[i].Question = question;
            }
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Question>(session, new QuestionEqualityComparer())
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.CallForProposal, call)
                .CheckProperty(c => c.QuestionType, questionType)
                .CheckProperty(c => c.Order, 12)
                .CheckProperty(c => c.Options, options)
                .VerifyTheMappings();
            #endregion Act/Assert
        }

        [TestMethod]
        public void TestCanCorrectlyMapQuestion4()
        {
            #region Arrange
            var id = QuestionRepository.Queryable.Max(x => x.Id) + 1;
            var question = CreateValidEntities.Question(1);
            question.SetIdTo(id);
            var session = NHibernateSessionManager.Instance.GetSession();
            var questionType = Repository.OfType<QuestionType>().Queryable.Where(a => a.Name == "Radio Buttons").Single();
            var template = Repository.OfType<Template>().Queryable.First();
            var call = Repository.OfType<CallForProposal>().Queryable.First();
            var validators = new List<Validator>();
            for (int i = 0; i < 3; i++)
            {
                validators.Add(Repository.OfType<Validator>().GetNullableById(i+1));
            }
            #endregion Arrange

            #region Act/Assert
            new PersistenceSpecification<Question>(session, new QuestionEqualityComparer())
                .CheckProperty(c => c.Id, id)
                .CheckProperty(c => c.Name, "Name")
                .CheckProperty(c => c.CallForProposal, call)
                .CheckProperty(c => c.QuestionType, questionType)
                .CheckProperty(c => c.Order, 12)
                .CheckProperty(c => c.Validators, validators)
                .VerifyTheMappings();
            #endregion Act/Assert
        }
        #endregion Fluent Mapping Tests

        public class QuestionEqualityComparer : IEqualityComparer
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

                if (x is IList<QuestionOption> && y is IList<QuestionOption>)
                {
                    var xVal = (IList<QuestionOption>)x;
                    var yVal = (IList<QuestionOption>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Name, yVal[i].Name);
                    }
                    return true;
                }

                if (x is IList<Validator> && y is IList<Validator>)
                {
                    var xVal = (IList<Validator>)x;
                    var yVal = (IList<Validator>)y;
                    Assert.AreEqual(xVal.Count, yVal.Count);
                    for (int i = 0; i < xVal.Count; i++)
                    {
                        Assert.AreEqual(xVal[i].Name, yVal[i].Name);
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
