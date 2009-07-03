using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectMonitorTest
	{
		private StubCurrentTimeProvider dateTimeProvider;
        private DynamicMock mockServerMonitor;
        private DynamicMock mockProjectManager;
		private ProjectMonitor monitor;
		private int pollCount;
		private int buildOccurredCount;
		private MonitorBuildOccurredEventArgs lastBuildOccurredArgs;
		private Message actualMessage;

	    private const string PROJECT_NAME = "Project1";

		[SetUp]
		public void SetUp()
		{
			buildOccurredCount = pollCount = 0;
            mockServerMonitor = new DynamicMock(typeof(ISingleServerMonitor));
            mockServerMonitor.Strict = true;
            mockProjectManager = new DynamicMock(typeof(ICruiseProjectManager));
			mockProjectManager.Strict = true;
			dateTimeProvider = new StubCurrentTimeProvider();
            monitor = new ProjectMonitor(null, (ICruiseProjectManager)mockProjectManager.MockInstance, (ISingleServerMonitor)mockServerMonitor.MockInstance, dateTimeProvider);
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
		public void WhenPollIsCalledRetrievesANewCopyOfTheProjectStatus()
		{
			ProjectStatus status = new ProjectStatus();
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);

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
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
            monitor.Poll();
			Assert.AreEqual(1, pollCount);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
            monitor.Poll();
			Assert.AreEqual(2, pollCount);
		}

		[Test]
		public void WhenPollingEncountersAnExceptionThePolledEventIsStillFired()
		{
			Assert.AreEqual(0, pollCount);

            mockProjectManager.ExpectAndThrow("ProjectName", new Exception("should be caught"));
			monitor.Poll();
			Assert.AreEqual(1, pollCount);
		}

		[Test]
		public void IfTheLastBuildDateHasChangedABuildOccuredEventIsFired()
		{
			Assert.AreEqual(0, buildOccurredCount);

		    ProjectStatus status = CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 1));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
			monitor.Poll();

			Assert.AreEqual(0, buildOccurredCount);

            status = CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 1));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
            monitor.Poll();

			Assert.AreEqual(0, buildOccurredCount);

            status = CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 2));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
            monitor.Poll();

			Assert.AreEqual(1, buildOccurredCount);

            status = CreateProjectStatus(IntegrationStatus.Success, new DateTime(2004, 1, 3));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
            monitor.Poll();

			Assert.AreEqual(2, buildOccurredCount);
		}

		[Test]
		public void ShouldCorrectlyReportEstimatedTimeWhenANewBuildStartsDuringThePollInterval()
		{
			ProjectStatus firstBuildStatus =
				ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building, new DateTime(2007, 1, 1, 0, 0, 0));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", firstBuildStatus, PROJECT_NAME);
            dateTimeProvider.SetNow(new DateTime(2007, 1, 1, 1, 0, 0));
			monitor.Poll();

			ProjectStatus secondBuildStatus =
				ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building, new DateTime(2007, 1, 1, 2, 0, 0));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", secondBuildStatus, PROJECT_NAME);
            dateTimeProvider.SetNow(new DateTime(2007, 1, 1, 3, 0, 0));
			monitor.Poll();

			ProjectStatus thirdBuildStatus =
				ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building, new DateTime(2007, 1, 1, 4, 0, 0));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", thirdBuildStatus, PROJECT_NAME);
            dateTimeProvider.SetNow(new DateTime(2007, 1, 1, 5, 0, 0));
			monitor.Poll();

			Assert.AreEqual(new TimeSpan(2, 0, 0), monitor.EstimatedTimeRemainingOnCurrentBuild);
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
            ProjectStatus status = CreateProjectStatus(initialIntegrationStatus, new DateTime(2004, 1, 1));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
			monitor.Poll();

			// then the build
            status = CreateProjectStatus(nextBuildIntegrationStatus, new DateTime(2004, 1, 2));
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
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

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.Sleeping), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Success, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Exception, ProjectActivity.Sleeping), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Broken, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Failure, ProjectActivity.Sleeping), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Broken, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Unknown, ProjectActivity.Sleeping), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Broken, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.Building), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Building, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.Building), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Building, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.CheckingModifications), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Success, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", null, PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.NotConnected, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Failure, ProjectActivity.Building), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.BrokenAndBuilding, monitor.ProjectState);

            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Exception, ProjectActivity.Building), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.BrokenAndBuilding, monitor.ProjectState);
		}

		[Test]
		public void DoNotTransitionProjectStateForNewInstanceOfSameProjectActivity()
		{
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Success, ProjectActivity.Building), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Building, monitor.ProjectState);
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", CreateProjectStatus(IntegrationStatus.Success, new ProjectActivity(ProjectActivity.Building.ToString())), PROJECT_NAME);
			monitor.Poll();
			Assert.AreEqual(ProjectState.Building, monitor.ProjectState);
		}

		[Test]
		public void ForceBuildIsForwardedOn()
		{
            mockServerMonitor.ExpectAndReturn("SessionToken", null);
            var parameters = new Dictionary<string, string>();
            mockProjectManager.Expect("ForceBuild", (string)null, parameters);
            monitor.ForceBuild(parameters);
		}

		[Test]
		public void SummaryStatusStringReturnsASummaryStatusStringWhenTheStateNotSuccess()
		{
			ProjectStatus status = ProjectStatusFixture.New(IntegrationStatus.Failure, ProjectActivity.Sleeping);
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);

			monitor.Poll();

			Assert.AreEqual(PROJECT_NAME + ": Broken", monitor.SummaryStatusString);
		}

		[Test]
		public void SummaryStatusStringReturnsEmptyStringWhenTheStateIsSuccess()
		{
			ProjectStatus status = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping);
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);

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
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", status, PROJECT_NAME);

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
            mockServerMonitor.ExpectAndReturn("SessionToken", null);
            mockProjectManager.Expect("FixBuild", null, "John");
			monitor.FixBuild("John");
			mockProjectManager.Verify();
		}

		[Test]
		public void DisplayBalloonMessageWhenNewMessageIsReceived()
		{
			ProjectStatus initial = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping);
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", initial, PROJECT_NAME);

			Message expectedMessage = new Message("foo");
			ProjectStatus newStatus = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping);
			newStatus.Messages = new Message[] {expectedMessage};
            mockProjectManager.ExpectAndReturn("ProjectName", PROJECT_NAME);
            mockServerMonitor.ExpectAndReturn("GetProjectStatus", newStatus, PROJECT_NAME);

            monitor.MessageReceived += new MessageEventHandler(OnMessageReceived);
			monitor.Poll();
			monitor.Poll();
			Assert.AreEqual(actualMessage, expectedMessage);
		}

		[Test]
		public void InvokeServerWhenCancelPendingRequest()
		{
            mockServerMonitor.ExpectAndReturn("SessionToken", null);
            mockProjectManager.Expect("CancelPendingRequest", (string)null);
			monitor.CancelPending();
			mockProjectManager.Verify();
		}

		[Test]
		public void IsNotPendingIfThereIsNoProjectStatus()
		{
			Assert.IsFalse(monitor.IsPending);
		}

		private void OnMessageReceived(string projectName, Message message)
		{
			actualMessage = message;
		}
	}
}
