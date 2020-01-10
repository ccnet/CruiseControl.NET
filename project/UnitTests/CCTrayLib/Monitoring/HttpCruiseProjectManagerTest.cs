using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class HttpCruiseProjectManagerTest
	{
        private MockRepository mocks = new MockRepository(MockBehavior.Default);
		private HttpCruiseProjectManager manager;
        private ICruiseServerManager serverManagerMock;
        private CruiseServerClientBase serverClient;

		const string CRUISE_SERVER_XML = @"<CruiseControl xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
    <Projects>
	    <Project name='yyy' activity='Sleeping' lastBuildStatus='Exception' lastBuildLabel='8' lastBuildTime='2005-09-28T10:30:34.6362160+01:00' nextBuildTime='2005-10-04T14:31:52.4509248+01:00' webUrl='http://xxx/server/local/project/yyy/ViewProjectReport.aspx'/>
	    <Project name='xxx' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='13' lastBuildTime='2005-09-15T17:33:07.6447696+01:00' nextBuildTime='2005-10-04T14:31:51.7799600+01:00' webUrl='http://xxx/server/local/ViewFarmReport.aspx'/>
    </Projects>
    <Queues>
        <Queue name='Queue1'>
            <Request projectName='yyy' activity='CheckingModifications' />
            <Request projectName='xxx' activity='Pending' />
        </Queue>
        <Queue name='Queue2'>
            <Request projectName='Missing' activity='Building' />
        </Queue>
    </Queues>
</CruiseControl>";

		[SetUp]
		public void SetUp()
		{
            serverClient = mocks.Create<CruiseServerClientBase>().Object;
            manager = new HttpCruiseProjectManager(serverClient, "yyy");		
		}

		[Test]
		public void ShouldNotThrowExceptionsOnCreation()
		{
            new HttpCruiseProjectManager(mocks.Create<CruiseServerClientBase>(MockBehavior.Strict).Object, 
                "foo");
		}

		[Test]
		public void ShouldNotUseTheWebRetrieverOrServerManagerOnCreation()
		{
            var client = mocks.Create<CruiseServerClientBase>(MockBehavior.Strict).Object;

            new HttpCruiseProjectManager(client, "foo");
            mocks.VerifyAll();
		}
		
		[Test]
		public void ForceBuild()
		{
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Mock.Get(serverClient).SetupSet(_serverClient => _serverClient.SessionToken = It.IsAny<string>());
            Mock.Get(serverClient).Setup(_serverClient => _serverClient.ForceBuild("yyy", It.Is<List<NameValuePair>>(_parameters => _parameters.SequenceEqual(NameValuePair.FromDictionary(parameters)))));

            manager.ForceBuild(null, parameters, null);
            mocks.VerifyAll();
		}

		[Test]
		public void AbortBuild()
		{
            Mock.Get(serverClient).SetupSet(_serverClient => _serverClient.SessionToken = It.IsAny<string>());
            Mock.Get(serverClient).Setup(_serverClient => _serverClient.AbortBuild("yyy"));
            manager.AbortBuild(null,"John Do");
            mocks.VerifyAll();
		}

		[Test]
		public void StartProject()
		{
            Mock.Get(serverClient).SetupSet(_serverClient => _serverClient.SessionToken = It.IsAny<string>());
            Mock.Get(serverClient).Setup(_serverClient => _serverClient.StartProject("yyy"));
            manager.StartProject(null);
            mocks.VerifyAll();
		}

		[Test]
		public void StopProject()
		{
            Mock.Get(serverClient).SetupSet(_serverClient => _serverClient.SessionToken = It.IsAny<string>());
            Mock.Get(serverClient).Setup(_serverClient => _serverClient.StopProject("yyy"));
            manager.StopProject(null);
            mocks.VerifyAll();
		}

        [Test(Description="Fix build not currently supported on projects monitored via HTTP")]
        public void FixBuildThrowsAnNotImplementedException()
        {
            Assert.That(delegate { manager.FixBuild(null, "John Do"); },
                        Throws.TypeOf<NotImplementedException>());
        }

        [Test(Description="Cancel pending not currently supported on projects monitored via HTTP")]
        public void CancelPendingRequestThrowsAnNotImplementedException()
        {
            Assert.That(delegate { manager.CancelPendingRequest(null); },
                        Throws.TypeOf<NotImplementedException>());
        }
	}
}
