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
    public class ParallelTaskTests
    {
        private MockRepository mocks = new MockRepository();

        #region Test methods
        [Test]
        public void ExecuteRunsMultipleTasks()
        {
            // Initialise the task
            var subTasks = new List<ParallelTestTask>();
            for (var loop = 1; loop <= 5; loop++)
            {
                subTasks.Add(new ParallelTestTask { TaskNumber = loop });
            }
            var task = new ParallelTask
            {
                Tasks = subTasks.ToArray()
            };

            // Setup the mocks
            var logger = mocks.DynamicMock<ILogger>();
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            for (var loop = 1; loop <= 5; loop++)
            {
                Expect.Call(() => { result.AddTaskResult(string.Format("Task #{0} has run", loop)); });
            }
            Expect.Call(result.Status).PropertyBehavior();
            Expect.Call(result.Clone()).Return(result).Repeat.Times(5);
            Expect.Call(() => { result.Merge(result); }).Repeat.Times(5);
            mocks.ReplayAll();

            // Run the actual task
            task.Run(result);

            // Verify the results
            mocks.VerifyAll();
            Assert.AreEqual(result.Status, IntegrationStatus.Success, "Status does not match");
        }
        #endregion

        #region Private classes
        private class ParallelTestTask
            : ITask
        {
            public int TaskNumber { get; set; }

            public void Run(IIntegrationResult result)
            {
                result.AddTaskResult(string.Format("Task #{0} has run", TaskNumber));
                result.Status = IntegrationStatus.Success;
            }
        }
        #endregion
    }
}
