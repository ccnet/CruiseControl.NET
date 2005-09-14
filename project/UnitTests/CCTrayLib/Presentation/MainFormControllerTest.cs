using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
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

		[SetUp]
		public void SetUp()
		{
			mockProjectMonitor = new DynamicMock(typeof (IProjectMonitor));
			mockProjectMonitor.Strict = true;
			projectMonitor = (IProjectMonitor) mockProjectMonitor.MockInstance;

			mockConfiguration = new DynamicMock(typeof (ICCTrayMultiConfiguration));
			configuration = (ICCTrayMultiConfiguration) mockConfiguration.MockInstance;

			mockConfiguration.SetupResult("GetProjectStatusMonitors", new IProjectMonitor[0]);
			mockConfiguration.SetupResult("Icons", new Icons());
		}

		[Test]
		public void WhenTheSelectedProjectChangesTheIsProjectSelectedPropertyChangesAndEventFires()
		{
			eventCount = 0;

			MainFormController controller = new MainFormController(configuration, null);

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
			MainFormController controller = new MainFormController(configuration, null);
			controller.SelectedProject = projectMonitor;

			mockProjectMonitor.Expect("ForceBuild");
			controller.ForceBuild();

			mockProjectMonitor.Verify();
		}

		[Test]
		public void ForceBuildDoesNothingIfNoProjectSelected()
		{
			MainFormController controller = new MainFormController(configuration, null);
			Assert.IsNull(controller.SelectedProject);
			controller.ForceBuild();
			mockProjectMonitor.Verify();
		}

	}

}