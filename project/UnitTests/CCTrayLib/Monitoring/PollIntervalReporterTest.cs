using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class PollIntervalReporterTest
	{
		[Test]
		public void BuildStartedIfLastBuildDateHasChangedAndStatusRemainedBuilding()
		{
			ProjectStatus oldProjectStatus =
				ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building, new DateTime(2007, 1, 1));
			ProjectStatus newProjectStatus =
				ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building, new DateTime(2007, 1, 2));

			bool result = new PollIntervalReporter(oldProjectStatus, newProjectStatus).HasNewBuildStarted;

			Assert.IsTrue(result);
		}

		[Test]
		public void BuildStartedIfStatusChangedToBuilding()
		{
			ProjectStatus oldProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping);
			ProjectStatus newProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building);

			bool result = new PollIntervalReporter(oldProjectStatus, newProjectStatus).HasNewBuildStarted;

			Assert.IsTrue(result);
		}

		[Test]
		public void NoBuildIfLastBuildDateIsSameAndStatusIsSame()
		{
			DateTime lastBuildDate = new DateTime(2007, 1, 1);

			ProjectStatus oldProjectStatus =
				ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building, lastBuildDate);
			ProjectStatus newProjectStatus =
				ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Building, lastBuildDate);
			bool result = new PollIntervalReporter(oldProjectStatus, newProjectStatus).HasNewBuildStarted;
			Assert.IsFalse(result);

			oldProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping, lastBuildDate);
			newProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Success, ProjectActivity.Sleeping, lastBuildDate);
			result = new PollIntervalReporter(oldProjectStatus, newProjectStatus).HasNewBuildStarted;
			Assert.IsFalse(result);
		}

		[Test]
		public void MessagesUpdatedIfNewStatusHasMoreMessagesThanOld()
		{
			ProjectStatus oldProjectStatus = ProjectStatusFixture.New("test project");
			oldProjectStatus.Messages = new Message[] {new Message("message")};
			ProjectStatus newProjectStatus = ProjectStatusFixture.New("test project");
			newProjectStatus.Messages = new Message[] {new Message("message"), new Message("another message")};

			Assert.IsTrue(new PollIntervalReporter(oldProjectStatus, newProjectStatus).WasNewStatusMessagesReceived);
		}

		[Test]
        public void AllStatusMessagesReturnsMustReturnThemAll()
		{
			Message latestMessage = new Message("latest message");
            Message firstMessage = new Message("message");


			ProjectStatus oldProjectStatus = ProjectStatusFixture.New("test project");
			oldProjectStatus.Messages = new Message[] {};
			ProjectStatus newProjectStatus = ProjectStatusFixture.New("test project");
            newProjectStatus.Messages = new Message[] { firstMessage, latestMessage };
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(newProjectStatus, newProjectStatus);

            System.Text.StringBuilder expected = new System.Text.StringBuilder();
            expected.AppendLine(firstMessage.Text);
            expected.Append(latestMessage.Text);


			Assert.AreEqual(new Message(expected.ToString()), pollIntervalReporter.AllStatusMessages);
		}

		[Test]
		public void CallingLatestStatusMessageWhenThereAreNoneIsSafe()
		{
			ProjectStatus newProjectStatus = ProjectStatusFixture.New("test project");
			newProjectStatus.Messages = new Message[] {};
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(newProjectStatus, newProjectStatus);

			Assert.AreEqual(new Message("").ToString(), pollIntervalReporter.AllStatusMessages.ToString());
		}

		[Test]
		public void TwoSuccessesMeansBuildIsStillSuccessful()
		{
			ProjectStatus lastProjectStatus = ProjectStatusFixture.New("last successful", IntegrationStatus.Success);
			ProjectStatus newProjectStatus = ProjectStatusFixture.New("new successful", IntegrationStatus.Success);
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(lastProjectStatus, newProjectStatus);

			Assert.AreEqual(BuildTransition.StillSuccessful, pollIntervalReporter.BuildTransition);
		}

		[Test]
		public void TwoFailuresMeansBuildIsStillFailing()
		{
			ProjectStatus lastProjectStatus = ProjectStatusFixture.New("last failed", IntegrationStatus.Failure);
			ProjectStatus newProjectStatus = ProjectStatusFixture.New("new failed", IntegrationStatus.Failure);
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(lastProjectStatus, newProjectStatus);

			Assert.AreEqual(BuildTransition.StillFailing, pollIntervalReporter.BuildTransition);
		}

		[Test]
		public void FailureThenSuccessMeansBuildIsFixed()
		{
			ProjectStatus lastProjectStatus = ProjectStatusFixture.New("last failed", IntegrationStatus.Failure);
			ProjectStatus newProjectStatus = ProjectStatusFixture.New("new success", IntegrationStatus.Success);
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(lastProjectStatus, newProjectStatus);

			Assert.AreEqual(BuildTransition.Fixed, pollIntervalReporter.BuildTransition);
		}
		
		[Test]
		public void SuccessThenFailureMeansBuildIsBroken()
		{
			ProjectStatus lastProjectStatus = ProjectStatusFixture.New("last success", IntegrationStatus.Success);
			ProjectStatus newProjectStatus = ProjectStatusFixture.New("new failed", IntegrationStatus.Failure);
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(lastProjectStatus, newProjectStatus);

			Assert.AreEqual(BuildTransition.Broken, pollIntervalReporter.BuildTransition);
		}		
		
		[Test]
		public void BuildCompletedDuringPollIntervalIfLastBuildDateChanged()
		{
			ProjectStatus lastProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Success, new DateTime(2007, 1, 1));
			ProjectStatus newProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Success, new DateTime(2007, 1, 2));
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(lastProjectStatus, newProjectStatus);

			Assert.IsTrue(pollIntervalReporter.IsAnotherBuildComplete);
		}
		
		[Test]
		public void LatestBuildWasSuccessfulIfNewProjectStatusIsSuccess()
		{
			ProjectStatus lastProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Failure, new DateTime(2007, 1, 1));
			ProjectStatus newProjectStatus = ProjectStatusFixture.New(IntegrationStatus.Success, new DateTime(2007, 1, 2));
			PollIntervalReporter pollIntervalReporter = new PollIntervalReporter(lastProjectStatus, newProjectStatus);

			Assert.IsTrue(pollIntervalReporter.WasLatestBuildSuccessful);			
		}
	}
}