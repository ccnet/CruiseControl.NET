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

		[Ignore("TODO when HttpCruiseServerManager.GetIntegrationQueueSnapshot() implemented")]
		[Test]
		public void RetrieveSnapshotFromManager()
		{
			IntegrationQueueSnapshot snapshot = new IntegrationQueueSnapshot();
			const string xmlContent = "<IntegrationQueue />";

			mockWebRetriever.ExpectAndReturn("Get", xmlContent, ServerUrl);
			mockDashboardXmlParser.ExpectAndReturn("ExtractAsIntegrationQueueSnapshot", snapshot, xmlContent, ServerUrl );
			IntegrationQueueSnapshot actual = manager.GetIntegrationQueueSnapshot();
			
			Assert.AreSame(snapshot, actual);

			mockWebRetriever.Verify();
			mockDashboardXmlParser.Verify();
		}
	}
}
