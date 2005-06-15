using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class DetailStringProviderTest
	{
		[Test]
		public void WhenTheProjecStatusIndicatesAnExceptionItsMessageIsReportedInTheDetailString()
		{
			TestingProjectMonitor monitor = new TestingProjectMonitor("name");
			DetailStringProvider provider = new DetailStringProvider();

			Assert.AreEqual("Connecting...", provider.FormatDetailString(monitor));

			monitor.SetUpAsIfExceptionOccurredOnConnect(new ApplicationException("message"));

			Assert.AreEqual("Error: message", provider.FormatDetailString(monitor));
		}

		[Test]
		public void WhenSleepingIndicatesTimeOfNextBuildCheck()
		{
			TestingProjectMonitor monitor = new TestingProjectMonitor("name");
			DetailStringProvider provider = new DetailStringProvider();
			DateTime nextBuildTime = new DateTime(2005, 7, 20, 15, 12, 30);

			monitor.ProjectStatus = new ProjectStatus(
				ProjectIntegratorState.Running, 
				IntegrationStatus.Unknown, 
				ProjectActivity.Sleeping, 
				"NAME", "url", DateTime.MinValue, "lastLabel", null, nextBuildTime);
			monitor.ProjectState = ProjectState.Success;

			Assert.AreEqual(
				string.Format("Next build check: {0:T}", nextBuildTime)		
				, provider.FormatDetailString(monitor));
			
		}
	}
}
