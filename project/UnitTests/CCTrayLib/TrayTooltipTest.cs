using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	[TestFixture]
	public class TrayTooltipTest
	{
		private DynamicMock mock;
		private DateTime now;

		[SetUp]
		public void Init()
		{
			now = new DateTime(2005, 1, 1, 10, 0, 0);
			mock = new DynamicMock(typeof (DateTimeProvider));
			mock.SetupResult("Now", this.now);
		}

		private TrayTooltip CreateToolTip(ProjectActivity activity, DateTime now)
		{
			ProjectStatus projectStatus = new ProjectStatus(ProjectIntegratorState.Running,
				IntegrationStatus.Success,
				activity, 
				"CCNet",
				"http://foo/index.html",
				now,
				"1.0.0.123",
				now.AddMinutes(6));

			return new TrayTooltip(projectStatus, (DateTimeProvider) mock.MockInstance);
		}

		[Test]
		public void ShouldShowProjectDetailsOnToolTip()
		{
			TrayTooltip tooltip = CreateToolTip(ProjectActivity.Sleeping, now);
			Assert.AreEqual("Server: Sleeping\nProject: CCNet\nLast Build: Success (1.0.0.123) \nNext Build in 6 Minutes", tooltip.Text);
		}

		[Test]
		public void ShouldNotShowNextBuildTimeIfServerIsNotSleeping()
		{
			TrayTooltip tooltip = CreateToolTip(ProjectActivity.Building, now);
			Assert.AreEqual("Server: Building\nProject: CCNet\nLast Build: Success (1.0.0.123)", tooltip.Text);
		}
	}
}