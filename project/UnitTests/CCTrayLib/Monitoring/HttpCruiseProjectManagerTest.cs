using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class HttpCruiseProjectManagerTest
	{
		private readonly Uri severUri = new Uri("http://xxx");
		private DynamicMock mockWebRetriever;
		private DynamicMock mockDashboardXmlParser;
		private HttpCruiseProjectManager manager;

		[SetUp]
		public void SetUp()
		{
			mockWebRetriever = new DynamicMock(typeof (IWebRetriever));
			mockDashboardXmlParser = new DynamicMock(typeof(IDashboardXmlParser));
			IWebRetriever webRetriever = (IWebRetriever) mockWebRetriever.MockInstance;
			IDashboardXmlParser dashboardXmlParser = (IDashboardXmlParser) mockDashboardXmlParser.MockInstance;
			manager = new HttpCruiseProjectManager(webRetriever, dashboardXmlParser, severUri, "yyy");			
		}
		
		[Test]
		[ExpectedException(typeof (NotImplementedException), "Force build not currently supported on projects monitored via HTTP")]
		public void ForceBuildThrowsAnNotImplementedException()
		{
			manager.ForceBuild();
		}
	}
}