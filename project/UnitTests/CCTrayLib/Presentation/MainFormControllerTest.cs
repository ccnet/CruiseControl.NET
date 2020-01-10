using System;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class MainFormControllerTest
	{
		private int eventCount = 0;
		private Mock<IProjectMonitor> mockProjectMonitor;
		private IProjectMonitor projectMonitor;
		private Mock<ICCTrayMultiConfiguration> mockConfiguration;
		private ICCTrayMultiConfiguration configuration;
		private MainFormController controller;

		[SetUp]
		public void SetUp()
		{
			mockProjectMonitor = new Mock<IProjectMonitor>(MockBehavior.Strict);
			projectMonitor = (IProjectMonitor) mockProjectMonitor.Object;

			mockConfiguration = new Mock<ICCTrayMultiConfiguration>();
			configuration = (ICCTrayMultiConfiguration) mockConfiguration.Object;
            
            ISingleServerMonitor[] serverMonitors = new ISingleServerMonitor[0];
            mockConfiguration.Setup(configuration => configuration.GetServerMonitors()).Returns(serverMonitors);
            mockConfiguration.Setup(configuration => configuration.GetProjectStatusMonitors(It.IsAny<ISingleServerMonitor[]>())).Returns(new IProjectMonitor[0]);
			mockConfiguration.SetupGet(configuration => configuration.Icons).Returns(new Icons());
            mockConfiguration.SetupGet(configuration => configuration.FixUserName).Returns("John");
            GrowlConfiguration growlConfig = new GrowlConfiguration();
            mockConfiguration.SetupGet(configuration => configuration.Growl).Returns(growlConfig);

			eventCount = 0;

			controller = new MainFormController(configuration, null, null);
		}

		[Test]
		public void WhenTheSelectedProjectChangesTheIsProjectSelectedPropertyChangesAndEventFires()
		{
			Assert.IsFalse(controller.IsProjectSelected);
			controller.IsProjectSelectedChanged += new EventHandler(Controller_IsProjectSelectedChanged);
			controller.SelectedProject = projectMonitor;

			Assert.IsTrue(controller.IsProjectSelected);
			Assert.AreEqual(1, eventCount);

		}

		private void Controller_IsProjectSelectedChanged(object sender, EventArgs e)
		{
			eventCount++;
		}

		[Test]
		public void ForceBuildInvokesForceBuildOnTheSelectedProject()
		{
			mockProjectMonitor.SetupGet(monitor => monitor.ProjectState).Returns(ProjectState.Success).Verifiable();
            mockProjectMonitor.Setup(monitor => monitor.ListBuildParameters()).Returns(() => null).Verifiable();
			controller.SelectedProject = projectMonitor;

			mockProjectMonitor.Setup(monitor => monitor.ForceBuild(null, "John")).Verifiable();
			controller.ForceBuild();

			mockProjectMonitor.Verify();
		}

		[Test]
		public void ForceBuildDoesNothingIfProjectIsNotConnected()
		{
			mockProjectMonitor.SetupGet(monitor => monitor.ProjectState).Returns(ProjectState.NotConnected).Verifiable();
            mockProjectMonitor.Setup(monitor => monitor.ListBuildParameters()).Returns(() => null).Verifiable();
			controller.SelectedProject = projectMonitor;

			controller.ForceBuild();

			mockProjectMonitor.Verify();
			mockProjectMonitor.VerifyNoOtherCalls();
		}

		[Test]
		public void ForceBuildDoesNothingIfNoProjectSelected()
		{
			Assert.IsNull(controller.SelectedProject);
			controller.ForceBuild();
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CanFixBuildIfBuildIsBroken()
		{
			mockProjectMonitor.SetupGet(monitor => monitor.ProjectState).Returns(ProjectState.Broken).Verifiable();
			controller.SelectedProject = projectMonitor;
			Assert.IsTrue(controller.CanFixBuild());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CanFixBuildIfBuildIsBrokenAndBuilding()
		{
			mockProjectMonitor.SetupGet(monitor => monitor.ProjectState).Returns(ProjectState.BrokenAndBuilding).Verifiable();
			controller.SelectedProject = projectMonitor;
			Assert.IsTrue(controller.CanFixBuild());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CannotFixBuildIfBuildIsWorking()
		{
			mockProjectMonitor.SetupGet(monitor => monitor.ProjectState).Returns(ProjectState.Success).Verifiable();
			controller.SelectedProject = projectMonitor;
			Assert.IsFalse(controller.CanFixBuild());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CannotFixBuildIfNoProjectIsSelected()
		{
			Assert.IsNull(controller.SelectedProject);
			Assert.IsFalse(controller.CanFixBuild());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void VolunteeringToFixBuildShouldInvokeServer()
		{
			controller.SelectedProject = projectMonitor;
			mockProjectMonitor.Setup(monitor => monitor.FixBuild("John")).Verifiable();
			controller.VolunteerToFixBuild();
			mockProjectMonitor.Verify();
		}

		[Test]
		public void VolunteeringToFixBuildShouldDoNothingIfNoProjectIsSelected()
		{
			Assert.IsNull(controller.SelectedProject);
			controller.VolunteerToFixBuild();
			mockProjectMonitor.Verify();
		}
		
		[Test]
		public void CanCancelPendingIfBuildIsPending()
		{
			mockProjectMonitor.SetupGet(monitor => monitor.IsPending).Returns(true).Verifiable();
			controller.SelectedProject = projectMonitor;
			Assert.IsTrue(controller.CanCancelPending());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CannotCancelPendingIfBuildIsNotPending()
		{
			mockProjectMonitor.SetupGet(monitor => monitor.IsPending).Returns(false).Verifiable();
			controller.SelectedProject = projectMonitor;
			Assert.IsFalse(controller.CanCancelPending());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CannotCancelPendingIfNoProjectIsSelected()
		{
			Assert.IsNull(controller.SelectedProject);
			Assert.IsFalse(controller.CanCancelPending());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CancelPendingShouldInvokeServer()
		{
			controller.SelectedProject = projectMonitor;
			mockProjectMonitor.Setup(monitor => monitor.CancelPending()).Verifiable();
			controller.CancelPending();
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CancelPendingShouldDoNothingIfNoProjectIsSelected()
		{
			Assert.IsNull(controller.SelectedProject);
			controller.CancelPending();
			mockProjectMonitor.Verify();
		}
	}
}
