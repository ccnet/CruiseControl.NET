using System;
using System.Collections.Generic;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CruiseProjectManagerFactoryTest
	{
		private const string ProjectName = "projectName";

		const string CRUISE_SERVER_XML = @"<CruiseControl xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
    <Projects>
	    <Project name='SvnTest' activity='Sleeping' lastBuildStatus='Exception' lastBuildLabel='8' lastBuildTime='2005-09-28T10:30:34.6362160+01:00' nextBuildTime='2005-10-04T14:31:52.4509248+01:00' webUrl='http://xxx/ccnet/'/>
	    <Project name='projectName' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='13' lastBuildTime='2005-09-15T17:33:07.6447696+01:00' nextBuildTime='2005-10-04T14:31:51.7799600+01:00' webUrl='http://xxx/ccnet/'/>
    </Projects>
    <Queues>
        <Queue name='Queue1'>
            <Request projectName='projectName' activity='CheckingModifications' />
            <Request projectName='SVNTest' activity='Pending' />
        </Queue>
        <Queue name='Queue2'>
            <Request projectName='Missing' activity='Building' />
        </Queue>
    </Queues>
</CruiseControl>";


		[Test]
		public void WhenRequestingACruiseProjectManagerWithATcpUrlAsksTheCruiseManagerFactory()
		{
			DynamicMock webRetriever = new DynamicMock(typeof(IWebRetriever));
			webRetriever.Strict = true;

			DashboardXmlParser dashboardXmlParser = new DashboardXmlParser();

			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);

			BuildServer server= new BuildServer("tcp://somethingOrOther");
			mockCruiseManagerFactory.ExpectAndReturn("GetCruiseManager", null, server.Url);

			IDictionary<BuildServer, ICruiseServerManager> serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();
			serverManagers[server] = new HttpCruiseServerManager((IWebRetriever) webRetriever.MockInstance, dashboardXmlParser, server);

			ICruiseProjectManager manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
			Assert.AreEqual(ProjectName, manager.ProjectName);

			mockCruiseManagerFactory.Verify();
			webRetriever.Verify();
		}

		[Test]
		public void WhenRequestingACruiseProjectManagerWithAnHttpUrlConstructsANewDashboardCruiseProjectManager()
		{
			BuildServer server = new BuildServer("http://somethingOrOther");
			
			DynamicMock webRetriever = new DynamicMock(typeof(IWebRetriever));
			webRetriever.ExpectAndReturn("Get", CRUISE_SERVER_XML, server.Uri);
			webRetriever.Strict = true;

			DashboardXmlParser dashboardXmlParser = new DashboardXmlParser();

			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);

			IDictionary<BuildServer, ICruiseServerManager> serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();
			serverManagers[server] = new HttpCruiseServerManager((IWebRetriever) webRetriever.MockInstance, dashboardXmlParser, server);

			ICruiseProjectManager manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
			Assert.AreEqual(ProjectName, manager.ProjectName);
			Assert.AreEqual(typeof (HttpCruiseProjectManager), manager.GetType());

			mockCruiseManagerFactory.Verify();
			webRetriever.Verify();
		}
	}
}
