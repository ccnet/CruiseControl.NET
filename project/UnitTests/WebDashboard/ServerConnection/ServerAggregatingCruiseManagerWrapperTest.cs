using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.ServerConnection
{
	[TestFixture]
	public class ServerAggregatingCruiseManagerWrapperTest
	{
		private Mock<IRemoteServicesConfiguration> configurationMock;
		private Mock<ICruiseServerClientFactory> cruiseManagerFactoryMock;
		private Mock<ICruiseServerClient> cruiseManagerMock;
		private ServerAggregatingCruiseManagerWrapper managerWrapper;
		private DefaultServerSpecifier serverSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;
		private DefaultBuildSpecifier buildSpecifierForUnknownServer;
		private ServerLocation serverLocation;
		private ServerLocation otherServerLocation;

		[SetUp]
		public void Setup()
		{
			configurationMock = new Mock<IRemoteServicesConfiguration>();
            cruiseManagerFactoryMock = new Mock<ICruiseServerClientFactory>();
			cruiseManagerMock = new Mock<ICruiseServerClient>();
			serverSpecifier = new DefaultServerSpecifier("myserver");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myproject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "mybuild");
			buildSpecifierForUnknownServer = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("unknownServer"), "myProject"), "myBuild");

			managerWrapper = new ServerAggregatingCruiseManagerWrapper(
				(IRemoteServicesConfiguration) configurationMock.Object,
                (ICruiseServerClientFactory)cruiseManagerFactoryMock.Object
				);

			serverLocation = new ServerLocation();
			serverLocation.Name = "myserver";
			serverLocation.Url = "http://myurl";
			serverLocation.AllowForceBuild = true;

			otherServerLocation = new ServerLocation();
			otherServerLocation.Name = "myotherserver";
			otherServerLocation.Url = "http://myotherurl";
		}

		private void VerifyAll()
		{
			configurationMock.Verify();
			cruiseManagerFactoryMock.Verify();
			cruiseManagerMock.Verify();
		}

		[Test]
		public void ThrowsCorrectExceptionIfServerNotKnown()
		{
			configurationMock.SetupGet(_configuration => _configuration.Servers).Returns(new ServerLocation[] {serverLocation}).Verifiable();
			try
			{
				managerWrapper.GetLog(buildSpecifierForUnknownServer, null);
				Assert.Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				Assert.AreEqual("unknownServer", e.RequestedServer);
			}

			configurationMock.SetupGet(_configuration => _configuration.Servers).Returns(new ServerLocation[] {serverLocation}).Verifiable();
			try
			{
				managerWrapper.GetLatestBuildSpecifier(buildSpecifierForUnknownServer.ProjectSpecifier, null);
				Assert.Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				Assert.AreEqual("unknownServer", e.RequestedServer);
			}

			VerifyAll();
		}

		[Test]
		public void ReturnsLatestLogNameFromCorrectProjectOnCorrectServer()
		{
            string buildName = "mylogformyserverformyproject";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetLatestBuildName(It.IsAny<string>()))
                        .Returns(buildName);
                });

			DefaultProjectSpecifier myProjectMyServer = new DefaultProjectSpecifier(new DefaultServerSpecifier("myserver"), "myproject");
            Assert.AreEqual(new DefaultBuildSpecifier(myProjectMyServer, buildName),
                serverWrapper.GetLatestBuildSpecifier(myProjectMyServer, null));
		}

		[Test]
		public void ReturnsCorrectLogFromCorrectProjectOnCorrectServer()
		{
            string buildLog = "content" + Environment.NewLine + "logdata";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                        .Returns(buildLog);
                });
            Assert.AreEqual(buildLog, serverWrapper.GetLog(new DefaultBuildSpecifier(projectSpecifier, "test"), null));
		}

		[Test]
		public void ReturnsCorrectLogNamesFromCorrectProjectOnCorrectServer()
		{
            string[] buildNames = new string[] { "log1", "log2" };
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetBuildNames(It.IsAny<string>()))
                        .Returns(buildNames);
                });
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log1"),
                serverWrapper.GetBuildSpecifiers(projectSpecifier, null)[0]);
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log2"),
                serverWrapper.GetBuildSpecifiers(projectSpecifier, null)[1]);
		}

		[Test]
		public void ReturnCorrectArtifactDirectoryFromCorrectProjectFromCorrectServer()
		{
            string artifactDirectory = @"c:\ArtifactDirectory";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetArtifactDirectory(It.IsAny<string>()))
                        .Returns(artifactDirectory);
                });
            Assert.AreEqual(artifactDirectory, serverWrapper.GetArtifactDirectory(projectSpecifier, null));
		}

		[Test]
		public void ReturnsCorrectBuildSpecifiersFromCorrectProjectOnCorrectServerWhenNumberOfBuildsSpecified()
		{
            string[] buildNames = new string[] { "log1", "log2" };
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetMostRecentBuildNames(It.IsAny<string>(), It.IsAny<int>()))
                        .Returns(buildNames);
                });
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log1"),
                serverWrapper.GetMostRecentBuildSpecifiers(projectSpecifier, 2, null)[0]);
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log2"),
                serverWrapper.GetMostRecentBuildSpecifiers(projectSpecifier, 2, null)[1]);
		}

		[Test]
		public void AddsProjectToCorrectServer()
		{
			string serializedProject = "myproject---";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks, null);

			/// Execute
            serverWrapper.AddProject(serverSpecifier, serializedProject, null);
		}

		[Test]
		public void DeletesProjectOnCorrectServer()
		{
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks, null);

			// Execute
            serverWrapper.DeleteProject(projectSpecifier, false, true, false, null);
		}

		[Test]
		public void GetsProjectFromCorrectServer()
		{
			string serializedProject = "a serialized project";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetProject(It.IsAny<string>()))
                        .Returns(serializedProject);
                });

			// Execute
            string returnedProject = serverWrapper.GetProject(projectSpecifier, null);

			// Verify
			Assert.AreEqual(serializedProject, returnedProject);
		}

		[Test]
		public void UpdatesProjectOnCorrectServer()
		{
			string serializedProject = "myproject---";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks, null);

			/// Execute
            serverWrapper.UpdateProject(projectSpecifier, serializedProject, null);
		}

		[Test]
		public void ReturnsServerLogFromCorrectServer()
		{
            string serverLog = "a server log";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetServerLog())
                        .Returns(serverLog);
                });
            Assert.AreEqual(serverLog, serverWrapper.GetServerLog(serverSpecifier, null));
		}

		[Test]
		public void ReturnsServerLogFromCorrectServerForCorrectProject()
		{
            string serverLog = "a server log";
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetServerLog(It.IsAny<string>()))
                        .Returns(serverLog);
                });
            Assert.AreEqual("a server log", serverWrapper.GetServerLog(projectSpecifier, null));
		}

		[Test]
		public void ReturnsServerNames()
		{
			ServerLocation[] servers = new ServerLocation[] {serverLocation, otherServerLocation};

			configurationMock.SetupGet(_configuration => _configuration.Servers).Returns(servers).Verifiable();
			IServerSpecifier[] serverSpecifiers = managerWrapper.GetServerSpecifiers();
			Assert.AreEqual(2, serverSpecifiers.Length);
			Assert.AreEqual("myserver", serverSpecifiers[0].ServerName);
			Assert.AreEqual("myotherserver", serverSpecifiers[1].ServerName);

			VerifyAll();
		}

		[Test]
		public void ForcesBuild()
		{
            var parameters = new Dictionary<string, string>();
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.ForceBuild(It.IsAny<string>(), It.IsAny<List<NameValuePair>>())).Verifiable();
                });

            serverWrapper.ForceBuild(projectSpecifier, null, parameters);
		}

		[Test]
        public void GetsExternalLinks()
        {
            ExternalLink[] links = new ExternalLink[] { new ExternalLink("1", "2"), new ExternalLink("3", "4") };
            MockRepository mocks = new MockRepository(MockBehavior.Default);
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(CruiseServerClientBase manager)
                {
                    Mock.Get(manager).Setup(_manager => _manager.GetExternalLinks(It.IsAny<string>()))
                        .Returns(links);
                });

            Assert.AreEqual(links, serverWrapper.GetExternalLinks(projectSpecifier, null));
		}

        private ServerAggregatingCruiseManagerWrapper InitialiseServerWrapper(MockRepository mocks,
            Action<CruiseServerClientBase> additionalSetup)
		{
            IRemoteServicesConfiguration configuration = mocks.Create<IRemoteServicesConfiguration>().Object;
            ICruiseServerClientFactory cruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>().Object;
            CruiseServerClientBase cruiseManager = mocks.Create<CruiseServerClientBase>().Object;

            ServerLocation[] servers = new ServerLocation[] { serverLocation, otherServerLocation };
            Mock.Get(configuration).SetupGet(_configuration => _configuration.Servers)
                .Returns(servers);
            Mock.Get(cruiseManagerFactory).Setup(_cruiseManagerFactory => _cruiseManagerFactory.GenerateClient(It.IsAny<string>(), It.IsAny <ClientStartUpSettings>()))
                .Returns(cruiseManager);

            ServerAggregatingCruiseManagerWrapper serverWrapper = new ServerAggregatingCruiseManagerWrapper(
                configuration,
                cruiseManagerFactory);

            if (additionalSetup != null) additionalSetup(cruiseManager);

            return serverWrapper;
		}

		[Test]
		public void ReturnsServerConfiguration()
		{
			ServerLocation[] servers = new ServerLocation[] {serverLocation, otherServerLocation};

			configurationMock.SetupGet(_configuration => _configuration.Servers).Returns(servers).Verifiable();

			IServerSpecifier specifier = managerWrapper.GetServerConfiguration("myserver");
			Assert.AreEqual(true, specifier.AllowForceBuild);

			VerifyAll();
		}
	}
}