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
	public class ProjectIntegratorTest : IntegrationFixture
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
			integrationTriggerMock.ExpectAndReturn("Fire", null);
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
			integrationTriggerMock.ExpectAndReturn("Fire", null);
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
			integrationTriggerMock.SetupResult("Fire", null);
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
			integrationTriggerMock.SetupResult("Fire", null);
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
			integrationTriggerMock.ExpectNoCall("Fire");
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

			integrationTriggerMock.ExpectAndReturn("Fire", ForceBuildRequest());
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
			integrationTriggerMock.SetupResult("Fire", null);
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
			integrationTriggerMock.SetupResult("Fire", null);
			integratableMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void TerminateCalledTwice()
		{
			integrationTriggerMock.SetupResult("Fire", null);
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
			integrationTriggerMock.ExpectNoCall("Fire");
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
			IntegrationRequest request = new IntegrationRequest(BuildCondition.IfModificationExists, "intervalTrigger");
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
			IntegrationRequest request = new IntegrationRequest(BuildCondition.IfModificationExists, "intervalTrigger");
			integratableMock.Expect("Integrate", request);
			integrationTriggerMock.Expect("IntegrationCompleted");
			integrationTriggerMock.ExpectAndReturn("Fire", null);

			integrator.Request(request);
			integratableMock.WaitForSignal();
			integrationTriggerMock.WaitForSignal();
			integrationTriggerMock.ResetLatch();	// should autoreset as soon as signalled.
			integrationTriggerMock.WaitForSignal();
			VerifyAll();
		}
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
		private VerifyException ex;

		public LatchMock(Type type) : base(type)
		{}

		public override object Invoke(string methodName, params object[] args)
		{
			try
			{
				return base.Invoke(methodName, args);
			}
			catch (VerifyException ex)
			{
				this.ex = ex;
				throw;
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
			if (ex != null)
			{
				throw ex;
			}
		}

		public void ResetLatch()
		{
			latch.Reset();
		}
	}
}