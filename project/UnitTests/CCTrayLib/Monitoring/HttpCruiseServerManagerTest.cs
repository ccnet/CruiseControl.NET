using System;
using System.Net;
using Moq;
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

        private MockRepository mocks = new MockRepository(MockBehavior.Default);
        private CruiseServerClientBase serverClient;
        private BuildServer buildServer;
        private HttpCruiseServerManager manager;

		[SetUp]
		public void SetUp()
		{
            serverClient = mocks.Create<CruiseServerClientBase>().Object;

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

            Mock.Get(serverClient).Setup(_serverClient => _serverClient.GetCruiseServerSnapshot()).Returns(snapshot);
			CruiseServerSnapshot actual = manager.GetCruiseServerSnapshot();
			
			Assert.AreSame(snapshot, actual);
		}

		[Test]
		public void CanHandleTimeouts(){
            Mock.Get(serverClient).Setup(_serverClient => _serverClient.GetCruiseServerSnapshot()).Throws(new WebException("The operation has timed out"));

			CruiseServerSnapshot actual = manager.GetCruiseServerSnapshot();
			
			Assert.IsNotNull(actual);
			// mainly want to make sure that the exception is caught, and return is not null. 
		}

        [Test]
        public void CancelPendingRequestThrowsAnNotImplementedException()
        {
            Assert.That(delegate { manager.CancelPendingRequest("myproject"); },
                        Throws.TypeOf<NotImplementedException>().With.Message.EqualTo("Cancel pending not currently supported on servers monitored via HTTP"));
        }
	}
}
