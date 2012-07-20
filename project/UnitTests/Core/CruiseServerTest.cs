using System;
using System.Collections.Generic;
using System.Threading;
using NMock;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;
using Rhino.Mocks.Interfaces;
using ThoughtWorks.CruiseControl.UnitTests.Remote;
using System.IO;
using ThoughtWorks.CruiseControl.Remote.Messages;
using NMock.Constraints;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class CruiseServerTest : IntegrationFixture
	{
        private MockRepository mocks = new MockRepository();
		private DynamicMock configServiceMock;
		private DynamicMock projectIntegratorListFactoryMock;
		private DynamicMock projectSerializerMock;
		private DynamicMock integratorMock1;
		private DynamicMock integratorMock2;
		private DynamicMock integratorMock3;
        private DynamicMock stateManagerMock;

		private CruiseServer server;

		private Configuration configuration;
		private Project project1;
		private Project project2;

		private IMock mockProject;
		private IProject mockProjectInstance;
		
		private IIntegrationQueue integrationQueue;

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
			projectSerializerMock = new DynamicMock(typeof (IProjectSerializer));

			integratorMock1 = new DynamicMock(typeof (IProjectIntegrator));
			integratorMock2 = new DynamicMock(typeof (IProjectIntegrator));
			integratorMock3 = new DynamicMock(typeof (IProjectIntegrator));
			integrator1 = (IProjectIntegrator) integratorMock1.MockInstance;
			integrator2 = (IProjectIntegrator) integratorMock2.MockInstance;
			integrator3 = (IProjectIntegrator) integratorMock3.MockInstance;
            integratorMock1.SetupResult("Name", "Project 1");
			integratorMock2.SetupResult("Name", "Project 2");
			integratorMock3.SetupResult("Name", "Project 3");

			fileSystem = mocks.DynamicMock<IFileSystem>();
			executionEnvironment = mocks.DynamicMock<IExecutionEnvironment>();

			SetupResult.For(executionEnvironment.IsRunningOnWindows).Return(true);
			SetupResult.For(executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server)).Return(applicationDataPath);
			SetupResult.For(fileSystem.DirectoryExists(applicationDataPath)).Return(true);
			mocks.ReplayAll();

			integrationQueue = null; // We have no way of injecting currently.

			configuration = new Configuration();
			project1 = new Project();
			project1.Name = "Project 1";
            integratorMock1.SetupResult("Project", project1);
			
			project2 = new Project();
			project2.Name = "Project 2";
            integratorMock2.SetupResult("Project", project1);

			mockProject = new DynamicMock(typeof(IProject));
			mockProject.SetupResult("Name", "Project 3");
            mockProject.SetupResult("QueueName", "Project 3");
            mockProject.SetupResult("QueueName", "Project 3");
            mockProjectInstance = (IProject)mockProject.MockInstance;
			mockProject.SetupResult("Name", "Project 3");
            mockProject.SetupResult("StartupMode", ProjectStartupMode.UseLastState);
            integratorMock3.SetupResult("Project", mockProjectInstance);

			configuration.AddProject(project1);
			configuration.AddProject(project2);
			configuration.AddProject(mockProjectInstance);

			integratorList = new ProjectIntegratorList();
			integratorList.Add(integrator1);
			integratorList.Add(integrator2);
			integratorList.Add(integrator3);
            
			configServiceMock = new DynamicMock(typeof (IConfigurationService));
			configServiceMock.ExpectAndReturn("Load", configuration);

			projectIntegratorListFactoryMock = new DynamicMock(typeof (IProjectIntegratorListFactory));
			projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);

            stateManagerMock = new DynamicMock(typeof(IProjectStateManager));
            stateManagerMock.SetupResult("CheckIfProjectCanStart", true, typeof(string));

			server = new CruiseServer((IConfigurationService) configServiceMock.MockInstance,
			                          (IProjectIntegratorListFactory) projectIntegratorListFactoryMock.MockInstance,
			                          (IProjectSerializer) projectSerializerMock.MockInstance,
                                      (IProjectStateManager)stateManagerMock.MockInstance,
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
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

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
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

			server.Start();

			integratorMock1.Expect("Stop",false);
			integratorMock1.Expect("WaitForExit");
			integratorMock2.Expect("Stop",false);
			integratorMock2.Expect("WaitForExit");

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
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

			server.Start();

			integratorMock1.Expect("Abort");
			integratorMock1.Expect("WaitForExit");
			integratorMock2.Expect("Abort");
			integratorMock2.Expect("WaitForExit");

			server.Abort();

			VerifyAll();
		}

		[Test]
		public void OnRestartKillAllIntegratorsRefreshConfigAndStartupNewIntegrators()
		{
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

			server.Start();

			integratorMock1.Expect("Stop",true);
			integratorMock1.Expect("WaitForExit");
			integratorMock2.Expect("Stop",true);
			integratorMock2.Expect("WaitForExit");

			configuration = new Configuration();
			configuration.AddProject(project1);
			integratorList = new ProjectIntegratorList();
			integratorList.Add(integrator1);
			configServiceMock.ExpectAndReturn("Load", configuration);
			projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);

			integratorMock1.Expect("Start");
			integratorMock2.ExpectNoCall("Start");

			server.Restart();

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
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

			server.Start();

            var parameters = new Dictionary<string, string>();
            integratorMock1.Expect("Request", new IntegrationRequestConstraint { Condition = BuildCondition.ForceBuild });

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
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

			server.Start();

			integratorMock1.Expect("WaitForExit");

			server.CruiseManager.WaitForExit("Project 1");

			VerifyAll();
		}

		[Test]
		public void ShouldOnlyDisposeOnce()
		{
			integratorMock1.Expect("Abort");
			integratorMock2.Expect("Abort");
			((IDisposable) server).Dispose();

			integratorMock1.ExpectNoCall("Abort");
			integratorMock2.ExpectNoCall("Abort");
			((IDisposable) server).Dispose();

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
            stateManagerMock.Expect("RecordProjectAsStopped", "Project 1");
            integratorMock1.Expect("Stop",false);
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
            stateManagerMock.Expect("RecordProjectAsStartable", "Project 2");
			integratorMock2.Expect("Start");
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
			integratorMock2.Expect("Request", request);
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
            integrator4 = mocks.DynamicMock<IProjectIntegrator>();
            SetupResult.For(integrator4.Name).Return("Project 4");
            integrator4.IntegrationStarted += null;
            IEventRaiser startEventRaiser = LastCall.IgnoreArguments()
                .GetEventRaiser();

            // Initialise a new cruise server with the new integrator
            mocks.ReplayAll();
            integratorList.Add(integrator4);
            configServiceMock.ExpectAndReturn("Load", configuration);
            projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);
            server = new CruiseServer((IConfigurationService)configServiceMock.MockInstance,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.MockInstance,
                                      (IProjectSerializer)projectSerializerMock.MockInstance,
									  (IProjectStateManager)stateManagerMock.MockInstance,
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


            startEventRaiser.Raise(integrator4, 
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
            integrator4 = mocks.DynamicMock<IProjectIntegrator>();
            SetupResult.For(integrator4.Name).Return("Project 4");
            integrator4.IntegrationCompleted += null;
            IEventRaiser startEventRaiser = LastCall.IgnoreArguments()
                .GetEventRaiser();

            // Initialise a new cruise server with the new integrator
            mocks.ReplayAll();
            integratorList.Add(integrator4);
            configServiceMock.ExpectAndReturn("Load", configuration);
            projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);
            server = new CruiseServer((IConfigurationService)configServiceMock.MockInstance,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.MockInstance,
                                      (IProjectSerializer)projectSerializerMock.MockInstance,
									  (IProjectStateManager)stateManagerMock.MockInstance,
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


            startEventRaiser.Raise(integrator4, 
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

            configServiceMock.ExpectAndReturn("Load", configuration);
            projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);
            server = new CruiseServer((IConfigurationService)configServiceMock.MockInstance,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.MockInstance,
                                      (IProjectSerializer)projectSerializerMock.MockInstance,
									  (IProjectStateManager)stateManagerMock.MockInstance,
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

            configServiceMock.ExpectAndReturn("Load", configuration);
            projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);

            Assert.That(delegate
                            {
                                new CruiseServer((IConfigurationService) configServiceMock.MockInstance,
                                                 (IProjectIntegratorListFactory)
                                                 projectIntegratorListFactoryMock.MockInstance,
                                                 (IProjectSerializer) projectSerializerMock.MockInstance,
                                                 (IProjectStateManager) stateManagerMock.MockInstance,
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

            configServiceMock.ExpectAndReturn("Load", configuration);
            projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);
            server = new CruiseServer((IConfigurationService)configServiceMock.MockInstance,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.MockInstance,
                                      (IProjectSerializer)projectSerializerMock.MockInstance,
									  (IProjectStateManager)stateManagerMock.MockInstance,
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
            this.mocks.ReplayAll();

            server.SecurityManager = securityManagerMock;
            var actual = server.GetFinalBuildStatus(request);

            this.mocks.VerifyAll();
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
            this.mocks.ReplayAll();

            server.SecurityManager = securityManagerMock;
            var actual = server.GetFinalBuildStatus(request);

            this.mocks.VerifyAll();
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
            var dataStoreMock = this.mocks.StrictMock<IDataStore>();
            SetupResult.For(dataStoreMock.LoadProjectSnapshot(project1, "Build #1"))
                .Return(new ProjectStatusSnapshot { Name = "Project 1" });
            this.mocks.ReplayAll();

            project1.DataStore = dataStoreMock;
            server.SecurityManager = securityManagerMock;
            var actual = server.GetFinalBuildStatus(request);

            this.mocks.VerifyAll();
            Assert.AreEqual(ResponseResult.Success, actual.Result);
            Assert.AreEqual("Project 1", actual.Snapshot.Name);
        }

        [Test]
        public void GetRSSFeedReturnsStatus()
        {
            var securityManagerMock = this.InitialiaseSecurityManagerMock(true, false);
            var request = new ProjectRequest("1234", "Project 1");
            var dataStoreMock = this.mocks.StrictMock<IDataStore>();
            SetupResult.For(dataStoreMock.LoadProjectSnapshot(project1, "Build #1"))
                .Return(new ProjectStatusSnapshot { Name = "Project 1" });
            this.mocks.ReplayAll();

            project1.RssFeedLoader = () => "RSS-Feed";
            server.SecurityManager = securityManagerMock;
            var actual = server.GetRSSFeed(request);

            this.mocks.VerifyAll();
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
            var securityManagerMock = this.mocks.StrictMock<ISecurityManager>();
            SetupResult.For(securityManagerMock.Channel).Return(null);
            SetupResult.For(securityManagerMock.RequiresSession).Return(true);
            SetupResult.For(securityManagerMock.GetUserName("1234")).Return("johnDoe");
            SetupResult.For(securityManagerMock.GetDisplayName("1234", null)).Return("John Doe");
            SetupResult.For(securityManagerMock.GetDefaultRight(SecurityPermission.ViewProject))
                .Return(SecurityRight.Inherit);
            SetupResult.For(securityManagerMock.CheckServerPermission("johnDoe", SecurityPermission.ViewProject))
                .Return(isAllowed);
            if (expectLogging)
            {
                Expect.Call(() => securityManagerMock.LogEvent(null, null, SecurityEvent.GetFinalBuildStatus, SecurityRight.Allow, null))
                    .IgnoreArguments();
            }

            return securityManagerMock;
        }
    }

    public class IntegrationRequestConstraint : BaseConstraint
    {
        public BuildCondition Condition { get; set; }
        private string message = null;

        public override bool Eval(object val)
        {
            if (val is IntegrationRequest)
            {
                if (!string.Equals(Condition, (val as IntegrationRequest).BuildCondition))
                {
                    message = "Conditions do not match";
                }
            }
            else
            {
                message = "Expected an IntegrationRequest";
            }
            return (message == null);
        }

        public override string Message
        {
            get { return message; }
        }
    }
}
