using System;
using Rhino.Mocks;
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

        private MockRepository mocks = new MockRepository();
        private CruiseServerClientBase serverClient;
        private BuildServer buildServer;
        private HttpCruiseServerManager manager;

		[SetUp]
		public void SetUp()
		{
            serverClient = mocks.DynamicMock<CruiseServerClientBase>();

			buildServer = new BuildServer(SERVER_URL);
            manager = new HttpCruiseServerManager(serverClient, buildServer);
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

            Expect.Call(serverClient.GetCruiseServerSnapshot()).Return(snapshot);
            mocks.ReplayAll();
			CruiseServerSnapshot actual = manager.GetCruiseServerSnapshot();
			
			Assert.AreSame(snapshot, actual);
		}

        [Test]
        [ExpectedException(typeof(NotImplementedException), "Cancel pending not currently supported on servers monitored via HTTP")]
        public void CancelPendingRequestThrowsAnNotImplementedException()
        {
            manager.CancelPendingRequest("myproject");
        }

	}
}
