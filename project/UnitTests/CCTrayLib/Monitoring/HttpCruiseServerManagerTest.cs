using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class HttpCruiseServerManagerTest
	{
		private const string ServerUrl = @"http://localhost/ccnet/XmlStatusReport.aspx";

		private DynamicMock mockWebRetriever;
		private DynamicMock mockDashboardXmlParser;
		BuildServer buildServer;
		HttpCruiseServerManager manager;

		[SetUp]
		public void SetUp()
		{
			mockWebRetriever = new DynamicMock(typeof (IWebRetriever));
			mockWebRetriever.Strict = true;
			mockDashboardXmlParser = new DynamicMock(typeof(IDashboardXmlParser));
			IDashboardXmlParser dashboardXmlParser = (IDashboardXmlParser) mockDashboardXmlParser.MockInstance;

			buildServer = new BuildServer(ServerUrl);
			manager = new HttpCruiseServerManager((IWebRetriever)mockWebRetriever.MockInstance, dashboardXmlParser, buildServer);
		}

		[Test]
		public void InitialisingReturnsCorrectServerProperties()
		{
			Assert.AreEqual(ServerUrl, manager.ServerUrl);
			Assert.AreEqual(@"localhost", manager.DisplayName);
			Assert.AreEqual(BuildServerTransport.HTTP, manager.Transport);
		}

		[Test]
        [Ignore("Grant to get working when HTTP is working.")]
		public void RetrieveSnapshotFromManager()
		{
			CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
			const string xmlContent = "<CruiseControl />";

			mockWebRetriever.ExpectAndReturn("Get", xmlContent, ServerUrl);
			mockDashboardXmlParser.ExpectAndReturn("ExtractAsCruiseServerSnapshot", snapshot, xmlContent, ServerUrl );
			CruiseServerSnapshot actual = manager.GetCruiseServerSnapshot();
			
			Assert.AreSame(snapshot, actual);

			mockWebRetriever.Verify();
			mockDashboardXmlParser.Verify();
		}
	}
}
