using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.UnitTests.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class CruiseServerTest : IntegrationFixture
	{
        private MockRepository mocks = new MockRepository(MockBehavior.Default);
		private Mock<IConfigurationService> configServiceMock;
		private Mock<IProjectIntegratorListFactory> projectIntegratorListFactoryMock;
		private Mock<IProjectSerializer> projectSerializerMock;
		private Mock<IProjectIntegrator> integratorMock1;
		private Mock<IProjectIntegrator> integratorMock2;
		private Mock<IProjectIntegrator> integratorMock3;
        private Mock<IProjectStateManager> stateManagerMock;

		private CruiseServer server;

		private Configuration configuration;
		private Project project1;
		private Project project2;

		private Mock<IProject> mockProject;
		private IProject mockProjectInstance;

		private IProjectIntegrator integrator1;
		private IProjectIntegrator integrator2;
		private IProjectIntegrator integrator3;
		private ProjectIntegratorList integratorList;
		private IFileSystem fileSystem;
		private IExecutionEnvironment executionEnvironment;

		private ManualResetEvent monitor;

		private string applicationDataPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			             Path.Combine("CruiseControl.NET", "Server"));

		[SetUp]
		protected void SetUp()
		{
			projectSerializerMock = new Mock<IProjectSerializer>();

			integratorMock1 = new Mock<IProjectIntegrator>();
			integratorMock2 = new Mock<IProjectIntegrator>();
			integratorMock3 = new Mock<IProjectIntegrator>();
			integrator1 = (IProjectIntegrator) integratorMock1.Object;
			integrator2 = (IProjectIntegrator) integratorMock2.Object;
			integrator3 = (IProjectIntegrator) integratorMock3.Object;
            integratorMock1.SetupGet(integrator => integrator.Name).Returns("Project 1");
			integratorMock2.SetupGet(integrator => integrator.Name).Returns("Project 2");
			integratorMock3.SetupGet(integrator => integrator.Name).Returns("Project 3");

			fileSystem = mocks.Create<IFileSystem>().Object;
			executionEnvironment = mocks.Create<IExecutionEnvironment>().Object;

			Mock.Get(executionEnvironment).SetupGet(_executionEnvironment => _executionEnvironment.IsRunningOnWindows).Returns(true);
			Mock.Get(executionEnvironment).Setup(_executionEnvironment => _executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server)).Returns(applicationDataPath);
			Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.DirectoryExists(applicationDataPath)).Returns(true);

			configuration = new Configuration();
			project1 = new Project();
			project1.Name = "Project 1";
            integratorMock1.SetupGet(integrator => integrator.Project).Returns(project1);
			
			project2 = new Project();
			project2.Name = "Project 2";
            integratorMock2.SetupGet(integrator => integrator.Project).Returns(project1);

			mockProject = new Mock<IProject>();
            mockProjectInstance = (IProject)mockProject.Object;
            mockProject.SetupGet(project => project.Name).Returns("Project 3");
            mockProject.SetupGet(project => project.QueueName).Returns("Project 3");
            mockProject.SetupGet(project => project.StartupMode).Returns(ProjectStartupMode.UseLastState);
            integratorMock3.SetupGet(integrator => integrator.Project).Returns(mockProjectInstance);

			configuration.AddProject(project1);
			configuration.AddProject(project2);
			configuration.AddProject(mockProjectInstance);

			integratorList = new ProjectIntegratorList();
			integratorList.Add(integrator1);
			integratorList.Add(integrator2);
			integratorList.Add(integrator3);
            
			configServiceMock = new Mock<IConfigurationService>();
			configServiceMock.Setup(service => service.Load()).Returns(configuration).Verifiable();

			projectIntegratorListFactoryMock = new Mock<IProjectIntegratorListFactory>();
			projectIntegratorListFactoryMock.Setup(factory => factory.CreateProjectIntegrators(configuration.Projects, It.IsAny<IntegrationQueueSet>()))
				.Returns(integratorList).Verifiable();

            stateManagerMock = new Mock<IProjectStateManager>();
            stateManagerMock.Setup(_manager => _manager.CheckIfProjectCanStart(It.IsAny<string>())).Returns(true);

			server = new CruiseServer((IConfigurationService) configServiceMock.Object,
			                          (IProjectIntegratorListFactory) projectIntegratorListFactoryMock.Object,
			                          (IProjectSerializer) projectSerializerMock.Object,
                                      (IProjectStateManager)stateManagerMock.Object,
									  fileSystem,
									  executionEnvironment,
                                      null);
		}

		private void VerifyAll()
		{
			configServiceMock.Verify();
			projectIntegratorListFactoryMock.Verify();
			integratorMock1.Verify();
			integratorMock2.Verify();
		}

		[Test]
		public void StartAllProjectsInCruiseServer()
		{
			integratorMock1.Setup(integrator => integrator.Start()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Start()).Verifiable();

			server.Start();

			VerifyAll();
		}

		[Test]
		public void CallingStopBeforeCallingStartDoesntCauseAnError()
		{
			server.Stop();
			VerifyAll();
		}

		[Test]
		public void CallingStopStopsIntegratorsAndWaitsForThemToFinish()
		{
			integratorMock1.Setup(integrator => integrator.Start()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Start()).Verifiable();

			server.Start();

			integratorMock1.Setup(integrator => integrator.Stop(false)).Verifiable();
			integratorMock1.Setup(integrator => integrator.WaitForExit()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Stop(false)).Verifiable();
			integratorMock2.Setup(integrator => integrator.WaitForExit()).Verifiable();

			server.Stop();

			VerifyAll();
		}

		[Test]
		public void CallingAbortBeforeCallingStartDoesntCauseAnError()
		{
			server.Stop();
			VerifyAll();
		}

		[Test]
		public void CallingAbortStopsIntegratorsAndWaitsForThemToFinish()
		{
			integratorMock1.Setup(integrator => integrator.Start()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Start()).Verifiable();

			server.Start();

			integratorMock1.Setup(integrator => integrator.Abort()).Verifiable();
			integratorMock1.Setup(integrator => integrator.WaitForExit()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Abort()).Verifiable();
			integratorMock2.Setup(integrator => integrator.WaitForExit()).Verifiable();

			server.Abort();

			VerifyAll();
		}

		[Test]
		public void OnRestartKillAllIntegratorsRefreshConfigAndStartupNewIntegrators()
		{
			integratorMock1.Setup(integrator => integrator.Start()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Start()).Verifiable();

			server.Start();

			integratorMock1.Setup(integrator => integrator.Stop(true)).Verifiable();
			integratorMock1.Setup(integrator => integrator.WaitForExit()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Stop(true)).Verifiable();
			integratorMock2.Setup(integrator => integrator.WaitForExit()).Verifiable();

			configuration = new Configuration();
			configuration.AddProject(project1);
			integratorList = new ProjectIntegratorList();
			integratorList.Add(integrator1);
			configServiceMock.Setup(service => service.Load()).Returns(configuration).Verifiable();
			projectIntegratorListFactoryMock.Setup(factory => factory.CreateProjectIntegrators(configuration.Projects, It.IsAny<IntegrationQueueSet>()))
				.Returns(integratorList).Verifiable();

			server.Restart();

			integratorMock1.Verify(integrator => integrator.Start(), Times.Exactly(2));
			integratorMock2.Verify(integrator => integrator.Start(), Times.Exactly(1));
			VerifyAll();
		}

		[Test]
		public void WaitForExitAfterStop()
		{
			monitor = new ManualResetEvent(false);

			Thread stopThread = new Thread(new ThreadStart(Stop));
			stopThread.Start();

			server.Start();
			monitor.Set();
			server.WaitForExit();
		}

		private void Stop()
		{
			monitor.WaitOne();
			Thread.Sleep(110);
			server.Stop();
		}

		[Test]
		public void WaitForExitAfterAbort()
		{
			monitor = new ManualResetEvent(false);

			Thread abortThread = new Thread(new ThreadStart(Abort));
			abortThread.Start();

			server.Start();
			monitor.Set();
			server.WaitForExit();
		}

		private void Abort()
		{
			monitor.WaitOne();
			Thread.Sleep(110);
			server.Abort();
		}

		[Test]
		public void ForceBuildForProject()
		{
			integratorMock1.Setup(integrator => integrator.Start()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Start()).Verifiable();

			server.Start();

            var parameters = new Dictionary<string, string>();
            integratorMock1.Setup(integrator => integrator.Request(It.Is<IntegrationRequest>(r => r.BuildCondition == BuildCondition.ForceBuild))).Verifiable();

            server.CruiseManager.ForceBuild("Project 1", "BuildForcer");

			VerifyAll();
		}

		[Test]
		public void AttemptToForceBuildOnProjectThatDoesNotExist()
		{
            Assert.That(delegate { server.CruiseManager.ForceBuild("foo", "BuildForcer"); },
                        Throws.TypeOf<CruiseControlException>());
		}

		[Test]
		public void WaitForExitForProject()
		{
			integratorMock1.Setup(integrator => integrator.Start()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Start()).Verifiable();

			server.Start();

			integratorMock1.Setup(integrator => integrator.WaitForExit()).Verifiable();

			server.CruiseManager.WaitForExit("Project 1");

			VerifyAll();
		}

		[Test]
		public void ShouldOnlyDisposeOnce()
		{
			integratorMock1.Setup(integrator => integrator.Abort()).Verifiable();
			integratorMock2.Setup(integrator => integrator.Abort()).Verifiable();
			((IDisposable) server).Dispose();

			((IDisposable) server).Dispose();

			integratorMock1.Verify(integrator => integrator.Abort(), Times.Once());
			integratorMock2.Verify(integrator => integrator.Abort(), Times.Once());
			integratorMock1.Verify();
			integratorMock2.Verify();
		}

		[Test]
		public void DetectVersionMethod()
		{
            string ServerVersion = server.CruiseManager.GetServerVersion();
			Assert.IsFalse(ServerVersion.Length == 0, "Version not retrieved");
		}

		[Test]
		public void StopSpecificProject()
		{
            stateManagerMock.Setup(_manager => _manager.RecordProjectAsStopped("Project 1")).Verifiable();
            integratorMock1.Setup(integrator => integrator.Stop(false)).Verifiable();
			server.CruiseManager.Stop("Project 1");
			integratorMock1.Verify();
            stateManagerMock.Verify();
        }

		[Test]
		public void ThrowExceptionIfProjectNotFound()
		{
            Assert.That(delegate { server.CruiseManager.Stop("Project unknown"); },
                        Throws.TypeOf<CruiseControlException>());
		}

		[Test]
		public void StartSpecificProject()
		{
            stateManagerMock.Setup(_manager => _manager.RecordProjectAsStartable("Project 2")).Verifiable();
			integratorMock2.Setup(integrator => integrator.Start()).Verifiable();
            server.CruiseManager.Start("Project 2");
			integratorMock2.Verify();
            stateManagerMock.Verify();
		}

		[Test]
		public void RequestNewIntegration()
		{
            var oldSource = Source;
            Source = Environment.MachineName;
			IntegrationRequest request = Request(BuildCondition.IfModificationExists);
            Source = oldSource;
			integratorMock2.Setup(integrator => integrator.Request(request)).Verifiable();
            server.CruiseManager.Request("Project 2", request);
			integratorMock1.Verify();
			integratorMock2.Verify();
		}

        [Test]
        public void ProjectStartFiresEvents()
        {
            string projectName = "Project 1";
            bool projectStartingFired = false;
            server.ProjectStarting += delegate(object o, CancelProjectEventArgs e)
            {
                projectStartingFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            bool projectStartedFired = false;
            server.ProjectStarted += delegate(object o, ProjectEventArgs e)
            {
                projectStartedFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            server.CruiseManager.Start(projectName);
            Assert.IsTrue(projectStartingFired, "ProjectStarting not fired");
            Assert.IsTrue(projectStartedFired, "ProjectStarted not fired");
        }

        [Test]
        public void ProjectStartCanBeCancelled()
        {
            string projectName = "Project 1";
            bool projectStartingFired = false;
            server.ProjectStarting += delegate(object o, CancelProjectEventArgs e)
            {
                projectStartingFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
                e.Cancel = true;
            };

            server.ProjectStarted += delegate(object o, ProjectEventArgs e)
            {
                Assert.Fail("ProjectStarted has been fired");
            };

            server.CruiseManager.Start(projectName);
            Assert.IsTrue(projectStartingFired, "ProjectStarting not fired");
        }

        [Test]
        public void ProjectStopFiresEvents()
        {
            string projectName = "Project 1";
            bool projectStoppingFired = false;
            server.ProjectStopping += delegate(object o, CancelProjectEventArgs e)
            {
                projectStoppingFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            bool projectStoppedFired = false;
            server.ProjectStopped += delegate(object o, ProjectEventArgs e)
            {
                projectStoppedFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            server.CruiseManager.Stop(projectName);
            Assert.IsTrue(projectStoppingFired, "ProjectStopping not fired");
            Assert.IsTrue(projectStoppedFired, "ProjectStopped not fired");
        }

        [Test]
        public void ProjectStopCanBeCancelled()
        {
            string projectName = "Project 1";
            bool projectStoppingFired = false;
            server.ProjectStopping += delegate(object o, CancelProjectEventArgs e)
            {
                projectStoppingFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
                e.Cancel = true;
            };

            server.ProjectStopped += delegate(object o, ProjectEventArgs e)
            {
                Assert.Fail("ProjectStopped has been fired");
            };

            server.CruiseManager.Stop(projectName);
            Assert.IsTrue(projectStoppingFired, "ProjectStopping not fired");
        }

        [Test]
        public void ForceBuildFiresEvents()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            bool forceBuildReceived = false;
            server.ForceBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                forceBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            bool forceBuildProcessed = false;
            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                forceBuildProcessed = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            server.CruiseManager.ForceBuild(projectName, enforcer);
            Assert.IsTrue(forceBuildReceived, "ForceBuildReceived not fired");
            Assert.IsTrue(forceBuildProcessed, "ForceBuildProcessed not fired");
        }

        [Test]
        public void ForceBuildCanBeCancelled()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            bool forceBuildReceived = false;
            server.ForceBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                forceBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
                e.Cancel = true;
            };

            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                Assert.Fail("ForceBuildProcessed has been fired");
            };

            server.CruiseManager.ForceBuild(projectName, enforcer);
            Assert.IsTrue(forceBuildReceived, "ForceBuildReceived not fired");
        }

        [Test]
        public void RequestFiresEvents()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer, null);
            bool forceBuildReceived = false;
            server.ForceBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                forceBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            bool forceBuildProcessed = false;
            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                forceBuildProcessed = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            server.CruiseManager.Request(projectName, request);
            Assert.IsTrue(forceBuildReceived, "ForceBuildReceived not fired");
            Assert.IsTrue(forceBuildProcessed, "ForceBuildProcessed not fired");
        }

        [Test]
        public void RequestCanBeCancelled()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer, null);
            bool forceBuildReceived = false;
            server.ForceBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                forceBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
                e.Cancel = true;
            };

            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                Assert.Fail("ForceBuildProcessed has been fired");
            };

            server.CruiseManager.Request(projectName, request);
            Assert.IsTrue(forceBuildReceived, "ForceBuildReceived not fired");
        }

        [Test]
        public void AbortBuildFiresEvents()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            bool abortBuildReceived = false;
            server.AbortBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                abortBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            bool abortBuildProcessed = false;
            server.AbortBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                abortBuildProcessed = true;
                Assert.AreEqual(projectName, e.ProjectName);
            };

            server.CruiseManager.AbortBuild(projectName, enforcer);
            Assert.IsTrue(abortBuildReceived, "AbortBuildReceived not fired");
            Assert.IsTrue(abortBuildProcessed, "AbortBuildProcessed not fired");
        }

        [Test]
        public void AbortBuildCanBeCancelled()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            bool abortBuildReceived = false;
            server.AbortBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                abortBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
                e.Cancel = true;
            };

            server.AbortBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                Assert.Fail("AbortBuildProcessed has been fired");
            };

            server.CruiseManager.AbortBuild(projectName, enforcer);
            Assert.IsTrue(abortBuildReceived, "AbortBuildReceived not fired");
        }

        [Test]
        public void SendMessageFiresEvents()
        {
            string projectName = "Project 1";
            Message message = new Message("This is a test message");
            bool sendMessageReceived = false;
            server.SendMessageReceived += delegate(object o, CancelProjectEventArgs<Message> e)
            {
                sendMessageReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(message.Text, e.Data.Text);
            };

            bool sendMessageProcessed = false;
            server.SendMessageProcessed += delegate(object o, ProjectEventArgs<Message> e)
            {
                sendMessageProcessed = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(message.Text, e.Data.Text);
            };

            server.CruiseManager.SendMessage(projectName, message);
            Assert.IsTrue(sendMessageReceived, "SendMessageReceived not fired");
            Assert.IsTrue(sendMessageProcessed, "SendMessageProcessed not fired");
        }

        [Test]
        public void SendMessageCanBeCancelled()
        {
            string projectName = "Project 1";
            Message message = new Message("This is a test message");
            bool sendMessageReceived = false;
            server.SendMessageReceived += delegate(object o, CancelProjectEventArgs<Message> e)
            {
                sendMessageReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(message.Text, e.Data.Text);
                e.Cancel = true;
            };

            server.SendMessageProcessed += delegate(object o, ProjectEventArgs<Message> e)
            {
                Assert.Fail("SendMessageProcessed has been fired");
            };

            server.CruiseManager.SendMessage(projectName, message);
            Assert.IsTrue(sendMessageReceived, "SendMessageReceived not fired");
        }

        [Test]
        public void IntegrationStartedIsFired()
        {
            string enforcer = "JohnDoe";
            string projectName = "Project 4";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer, null);

            // Need to set up a new integrator that can return an event
            IProjectIntegrator integrator4;
            integrator4 = mocks.Create<IProjectIntegrator>().Object;
            Mock.Get(integrator4).SetupGet(_integrator4 => _integrator4.Name).Returns("Project 4");

            // Initialise a new cruise server with the new integrator
            integratorList.Add(integrator4);
            configServiceMock.Setup(service => service.Load()).Returns(configuration).Verifiable();
            projectIntegratorListFactoryMock.Setup(factory => factory.CreateProjectIntegrators(configuration.Projects, It.IsAny<IntegrationQueueSet>()))
                .Returns(integratorList).Verifiable();
            server = new CruiseServer((IConfigurationService)configServiceMock.Object,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.Object,
                                      (IProjectSerializer)projectSerializerMock.Object,
									  (IProjectStateManager)stateManagerMock.Object,
									  fileSystem,
									  executionEnvironment,
                                      null);

            bool eventFired = false;
            server.IntegrationStarted += delegate(object o, IntegrationStartedEventArgs e)
            {
                eventFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreSame(request, e.Request);
            };


            Mock.Get(integrator4).Raise(_integrator4 => _integrator4.IntegrationStarted += null,
                new IntegrationStartedEventArgs(request, projectName));
            Assert.IsTrue(eventFired, "IntegrationStarted not fired");
        }

        [Test]
        public void IntegrationCompletedIsFired()
        {
            string enforcer = "JohnDoe";
            string projectName = "Project 4";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer, null);

            // Need to set up a new integrator that can return an event
            IProjectIntegrator integrator4;
            integrator4 = mocks.Create<IProjectIntegrator>().Object;
            Mock.Get(integrator4).SetupGet(_integrator4 => _integrator4.Name).Returns("Project 4");

            // Initialise a new cruise server with the new integrator
            integratorList.Add(integrator4);
            configServiceMock.Setup(service => service.Load()).Returns(configuration).Verifiable();
            projectIntegratorListFactoryMock.Setup(factory => factory.CreateProjectIntegrators(configuration.Projects, It.IsAny<IntegrationQueueSet>()))
                .Returns(integratorList).Verifiable();
            server = new CruiseServer((IConfigurationService)configServiceMock.Object,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.Object,
                                      (IProjectSerializer)projectSerializerMock.Object,
									  (IProjectStateManager)stateManagerMock.Object,
									  fileSystem,
									  executionEnvironment,
                                      null);

            bool eventFired = false;
            server.IntegrationCompleted += delegate(object o, IntegrationCompletedEventArgs e)
            {
                eventFired = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(IntegrationStatus.Success, e.Status);
                Assert.AreSame(request, e.Request);
            };


            Mock.Get(integrator4).Raise(_integrator4 => _integrator4.IntegrationCompleted += null,
                new IntegrationCompletedEventArgs(request, projectName, IntegrationStatus.Success));
            Assert.IsTrue(eventFired, "IntegrationCompleted not fired");
        }

        [Test]
        public void StartAndStopExtensions()
        {
            List<ExtensionConfiguration> extensions = new List<ExtensionConfiguration>();
            ExtensionConfiguration extensionStub = new ExtensionConfiguration();
            extensionStub.Type = "ThoughtWorks.CruiseControl.UnitTests.Remote.ServerExtensionStub,ThoughtWorks.CruiseControl.UnitTests";
            extensions.Add(extensionStub);

            configServiceMock.Setup(service => service.Load()).Returns(configuration).Verifiable();
            projectIntegratorListFactoryMock.Setup(factory => factory.CreateProjectIntegrators(configuration.Projects, It.IsAny<IntegrationQueueSet>()))
                .Returns(integratorList).Verifiable();
            server = new CruiseServer((IConfigurationService)configServiceMock.Object,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.Object,
                                      (IProjectSerializer)projectSerializerMock.Object,
									  (IProjectStateManager)stateManagerMock.Object,
									  fileSystem,
									  executionEnvironment,
                                      extensions);
            Assert.IsTrue(ServerExtensionStub.HasInitialised);

            server.Start();
            Assert.IsTrue(ServerExtensionStub.HasStarted);

            server.Stop();
            Assert.IsTrue(ServerExtensionStub.HasStopped);
        }

        [Test]
        public void InitialiseingANonExistantExtensionThrowsAnException()
        {
            List<ExtensionConfiguration> extensions = new List<ExtensionConfiguration>();
            ExtensionConfiguration extensionStub = new ExtensionConfiguration();
            extensionStub.Type = "ThoughtWorks.CruiseControl.UnitTests.Remote.Garbage,ThoughtWorks.CruiseControl.UnitTests";
            extensions.Add(extensionStub);

            configServiceMock.Setup(service => service.Load()).Returns(configuration).Verifiable();
            projectIntegratorListFactoryMock.Setup(factory => factory.CreateProjectIntegrators(configuration.Projects, It.IsAny<IntegrationQueueSet>()))
                .Returns(integratorList).Verifiable();

            Assert.That(delegate
                            {
                                new CruiseServer((IConfigurationService) configServiceMock.Object,
                                                 (IProjectIntegratorListFactory)
                                                 projectIntegratorListFactoryMock.Object,
                                                 (IProjectSerializer) projectSerializerMock.Object,
                                                 (IProjectStateManager) stateManagerMock.Object,
                                                 fileSystem,
                                                 executionEnvironment,
                                                 extensions);
                            },
                        Throws.TypeOf<NullReferenceException>().With.Message.EqualTo(
                            "Unable to find extension 'ThoughtWorks.CruiseControl.UnitTests.Remote.Garbage,ThoughtWorks.CruiseControl.UnitTests'"));
        }

        [Test]
        public void StartAndAbortExtensions()
        {
            List<ExtensionConfiguration> extensions = new List<ExtensionConfiguration>();
            ExtensionConfiguration extensionStub = new ExtensionConfiguration();
            extensionStub.Type = "ThoughtWorks.CruiseControl.UnitTests.Remote.ServerExtensionStub,ThoughtWorks.CruiseControl.UnitTests";
            extensions.Add(extensionStub);

            configServiceMock.Setup(service => service.Load()).Returns(configuration).Verifiable();
            projectIntegratorListFactoryMock.Setup(factory => factory.CreateProjectIntegrators(configuration.Projects, It.IsAny<IntegrationQueueSet>()))
                .Returns(integratorList).Verifiable();
            server = new CruiseServer((IConfigurationService)configServiceMock.Object,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.Object,
                                      (IProjectSerializer)projectSerializerMock.Object,
									  (IProjectStateManager)stateManagerMock.Object,
									  fileSystem,
									  executionEnvironment,
                                      extensions);
            Assert.IsTrue(ServerExtensionStub.HasInitialised);

            server.Start();
            Assert.IsTrue(ServerExtensionStub.HasStarted);

            server.Abort();
            Assert.IsTrue(ServerExtensionStub.HasAborted);
        }

        [Test]
        public void TakeSnapshotThrowsExceptionForUnknownProject()
        {
            var request = GenerateProjectRequest("garbage project");
            var response = server.TakeStatusSnapshot(request);
            Assert.AreEqual(ResponseResult.Failure, response.Result);
        }

        [Test]
        public void TakeSnapshotReturnsAValidSnapshot()
        {
            var request = GenerateProjectRequest("Project 1");
            var response = server.TakeStatusSnapshot(request);
            ProjectStatusSnapshot snapshot = response.Snapshot;
            Assert.IsNotNull(snapshot, "Snapshot not taken");
            Assert.AreEqual("Project 1", snapshot.Name, "Name not set");
        }

        [Test]
        public void RetrieveFileTransferOnlyWorksForFilesInArtefactFolder()
        {
        	Assert.That(delegate { server.CruiseManager.RetrieveFileTransfer("Project 1", Path.Combine("..", "testfile.txt")); },
                        Throws.TypeOf<CruiseControlException>());
        }

        [Test]
        public void RetrieveFileTransferFailsForBuildLogsFolder()
        {
            Assert.That(delegate { server.CruiseManager.RetrieveFileTransfer("Project 1", Path.Combine("buildlogs", "testfile.txt")); },
                        Throws.TypeOf<CruiseControlException>());
        }

        [Test]
        public void RetrieveFileTransferFailsForAbsolutePaths()
        {
            Assert.That(delegate { server.CruiseManager.RetrieveFileTransfer("Project 1", Path.GetFullPath(Path.Combine(".", "MyFile.txt"))); },
                        Throws.TypeOf<CruiseControlException>());
        }

        [Test]
        public void RetrieveFileTransferGeneratesTransferForValidFile()
        {
            var tempFile = Path.GetTempFileName();
            if (!File.Exists(tempFile)) File.WriteAllText(tempFile, "This is a test");
            project1.ConfiguredArtifactDirectory = Path.GetDirectoryName(tempFile);
            var transfer = server.CruiseManager.RetrieveFileTransfer("Project 1", Path.GetFileName(tempFile));
            Assert.IsNotNull(transfer);
        }

        [Test]
        public void RetrieveFileTransferGeneratesNullForInvalidFile()
        {
            var transfer = server.CruiseManager.RetrieveFileTransfer("Project 1", "GarbageFileNameThatShouldNotExist.NotHere");
            Assert.IsNull(transfer);
        }

        [Test]
        public void GetFinalBuildStatusRequiresViewProjectPermission()
        {
            var securityManagerMock = this.InitialiaseSecurityManagerMock(false, true);
            var request = new BuildRequest("1234", "Project 1")
                {
                    BuildName = "Build #1"
                };

            server.SecurityManager = securityManagerMock;
            var actual = server.GetFinalBuildStatus(request);

            this.mocks.Verify();
            Assert.AreEqual(ResponseResult.Failure, actual.Result);
            Assert.AreEqual("Permission to execute 'ViewProject' has been denied.", actual.ErrorMessages[0].Message);
        }

        [Test]
        public void GetFinalBuildStatusReturnsWarningIfNoStatus()
        {
            var securityManagerMock = this.InitialiaseSecurityManagerMock(true, true);
            var request = new BuildRequest("1234", "Project 1")
                {
                    BuildName = "Build #1"
                };

            server.SecurityManager = securityManagerMock;
            var actual = server.GetFinalBuildStatus(request);

            this.mocks.Verify();
            Assert.AreEqual(ResponseResult.Warning, actual.Result);
            Assert.AreEqual("Build status does not exist", actual.ErrorMessages[0].Message);
        }

        [Test]
        public void GetFinalBuildStatusReturnsStatus()
        {
            var securityManagerMock = this.InitialiaseSecurityManagerMock(true, true);
            var request = new BuildRequest("1234", "Project 1")
                {
                    BuildName = "Build #1"
                };
            var dataStoreMock = this.mocks.Create<IDataStore>(MockBehavior.Strict).Object;
            Mock.Get(dataStoreMock).Setup(_dataStoreMock => _dataStoreMock.LoadProjectSnapshot(project1, "Build #1"))
                .Returns(new ProjectStatusSnapshot { Name = "Project 1" });

            project1.DataStore = dataStoreMock;
            server.SecurityManager = securityManagerMock;
            var actual = server.GetFinalBuildStatus(request);

            this.mocks.Verify();
            Assert.AreEqual(ResponseResult.Success, actual.Result);
            Assert.AreEqual("Project 1", actual.Snapshot.Name);
        }

        [Test]
        public void GetRSSFeedReturnsStatus()
        {
            var securityManagerMock = this.InitialiaseSecurityManagerMock(true, false);
            var request = new ProjectRequest("1234", "Project 1");
            var dataStoreMock = this.mocks.Create<IDataStore>(MockBehavior.Strict).Object;
            Mock.Get(dataStoreMock).Setup(_dataStoreMock => _dataStoreMock.LoadProjectSnapshot(project1, "Build #1"))
                .Returns(new ProjectStatusSnapshot { Name = "Project 1" });

            project1.RssFeedLoader = () => "RSS-Feed";
            server.SecurityManager = securityManagerMock;
            var actual = server.GetRSSFeed(request);

            this.mocks.Verify();
            Assert.AreEqual(ResponseResult.Success, actual.Result);
            Assert.AreEqual("RSS-Feed", actual.Data);
        }

        private ProjectRequest GenerateProjectRequest(string projectName)
        {
            var request = new ProjectRequest(null, projectName);
            return request;
        }

        private ISecurityManager InitialiaseSecurityManagerMock(bool isAllowed, bool expectLogging)
        {
            var securityManagerMock = this.mocks.Create<ISecurityManager>(MockBehavior.Strict).Object;
            Mock.Get(securityManagerMock).SetupGet(_securityManagerMock => _securityManagerMock.Channel).Returns((IChannelSecurity)null);
            Mock.Get(securityManagerMock).SetupGet(_securityManagerMock => _securityManagerMock.RequiresSession).Returns(true);
            Mock.Get(securityManagerMock).Setup(_securityManagerMock => _securityManagerMock.GetUserName("1234")).Returns("johnDoe");
            Mock.Get(securityManagerMock).Setup(_securityManagerMock => _securityManagerMock.GetDisplayName("1234", null)).Returns("John Doe");
            Mock.Get(securityManagerMock).Setup(_securityManagerMock => _securityManagerMock.GetDefaultRight(SecurityPermission.ViewProject))
                .Returns(SecurityRight.Inherit);
            Mock.Get(securityManagerMock).Setup(_securityManagerMock => _securityManagerMock.CheckServerPermission("johnDoe", SecurityPermission.ViewProject))
                .Returns(isAllowed);
            if (expectLogging)
            {
                Mock.Get(securityManagerMock).Setup(_securityManagerMock => _securityManagerMock.LogEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SecurityEvent>(), It.IsAny<SecurityRight>(), It.IsAny<string>()))
                    .Verifiable();
            }

            return securityManagerMock;
        }
    }
}
