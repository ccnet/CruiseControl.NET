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
		private TrayTooltip _tooltip;

		[SetUp]
		public void Init()
		{
			DateTime now = new DateTime(2005, 1, 1, 10, 00, 00);
			ProjectStatus projectStatus = new ProjectStatus(ProjectIntegratorState.Running,
			                                                IntegrationStatus.Success,
			                                                ProjectActivity.Building,
			                                                "CCNet",
			                                                "http://foo/index.html",
			                                                now,
			                                                "1.0.0.123",
			                                                now.AddMinutes(6));
			Mock mockDateTime = new DynamicMock(typeof (DateTimeProvider));
			mockDateTime.SetupResult("Now", now);
			_tooltip = new TrayTooltip(projectStatus, (DateTimeProvider) mockDateTime.MockInstance);
		}

		[Test]
		public void ShouldShowProjectDetailsOnToolTip()
		{
			Assert.AreEqual("Server: Building\nProject: CCNet\nLast Build: Success (1.0.0.123) \nNext Build in 0 Day(s), 0 Hour(s), 6 Minute(s)", _tooltip.Text);
		}
	}
}