using System;
using System.Threading;
using System.Xml;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class CruiseServerTest : IntegrationFixture
	{
		private DynamicMock configServiceMock;
		private DynamicMock projectIntegratorListFactoryMock;
		private DynamicMock projectSerializerMock;
		private DynamicMock integratorMock1;
		private DynamicMock integratorMock2;
		private DynamicMock integratorMock3;

		private CruiseServer server;

		private Configuration configuration;
		private Project project1;
		private Project project2;

		private IMock mockProject;
		private IProject mockProjectInstance;
		
		private IProjectIntegrator integrator1;
		private IProjectIntegrator integrator2;
		private IProjectIntegrator integrator3;
		private ProjectIntegratorList integratorList;

		private ManualResetEvent monitor;
		private XmlDocument statistics;
		private XmlDocument statisticsClone;

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

			configuration = new Configuration();
			project1 = new Project();
			project1.Name = "Project 1";
			
			project2 = new Project();
			project2.Name = "Project 2";

			mockProject = new DynamicMock(typeof(IProject));
			statistics = new XmlDocument();
			XmlElement root = statistics.CreateElement("statistics");
			statistics.AppendChild(root);
			statisticsClone = new XmlDocument();
			statisticsClone.LoadXml(statistics.OuterXml);
			mockProject.ExpectAndReturn("Name", "Project 3");
			mockProject.ExpectAndReturn("Statistics", statistics);
			mockProjectInstance = (IProject) mockProject.MockInstance;
			integratorMock3.ExpectAndReturn("Project", mockProjectInstance);

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

			integratorMock1.Expect("ForceBuild");

			server.ForceBuild("Project 1");

			VerifyAll();
		}

		[Test, ExpectedException(typeof (NoSuchProjectException))]
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
			integratorMock1.Expect("Stop");
			server.Stop("Project 1");
			integratorMock1.Verify();
		}

		[Test, ExpectedException(typeof(NoSuchProjectException))]
		public void ThrowExceptionIfProjectNotFound()
		{
			server.Stop("Project unknown");			
		}

		[Test]
		public void StartSpecificProject()
		{
			integratorMock2.Expect("Start");
			server.Start("Project 2");
			integratorMock2.Verify();			
		}

		[Test]
		public void RequestNewIntegration()
		{
			IntegrationRequest request = Request(BuildCondition.IfModificationExists);
			integratorMock2.Expect("Request", request);
			server.Request("Project 2", request);
			integratorMock1.Verify();
			integratorMock2.Verify();
		}

		[Test]
		public void GetStatisticsDocumentSetsCurrentDate()
		{
			mockProject.ExpectAndReturn("Name", "Project 3");
			string statisticsDocument = server.GetStatisticsDocument(mockProjectInstance.Name);
			XmlDocument fromServer = new XmlDocument();
			fromServer.LoadXml(statisticsDocument);
			Assert.AreEqual(statisticsClone.DocumentElement.ChildNodes.Count + 1, fromServer.DocumentElement.ChildNodes.Count);
//			XPathNavigator navigator = fromServer.CreateNavigator();
//			object o = navigator.Evaluate("/statistics//timestamp/@day");
//			string s = o.ToString();
//			int day = (int) o;
//			int month = (int) navigator.Evaluate("/statistics//timestamp/@month");
//			int year = (int) navigator.Evaluate("/statistics//timestamp/@year");
//
//			Assert.AreEqual(DateTime.Now.Day, day);
//			Assert.AreEqual(DateTime.Now.Month, month);
//			Assert.AreEqual(DateTime.Now.Year, year);
		}
	}
}