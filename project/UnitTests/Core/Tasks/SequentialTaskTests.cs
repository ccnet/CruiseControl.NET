using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class SequentialTaskTests
    {
        private MockRepository mocks = new MockRepository(MockBehavior.Default);

        #region Test methods
        [Test]
        public void ExecuteRunsMultipleSuccessfulTasks()
        {
            // Initialise the task
            var subTasks = new List<SequentialTestTask>();
            for (var loop = 1; loop <= 5; loop++)
            {
                subTasks.Add(new SequentialTestTask { TaskNumber = loop, Result = IntegrationStatus.Success });
            }
            var task = new SequentialTask
            {
                Tasks = subTasks.ToArray()
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;
            var result = GenerateResultMock(5);

            // Run the actual task
            task.Run(result);

            // Verify the results
            VerifyResultMock(result, 5);
            mocks.Verify();
            Assert.AreEqual(IntegrationStatus.Success, result.Status, "Status does not match");
        }

        [Test]
        public void ExecuteStopsOnFirstFailure()
        {
            // Initialise the task
            var subTasks = new List<SequentialTestTask>();
            for (var loop = 1; loop <= 5; loop++)
            {
                subTasks.Add(new SequentialTestTask { TaskNumber = loop, Result = loop > 3 ? IntegrationStatus.Failure : IntegrationStatus.Success });
            }
            var task = new SequentialTask
            {
                Tasks = subTasks.ToArray()
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;
            var result = GenerateResultMock(4);

            // Run the actual task
            task.Run(result);

            // Verify the results
            VerifyResultMock(result, 4);
            mocks.Verify();
            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
        }

        [Test]
        public void ExecuteIgnoresFailures()
        {
            // Initialise the task
            var subTasks = new List<SequentialTestTask>();
            for (var loop = 1; loop <= 5; loop++)
            {
                subTasks.Add(new SequentialTestTask { TaskNumber = loop, Result = loop > 3 ? IntegrationStatus.Failure : IntegrationStatus.Success });
            }
            var task = new SequentialTask
            {
                Tasks = subTasks.ToArray(),
                ContinueOnFailure = true
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;
            var result = GenerateResultMock(5);

            // Run the actual task
            task.Run(result);

            // Verify the results
            VerifyResultMock(result, 5);
            mocks.Verify();
            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
        }

        [Test]
        public void ExecuteHandlesAnExceptionInATask()
        {
            // Initialise the task
            var task = new SequentialTask
            {
                Tasks = new ITask[] 
                {
                    new ExceptionTestTask()
                }
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;
            var result = GenerateResultMock(0);
            Mock.Get(result).SetupSet(_result => _result.ExceptionResult = It.IsAny<Exception>()).Verifiable();
            Mock.Get(result).SetupProperty(_result => _result.ExceptionResult);

            // Run the actual task
            task.Run(result);

            // Verify the results
            VerifyResultMock(result, 0);
            mocks.Verify();
            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
        }

        [Test]
        public void ExecuteIgnoreFailureStillRunInnerSequentialTasks()
        {
            const int innerCount = 3;
            const int leafCount = 2;

            // Initialise the task
            var innerTasks = new List<SequentialTask>();
            for (var innerLoop = 1; innerLoop <= innerCount; innerLoop++)
            {
                var leafTasks = new List<SequentialTestTask>();
                for (var leafLoop = 1; leafLoop <= leafCount; leafLoop++)
                    leafTasks.Add(new SequentialTestTask { TaskNumber = innerLoop * 10 + leafLoop, Result = (innerLoop == 2) && (leafLoop == 2) ? IntegrationStatus.Failure : IntegrationStatus.Success });

                innerTasks.Add(new SequentialTask { ContinueOnFailure = false, Tasks = leafTasks.ToArray() });
            }
            var task = new SequentialTask
            {
                Tasks = innerTasks.ToArray(),
                ContinueOnFailure = true
            };

            // Setup the mocks
            var logger = mocks.Create<ILogger>().Object;

            // We cannot use a mock object here because having Clone return the original result means the test 
            // will not be able to catch the error we are after.
            var result = new IntegrationResult();
            result.ProjectName = ""; // must set to an empty string because the null default value makes Clone crash

            // Run the actual task
            task.Run(result);

            // Verify the results
            mocks.Verify();
            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
            Assert.AreEqual(innerCount * leafCount, result.TaskResults.Count, "Bad task results count");
        }

        private IIntegrationResult GenerateResultMock(int runCount)
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            for (var loop = 1; loop <= runCount; loop++)
            {
                string taskResult = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Task #{0} has run", loop);
                Mock.Get(result).Setup(_result => _result.AddTaskResult(taskResult)).Verifiable();
            }
            Mock.Get(result).SetupSet(_result => _result.Status = It.IsAny<IntegrationStatus>()).Verifiable();
            Mock.Get(result).SetupProperty(_result => _result.Status);
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result).Verifiable();
            if(runCount > 0) Mock.Get(result).Setup(_result => _result.Merge(result)).Verifiable();
            return result;
        }
        private void VerifyResultMock(IIntegrationResult result, int runCount)
        {
            Mock.Get(result).Verify(_result => _result.Clone(), Times.Exactly(runCount == 0 ? 1 : runCount));
            if(runCount > 0) Mock.Get(result).Verify(_result => _result.Merge(result), Times.Exactly(runCount));
        }
        #endregion

        #region Private classes
        private class SequentialTestTask
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
