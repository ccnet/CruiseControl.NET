using System;
using System.Threading;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class CruiseServerTest
	{
		private DynamicMock configServiceMock;
		private DynamicMock projectIntegratorListFactoryMock;
		private DynamicMock projectSerializerMock;
		private DynamicMock integratorMock1;
		private DynamicMock integratorMock2;

		private CruiseServer server;

		private Configuration configuration;
		private Project project1;
		private Project project2;
		private IProjectIntegrator integrator1;
		private IProjectIntegrator integrator2;
		private ProjectIntegratorList integratorList;

		private ManualResetEvent monitor;

		[SetUp]
		protected void SetUp()
		{
			projectSerializerMock = new DynamicMock(typeof(IProjectSerializer));

			integratorMock1 = new DynamicMock(typeof(IProjectIntegrator));
			integratorMock2 = new DynamicMock(typeof(IProjectIntegrator));
			integrator1 = (IProjectIntegrator) integratorMock1.MockInstance;
			integrator2 = (IProjectIntegrator) integratorMock2.MockInstance;
			integratorMock1.SetupResult("Name", "Project 1");
			integratorMock2.SetupResult("Name", "Project 2");

			configuration = new Configuration();
			project1 = new Project();
			project1.Name = "Project 1";
			project2 = new Project();
			project2.Name = "Project 2";
			configuration.AddProject(project1);
			configuration.AddProject(project2);

			integratorList = new ProjectIntegratorList();
			integratorList.Add(integrator1);
			integratorList.Add(integrator2);

			configServiceMock = new DynamicMock(typeof(IConfigurationService));
			configServiceMock.ExpectAndReturn("Load", configuration);

			projectIntegratorListFactoryMock = new DynamicMock(typeof(IProjectIntegratorListFactory));
			projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects);

			server = new CruiseServer((IConfigurationService) configServiceMock.MockInstance, 
				(IProjectIntegratorListFactory) projectIntegratorListFactoryMock.MockInstance,
				(IProjectSerializer) projectSerializerMock.MockInstance);
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
			projectIntegratorListFactoryMock.ExpectAndReturn("CreateProjectIntegrators", integratorList, configuration.Projects);

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
			Thread.Sleep(0);
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
			Thread.Sleep(0);
			server.Abort();
		}

		[Test]
		public void ForceBuildForProject()
		{
			integratorMock1.Expect("Start");
			integratorMock2.Expect("Start");

			server.Start();

			integratorMock1.Expect("ForceBuild");

			server.ForceBuild("Project 1");

			VerifyAll();
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void AttemptToForceBuildOnProjectThatDoesNotExist()
		{
			server.ForceBuild("foo");
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
			((IDisposable)server).Dispose();

			integratorMock1.ExpectNoCall("Abort");
			integratorMock2.ExpectNoCall("Abort");
			((IDisposable)server).Dispose();

			integratorMock1.Verify();
			integratorMock2.Verify();
		}
	}
}
