using System;
using NMock;
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
		private DynamicMock mockProjectMonitor;
		private IProjectMonitor projectMonitor;
		private DynamicMock mockConfiguration;
		private ICCTrayMultiConfiguration configuration;
		private MainFormController controller;

		[SetUp]
		public void SetUp()
		{
			mockProjectMonitor = new DynamicMock(typeof (IProjectMonitor));
			mockProjectMonitor.Strict = true;
			projectMonitor = (IProjectMonitor) mockProjectMonitor.MockInstance;

			mockConfiguration = new DynamicMock(typeof (ICCTrayMultiConfiguration));
			configuration = (ICCTrayMultiConfiguration) mockConfiguration.MockInstance;

			mockConfiguration.SetupResult("GetServerMonitors", new ISingleServerMonitor[0]);
			mockConfiguration.SetupResult("GetProjectStatusMonitors", new IProjectMonitor[0]);
			mockConfiguration.SetupResult("Icons", new Icons());

			eventCount = 0;

			controller = new MainFormController(configuration, null, (ICache) new DynamicMock(typeof (ICache)).MockInstance );
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
			controller.SelectedProject = projectMonitor;

			mockProjectMonitor.Expect("ForceBuild");
			controller.ForceBuild();

			mockProjectMonitor.Verify();
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
			mockProjectMonitor.ExpectAndReturn("ProjectState", ProjectState.Broken);
			controller.SelectedProject = projectMonitor;
			Assert.IsTrue(controller.CanFixBuild());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CanFixBuildIfBuildIsBrokenAndBuilding()
		{
			mockProjectMonitor.ExpectAndReturn("ProjectState", ProjectState.BrokenAndBuilding);
			mockProjectMonitor.ExpectAndReturn("ProjectState", ProjectState.BrokenAndBuilding);
			controller.SelectedProject = projectMonitor;
			Assert.IsTrue(controller.CanFixBuild());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CannotFixBuildIfBuildIsWorking()
		{
			mockProjectMonitor.ExpectAndReturn("ProjectState", ProjectState.Success);
			mockProjectMonitor.ExpectAndReturn("ProjectState", ProjectState.Success);
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
			mockProjectMonitor.Expect("FixBuild");
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
			mockProjectMonitor.ExpectAndReturn("IsPending", true);
			controller.SelectedProject = projectMonitor;
			Assert.IsTrue(controller.CanCancelPending());
			mockProjectMonitor.Verify();
		}

		[Test]
		public void CannotCancelPendingIfBuildIsNotPending()
		{
			mockProjectMonitor.ExpectAndReturn("IsPending", false);
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
			mockProjectMonitor.Expect("CancelPending");
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
