using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class TaskContainerBaseTests
    {
        #region Private fields
        private MockRepository mocks = new MockRepository();
        #endregion

        #region Tests
        #region Validate()
        [Test]
        public void ValidateHandlesNull()
        {
            var task = new TestTask
            {
                Tasks = null
            };
            task.Validate(null, null, null);
        }

        [Test]
        public void ValidateHandlesEmpty()
        {
            var task = new TestTask
            {
                Tasks = new ITask[0]
            };
            task.Validate(null, null, null);
        }

        [Test]
        public void ValidateHandlesValidationTasks()
        {
            var subTask = new MockTask();
            var task = new TestTask
            {
                Tasks = new ITask[] 
                {
                    subTask
                }
            };

            task.Validate(null, null, null);
            Assert.IsTrue(subTask.IsValided);
        }

        [Test]
        public void ValidateHandlesNonValidationTasks()
        {
            var subTask = mocks.StrictMock<ITask>();
            var task = new TestTask
            {
                Tasks = new ITask[] 
                {
                    subTask
                }
            };

            mocks.ReplayAll();
            task.Validate(null, null, null);
            mocks.VerifyAll();
        }
        #endregion

        #region ApplyParameters()
        [Test]
        public void ApplyParametersStoresTheArguments()
        {
            var parameters = new Dictionary<string, string>();
            var definitions = new List<ParameterBase>();
            var subTask = new MockTask();
            var task = new TestTask
            {
                Tasks = new ITask[] 
                {
                    subTask
                }
            };
            var result = mocks.DynamicMock<IIntegrationResult>();

            mocks.ReplayAll();
            task.ApplyParameters(parameters, definitions);
            task.Run(result);
            mocks.VerifyAll();

            Assert.AreSame(parameters, subTask.Parameters);
            Assert.AreSame(definitions, subTask.Definitions);
            Assert.IsTrue(subTask.Executed);
        }
        #endregion

        #region InitialiseStatus()
        [Test]
        public void InitialiseStatusHandlesNull()
        {
            var task = new TestTask
            {
                Tasks = null
            };
            task.TestStatus();
        }

        [Test]
        public void InitialiseStatusHandlesEmpty()
        {
            var task = new TestTask
            {
                Tasks = new ITask[0]
            };
            task.TestStatus();
            Assert.IsNotNull(task.CurrentStatus);
        }

        [Test]
        public void InitialiseStatusHandlesStatusTask()
        {
            var subTask = new MockTask();
            var task = new TestTask
            {
                Tasks = new ITask[] 
                {
                    subTask
                }
            };

            task.TestStatus();

            Assert.IsNotNull(task.CurrentStatus);
            Assert.IsTrue(subTask.SnapshotGenerated);
        }

        [Test]
        public void InitialiseStatusHandlesNonStatusTask()
        {
            var subTask = mocks.StrictMock<ITask>();
            var task = new TestTask
            {
                Tasks = new ITask[] 
                {
                    subTask
                }
            };

            mocks.ReplayAll();
            task.TestStatus();
            mocks.VerifyAll();

            Assert.IsNotNull(task.CurrentStatus);
        }
        #endregion
        #endregion

        #region Private classes
        private class TestTask
            : TaskContainerBase
        {
            protected override bool Execute(IIntegrationResult result)
            {
                foreach (var task in Tasks)
                {
                    RunTask(task, result);
                }
                return true;
            }

            public void TestStatus()
            {
                InitialiseStatus();
            }
        }

        private class MockTask
            : ITask, IConfigurationValidation, IStatusSnapshotGenerator, IParamatisedItem
        {
            public bool Executed { get; set; }
            public bool IsValided { get; set; }
            public bool SnapshotGenerated { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
            public IEnumerable<ParameterBase> Definitions { get; set; }

            public void Run(IIntegrationResult result)
            {
                Executed = true;
            }

            public void Validate(IConfiguration configuration, object parent, IConfigurationErrorProcesser errorProcesser)
            {
                IsValided = true;
            }

            public ItemStatus GenerateSnapshot()
            {
                SnapshotGenerated = true;
                return new ItemStatus();
            }

            public void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
            {
                Parameters = parameters;
                Definitions = parameterDefinitions;
            }
        }
        #endregion
    }
}
