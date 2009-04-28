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
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;
using Rhino.Mocks.Interfaces;
using ThoughtWorks.CruiseControl.UnitTests.Remote;
using System.IO;

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

		private ManualResetEvent monitor;

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

			integratorMock1.Expect("Stop");
			integratorMock1.Expect("WaitForExit");
			integratorMock2.Expect("Stop");
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

			integratorMock1.Expect("Stop");
			integratorMock1.Expect("WaitForExit");
			integratorMock2.Expect("Stop");
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

            integratorMock1.Expect("ForceBuild", "BuildForcer");

			server.ForceBuild(null,"Project 1","BuildForcer");

			VerifyAll();
		}

		[Test, ExpectedException(typeof (NoSuchProjectException))]
		public void AttemptToForceBuildOnProjectThatDoesNotExist()
		{
            server.ForceBuild(null,"foo", "BuildForcer");
		}

		[Test]
		public void WaitForExitForProject()
		{
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

			server.Start();

			integratorMock1.Expect("WaitForExit");

			server.WaitForExit("Project 1");

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
			string ServerVersion = server.GetVersion();
			Assert.IsFalse(ServerVersion.Length == 0, "Version not retrieved");
		}

		[Test]
		public void StopSpecificProject()
		{
            stateManagerMock.Expect("RecordProjectAsStopped", "Project 1");
            integratorMock1.Expect("Stop");
			server.Stop(null,"Project 1");
			integratorMock1.Verify();
            stateManagerMock.Verify();
        }

		[Test, ExpectedException(typeof(NoSuchProjectException))]
		public void ThrowExceptionIfProjectNotFound()
		{
			server.Stop(null, "Project unknown");			
		}

		[Test]
		public void StartSpecificProject()
		{
            stateManagerMock.Expect("RecordProjectAsStartable", "Project 2");
			integratorMock2.Expect("Start");
			server.Start(null,"Project 2");
			integratorMock2.Verify();
            stateManagerMock.Verify();
		}

		[Test]
		public void RequestNewIntegration()
		{
			IntegrationRequest request = Request(BuildCondition.IfModificationExists);
			integratorMock2.Expect("Request", request);
			server.Request(null,"Project 2", request);
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

            server.Start(null, projectName);
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

            server.Start(null, projectName);
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

            server.Stop(null, projectName);
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

            server.Stop(null, projectName);
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
                Assert.AreEqual(enforcer, e.Data);
            };

            bool forceBuildProcessed = false;
            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                forceBuildProcessed = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(enforcer, e.Data);
            };

            server.ForceBuild(null, projectName, enforcer);
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
                Assert.AreEqual(enforcer, e.Data);
                e.Cancel = true;
            };

            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                Assert.Fail("ForceBuildProcessed has been fired");
            };

            server.ForceBuild(null, projectName, enforcer);
            Assert.IsTrue(forceBuildReceived, "ForceBuildReceived not fired");
        }

        [Test]
        public void RequestFiresEvents()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer);
            bool forceBuildReceived = false;
            server.ForceBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                forceBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(enforcer, e.Data);
            };

            bool forceBuildProcessed = false;
            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                forceBuildProcessed = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(enforcer, e.Data);
            };

            server.Request(null, projectName, request);
            Assert.IsTrue(forceBuildReceived, "ForceBuildReceived not fired");
            Assert.IsTrue(forceBuildProcessed, "ForceBuildProcessed not fired");
        }

        [Test]
        public void RequestCanBeCancelled()
        {
            string projectName = "Project 1";
            string enforcer = "JohnDoe";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer);
            bool forceBuildReceived = false;
            server.ForceBuildReceived += delegate(object o, CancelProjectEventArgs<string> e)
            {
                forceBuildReceived = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(enforcer, e.Data);
                e.Cancel = true;
            };

            server.ForceBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                Assert.Fail("ForceBuildProcessed has been fired");
            };

            server.Request(null, projectName, request);
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
                Assert.AreEqual(enforcer, e.Data);
            };

            bool abortBuildProcessed = false;
            server.AbortBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                abortBuildProcessed = true;
                Assert.AreEqual(projectName, e.ProjectName);
                Assert.AreEqual(enforcer, e.Data);
            };

            server.AbortBuild(null, projectName, enforcer);
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
                Assert.AreEqual(enforcer, e.Data);
                e.Cancel = true;
            };

            server.AbortBuildProcessed += delegate(object o, ProjectEventArgs<string> e)
            {
                Assert.Fail("AbortBuildProcessed has been fired");
            };

            server.AbortBuild(null, projectName, enforcer);
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

            server.SendMessage(null, projectName, message);
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

            server.SendMessage(null, projectName, message);
            Assert.IsTrue(sendMessageReceived, "SendMessageReceived not fired");
        }

        [Test]
        public void IntegrationStartedIsFired()
        {
            string enforcer = "JohnDoe";
            string projectName = "Project 4";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer);

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
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, enforcer);

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
                                      extensions);
            Assert.IsTrue(ServerExtensionStub.HasInitialised);

            server.Start();
            Assert.IsTrue(ServerExtensionStub.HasStarted);

            server.Stop();
            Assert.IsTrue(ServerExtensionStub.HasStopped);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException),
            ExpectedMessage = "Unable to find extension 'ThoughtWorks.CruiseControl.UnitTests.Remote.Garbage,ThoughtWorks.CruiseControl.UnitTests'")]
        public void InitialiseingANonExistantExtensionThrowsAnException()
        {
            List<ExtensionConfiguration> extensions = new List<ExtensionConfiguration>();
            ExtensionConfiguration extensionStub = new ExtensionConfiguration();
            extensionStub.Type = "ThoughtWorks.CruiseControl.UnitTests.Remote.Garbage,ThoughtWorks.CruiseControl.UnitTests";
            extensions.Add(extensionStub);

            configServiceMock.ExpectAndReturn("Load", configuration);
            projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects, integrationQueue);
            server = new CruiseServer((IConfigurationService)configServiceMock.MockInstance,
                                      (IProjectIntegratorListFactory)projectIntegratorListFactoryMock.MockInstance,
                                      (IProjectSerializer)projectSerializerMock.MockInstance,
                                      (IProjectStateManager)stateManagerMock.MockInstance,
                                      extensions);
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
                                      extensions);
            Assert.IsTrue(ServerExtensionStub.HasInitialised);

            server.Start();
            Assert.IsTrue(ServerExtensionStub.HasStarted);

            server.Abort();
            Assert.IsTrue(ServerExtensionStub.HasAborted);
        }

        [Test]
        [ExpectedException(typeof(NoSuchProjectException))]
        public void TakeSnapshotThrowsExceptionForUnknownProject()
        {
            server.TakeStatusSnapshot("garbage project");
        }

        [Test]
        public void TakeSnapshotReturnsAValidSnapshot()
        {
            ProjectStatusSnapshot snapshot = server.TakeStatusSnapshot("Project 1");
            Assert.IsNotNull(snapshot, "Snapshot not taken");
            Assert.AreEqual("Project 1", snapshot.Name, "Name not set");
        }

        [Test]
        [ExpectedException(typeof(CruiseControlException))]
        public void RetrieveFileTransferOnlyWorksForFilesInArtefactFolder()
        {
            server.RetrieveFileTransfer("Project 1", @"..\testfile.txt");
        }

        [Test]
        [ExpectedException(typeof(CruiseControlException))]
        public void RetrieveFileTransferFailsForBuildLogsFolder()
        {
            server.RetrieveFileTransfer("Project 1", @"BuildLogs\testfile.txt");
        }

        [Test]
        [ExpectedException(typeof(CruiseControlException))]
        public void RetrieveFileTransferFailsForAbsolutePaths()
        {
            server.RetrieveFileTransfer("Project 1", @"C:\MyFile.txt");
        }

        [Test]
        public void RetrieveFileTransferGeneratesTransferForValidFile()
        {
            var tempFile = Path.GetTempFileName();
            if (!File.Exists(tempFile)) File.WriteAllText(tempFile, "This is a test");
            project1.ConfiguredArtifactDirectory = Path.GetDirectoryName(tempFile);
            var transfer = server.RetrieveFileTransfer("Project 1", Path.GetFileName(tempFile));
            Assert.IsNotNull(transfer);
        }

        [Test]
        public void RetrieveFileTransferGeneratesNullForInvalidFile()
        {
            var transfer = server.RetrieveFileTransfer("Project 1", "GarbageFileNameThatShouldNotExist.NotHere");
            Assert.IsNull(transfer);
        }
    }
}
