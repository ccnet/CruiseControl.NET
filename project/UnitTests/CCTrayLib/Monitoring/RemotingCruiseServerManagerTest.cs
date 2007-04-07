using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class RemotingCruiseServerManagerTest
	{
		private const string ServerUrl = "tcp://blah:1000/";
		private DynamicMock cruiseManagerMock;
		BuildServer buildServer;
		RemotingCruiseServerManager manager;

		[SetUp]
		public void SetUp()
		{
			cruiseManagerMock = new DynamicMock(typeof (ICruiseManager));
			cruiseManagerMock.Strict = true;

			buildServer = new BuildServer(ServerUrl);
			manager = new RemotingCruiseServerManager((ICruiseManager)cruiseManagerMock.MockInstance, buildServer);
		}

		[Test]
		public void InitialisingReturnsCorrectServerProperties()
		{
			Assert.AreEqual(ServerUrl, manager.ServerUrl);
			Assert.AreEqual(@"blah:1000", manager.DisplayName);
			Assert.AreEqual(BuildServerTransport.Remoting, manager.Transport);
		}

		[Test]
		public void RetrieveSnapshotFromManager()
		{
			IntegrationQueueSnapshot snapshot = new IntegrationQueueSnapshot();
			cruiseManagerMock.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);

			IntegrationQueueSnapshot result = manager.GetIntegrationQueueSnapshot();
			Assert.AreEqual(snapshot, result);

			cruiseManagerMock.Verify();
		}
	}
}
