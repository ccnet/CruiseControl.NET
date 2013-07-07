using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class SequentialTaskTests
    {
        private MockRepository mocks = new MockRepository();

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
            var logger = mocks.DynamicMock<ILogger>();
            var result = GenerateResultMock(5);
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var result = GenerateResultMock(4);
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var result = GenerateResultMock(5);
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var result = GenerateResultMock(0);
            Expect.Call(result.ExceptionResult).PropertyBehavior();
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();

            // We cannot use a mock object here because having Clone return the original result means the test 
            // will not be able to catch the error we are after.
            var result = new IntegrationResult();
            result.ProjectName = ""; // must set to an empty string because the null default value makes Clone crash
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
            mocks.VerifyAll();
            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
            Assert.AreEqual(innerCount * leafCount, result.TaskResults.Count, "Bad task results count");
        }

        private IIntegrationResult GenerateResultMock(int runCount)
        {
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            for (var loop = 1; loop <= runCount; loop++)
            {
                Expect.Call(() => { result.AddTaskResult(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Task #{0} has run", loop)); });
            }
            Expect.Call(result.Status).PropertyBehavior();
            Expect.Call(result.Clone()).Return(result).Repeat.Times(runCount == 0 ? 1 : runCount);
            if (runCount > 0) Expect.Call(() => { result.Merge(result); }).Repeat.Times(runCount);
            return result;
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
