using System;
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
			ProjectStatus projectStatus = new ProjectStatus(ProjectIntegratorState.Running, IntegrationStatus.Success, ProjectActivity.Building, "CCNet", "http://foo/index.html", DateTime.Now,"1.0.0.123");
			_tooltip = new TrayTooltip(projectStatus);
		}

		[Test]
		public void ShouldShowProjectDetailsOnToolTip()
		{
			Assert.AreEqual("Server: Building\nProject: CCNet\nLast Build: Success (1.0.0.123)", _tooltip.Text);
		}
	}
}
