using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class HttpCruiseProjectManagerTest
	{
		private readonly Uri severUri = new Uri("http://xxx");
		private DynamicMock mockWebRetriever;
		private HttpCruiseProjectManager manager;

		const string CRUISE_SERVER_XML = @"<CruiseControl xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
    <Projects>
	    <Project name='yyy' activity='Sleeping' lastBuildStatus='Exception' lastBuildLabel='8' lastBuildTime='2005-09-28T10:30:34.6362160+01:00' nextBuildTime='2005-10-04T14:31:52.4509248+01:00' webUrl='http://xxx/ccnet/server/someserver/project/yyy/ViewProjectReport.aspx'/>
	    <Project name='xxx' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='13' lastBuildTime='2005-09-15T17:33:07.6447696+01:00' nextBuildTime='2005-10-04T14:31:51.7799600+01:00' webUrl='http://xxx/ccnet/server/someserver/ViewFarmReport.aspx'/>
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
			mockWebRetriever = new DynamicMock(typeof (IWebRetriever));
			mockWebRetriever.ExpectAndReturn("Get", CRUISE_SERVER_XML, new object[] { severUri });
			IWebRetriever webRetriever = (IWebRetriever) mockWebRetriever.MockInstance;

			DashboardXmlParser dashboardXmlParser = new DashboardXmlParser();
			manager = new HttpCruiseProjectManager(webRetriever, dashboardXmlParser, severUri, "yyy");			
		}
		
		[Test]
		public void ForceBuild()
		{
			manager.ForceBuild();
		}


        [Test]
        [ExpectedException(typeof(NotImplementedException), "Fix build not currently supported on projects monitored via HTTP")]
        public void FixBuildThrowsAnNotImplementedException()
        {
            manager.FixBuild();
        }


        [Test]
        [ExpectedException(typeof(NotImplementedException), "Cancel pending not currently supported on projects monitored via HTTP")]
        public void CancelPendingRequestThrowsAnNotImplementedException()
        {
            manager.CancelPendingRequest();
        }

	}
}
