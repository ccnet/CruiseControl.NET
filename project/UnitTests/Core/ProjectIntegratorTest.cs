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
		private IProject project;
		private ProjectIntegrator integrator;
		private TraceListenerBackup backup;

		[SetUp]
		public void SetUp()
		{
			backup = new TraceListenerBackup();
			integrationTriggerMock = new DynamicMock(typeof(ITrigger));
			integratableMock = new DynamicMock(typeof(IIntegratable));
			projectMock = new DynamicMock(typeof(IProject));

			Trigger = (ITrigger) integrationTriggerMock.MockInstance;
			project = (IProject) projectMock.MockInstance;

			integrator = new ProjectIntegrator(Trigger, project, new ThreadPond());
		}

		[TearDown]
		public void TearDown()
		{
			backup.Reset();

			if (integrator != null)
			{
				integrator.Stop();
				integrator.WaitForExit();
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

			integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			VerifyAll();
		}

		[Test]
		public void ShouldStopWhenStoppedExternally()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);

			integrator.Stop();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]	// remove sleep!
		public void StartMultipleTimes()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrator.Start();
			Thread.Sleep(110);
			integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Stop();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]	// remove sleep!
		public void RestartScheduler()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			Thread.Sleep(110);
			integrator.Stop();
			integrator.WaitForExit();

			integrator.Start();
			Thread.Sleep(110);
			integrator.Stop();
			integrator.WaitForExit();		
			VerifyAll();
		}

		[Test]
		public void StopUnstartedIntegrator()
		{
			integrationTriggerMock.ExpectNoCall("ShouldRunIntegration");
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Stop();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
		}

		[Test, Ignore("Owen - test needss to be reworked")]	// remove sleep!
		public void VerifySchedulerStateAfterException()
		{
			backup.Reset();
			TestTraceListener listener = backup.AddTestTraceListener();
			string exceptionMessage = "Intentional exception";

			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			integratableMock.ExpectAndThrow("RunIntegration", new CruiseControlException(exceptionMessage), BuildCondition.ForceBuild);
			integrationTriggerMock.Expect("IntegrationCompleted");

			integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			Thread.Sleep(110);
			integrator.Stop();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);

			Assert.IsTrue(listener.Traces.Count > 0);
			Assert.IsTrue(listener.Traces[0].ToString().IndexOf(exceptionMessage) > 0);

			VerifyAll();
		}

		[Test]	// remove sleep!
		public void Abort()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			Thread.Sleep(110);
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Abort();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
		}

		[Test]
		public void TerminateWhenProjectIsntStarted()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
		}

		[Test]	// remove sleep!
		public void TerminateCalledTwice()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			Thread.Sleep(110);
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Abort();
			integrator.Abort();
		}

		[Test]
		public void ForceBuild()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("RunIntegration", typeof(BuildCondition));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");
			integrator.ForceBuild();
		}
	}
}