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

        [Test]
        public void WhenRequestingACruiseProjectManagerWithAnExtensionProtocolValidExtension()
        {
            BuildServer server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests", "");

            DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
            mockCruiseManagerFactory.Strict = true;
            CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory)mockCruiseManagerFactory.MockInstance);

            IDictionary<BuildServer, ICruiseServerManager> serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();
            ICruiseProjectManager manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
            Assert.AreEqual(ProjectName, manager.ProjectName);

            mockCruiseManagerFactory.Verify();
        }

        [Test]
        public void GetProjectListWithAnExtensionProtocolValidExtension()
        {
            BuildServer server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests", "");

            DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
            mockCruiseManagerFactory.Strict = true;
            CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory)mockCruiseManagerFactory.MockInstance);

            CCTrayProject[] projectList = factory.GetProjectList(server);
            Assert.AreNotEqual(0, projectList.Length);

            mockCruiseManagerFactory.Verify();
        }
	}
}
