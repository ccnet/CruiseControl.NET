namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class CruiseServerControlTaskTests
    {
        #region Private fields
        private MockRepository mocks;
        private ILogger logger;
        private ICruiseServerClientFactory factory;
        private IIntegrationResult result;
        private BuildProgressInformation buildInfo;
        private CruiseServerClientBase client;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Default);
            this.logger = this.mocks.Create<ILogger>(MockBehavior.Strict).Object;
            this.factory = this.mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            this.result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(this.result).SetupProperty(_result => _result.Status);
            this.result.Status = IntegrationStatus.Unknown;
            this.buildInfo = this.mocks.Create<BuildProgressInformation>(MockBehavior.Strict, "somewhere", "test").Object;
            Mock.Get(this.result).SetupGet(_result => _result.BuildProgressInformation).Returns(this.buildInfo);
            this.client = this.mocks.Create<CruiseServerClientBase>(MockBehavior.Strict).Object;
        }
        #endregion

        #region Tests
        #region Run() tests
        /// <summary>
        /// While a configuration with no actions is invalid, the task should not crash and burn if encountered!
        /// </summary>
        [Test]
        public void RunHandlesNoActions()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            MockSequence sequence = new MockSequence();
            this.InitialiseStandardBuildInfo(sequence);
            this.InitialiseClient(sequence, "tcp://localhost:21234", "Test", "Dummy");
            this.InitialiseTermination(sequence, 0);

            // Run the test
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// Sends the correct command to a single project.
        /// </summary>
        [Test]
        public void RunASingleActionOnSingleProject()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            MockSequence sequence = new MockSequence();
            this.InitialiseStandardBuildInfo(sequence);
            this.InitialiseClient(sequence, "tcp://localhost:21234", "Test", "Dummy");
            this.InitialiseActionEvents(sequence,
                "Dummy", 
                "Performing start project action", 
                CruiseServerControlTaskActionType.StartProject, 
                "Dummy");
            this.InitialiseTermination(sequence, 1);

            // Add the action
            task.Actions = new CruiseServerControlTaskAction[] 
            {
                new CruiseServerControlTaskAction
                {
                    Project = "Dummy",
                    Type = CruiseServerControlTaskActionType.StartProject
                }
            };

            // Run the test
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// Sends the correct command to multiple projects.
        /// </summary>
        [Test]
        public void RunASingleActionOnMultipleProjects()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            MockSequence sequence = new MockSequence();
            this.InitialiseStandardBuildInfo(sequence);
            this.InitialiseClient(sequence, "tcp://localhost:21234", "Test", "Dummy");
            this.InitialiseActionEvents(sequence,
                "*",
                "Performing start project action",
                CruiseServerControlTaskActionType.StartProject,
                "Test",
                "Dummy");
            this.InitialiseTermination(sequence, 2);

            // Add the action
            task.Actions = new CruiseServerControlTaskAction[] 
            {
                new CruiseServerControlTaskAction
                {
                    Project = "*",
                    Type = CruiseServerControlTaskActionType.StartProject
                }
            };

            // Run the test
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// Sends the correct commands to multiple projects.
        /// </summary>
        [Test]
        public void RunMultipleCommands()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            MockSequence sequence = new MockSequence();
            this.InitialiseStandardBuildInfo(sequence);
            this.InitialiseClient(sequence, "tcp://localhost:21234", "Test1", "Dummy", "Test2");
            this.InitialiseActionEvents(sequence,
                "Test?",
                "Performing start project action",
                CruiseServerControlTaskActionType.StartProject,
                "Test1",
                "Test2");
            this.InitialiseActionEvents(sequence,
                "Dummy",
                "Performing stop project action",
                CruiseServerControlTaskActionType.StopProject,
                "Dummy");
            this.InitialiseTermination(sequence, 3);

            // Add the action
            task.Actions = new CruiseServerControlTaskAction[] 
            {
                new CruiseServerControlTaskAction
                {
                    Project = "Test?",
                    Type = CruiseServerControlTaskActionType.StartProject
                },
                new CruiseServerControlTaskAction
                {
                    Project = "Dummy",
                    Type = CruiseServerControlTaskActionType.StopProject
                }
            };

            // Run the test
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }
        #endregion

        #region Validate() tests
        /// <summary>
        /// If the configuration is valid, then no errors or warnings should be generated.
        /// </summary>
        [Test]
        public void ValidateHandlesValidConfig()
        {
            var task = new CruiseServerControlTask();
            task.Actions = new CruiseServerControlTaskAction[]
            {
                new CruiseServerControlTaskAction
                {
                    Project = "*",
                    Type = CruiseServerControlTaskActionType.StopProject
                }
            };
            var processor = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            task.Validate(null, null, processor);
            this.mocks.Verify();
        }

        /// <summary>
        /// If the configuration is valid, then no errors or warnings should be generated.
        /// </summary>
        [Test]
        public void ValidateGeneratesWarningWithNullTasks()
        {
            var task = new CruiseServerControlTask();
            task.Actions = null;
            var processor = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            Mock.Get(processor).Setup(_processor => _processor.ProcessWarning("This task will not do anything - no actions specified")).Verifiable();
            task.Validate(null, null, processor);
            this.mocks.Verify();
        }

        /// <summary>
        /// If the configuration is valid, then no errors or warnings should be generated.
        /// </summary>
        [Test]
        public void ValidateGeneratesWarningWithNoTasks()
        {
            var task = new CruiseServerControlTask();
            task.Actions = new CruiseServerControlTaskAction[0];
            var processor = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            Mock.Get(processor).Setup(_processor => _processor.ProcessWarning("This task will not do anything - no actions specified")).Verifiable();
            task.Validate(null, null, processor);
            this.mocks.Verify();
        }
        #endregion
        #endregion

        #region Helpers
        private void InitialiseStandardBuildInfo(MockSequence sequence)
        {
            Mock.Get(this.buildInfo).InSequence(sequence).Setup(_buildInfo => _buildInfo.SignalStartRunTask("Performing server actions")).Verifiable();
            Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Info("Performing server actions")).Verifiable();
            Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Debug("Initialising client")).Verifiable();
        }

        private void InitialiseClient(MockSequence sequence, string address, params string[] projects)
        {
            Mock.Get(this.factory).InSequence(sequence).Setup(_factory => _factory.GenerateClient(address))
                .Returns(this.client).Verifiable();
            Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Info("Retrieving projects from server")).Verifiable();

            var list = new List<ProjectStatus>();
            foreach (var project in projects)
            {
                list.Add(new ProjectStatus(project, IntegrationStatus.Unknown, DateTime.Now));
            }

            Mock.Get(this.client).InSequence(sequence).Setup(_client => _client.GetProjectStatus())
                .Returns(list.ToArray()).Verifiable();
            Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Debug(projects.Length.ToString() + " project(s) retrieved")).Verifiable();
        }

        private void InitialiseActionEvents(MockSequence sequence, string pattern, string actionMessage, CruiseServerControlTaskActionType taskActionType, params string[] projects)
        {
            Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Info("Found " + projects.Length + " project(s) for pattern '" + pattern + "'")).Verifiable();
            Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Info(actionMessage)).Verifiable();

            foreach (var project in projects)
            {
                Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Debug("Sending action to " + project)).Verifiable();
                if (taskActionType == CruiseServerControlTaskActionType.StartProject)
                {
                    Mock.Get(this.client).InSequence(sequence).Setup(_client => _client.StartProject(project)).Verifiable();
                }
                else if (taskActionType == CruiseServerControlTaskActionType.StopProject)
                {
                    Mock.Get(this.client).InSequence(sequence).Setup(_client => _client.StopProject(project)).Verifiable();
                }
                else
                {
                    throw new ArgumentException("Unsupported argument.", "taskActionType");
                }
            }
        }

        private void InitialiseTermination(MockSequence sequence, int commandCount)
        {
            Mock.Get(this.logger).InSequence(sequence).Setup(_logger => _logger.Info("Server actions completed: " + commandCount + " command(s) sent")).Verifiable();
        }
        #endregion
    }
}
