using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectMonitorTest
	{
		private DynamicMock mockProjectManager;
		private ProjectMonitor monitor;
		private int pollCount;
		private int buildOccurredCount;
		private BuildOccurredEventArgs lastBuildOccurredArgs;


		[SetUp]
		public void SetUp()
		{
			buildOccurredCount = pollCount = 0;
			mockProjectManager = new DynamicMock(typeof (ICruiseProjectManager));
			mockProjectManager.Strict = true;
			monitor = new ProjectMonitor((ICruiseProjectManager) mockProjectManager.MockInstance);
			monitor.Polled += new PolledEventHandler(Monitor_Polled);
			monitor.BuildOccurred += new BuildOccurredEventHandler(Monitor_BuildOccurred);
		}

		[TearDown]
		public void TearDown()
		{
			mockProjectManager.Verify();
		}

		[Test]
		public void WhenPollIsCalledRetrivesANewCopyOfTheProjectStatus()
		{
			ProjectStatus status = new ProjectStatus();
			mockProjectManager.ExpectAndReturn("ProjectStatus", status);

			monitor.Poll();

			// deliberately called twice: should not go back to server on 2nd
			// call
			Assert.AreSame(status, monitor.ProjectStatus);
			Assert.AreSame(status, monitor.ProjectStatus);
		}

		[Test]
		public void ThePollEventIsFiredWhenPollIsInvoked()
		{
			Assert.AreEqual(0, pollCount);

			ProjectStatus status = new ProjectStatus();
			mockProjectManager.ExpectAndReturn("ProjectStatus", status);
			monitor.Poll();
			Assert.AreEqual(1, pollCount);

			mockProjectManager.ExpectAndReturn("ProjectStatus", status);
			monitor.Poll();
			Assert.AreEqual(2, pollCount);
		}

		[Test]
		public void IfTheLastBuildDateHasChangedABuildOccuredEventIsFired()
		{
			Assert.AreEqual(0, buildOccurredCount);

			mockProjectManager.ExpectAndReturn("ProjectStatus", 
				CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 1)));
			monitor.Poll();

			Assert.AreEqual(0, buildOccurredCount);

			mockProjectManager.ExpectAndReturn("ProjectStatus", 
				CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 1)));
			monitor.Poll();

			Assert.AreEqual(0, buildOccurredCount);

			mockProjectManager.ExpectAndReturn("ProjectStatus", 
				CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 2)));
			monitor.Poll();

			Assert.AreEqual(1, buildOccurredCount);

			mockProjectManager.ExpectAndReturn("ProjectStatus", 
				CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 3)));
			monitor.Poll();

			Assert.AreEqual(2, buildOccurredCount);
		}

		
		[Test]
		public void NotifiesCorrectlyForStillSuccessfulBuild()
		{
			AssertTransition(IntegrationStatus.Success, IntegrationStatus.Success, BuildTransition.StillSuccessful);
		}

		[Test]
		public void NotifiesCorrectlyForBrokenBuild()
		{
			AssertTransition(IntegrationStatus.Success, IntegrationStatus.Failure, BuildTransition.Broken);
		}

		[Test]
		public void NotifiesCorrectlyForStillFailingBuild()
		{
			AssertTransition(IntegrationStatus.Failure, IntegrationStatus.Failure, BuildTransition.StillFailing);
		}

		[Test]
		public void NotifiesCorrectlyForFixedBuild()
		{
			AssertTransition(IntegrationStatus.Failure, IntegrationStatus.Success, BuildTransition.Fixed);
		}

		private void AssertTransition(
			IntegrationStatus initialIntegrationStatus,
			IntegrationStatus nextBuildIntegrationStatus,
			BuildTransition expectedBuildTransition)
		{
			// initial connection
			mockProjectManager.ExpectAndReturn("ProjectStatus", 
				CreateProjectStatus(initialIntegrationStatus, new DateTime(2004, 1, 1)));
			monitor.Poll();

			// then the build
			mockProjectManager.ExpectAndReturn("ProjectStatus", 
				CreateProjectStatus(nextBuildIntegrationStatus, new DateTime(2004, 1, 2)));
			monitor.Poll();

			Assert.AreEqual(1, buildOccurredCount);
			Assert.AreEqual(expectedBuildTransition, lastBuildOccurredArgs.BuildTransition);

			buildOccurredCount = 0;

		}



		private void Monitor_Polled(object sauce, PolledEventArgs e)
		{
			pollCount++;
		}

		private void Monitor_BuildOccurred(object sauce, BuildOccurredEventArgs e)
		{
			buildOccurredCount++;
			lastBuildOccurredArgs = e;
		}

		private ProjectStatus CreateProjectStatus(
			IntegrationStatus integrationStatus, DateTime lastBuildDate)
		{
			ProjectStatus status = new ProjectStatus();
			status.BuildStatus = integrationStatus;
			status.LastBuildDate = lastBuildDate;
			return status;
		}
	}
}