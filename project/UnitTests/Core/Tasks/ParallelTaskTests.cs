using System;
using System.Collections.Generic;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    [Ignore("Ignored until a random fail on Windows and a deadlock on Unix is fixed. (Deadlock on Unix is in ParallelTaskTests.ExecuteRunsMultipleSuccessfulTasks)")]
    public class ParallelTaskTests
    {
        private MockRepository mocks = new MockRepository(MockBehavior.Default);

        #region Test methods
        [Test]
        public void ExecuteRunsMultipleSuccessfulTasks()
        {
            // Initialise the task
            var subTasks = new List<ParallelTestTask>();
            for (var loop = 1; loop <= 5; loop++)
            {
                subTasks.Add(new ParallelTestTask { TaskNumber = loop, Result = IntegrationStatus.Success });
            }
            var task = new ParallelTask
            {
                Tasks = subTasks.ToArray()
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;
            var result = GenerateResultMock(false);

            // Run the actual task
            task.Run(result);

            // Verify the results
            VerifyResultMock(result, false);
            mocks.VerifyAll();
            Assert.AreEqual(IntegrationStatus.Success, result.Status, "Status does not match");
        }

        [Test]
        public void ExecuteRunsSuccessAndFailureTasks()
        {
            // Initialise the task
            var subTasks = new List<ParallelTestTask>();
            for (var loop = 1; loop <= 5; loop++)
            {
                subTasks.Add(new ParallelTestTask { TaskNumber = loop, Result = loop >= 3 ? IntegrationStatus.Failure : IntegrationStatus.Success });
            }
            var task = new ParallelTask
            {
                Tasks = subTasks.ToArray()
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;
            var result = GenerateResultMock(false);

            // Run the actual task
            task.Run(result);

            // Verify the results
            VerifyResultMock(result, false);
            mocks.VerifyAll();
            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
        }

        [Test]
        public void ExecuteRunsHandlesExceptionTask()
        {
            // Initialise the task
            var task = new ParallelTask
            {
                Tasks = new ITask[] 
                {
                    new ExceptionTestTask()
                }
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;
            var result = GenerateResultMock(true);

            // Run the actual task
            task.Run(result);

            // Verify the results
            VerifyResultMock(result, true);
            mocks.VerifyAll();
            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
        }

        [Test]
        public void ReadMinimalConfig()
        {
            var config = @"<parallel>
    <tasks/>
</parallel>";
            var task = new ParallelTask();
            NetReflector.Read(config, task);
        }

        [Test]
        public void ReadFullConfig()
        {
            var config = @"<parallel>
    <description>Testing</description>
    <tasks/>
    <dynamicValues/>
</parallel>";
            var task = new ParallelTask();
            NetReflector.Read(config, task);
            Assert.AreEqual("Testing", task.Description);
        }

        [Test]
        public void ValidatePassesForTasksSection()
        {
            var task = new ParallelTask();
            var project = new Project
            {
                Tasks = new ITask[]
                {
                    task
                }
            };
            var errorProcessor = mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;

            task.Validate(null, ConfigurationTrace.Start(project), errorProcessor);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateFailsForPublishersSection()
        {
            var task = new ParallelTask();
            var project = new Project
            {
                Publishers = new ITask[]
                {
                    task
                }
            };
            var errorProcessor = mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            Mock.Get(errorProcessor).Setup(_errorProcessor => _errorProcessor.ProcessWarning(It.IsAny<string>())).Verifiable();

            task.Validate(null, ConfigurationTrace.Start(project), errorProcessor);
            mocks.VerifyAll();
        }
        #endregion

        #region Private methods
        private IIntegrationResult GenerateResultMock(bool forException)
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            Mock.Get(result).SetupGet(_result => _result.ProjectName).Returns("Project name");
            for (var loop = 1; loop <= (forException ? 0 : 5); loop++)
            {
                string taskResult = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Task #{0} has run", loop);
                Mock.Get(result).Setup(_result => _result.AddTaskResult(taskResult)).Verifiable();
            }
            Mock.Get(result).SetupSet(_result => _result.Status = It.IsAny<IntegrationStatus>()).Verifiable();
            Mock.Get(result).SetupProperty(_result => _result.Status);
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result).Verifiable();
            Mock.Get(result).Setup(_result => _result.Merge(result)).Verifiable();

            if (forException)
            {
                Mock.Get(result).SetupSet(_result => _result.ExceptionResult = It.IsAny<Exception>()).Verifiable();
                Mock.Get(result).SetupProperty(_result => _result.ExceptionResult);
            }
            return result;
        }
        private void VerifyResultMock(IIntegrationResult result, bool forException)
        {
            Mock.Get(result).Verify(_result => _result.Clone(), Times.Exactly(forException ? 1 : 5));
            Mock.Get(result).Verify(_result => _result.Merge(result), Times.Exactly(forException ? 1 : 5));
        }
        #endregion

        #region Private classes
        private class ParallelTestTask
            : ITask
        {
            public IntegrationStatus Result { get; set; }
            public int TaskNumber { get; set; }

            public void Run(IIntegrationResult result)
            {
                result.AddTaskResult(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Task #{0} has run", TaskNumber));
                result.Status = Result;
            }
        }

        private class ExceptionTestTask
            : ITask
        {
            public void Run(IIntegrationResult result)
            {
                throw new CruiseControlException();
            }
        }
        #endregion
    }
}
