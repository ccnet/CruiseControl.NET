using System;
using System.Threading;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ProjectIntegratorTest : CustomAssertion
	{
		private LatchMock integrationTriggerMock;
		private LatchMock integratableMock;
		private DynamicMock projectMock;
		private ITrigger Trigger;
		private IIntegratable integratable;
		private IProject project;
		private ProjectIntegrator integrator;

		[SetUp]
		public void SetUp()
		{
			integrationTriggerMock = new LatchMock(typeof (ITrigger));
			integratableMock = new LatchMock(typeof (IIntegratable));
			projectMock = new DynamicMock(typeof (IProject));

			Trigger = (ITrigger) integrationTriggerMock.MockInstance;
			integratable = (IIntegratable) integratableMock.MockInstance;
			project = (IProject) projectMock.MockInstance;

			integrator = new ProjectIntegrator(Trigger, integratable, project);
		}

		[TearDown]
		public void TearDown()
		{
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
			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			VerifyAll();
		}

		[Test]
		public void ShouldStopWhenStoppedExternally()
		{
			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);

			integrator.Stop();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void StartMultipleTimes()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrator.Start();
			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Stop();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void RestartScheduler()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			integrator.Stop();
			integrator.WaitForExit();

			integrationTriggerMock.ResetLatch();
			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			integrator.Stop();
			integrator.WaitForExit();
			VerifyAll();
		}

		[Test]
		public void StopUnstartedIntegrator()
		{
			integrationTriggerMock.ExpectNoCall("ShouldRunIntegration");
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Stop();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void VerifySchedulerStateAfterException()
		{
			string exceptionMessage = "Intentional exception";

			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			integratableMock.ExpectAndThrow("Integrate", new CruiseControlException(exceptionMessage), new HasForceBuildCondition());
			integrationTriggerMock.Expect("IntegrationCompleted");

			integrator.Start();
			integratableMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Stop();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void Abort()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Abort();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void TerminateWhenProjectIsntStarted()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void TerminateCalledTwice()
		{
			integrationTriggerMock.SetupResult("ShouldRunIntegration", BuildCondition.NoBuild);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Abort();
			integrator.Abort();
			VerifyAll();
		}

		[Test]
		public void ForceBuild()
		{
			integrationTriggerMock.ExpectNoCall("ShouldRunIntegration");
			integratableMock.Expect("Integrate", new HasForceBuildCondition());
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.Expect("IntegrationCompleted");
			integrator.ForceBuild();
			integratableMock.WaitForSignal();
			integrationTriggerMock.WaitForSignal();
			VerifyAll();
		}

		[Test]
		public void RequestIntegration()
		{
			integratableMock.Strict = true;
			IntegrationRequest request = new IntegrationRequest("project", BuildCondition.IfModificationExists);
			integratableMock.Expect("Integrate", request);
			integrator.Request(request);
			integratableMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			VerifyAll();
		}

		[Test]
		public void ShouldClearRequestQueueAsSoonAsRequestIsProcessed()
		{
			integratableMock.Strict = true;
			integrationTriggerMock.Strict = true;
			IntegrationRequest request = new IntegrationRequest("project", BuildCondition.IfModificationExists);
			integratableMock.Expect("Integrate", request);
			integrationTriggerMock.Expect("IntegrationCompleted");
			integrationTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);

			integrator.Request(request);
			integratableMock.WaitForSignal();
			integrationTriggerMock.WaitForSignal();
			integrationTriggerMock.ResetLatch();	// should autoreset as soon as signalled.
			integrationTriggerMock.WaitForSignal();
			VerifyAll();
		}

		// should clear request queue as soon as request is processed.
	}

	public class HasForceBuildCondition : BaseConstraint
	{
		public override bool Eval(object val)
		{
			return ((IntegrationRequest) val).BuildCondition == BuildCondition.ForceBuild;
		}

		public override string Message
		{
			get { return "IntegrationRequest is not ForceBuild."; }
		}
	}

	public class LatchMock : DynamicMock
	{
		private ManualResetEvent latch = new ManualResetEvent(false);

		public LatchMock(Type type) : base(type)
		{}

		public override object Invoke(string methodName, params object[] args)
		{
			try
			{
				return base.Invoke(methodName, args);
			}
			finally
			{
				latch.Set();
			}
		}

		public void WaitForSignal()
		{
			bool signalled = latch.WaitOne(2000, false);
			if (! signalled)
			{
				throw new Exception("Latch has not been signalled before the timeout expired!");
			}
		}

		public void ResetLatch()
		{
			latch.Reset();
		}
	}
}