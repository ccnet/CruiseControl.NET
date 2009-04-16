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
		private const string SERVER_URL = @"http://localhost/ccnet/XmlServerReport.aspx";

		private DynamicMock mockWebRetriever;
		private DynamicMock mockDashboardXmlParser;
        private BuildServer buildServer;
        private HttpCruiseServerManager manager;

		[SetUp]
		public void SetUp()
		{
			mockWebRetriever = new DynamicMock(typeof (IWebRetriever));
			mockWebRetriever.Strict = true;
			mockDashboardXmlParser = new DynamicMock(typeof(IDashboardXmlParser));
			IDashboardXmlParser dashboardXmlParser = (IDashboardXmlParser) mockDashboardXmlParser.MockInstance;

			buildServer = new BuildServer(SERVER_URL);
			manager = new HttpCruiseServerManager((IWebRetriever)mockWebRetriever.MockInstance, dashboardXmlParser, buildServer);
		}

		[Test]
		public void InitialisingReturnsCorrectServerProperties()
		{
			Assert.AreEqual(SERVER_URL, manager.Configuration.Url);
			Assert.AreEqual(@"localhost", manager.DisplayName);
			Assert.AreEqual(BuildServerTransport.HTTP, manager.Configuration.Transport);
		}

		[Test]
		public void RetrieveSnapshotFromManager()
		{
			CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
			const string xmlContent = "<CruiseControl />";

			mockWebRetriever.ExpectAndReturn("Get", xmlContent, new Uri(SERVER_URL));
			mockDashboardXmlParser.ExpectAndReturn("ExtractAsCruiseServerSnapshot", snapshot, xmlContent);
			CruiseServerSnapshot actual = manager.GetCruiseServerSnapshot();
			
			Assert.AreSame(snapshot, actual);

			mockWebRetriever.Verify();
			mockDashboardXmlParser.Verify();
		}

        [Test]
        [ExpectedException(typeof(NotImplementedException), "Cancel pending not currently supported on servers monitored via HTTP")]
        public void CancelPendingRequestThrowsAnNotImplementedException()
        {
            manager.CancelPendingRequest("myproject");
        }

	}
}
