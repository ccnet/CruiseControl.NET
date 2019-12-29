namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Queues;
    using ThoughtWorks.CruiseControl.Core.Security;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote.Events;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// Tests for the compression methods on <see cref="CruiseServer"/>
    /// </summary>
    [TestFixture]
    public class CruiseServerCompressionTests
    {
        #region Private constants
        private const string testProjectName = "Test Project";
        private const string testBuildName = "Test Build";
        #endregion

        #region Teardown
        [TearDown]
        public void Teardown()
        {
            IntegrationQueueManagerFactory.ResetFactory();
        }
        #endregion

        #region Tests
        #region GetLog() tests
        [Test(Description = "GetLog() should compress the log data.")]
        public void GetLogCompressesData()
        {
            // Initialise the mocks
            var mocks = new MockRepository(MockBehavior.Default);
            var data = "This is some test - line 1, line 2, line 3 - this is some test data";

            // Perform the tests
            var server = InitialiseServer(mocks, testBuildName, data);
            var request = new BuildRequest(null, testProjectName);
            request.BuildName = testBuildName;
            request.CompressData = true;
            var response = server.GetLog(request);

            // Verify the results
            mocks.Verify();
            Assert.AreEqual(ResponseResult.Success, response.Result);
            Assert.AreNotEqual(data, response.Data);
        }
        #endregion
        #endregion

        #region Helper methods
        #region InitialiseServer()
        /// <summary>
        /// Initialises the server.
        /// </summary>
        /// <param name="mocks">The mocks repository.</param>
        /// <param name="buildName">Name of the build.</param>
        /// <param name="buildLog">The build log.</param>
        /// <returns>The initialised server.</returns>
        private static CruiseServer InitialiseServer(MockRepository mocks, string buildName, string buildLog)
        {
            // Add some projects
            var projects = new ProjectList();
            var project = new Project
            {
                Name = testProjectName
            };
            projects.Add(project);

            // Mock the configuration
            var configuration = mocks.Create<IConfiguration>(MockBehavior.Strict).Object;
            Mock.Get(configuration).SetupGet(_configuration => _configuration.Projects)
                .Returns(projects);
            Mock.Get(configuration).SetupGet(_configuration => _configuration.SecurityManager)
                .Returns(new NullSecurityManager());

            // Mock the configuration service
            var configService = mocks.Create<IConfigurationService>(MockBehavior.Strict).Object;
            Mock.Get(configService).Setup(_configService => _configService.Load())
                .Returns(configuration);
            Mock.Get(configService).Setup(_configService => _configService.AddConfigurationUpdateHandler(It.IsAny<ConfigurationUpdateHandler>())).Verifiable();

            // Mock the integration repostory
            var repository = mocks.Create<IIntegrationRepository>(MockBehavior.Strict).Object;
            Mock.Get(repository).Setup(_repository => _repository.GetBuildLog(buildName))
                .Returns(buildLog);
            
            // Mock the project integrator
            var projectIntegrator = mocks.Create<IProjectIntegrator>(MockBehavior.Strict).Object;
            Mock.Get(projectIntegrator).SetupGet(_projectIntegrator => _projectIntegrator.Project)
                .Returns(project);
            Mock.Get(projectIntegrator).SetupGet(_projectIntegrator => _projectIntegrator.IntegrationRepository)
                .Returns(repository);

            // Mock the queue manager
            var queueManager = mocks.Create<IQueueManager>(MockBehavior.Strict).Object;
            Mock.Get(queueManager).Setup(_queueManager => _queueManager.AssociateIntegrationEvents(It.IsAny<EventHandler<IntegrationStartedEventArgs>>(), It.IsAny<EventHandler<IntegrationCompletedEventArgs>>())).Verifiable();
            Mock.Get(queueManager).Setup(_queueManager => _queueManager.GetIntegrator(testProjectName))
                .Returns(projectIntegrator);

            // Mock the queue manager factory
            var queueManagerFactory = mocks.Create<IQueueManagerFactory>(MockBehavior.Strict).Object;
            Mock.Get(queueManagerFactory).Setup(_queueManagerFactory => _queueManagerFactory.Create(null, configuration, null))
                .Returns(queueManager);
            IntegrationQueueManagerFactory.OverrideFactory(queueManagerFactory);

            // Mock the execution environment
            var execEnviron = mocks.Create<IExecutionEnvironment>(MockBehavior.Strict).Object;
            Mock.Get(execEnviron).Setup(_execEnviron => _execEnviron.GetDefaultProgramDataFolder(ApplicationType.Server))
                .Returns(string.Empty);

            // Initialise the server
            var server = new CruiseServer(
                configService,
                null,
                null,
                null,
                null,
                execEnviron,
                null);
            return server;
        }
        #endregion
        #endregion
    }
}
