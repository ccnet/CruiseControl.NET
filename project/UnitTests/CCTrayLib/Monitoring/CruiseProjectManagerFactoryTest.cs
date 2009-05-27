using System;
using System.Collections.Generic;
using Rhino.Mocks;
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
        private MockRepository mocks = new MockRepository();

		[Test]
		public void WhenRequestingACruiseProjectManagerWithATcpUrlAsksTheCruiseManagerFactory()
		{
            var serverAddress = "tcp://somethingOrOther";
			var webRetriever = mocks.StrictMock<IWebRetriever>();
			var dashboardXmlParser = new DashboardXmlParser();

            var client = mocks.DynamicMock<CruiseServerClientBase>();
			var clientFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            Expect.Call(clientFactory.GenerateRemotingClient(serverAddress))
                .Return(client);
            var factory = new CruiseProjectManagerFactory(clientFactory);

			var server= new BuildServer(serverAddress);

			var serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();
			serverManagers[server] = new HttpCruiseServerManager(webRetriever, dashboardXmlParser, server);
            mocks.ReplayAll();

			var manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
			Assert.AreEqual(ProjectName, manager.ProjectName);

            mocks.VerifyAll();
		}

		[Test]
		public void WhenRequestingACruiseProjectManagerWithAnHttpUrlConstructsANewDashboardCruiseProjectManager()
		{
			var server = new BuildServer("http://somethingOrOther");
			var webRetriever = mocks.StrictMock<IWebRetriever>();
            var dashboardXmlParser = new DashboardXmlParser();

            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseProjectManagerFactory(mockCruiseManagerFactory);

			var serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();
			serverManagers[server] = new HttpCruiseServerManager(webRetriever, dashboardXmlParser, server);

            mocks.ReplayAll();
			var manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
			Assert.AreEqual(ProjectName, manager.ProjectName);
			Assert.AreEqual(typeof(HttpCruiseProjectManager), manager.GetType());

            mocks.VerifyAll();
		}

        [Test]
        public void WhenRequestingACruiseProjectManagerWithAnExtensionProtocolValidExtension()
        {
            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests", "");
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseProjectManagerFactory(mockCruiseManagerFactory);
            var serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();

            mocks.ReplayAll();
            var manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
            Assert.AreEqual(ProjectName, manager.ProjectName);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProjectListWithAnExtensionProtocolValidExtension()
        {
            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests", "");
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseProjectManagerFactory(mockCruiseManagerFactory);

            mocks.ReplayAll();
            CCTrayProject[] projectList = factory.GetProjectList(server);
            Assert.AreNotEqual(0, projectList.Length);

            mocks.VerifyAll();
        }
	}
}
