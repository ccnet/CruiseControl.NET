using NMock;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class ProjectIntegratorTest : CustomAssertion
	{
		private DynamicMock projectStopTriggerMock;
		private DynamicMock integrationTriggerMock;
		private DynamicMock integratableMock;
		private DynamicMock projectMock;
		private IIntegrationTrigger integrationTrigger;
		private IStopProjectTrigger StopProjectTrigger;
		private IIntegratable integratable;
		private IProject project;
		private ProjectIntegrator _integrator;

		[SetUp]
		public void SetUp()
		{
			integrationTriggerMock = new DynamicMock(typeof(IIntegrationTrigger));
			projectStopTriggerMock = new DynamicMock(typeof(IStopProjectTrigger));
			integratableMock = new DynamicMock(typeof(IIntegratable));
			projectMock = new DynamicMock(typeof(IProject));

			integrationTrigger = (IIntegrationTrigger) integrationTriggerMock.MockInstance;
			StopProjectTrigger = (IStopProjectTrigger) projectStopTriggerMock.MockInstance;
			integratable = (IIntegratable) integratableMock.MockInstance;
			project = (IProject) projectMock.MockInstance;

			_integrator = new ProjectIntegrator(integrationTrigger, StopProjectTrigger, integratable, project);
		}

		[TearDown]
		public void TearDown()
		{
			if (_integrator != null)
			{
				_integrator.Stop();
				_integrator.WaitForExit();
			}
		}

		private void VerifyAll()
		{
			integrationTriggerMock.Verify();
			projectStopTriggerMock.Verify();
			integratableMock.Verify();
			projectMock.Verify();
		}

		[Test]
		public void RunProjectOnce()
		{
			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			integratableMock.Expect("RunIntegration", BuildCondition.ForceBuild);
			integrationTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.ExpectAndReturn("ShouldStopProject", true);

			_integrator.Start();
			_integrator.WaitForExit();

			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
			VerifyAll();
		}

		[Test]
		public void RunProjectTwice()
		{
			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			integratableMock.Expect("RunIntegration", BuildCondition.ForceBuild);
			integrationTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.ExpectAndReturn("ShouldStopProject", false);

			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			integratableMock.Expect("RunIntegration", BuildCondition.IfModificationExists);
			integrationTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.ExpectAndReturn("ShouldStopProject", true);

			_integrator.Start();
			_integrator.WaitForExit();

			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
			VerifyAll();
		}

		[Test]
		public void RunProjectTwiceWithAGapInBetween()
		{
			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			integratableMock.Expect("RunIntegration", BuildCondition.ForceBuild);
			integrationTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.ExpectAndReturn("ShouldStopProject", false);

			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			projectStopTriggerMock.ExpectAndReturn("ShouldStopProject", false);

			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			integratableMock.Expect("RunIntegration", BuildCondition.IfModificationExists);
			integrationTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.ExpectAndReturn("ShouldStopProject", true);

			_integrator.Start();
			_integrator.WaitForExit();

			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
			VerifyAll();
		}

		[Test]
		public void ShouldContinueRunningIfNotToldToStop()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);

			_integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			VerifyAll();
		}

		[Test]
		public void ShouldStopWhenStoppedExternally()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);

			_integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);

			_integrator.Stop();
			_integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
			VerifyAll();
		}

		[Test]
		public void StartMultipleTimes()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);

			_integrator.Start();
			_integrator.Start();
			Thread.Sleep(0);
			_integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			_integrator.Stop();
			_integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
			VerifyAll();
		}

		[Test]
		public void RestartScheduler()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);

			_integrator.Start();
			Thread.Sleep(0);
			_integrator.Stop();
			_integrator.WaitForExit();

			_integrator.Start();
			Thread.Sleep(0);
			_integrator.Stop();
			_integrator.WaitForExit();		
			VerifyAll();
		}

		[Test]
		public void StopUnstartedIntegrator()
		{
			integrationTriggerMock.ExpectNoCall("ShouldRunIntegration");
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("ShouldStopProject");

			_integrator.Stop();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void VerifySchedulerStateAfterException()
		{
			TestTraceListener listener = new TestTraceListener();
			Trace.Listeners.Add(listener);
			string exceptionMessage = "Intentional exception";

			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			integratableMock.ExpectAndThrow("RunIntegration", new Exception(exceptionMessage), BuildCondition.ForceBuild);
			integrationTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.Expect("IntegrationCompleted");
			projectStopTriggerMock.ExpectAndReturn("ShouldStopProject", true);

			_integrator.Start();
			_integrator.WaitForExit();

			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
			Assert.IsTrue(listener.Traces.Count > 0);
			Assert.IsTrue(listener.Traces[0].ToString().IndexOf(exceptionMessage) > 0);
			
			Trace.Listeners.Remove(listener);
			VerifyAll();
		}

		[Test]
		public void Abort()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);

			_integrator.Start();
			Thread.Sleep(0);
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			_integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void TerminateWhenProjectIsntStarted()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);

			_integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void TerminateCalledTwice()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);

			_integrator.Start();
			Thread.Sleep(0);
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			_integrator.Abort();
			_integrator.Abort();
		}

		[Test]
		public void ForceBuild()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.ExpectNoCall("IntegrationCompleted");
			projectStopTriggerMock.SetupResult("ShouldStopProject", false);
			_integrator.ForceBuild();
		}
	}
}
