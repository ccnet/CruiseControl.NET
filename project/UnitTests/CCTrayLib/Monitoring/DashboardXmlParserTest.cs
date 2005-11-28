using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class DashboardXmlParserTest
	{
		[Test]
		[ExpectedException(typeof(ApplicationException), "Project projectName is not known to the dashboard")]
		public void ThrowsWhenParsingXmlThatDoesNotContainProjectEntry()
		{
			DashboardXmlParser parser = new DashboardXmlParser();
			parser.ExtractAsProjectStatus("<Projects />", "projectName");
		}
		
		[Test]
		public void ReturnsCorrectProjectDetailsFromXml()
		{
			DashboardXmlParser parser = new DashboardXmlParser();
			
			const string xml = @"<Projects xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:noNamespaceSchemaLocation='C:\SF\ccnet\dashboard.xsd'>
	<Project name='SvnTest' activity='Sleeping' lastBuildStatus='Exception' lastBuildLabel='8' lastBuildTime='2005-09-28T10:30:34.6362160+01:00' nextBuildTime='2005-10-04T14:31:52.4509248+01:00' webUrl='http://mrtickle/ccnet/'/>
	<Project name='projectName' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='13' lastBuildTime='2005-09-15T17:33:07.6447696+01:00' nextBuildTime='2005-10-04T14:31:51.7799600+01:00' webUrl='http://mrtickle/ccnet/'/>
</Projects>";

			ProjectStatus status = parser.ExtractAsProjectStatus(xml, "projectName");
			Assert.IsNotNull(status);
			
			Assert.AreEqual("projectName", status.Name);
			Assert.AreEqual(ProjectActivity.Sleeping, status.Activity);
			Assert.AreEqual(IntegrationStatus.Success, status.BuildStatus);
			Assert.AreEqual("13", status.LastBuildLabel);
			Assert.AreEqual("http://mrtickle/ccnet/", status.WebURL);
		}
	}
}