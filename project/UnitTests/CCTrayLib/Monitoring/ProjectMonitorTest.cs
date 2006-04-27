using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectMonitorTest
	{
		private DynamicMock mockProjectManager;
		private ProjectMonitor monitor;
		private int pollCount;
		private int buildOccurredCount;
		private MonitorBuildOccurredEventArgs lastBuildOccurredArgs;
		private Message actualMessage;

		[SetUp]
		public void SetUp()
		{
			buildOccurredCount = pollCount = 0;
			mockProjectManager = new DynamicMock(typeof (ICruiseProjectManager));
			mockProjectManager.Strict = true;
			monitor = new ProjectMonitor((ICruiseProjectManager) mockProjectManager.MockInstance);
			monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
			monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
		}

		[TearDown]
		public void TearDown()
		{
			mockProjectManager.Verify();
			actualMessage = null;
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
		public void WhenPollingEncountersAnExceptionThePolledEventIsStillFired()
		{
			Assert.AreEqual(0, pollCount);

			mockProjectManager.ExpectAndThrow("ProjectStatus", new Exception("should be caught"));
			monitor.Poll();
			Assert.AreEqual(1, pollCount);
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

		private void Monitor_Polled(object sauce, MonitorPolledEventArgs args)
		{
			pollCount++;
		}

		private void Monitor_BuildOccurred(object sauce, MonitorBuildOccurredEventArgs e)
		{
			buildOccurredCount++;
			lastBuildOccurredArgs = e;
		}

		private ProjectStatus CreateProjectStatus(IntegrationStatus integrationStatus, DateTime lastBuildDate)
		{
			return ProjectStatusFixture.New(integrationStatus, lastBuildDate);
		}

		private ProjectStatus CreateProjectStatus(IntegrationStatus integrationStatus, ProjectActivity activity)
		{
			return ProjectStatusFixture.New(integrationStatus, activity);
		}

		[Test]
		public void CorrectlyDeterminesProjectState()
		{
			Assert.AreEqual(ProjectState.NotConnected, monitor.ProjectState);

			mockProjectManager.ExpectAndReturn("ProjectStatus",
			                                   CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.Sleeping));
			monitor.Poll();
			Assert.AreEqual(ProjectState.Success, monitor.ProjectState);

			mockProjectManager.ExpectAndReturn("ProjectStatus",
			                                   CreateProjectStatus(IntegrationStatus.Exception, ProjectActivity.Sleeping));
			monitor.Poll();
			Assert.AreEqual(ProjectState.Broken, monitor.ProjectState);

			mockProjectManager.ExpectAndReturn("ProjectStatus",
			                                   CreateProjectStatus(IntegrationStatus.Failure, ProjectActivity.Sleeping));
			monitor.Poll();
			Assert.AreEqual(ProjectState.Broken, monitor.ProjectState);

			mockProjectManager.ExpectAndReturn("ProjectStatus",
			                                   CreateProjectStatus(IntegrationStatus.Unknown, ProjectActivity.Sleeping));
			monitor.Poll();
			Assert.AreEqual(ProjectState.Broken, monitor.ProjectState);

			mockProjectManager.ExpectAndReturn("ProjectStatus",
			                                   CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.Building));
			monitor.Poll();

			mockProjectManager.ExpectAndReturn("ProjectStatus",
			                                   CreateProjectStatus(IntegrationStatus.Success,
			                                                       ProjectActivity.CheckingModifications));
			monitor.Poll();
			Assert.AreEqual(ProjectState.Success, monitor.ProjectState);

			mockProjectManager.ExpectAndReturn("ProjectStatus",
			                                   null);
			monitor.Poll();
			Assert.AreEqual(ProjectState.NotConnected, monitor.ProjectState);
		}

		[Test]
		public void DoNotTransitionProjectStateForNewInstanceOfSameProjectActivity()
		{
			mockProjectManager.ExpectAndReturn("ProjectStatus", CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.Building));
			monitor.Poll();
			Assert.AreEqual(ProjectState.Building, monitor.ProjectState);			
			mockProjectManager.ExpectAndReturn("ProjectStatus", CreateProjectStatus(IntegrationStatus.Success, new ProjectActivity(ProjectActivity.Building.ToString())));
			monitor.Poll();
			Assert.AreEqual(ProjectState.Building, monitor.ProjectState);			
		}

		[Test]
		public void ForceBuildIsForwardedOn()
		{
			mockProjectManager.Expect("ForceBuild");
			monitor.ForceBuild();
		}

		[Test]
		public void SummaryStatusStringReturnsASummaryStatusStringWhenTheStateNotSuccess()
		{
			ProjectStatus status = ProjectStatusFixture.New(IntegrationStatus.Failure, ProjectActivity.Sleeping);
			mockProjectManager.ExpectAndReturn("ProjectStatus", status);
			mockProjectManager.ExpectAndReturn("ProjectName", "projName");

			monitor.Poll();

			Assert.AreEqual("projName: Broken", monitor.SummaryStatusString);
		}

		[Test]
		public void SummaryStatusStringReturnsEmptyStringWhenTheStateIsSuccess()
		{
			ProjectStatus status = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping);
			mockProjectManager.ExpectAndReturn("ProjectStatus", status);

			monitor.Poll();

			Assert.AreEqual(string.Empty, monitor.SummaryStatusString);
		}
		
		[Test]
		public void ExposesTheIntegrationStatusOfTheContainedProject()
		{
			AssertIntegrationStateReturned(IntegrationStatus.Failure);
			AssertIntegrationStateReturned(IntegrationStatus.Exception);
			AssertIntegrationStateReturned(IntegrationStatus.Success);
			AssertIntegrationStateReturned(IntegrationStatus.Unknown);
		}

		private void AssertIntegrationStateReturned(IntegrationStatus integrationStatus)
		{
			ProjectStatus status = ProjectStatusFixture.New(integrationStatus, ProjectActivity.CheckingModifications);
			mockProjectManager.ExpectAndReturn("ProjectStatus", status);

			monitor.Poll();
			
			Assert.AreEqual(integrationStatus, monitor.IntegrationStatus);
		}

		[Test]
		public void WhenNoConnectionHasBeenMadeToTheBuildServerTheIntegrationStatusIsUnknown()
		{
			Assert.AreEqual(IntegrationStatus.Unknown, monitor.IntegrationStatus);			
		}

		[Test]
		public void InvokeServerWhenVolunteeringToFixBuild()
		{
			mockProjectManager.Expect("FixBuild");
			monitor.FixBuild();
			mockProjectManager.Verify();
		}

		[Test]
		public void DisplayBalloonMessageWhenNewMessageIsReceived()
		{
			ProjectStatus initial = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping);
			mockProjectManager.ExpectAndReturn("ProjectStatus", initial);

			Message expectedMessage = new Message("foo");
			ProjectStatus newStatus = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping);
			newStatus.Messages = new Message[] { expectedMessage};
			mockProjectManager.ExpectAndReturn("ProjectStatus", newStatus);

			monitor.MessageReceived += new MessageEventHandler(OnMessageReceived);
			monitor.Poll();
			monitor.Poll();
			Assert.AreEqual(actualMessage, expectedMessage);
		}

		private void OnMessageReceived(Message message)
		{
			actualMessage = message;
		}
	}
}