using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ProjectIntegratorTest : IntegrationFixture
	{
		private LatchMock integrationTriggerMock;
		private LatchMock projectMock;
		private ProjectIntegrator integrator;

		[SetUp]
		public void SetUp()
		{
			integrationTriggerMock = new LatchMock(typeof (ITrigger));
			integrationTriggerMock.Strict = true;
			projectMock = new LatchMock(typeof (IProject));
			projectMock.Strict = true;
			projectMock.SetupResult("Name", "project");
			projectMock.SetupResult("Triggers", integrationTriggerMock.MockInstance);

			integrator = new ProjectIntegrator((IProject) projectMock.MockInstance);
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
			projectMock.Verify();
		}

		[Test]
		public void ShouldContinueRunningIfNotToldToStop()
		{
			integrationTriggerMock.SetupResultAndSignal("Fire", null);
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Start();
			integrationTriggerMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			VerifyAll();
		}

		[Test]
		public void ShouldStopWhenStoppedExternally()
		{
			integrationTriggerMock.SetupResultAndSignal("Fire", null);
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
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
			integrationTriggerMock.SetupResultAndSignal("Fire", null);
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
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
		public void RestartIntegrator()
		{
			integrationTriggerMock.SetupResultAndSignal("Fire", null);
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
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
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Stop();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void VerifyStateAfterException()
		{
			string exceptionMessage = "Intentional exception";

			integrationTriggerMock.ExpectAndReturn("Fire", ForceBuildRequest());
			projectMock.ExpectAndThrowAndSignal("Integrate", new CruiseControlException(exceptionMessage),
			                                    new HasForceBuildCondition());
			integrationTriggerMock.Expect("IntegrationCompleted");

			integrator.Start();
			projectMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			integrator.Stop();
			integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void Abort()
		{
			integrationTriggerMock.SetupResultAndSignal("Fire", null);
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
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
			integrationTriggerMock.SetupResultAndSignal("Fire", null);
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectNoCall("IntegrationCompleted");

			integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, integrator.State);
			VerifyAll();
		}

		[Test]
		public void TerminateCalledTwice()
		{
			integrationTriggerMock.SetupResultAndSignal("Fire", null);
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
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
			projectMock.ExpectAndSignal("Integrate", new HasForceBuildCondition());
			projectMock.ExpectNoCall("Integrate", typeof (IntegrationRequest));
			integrationTriggerMock.ExpectAndSignal("IntegrationCompleted");
			integrator.ForceBuild();
			projectMock.WaitForSignal();
			integrationTriggerMock.WaitForSignal();
			VerifyAll();
		}

		[Test]
		public void RequestIntegration()
		{
			IntegrationRequest request = new IntegrationRequest(BuildCondition.IfModificationExists, "intervalTrigger");
			projectMock.ExpectAndSignal("Integrate", request);
			integrationTriggerMock.ExpectAndSignal("IntegrationCompleted");
			integrator.Request(request);
			projectMock.WaitForSignal();
			integrationTriggerMock.WaitForSignal();
			Assert.AreEqual(ProjectIntegratorState.Running, integrator.State);
			VerifyAll();
		}

		[Test]
		public void ShouldClearRequestQueueAsSoonAsRequestIsProcessed()
		{
			IntegrationRequest request = new IntegrationRequest(BuildCondition.IfModificationExists, "intervalTrigger");
			projectMock.ExpectAndSignal("Integrate", request);
			integrationTriggerMock.ExpectAndSignal("IntegrationCompleted");
			integrationTriggerMock.ExpectAndReturnAndSignal("Fire", null);

			integrator.Request(request);
			projectMock.WaitForSignal();
			integrationTriggerMock.WaitForSignal();
			integrationTriggerMock.ResetLatch(); // should autoreset as soon as signalled.
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
}