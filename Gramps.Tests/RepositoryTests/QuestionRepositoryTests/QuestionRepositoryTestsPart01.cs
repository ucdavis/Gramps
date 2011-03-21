using System;
using Gramps.Core.Domain;
using Gramps.Tests.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace Gramps.Tests.RepositoryTests.QuestionRepositoryTests
{

    public partial class QuestionRepositoryTests
    {
        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = null;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = string.Empty;
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = " ";
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            Question question = null;
            try
            {
                #region Arrange
                question = GetValid(9);
                question.Name = "x".RepeatTimes((500 + 1));
                #endregion Arrange

                #region Act
                QuestionRepository.DbContext.BeginTransaction();
                QuestionRepository.EnsurePersistent(question);
                QuestionRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(question);
                Assert.AreEqual(500 + 1, question.Name.Length);
                var results = question.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 500");
                Assert.IsTrue(question.IsTransient());
                Assert.IsFalse(question.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Name = "x";
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var question = GetValid(9);
            question.Name = "x".RepeatTimes(500);
            #endregion Arrange

            #region Act
            QuestionRepository.DbContext.BeginTransaction();
            QuestionRepository.EnsurePersistent(question);
            QuestionRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(500, question.Name.Length);
            Assert.IsFalse(question.IsTransient());
            Assert.IsTrue(question.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests
    }
}