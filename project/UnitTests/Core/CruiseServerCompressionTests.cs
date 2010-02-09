namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote.Messages;
    using ThoughtWorks.CruiseControl.Core.Config;
    using System.Diagnostics;
    using ThoughtWorks.CruiseControl.Core.Queues;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Core.Security;

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
        [Explicit("Code is not ready yet, just adding to SVN so it is not lost")]
        public void GetLogCompressesData()
        {
            // Initialise the mocks
            var mocks = new MockRepository();
            var data = "This is some test - line 1, line 2, line 3 - this is some test data";

            // Perform the tests
            var server = InitialiseServer(mocks, testBuildName, data);
            var request = new BuildRequest(null, testProjectName);
            request.BuildName = testBuildName;
            request.CompressData = true;
            var response = server.GetLog(request);

            // Verify the results
            mocks.VerifyAll();
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
            var configuration = mocks.StrictMock<IConfiguration>();
            SetupResult.For(configuration.Projects)
                .Return(projects);
            SetupResult.For(configuration.SecurityManager)
                .Return(new NullSecurityManager());

            // Mock the configuration service
            var configService = mocks.StrictMock<IConfigurationService>();
            SetupResult.For(configService.Load())
                .Return(configuration);
            Expect.Call(() => { configService.AddConfigurationUpdateHandler(null); })
                .IgnoreArguments();

            // Mock the integration repostory
            var repository = mocks.StrictMock<IIntegrationRepository>();
            SetupResult.For(repository.GetBuildLog(buildName))
                .Return(buildLog);
            
            // Mock the project integrator
            var projectIntegrator = mocks.StrictMock<IProjectIntegrator>();
            SetupResult.For(projectIntegrator.Project)
                .Return(project);
            SetupResult.For(projectIntegrator.IntegrationRepository)
                .Return(repository);

            // Mock the queue manager
            var queueManager = mocks.StrictMock<IQueueManager>();
            Expect.Call(() => { queueManager.AssociateIntegrationEvents(null, null); })
                .IgnoreArguments();
            SetupResult.For(queueManager.GetIntegrator(testProjectName))
                .Return(projectIntegrator);

            // Mock the queue manager factory
            var queueManagerFactory = mocks.StrictMock<IQueueManagerFactory>();
            SetupResult.For(queueManagerFactory.Create(null, configuration, null))
                .Return(queueManager);
            IntegrationQueueManagerFactory.OverrideFactory(queueManagerFactory);

            // Mock the execution environment
            var execEnviron = mocks.StrictMock<IExecutionEnvironment>();
            SetupResult.For(execEnviron.GetDefaultProgramDataFolder(ApplicationType.Server))
                .Return(string.Empty);

            // Initialise the server
            mocks.ReplayAll();
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
