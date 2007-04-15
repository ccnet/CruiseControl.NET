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
			CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
			cruiseManagerMock.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);

            CruiseServerSnapshot result = manager.GetCruiseServerSnapshot();
			Assert.AreEqual(snapshot, result);

			cruiseManagerMock.Verify();
		}
	}
}
