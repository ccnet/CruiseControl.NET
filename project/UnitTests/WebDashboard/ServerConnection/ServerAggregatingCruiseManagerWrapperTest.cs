using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.ServerConnection
{
	[TestFixture]
	public class ServerAggregatingCruiseManagerWrapperTest
	{
		private DynamicMock configurationGetterMock;
		private DynamicMock cruiseManagerFactoryMock;
		private DynamicMock cruiseManagerMock;
		private ServerAggregatingCruiseManagerWrapper managerWrapper;
		private DefaultServerSpecifier serverSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;
		private DefaultBuildSpecifier buildSpecifierForUnknownServer;

		[SetUp]
		public void Setup()
		{
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			cruiseManagerFactoryMock = new DynamicMock(typeof(ICruiseManagerFactory));
			cruiseManagerMock = new DynamicMock(typeof(ICruiseManager));
			serverSpecifier = new DefaultServerSpecifier("myserver");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myproject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "mybuild");
			buildSpecifierForUnknownServer = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("unknownServer"), "myProject"), "myBuild");

			managerWrapper = new ServerAggregatingCruiseManagerWrapper(
				(IConfigurationGetter) configurationGetterMock.MockInstance, 
				(ICruiseManagerFactory)cruiseManagerFactoryMock.MockInstance
				);
		}

		private void VerifyAll()
		{
			configurationGetterMock.Verify();
			cruiseManagerFactoryMock.Verify();
			cruiseManagerMock.Verify();
		}

		[Test]
		public void ThrowsCorrectExceptionIfServerNotKnown()
		{
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerLocation[] { new ServerLocation("myserver", "url")}, ServersSectionHandler.SectionName);
			try
			{
				managerWrapper.GetLog(buildSpecifierForUnknownServer);
				Assert.Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				Assert.AreEqual("unknownServer", e.RequestedServer);
			}

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerLocation[] { new ServerLocation("myserver", "url")}, ServersSectionHandler.SectionName);
			try
			{
				managerWrapper.GetLatestBuildSpecifier(buildSpecifierForUnknownServer.ProjectSpecifier);
				Assert.Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				Assert.AreEqual("unknownServer", e.RequestedServer);
			}

			VerifyAll();
		}

		[Test]
		public void ThrowsCorrectExceptionIfProjectNotRunningOnServer()
		{
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerLocation[] { new ServerLocation("myserver", "http://myurl")}, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndThrow("GetLatestBuildName", new NoSuchProjectException("myproject"), "myproject");

			try
			{
				managerWrapper.GetLatestBuildSpecifier(projectSpecifier);
				Assert.Fail("Should throw exception");
			}
			catch (NoSuchProjectException e)
			{
				Assert.AreEqual("myproject", e.RequestedProject);
			}

			VerifyAll();
		}

		[Test]
		public void ReturnsLatestLogNameFromCorrectProjectOnCorrectServer()
		{
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			DefaultProjectSpecifier myProjectMyServer = new DefaultProjectSpecifier(new DefaultServerSpecifier("myserver"),"myproject");
			DefaultProjectSpecifier myOtherProjectMyServer = new DefaultProjectSpecifier(new DefaultServerSpecifier("myserver"),"myotherproject");
			DefaultProjectSpecifier myProjectMyOtherServer = new DefaultProjectSpecifier(new DefaultServerSpecifier("myotherserver"),"myproject");
			DefaultProjectSpecifier myOtherProjectMyOtherServer = new DefaultProjectSpecifier(new DefaultServerSpecifier("myotherserver"),"myotherproject");

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyserverformyproject", "myproject");
			Assert.AreEqual(new DefaultBuildSpecifier(myProjectMyServer, "mylogformyserverformyproject"), managerWrapper.GetLatestBuildSpecifier(myProjectMyServer));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyserverformyotherproject", "myotherproject");
			Assert.AreEqual(new DefaultBuildSpecifier(myOtherProjectMyServer, "mylogformyserverformyotherproject"), managerWrapper.GetLatestBuildSpecifier(myOtherProjectMyServer));
			
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyotherserverformyproject", "myproject");
			Assert.AreEqual(new DefaultBuildSpecifier(myProjectMyOtherServer, "mylogformyotherserverformyproject"), managerWrapper.GetLatestBuildSpecifier(myProjectMyOtherServer));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyotherserverformyotherproject", "myotherproject");
			Assert.AreEqual(new DefaultBuildSpecifier(myOtherProjectMyOtherServer, "mylogformyotherserverformyotherproject"), managerWrapper.GetLatestBuildSpecifier(myOtherProjectMyOtherServer));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectLogFromCorrectProjectOnCorrectServer()
		{
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLog", "log\r\ncontents", "myproject", "mybuild");
			Assert.AreEqual("log\r\ncontents", managerWrapper.GetLog(buildSpecifier));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectLogNamesFromCorrectProjectOnCorrectServer()
		{
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", new string[] {"log1", "log2"}, "myproject");
			Assert.AreEqual(new DefaultBuildSpecifier(projectSpecifier,  "log1"), managerWrapper.GetBuildSpecifiers(projectSpecifier)[0]);
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectBuildSpecifiersFromCorrectProjectOnCorrectServerWhenNumberOfBuildsSpecified()
		{
			// Setup
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetMostRecentBuildNames", new string[] {"log1", "log2"}, "myproject", 99);

			// Execute
			IBuildSpecifier[] returnedBuildSpecifiers = managerWrapper.GetMostRecentBuildSpecifiers(projectSpecifier, 99);

			// Verify
			Assert.AreEqual("log1", returnedBuildSpecifiers[0].BuildName);
			Assert.AreEqual(projectSpecifier, returnedBuildSpecifiers[0].ProjectSpecifier);
			Assert.AreEqual(2, returnedBuildSpecifiers.Length);
			
			VerifyAll();
		}

		[Test]
		public void AddsProjectToCorrectServer()
		{
			/// Setup
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};
			string serializedProject = "myproject---";

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.Expect("AddProject", serializedProject);
			
			/// Execute
			managerWrapper.AddProject(serverSpecifier, serializedProject);
			
			/// Verify
			VerifyAll();
		}

		[Test]
		public void DeletesProjectOnCorrectServer()
		{
			// Setup
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.Expect("DeleteProject", "myproject", false, true, false);

			// Execute
			managerWrapper.DeleteProject(projectSpecifier, false, true, false);

			// Verify
			VerifyAll();
		}

		[Test]
		public void GetsProjectFromCorrectServer()
		{
			// Setup
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};
			string serializedProject = "a serialized project";

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetProject", serializedProject, "myproject");

			// Execute
			string returnedProject = managerWrapper.GetProject(projectSpecifier);

			// Verify
			VerifyAll();
			Assert.AreEqual(serializedProject, returnedProject);
		}

		[Test]
		public void UpdatesProjectOnCorrectServer()
		{
			/// Setup
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};
			string serializedProject = "myproject---";

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.Expect("UpdateProject", "myproject", serializedProject);
			
			/// Execute
			managerWrapper.UpdateProject(projectSpecifier, serializedProject);
			
			/// Verify
			VerifyAll();
		}

		[Test]
		public void ReturnsServerLogFromCorrectServer()
		{
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetServerLog", "a server log");
			Assert.AreEqual("a server log", managerWrapper.GetServerLog(serverSpecifier));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsServerNames()
		{
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			IServerSpecifier[] serverSpecifiers = managerWrapper.GetServerSpecifiers();
			Assert.AreEqual(2, serverSpecifiers.Length);
			Assert.AreEqual("myserver", serverSpecifiers[0].ServerName);
			Assert.AreEqual("myotherserver", serverSpecifiers[1].ServerName);
			
			VerifyAll();
		}

		[Test]
		public void ForcesBuild()
		{
			ServerLocation[] servers = new ServerLocation[] { new ServerLocation("myserver", "http://myurl"), new ServerLocation("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.Expect("ForceBuild", "myproject");

			managerWrapper.ForceBuild(projectSpecifier);
			
			VerifyAll();
		}
	}
}
