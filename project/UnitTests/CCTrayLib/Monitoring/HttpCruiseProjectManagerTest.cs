using System;
using Rhino.Mocks;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class HttpCruiseProjectManagerTest
	{
        private MockRepository mocks = new MockRepository();
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
            serverClient = mocks.DynamicMock<CruiseServerClientBase>();
            serverManagerMock = mocks.StrictMock<ICruiseServerManager>();
            manager = new HttpCruiseProjectManager(serverClient, "yyy", serverManagerMock);		
		}

		[Test]
		public void ShouldNotThrowExceptionsOnCreation()
		{
            new HttpCruiseProjectManager(mocks.StrictMock<CruiseServerClientBase>(), 
                "foo",
                mocks.StrictMock<ICruiseServerManager>());
		}

		[Test]
		public void ShouldNotUseTheWebRetrieverOrServerManagerOnCreation()
		{
            var client = mocks.StrictMock<CruiseServerClientBase>();
            var server = mocks.StrictMock<ICruiseServerManager>();

            mocks.ReplayAll();
            new HttpCruiseProjectManager(client, "foo", server);
            mocks.VerifyAll();
		}
		
		[Test]
		public void ForceBuild()
		{
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Expect.Call(serverClient.SessionToken).PropertyBehavior();
            Expect.Call(() =>
            {
                serverClient.ForceBuild("yyy", NameValuePair.FromDictionary(parameters));
            });
            mocks.ReplayAll();

            manager.ForceBuild(null, parameters, null);
            mocks.VerifyAll();
		}

		[Test]
		public void AbortBuild()
		{
            Expect.Call(serverClient.SessionToken).PropertyBehavior();
            Expect.Call(() =>
            {
                serverClient.AbortBuild("yyy");
            });
            mocks.ReplayAll();
            manager.AbortBuild(null);
            mocks.VerifyAll();
		}

		[Test]
		public void StartProject()
		{
            Expect.Call(serverClient.SessionToken).PropertyBehavior();
            Expect.Call(() =>
            {
                serverClient.StartProject("yyy");
            });
            mocks.ReplayAll();
            manager.StartProject(null);
            mocks.VerifyAll();
		}

		[Test]
		public void StopProject()
		{
            Expect.Call(serverClient.SessionToken).PropertyBehavior();
            Expect.Call(() =>
            {
                serverClient.StopProject("yyy");
            });
            mocks.ReplayAll();
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
