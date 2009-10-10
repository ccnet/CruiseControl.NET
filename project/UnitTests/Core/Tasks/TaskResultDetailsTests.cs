//-----------------------------------------------------------------------
// <copyright file="TaskResultDetailsTests.cs" company="Craig Sutherland">
//     Copyright (c) 2009 Craig Sutherland. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Tasks;

    /// <summary>
    /// Unit tests for <see cref="TaskResultDetails"/>.
    /// </summary>
    [TestFixture]
    public class TaskResultDetailsTests
    {
        #region Private fields
        #region mocks
        /// <summary>
        /// The mocks repository.
        /// </summary>
        private MockRepository mocks;
        #endregion
        #endregion

        #region Setup
        /// <summary>
        /// Initialises the mocks for each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        #region Constructor() tests
        /// <summary>
        /// Tests that the constructor validates all arguments.
        /// </summary>
        [Test]
        public void ConstructorValidatesAllArguments()
        {
            Assert.AreEqual(
                "taskName",
                Assert.Throws<ArgumentException>(() => new TaskResultDetails(null, "type", "file")).ParamName);
            Assert.AreEqual(
                "taskName",
                Assert.Throws<ArgumentException>(() => new TaskResultDetails(string.Empty, "type", "file")).ParamName);

            Assert.AreEqual(
                "taskType",
                Assert.Throws<ArgumentException>(() => new TaskResultDetails("name", null, "file")).ParamName);
            Assert.AreEqual(
                "taskType",
                Assert.Throws<ArgumentException>(() => new TaskResultDetails("name", string.Empty, "file")).ParamName);

            Assert.AreEqual(
                "fileName",
                Assert.Throws<ArgumentException>(() => new TaskResultDetails("name", "type", null)).ParamName);
            Assert.AreEqual(
                "fileName",
                Assert.Throws<ArgumentException>(() => new TaskResultDetails("name", "type", string.Empty)).ParamName);
        }

        /// <summary>
        /// Tests that the properties are set from the contructor.
        /// </summary>
        [Test]
        public void ConstructorSetProperties()
        {
            var taskName = "Test Task";
            var taskType = "Test Type";
            var fileName = "Name of file";
            var results = new TaskResultDetails(taskName, taskType, fileName);
            Assert.AreEqual(taskName, results.TaskName);
            Assert.AreEqual(taskType, results.TaskType);
            Assert.AreEqual(fileName, results.FileName);
        }
        #endregion
        #endregion
    }
}
