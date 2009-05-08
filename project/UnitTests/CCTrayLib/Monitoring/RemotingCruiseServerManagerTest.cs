using System;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class RemotingCruiseServerManagerTest
	{
		private const string ServerUrl = "tcp://blah:1000/";
        private MockRepository mocks = new MockRepository();
        private CruiseServerClient cruiseManagerMock;
		BuildServer buildServer;
		RemotingCruiseServerManager manager;

		[SetUp]
		public void SetUp()
		{
			cruiseManagerMock = mocks.StrictMock<CruiseServerClient>();

			buildServer = new BuildServer(ServerUrl);
			manager = new RemotingCruiseServerManager(cruiseManagerMock, buildServer);
		}

		[Test]
		public void InitialisingReturnsCorrectServerProperties()
		{
			Assert.AreEqual(ServerUrl, manager.Configuration.Url);
			Assert.AreEqual(@"blah:1000", manager.DisplayName);
			Assert.AreEqual(BuildServerTransport.Remoting, manager.Configuration.Transport);
		}

		[Test]
		public void RetrieveSnapshotFromManager()
		{
            var snapshot= new CruiseServerSnapshot();
            Expect.Call(cruiseManagerMock.GetCruiseServerSnapshot())
                .IgnoreArguments()
                .Return(snapshot);
            mocks.ReplayAll();

            CruiseServerSnapshot result = manager.GetCruiseServerSnapshot();
			Assert.AreEqual(snapshot, result);
		}
	}
}
