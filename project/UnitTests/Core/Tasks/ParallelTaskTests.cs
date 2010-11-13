using System;
using System.Collections.Generic;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
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
        private MockRepository mocks = new MockRepository();

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
            var logger = mocks.DynamicMock<ILogger>();
            var result = GenerateResultMock(false);
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
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
            var logger = mocks.DynamicMock<ILogger>();
            var result = GenerateResultMock(false);
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
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
            var logger = mocks.DynamicMock<ILogger>();
            var result = GenerateResultMock(true);
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
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
            var errorProcessor = mocks.StrictMock<IConfigurationErrorProcesser>();
            mocks.ReplayAll();

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
            var errorProcessor = mocks.StrictMock<IConfigurationErrorProcesser>();
            Expect.Call(() =>
            {
                errorProcessor.ProcessWarning(string.Empty);
            }).IgnoreArguments();
            mocks.ReplayAll();

            task.Validate(null, ConfigurationTrace.Start(project), errorProcessor);
            mocks.VerifyAll();
        }
        #endregion

        #region Private methods
        private IIntegrationResult GenerateResultMock(bool forException)
        {
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            SetupResult.For(result.ProjectName).Return("Project name");
            for (var loop = 1; loop <= (forException ? 0 : 5); loop++)
            {
                Expect.Call(() => { result.AddTaskResult(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Task #{0} has run", loop)); });
            }
            Expect.Call(result.Status).PropertyBehavior();
            Expect.Call(result.Clone()).Return(result).Repeat.Times((forException ? 1 : 5));
            Expect.Call(() => { result.Merge(result); }).Repeat.Times((forException ? 1 : 5));

            if (forException)
            {
                Expect.Call(result.ExceptionResult).PropertyBehavior();
            }
            return result;
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
