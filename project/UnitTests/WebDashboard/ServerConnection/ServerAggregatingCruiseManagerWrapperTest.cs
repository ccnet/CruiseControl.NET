using NMock;
using Rhino.Mocks;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Messages;
using System;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.ServerConnection
{
	[TestFixture]
	public class ServerAggregatingCruiseManagerWrapperTest
	{
		private DynamicMock configurationMock;
		private DynamicMock cruiseManagerFactoryMock;
		private DynamicMock cruiseManagerMock;
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
			configurationMock = new DynamicMock(typeof (IRemoteServicesConfiguration));
			cruiseManagerFactoryMock = new DynamicMock(typeof (ICruiseManagerFactory));
			cruiseManagerMock = new DynamicMock(typeof (ICruiseServerClient));
			serverSpecifier = new DefaultServerSpecifier("myserver");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myproject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "mybuild");
			buildSpecifierForUnknownServer = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("unknownServer"), "myProject"), "myBuild");

			managerWrapper = new ServerAggregatingCruiseManagerWrapper(
				(IRemoteServicesConfiguration) configurationMock.MockInstance,
				(ICruiseManagerFactory) cruiseManagerFactoryMock.MockInstance
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
			configurationMock.ExpectAndReturn("Servers", new ServerLocation[] {serverLocation});
			try
			{
				managerWrapper.GetLog(buildSpecifierForUnknownServer, null);
				Assert.Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				Assert.AreEqual("unknownServer", e.RequestedServer);
			}

			configurationMock.ExpectAndReturn("Servers", new ServerLocation[] {serverLocation});
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
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataResponse response = new DataResponse();
                    response.Result = ResponseResult.Success;
                    response.Data = buildName;
                    SetupResult.For(manager.GetLatestBuildName(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();

			DefaultProjectSpecifier myProjectMyServer = new DefaultProjectSpecifier(new DefaultServerSpecifier("myserver"), "myproject");
            Assert.AreEqual(new DefaultBuildSpecifier(myProjectMyServer, buildName),
                serverWrapper.GetLatestBuildSpecifier(myProjectMyServer, null));
		}

		[Test]
		public void ReturnsCorrectLogFromCorrectProjectOnCorrectServer()
		{
            string buildLog = "content\r\nlogdata";
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataResponse response = new DataResponse();
                    response.Result = ResponseResult.Success;
                    response.Data = buildLog;
                    SetupResult.For(manager.GetLog(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();
            Assert.AreEqual(buildLog, serverWrapper.GetLog(new DefaultBuildSpecifier(projectSpecifier, "test"), null));
		}

		[Test]
		public void ReturnsCorrectLogNamesFromCorrectProjectOnCorrectServer()
		{
            string[] buildNames = new string[] { "log1", "log2" };
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataListResponse response = new DataListResponse();
                    response.Result = ResponseResult.Success;
                    response.Data.AddRange(buildNames);
                    SetupResult.For(manager.GetBuildNames(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log1"),
                serverWrapper.GetBuildSpecifiers(projectSpecifier, null)[0]);
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log2"),
                serverWrapper.GetBuildSpecifiers(projectSpecifier, null)[1]);
		}

		[Test]
		public void ReturnCorrectArtifactDirectoryFromCorrectProjectFromCorrectServer()
		{
            string artifactDirectory = @"c:\ArtifactDirectory";
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataResponse response = new DataResponse();
                    response.Result = ResponseResult.Success;
                    response.Data = artifactDirectory;
                    SetupResult.For(manager.GetArtifactDirectory(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();
            Assert.AreEqual(artifactDirectory, serverWrapper.GetArtifactDirectory(projectSpecifier, null));
		}

		[Test]
		public void ReturnsCorrectBuildSpecifiersFromCorrectProjectOnCorrectServerWhenNumberOfBuildsSpecified()
		{
            string[] buildNames = new string[] { "log1", "log2" };
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataListResponse response = new DataListResponse();
                    response.Result = ResponseResult.Success;
                    response.Data.AddRange(buildNames);
                    SetupResult.For(manager.GetMostRecentBuildNames(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log1"),
                serverWrapper.GetMostRecentBuildSpecifiers(projectSpecifier, 2, null)[0]);
            Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier, "log2"),
                serverWrapper.GetMostRecentBuildSpecifiers(projectSpecifier, 2, null)[1]);
		}

		[Test]
		public void AddsProjectToCorrectServer()
		{
			string serializedProject = "myproject---";
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    Response response = new Response();
                    response.Result = ResponseResult.Success;
                    SetupResult.For(manager.AddProject(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();

			/// Execute
            serverWrapper.AddProject(serverSpecifier, serializedProject, null);
		}

		[Test]
		public void DeletesProjectOnCorrectServer()
		{
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    Response response = new Response();
                    response.Result = ResponseResult.Success;
                    SetupResult.For(manager.DeleteProject(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();

			// Execute
            serverWrapper.DeleteProject(projectSpecifier, false, true, false, null);
		}

		[Test]
		public void GetsProjectFromCorrectServer()
		{
			string serializedProject = "a serialized project";
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataResponse response = new DataResponse();
                    response.Result = ResponseResult.Success;
                    response.Data = serializedProject;
                    SetupResult.For(manager.GetProject(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();

			// Execute
            string returnedProject = serverWrapper.GetProject(projectSpecifier, null);

			// Verify
			Assert.AreEqual(serializedProject, returnedProject);
		}

		[Test]
		public void UpdatesProjectOnCorrectServer()
		{
			string serializedProject = "myproject---";
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    Response response = new Response();
                    response.Result = ResponseResult.Success;
                    SetupResult.For(manager.UpdateProject(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();

			/// Execute
            serverWrapper.UpdateProject(projectSpecifier, serializedProject, null);
		}

		[Test]
		public void ReturnsServerLogFromCorrectServer()
		{
            string serverLog = "a server log";
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataResponse response = new DataResponse();
                    response.Result = ResponseResult.Success;
                    response.Data = serverLog;
                    SetupResult.For(manager.GetServerLog(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();
            Assert.AreEqual(serverLog, serverWrapper.GetServerLog(serverSpecifier, null));
		}

		[Test]
		public void ReturnsServerLogFromCorrectServerForCorrectProject()
		{
            string serverLog = "a server log";
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
                {
                    DataResponse response = new DataResponse();
                    response.Result = ResponseResult.Success;
                    response.Data = serverLog;
                    SetupResult.For(manager.GetServerLog(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();
            Assert.AreEqual("a server log", serverWrapper.GetServerLog(projectSpecifier, null));
		}

		[Test]
		public void ReturnsServerNames()
		{
			ServerLocation[] servers = new ServerLocation[] {serverLocation, otherServerLocation};

			configurationMock.SetupResult("Servers", servers);
			IServerSpecifier[] serverSpecifiers = managerWrapper.GetServerSpecifiers();
			Assert.AreEqual(2, serverSpecifiers.Length);
			Assert.AreEqual("myserver", serverSpecifiers[0].ServerName);
			Assert.AreEqual("myotherserver", serverSpecifiers[1].ServerName);

			VerifyAll();
		}

		[Test]
		public void ForcesBuild()
		{
			ServerLocation[] servers = new ServerLocation[] {serverLocation, otherServerLocation};

			configurationMock.ExpectAndReturn("Servers", servers);
            cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseServerClient", cruiseManagerMock.MockInstance, "http://myurl");
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Response response = new Response();
            response.Result = ResponseResult.Success;
            cruiseManagerMock.SetupResult("ForceBuild", response, typeof(BuildIntegrationRequest));

            managerWrapper.ForceBuild(projectSpecifier, null, parameters);

			VerifyAll();
		}

		[Test]
        public void GetsExternalLinks()
        {
            ExternalLink[] links = new ExternalLink[] { new ExternalLink("1", "2"), new ExternalLink("3", "4") };
            MockRepository mocks = new MockRepository();
            ServerAggregatingCruiseManagerWrapper serverWrapper = InitialiseServerWrapper(mocks,
                delegate(ICruiseServerClient manager)
		{
                    ExternalLinksListResponse response = new ExternalLinksListResponse();
                    response.Result = ResponseResult.Success;
                    response.ExternalLinks.AddRange(links);
                    SetupResult.For(manager.GetExternalLinks(null))
                        .IgnoreArguments()
                        .Return(response);
                });
            mocks.ReplayAll();

            Assert.AreEqual(links, serverWrapper.GetExternalLinks(projectSpecifier, null));
		}

        private ServerAggregatingCruiseManagerWrapper InitialiseServerWrapper(MockRepository mocks,
            Action<ICruiseServerClient> additionalSetup)
		{
            IRemoteServicesConfiguration configuration = mocks.DynamicMock<IRemoteServicesConfiguration>();
            ICruiseManagerFactory cruiseManagerFactory = mocks.DynamicMock<ICruiseManagerFactory>();
            ICruiseServerClient cruiseManager = mocks.DynamicMock<ICruiseServerClient>();

            ServerLocation[] servers = new ServerLocation[] { serverLocation, otherServerLocation };
            SetupResult.For(configuration.Servers)
                .Return(servers);
            SetupResult.For(cruiseManagerFactory.GetCruiseServerClient("http://myurl"))
                .Return(cruiseManager);

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

			configurationMock.ExpectAndReturn("Servers", servers);

			IServerSpecifier specifier = managerWrapper.GetServerConfiguration("myserver");
			Assert.AreEqual(true, specifier.AllowForceBuild);

			VerifyAll();
		}
	}
}