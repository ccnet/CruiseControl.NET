using System.Threading;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ProjectIntegratorTest : CustomAssertion
	{
		private DynamicMock integrationTriggerMock;
		private DynamicMock integratableMock;
		private DynamicMock projectMock;
		private ITrigger Trigger;
		private IIntegratable integratable;
		private IProject project;
		private ProjectIntegrator _integrator;
		private TraceListenerBackup backup;

		[SetUp]
		public void SetUp()
		{
			backup = new TraceListenerBackup();
			integrationTriggerMock = new DynamicMock(typeof(ITrigger));
			integratableMock = new DynamicMock(typeof(IIntegratable));
			projectMock = new DynamicMock(typeof(IProject));

			Trigger = (ITrigger) integrationTriggerMock.MockInstance;
			integratable = (IIntegratable) integratableMock.MockInstance;
			project = (IProject) projectMock.MockInstance;

			_integrator = new ProjectIntegrator(Trigger, integratable, project);
		}

		[TearDown]
		public void TearDown()
		{
			backup.Reset();

			if (_integrator != null)
			{
				_integrator.Stop();
				_integrator.WaitForExit();
			}
		}

		private void VerifyAll()
		{
			integrationTriggerMock.Verify();
			integratableMock.Verify();
			projectMock.Verify();
		}

		[Test]
		public void ShouldContinueRunningIfNotToldToStop()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

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

			_integrator.Start();
			_integrator.Start();
			Thread.Sleep(110);
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

			_integrator.Start();
			Thread.Sleep(110);
			_integrator.Stop();
			_integrator.WaitForExit();

			_integrator.Start();
			Thread.Sleep(110);
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

			_integrator.Stop();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void VerifySchedulerStateAfterException()
		{
			TestTraceListener listener = backup.AddTestTraceListener();
			string exceptionMessage = "Intentional exception";

			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			integratableMock.ExpectAndThrow("RunIntegration", new CruiseControlException(exceptionMessage), BuildCondition.ForceBuild);
			integrationTriggerMock.Expect("IntegrationCompleted");

			_integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			Thread.Sleep(110);
			_integrator.Stop();
			_integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);

			Assert.IsTrue(listener.Traces.Count > 0);
			Assert.IsTrue(listener.Traces[0].ToString().IndexOf(exceptionMessage) > 0);

			VerifyAll();
		}

		[Test]
		public void Abort()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			_integrator.Start();
			Thread.Sleep(110);
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			_integrator.Abort();
			_integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void TerminateWhenProjectIsntStarted()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			_integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void TerminateCalledTwice()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			_integrator.Start();
			Thread.Sleep(110);
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
			_integrator.ForceBuild();
		}
	}
}
