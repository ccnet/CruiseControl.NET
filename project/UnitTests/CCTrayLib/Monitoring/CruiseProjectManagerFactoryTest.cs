using System;
using System.Collections.Generic;
using Moq;
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
        private MockRepository mocks = new MockRepository(MockBehavior.Default);

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository(MockBehavior.Default);
        }

		[Test]
		public void WhenRequestingACruiseProjectManagerWithATcpUrlAsksTheCruiseManagerFactory()
		{
            var serverAddress = "tcp://somethingOrOther";

            var client = mocks.Create<CruiseServerClientBase>().Object;
			var clientFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            Mock.Get(clientFactory).Setup(_clientFactory => _clientFactory.GenerateRemotingClient(serverAddress, It.IsAny<ClientStartUpSettings>()))
                .Returns(client);
            var factory = new CruiseProjectManagerFactory(clientFactory);

			var server= new BuildServer(serverAddress);

			var serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();
			serverManagers[server] = new HttpCruiseServerManager(client, server);

			var manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
			Assert.AreEqual(ProjectName, manager.ProjectName);

            mocks.VerifyAll();
		}

		[Test]
		public void WhenRequestingACruiseProjectManagerWithAnHttpUrlConstructsANewDashboardCruiseProjectManager()
		{
            var serverAddress = "http://somethingOrOther";
            var server = new BuildServer(serverAddress);
            var client = mocks.Create<CruiseServerClientBase>().Object;

            var clientFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            Mock.Get(clientFactory).Setup(_clientFactory => _clientFactory.GenerateHttpClient(serverAddress, It.IsAny<ClientStartUpSettings>()))
                .Returns(client);
            var factory = new CruiseProjectManagerFactory(clientFactory);

			var serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();
			serverManagers[server] = new HttpCruiseServerManager(client, server);

			var manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
			Assert.AreEqual(ProjectName, manager.ProjectName);
			Assert.AreEqual(typeof(HttpCruiseProjectManager), manager.GetType());

            mocks.VerifyAll();
		}

        [Test]
        public void WhenRequestingACruiseProjectManagerWithAnExtensionProtocolValidExtension()
        {
            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests",string.Empty);
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseProjectManagerFactory(mockCruiseManagerFactory);
            var serverManagers = new Dictionary<BuildServer, ICruiseServerManager>();

            var manager = factory.Create(new CCTrayProject(server, ProjectName), serverManagers);
            Assert.AreEqual(ProjectName, manager.ProjectName);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProjectListWithAnExtensionProtocolValidExtension()
        {
            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests",string.Empty);
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseProjectManagerFactory(mockCruiseManagerFactory);

            CCTrayProject[] projectList = factory.GetProjectList(server, false);
            Assert.AreNotEqual(0, projectList.Length);

            mocks.VerifyAll();
        }
	}
}
