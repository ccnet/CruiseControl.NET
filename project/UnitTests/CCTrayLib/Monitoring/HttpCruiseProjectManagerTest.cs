using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class HttpCruiseProjectManagerTest
	{
		private readonly string serverUrl = "http://xxx";
		private readonly string serverAlias = "local";
		private DynamicMock webRetrieverMock;
		private DynamicMock serverManagerMock;
		private HttpCruiseProjectManager manager;

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
			webRetrieverMock = new DynamicMock(typeof (IWebRetriever));
			webRetrieverMock.ExpectAndReturn("Post", null, new object[] { new Uri(new WebDashboardUrl(serverUrl, serverAlias).ViewFarmReport), null });
			webRetrieverMock.Strict = true;
			IWebRetriever webRetriever = (IWebRetriever)webRetrieverMock.MockInstance;

			serverManagerMock = new DynamicMock(typeof(ICruiseServerManager));
			serverManagerMock.ExpectAndReturn("GetCruiseServerSnapshot", new DashboardXmlParser().ExtractAsCruiseServerSnapshot(CRUISE_SERVER_XML), null);
            serverManagerMock.ExpectAndReturn("Configuration", new BuildServer(serverUrl));
			serverManagerMock.Strict = true;
			ICruiseServerManager serverManager = (ICruiseServerManager) serverManagerMock.MockInstance;

			manager = new HttpCruiseProjectManager(webRetriever, "yyy", serverManager);		
		}

		[Test]
		public void ShouldNotThrowExceptionsOnCreation()
		{
			new HttpCruiseProjectManager((IWebRetriever)new DynamicMock(typeof(IWebRetriever)).MockInstance, "foo",
										 (ICruiseServerManager)new DynamicMock(typeof(ICruiseServerManager)).MockInstance);
		}

		[Test]
		public void ShouldNotUseTheWebRetrieverOrServerManagerOnCreation()
		{
			webRetrieverMock = new DynamicMock(typeof(IWebRetriever));
			serverManagerMock = new DynamicMock(typeof(ICruiseServerManager));
			webRetrieverMock.Strict = true;
			serverManagerMock.Strict = true;
			new HttpCruiseProjectManager((IWebRetriever)webRetrieverMock.MockInstance, "foo",
										 (ICruiseServerManager)serverManagerMock.MockInstance);
			webRetrieverMock.Verify();
			serverManagerMock.Verify();
		}
		
		[Test]
		public void ForceBuild()
		{
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            serverManagerMock.ExpectAndReturn("Configuration", new BuildServer(serverUrl));
            manager.ForceBuild(null, parameters);
			serverManagerMock.Verify();
			webRetrieverMock.Verify();
		}

		[Test]
		public void AbortBuild()
		{
            serverManagerMock.ExpectAndReturn("Configuration", new BuildServer(serverUrl));
            manager.AbortBuild(null);
			serverManagerMock.Verify();
			webRetrieverMock.Verify();
		}

		[Test]
		public void StartProject()
		{
            serverManagerMock.ExpectAndReturn("Configuration", new BuildServer(serverUrl));
            manager.StartProject(null);
			serverManagerMock.Verify();
			webRetrieverMock.Verify();
		}

		[Test]
		public void StopProject()
		{
            serverManagerMock.ExpectAndReturn("Configuration", new BuildServer(serverUrl));
            manager.StopProject(null);
			serverManagerMock.Verify();
			webRetrieverMock.Verify();
		}

        [Test]
        [ExpectedException(typeof(NotImplementedException), "Fix build not currently supported on projects monitored via HTTP")]
        public void FixBuildThrowsAnNotImplementedException()
        {
            manager.FixBuild(null, "John Do");
			serverManagerMock.Verify();
			webRetrieverMock.Verify();
        }


        [Test]
        [ExpectedException(typeof(NotImplementedException), "Cancel pending not currently supported on projects monitored via HTTP")]
        public void CancelPendingRequestThrowsAnNotImplementedException()
        {
            manager.CancelPendingRequest(null);
			serverManagerMock.Verify();
			webRetrieverMock.Verify();
        }
	}
}
