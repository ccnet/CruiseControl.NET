using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.config;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
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

		[SetUp]
		public void Setup()
		{
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			cruiseManagerFactoryMock = new DynamicMock(typeof(ICruiseManagerFactory));
			cruiseManagerMock = new DynamicMock(typeof(ICruiseManager));

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
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerSpecification[] { new ServerSpecification("myserver", "url")}, ServersSectionHandler.SectionName);
			try
			{
				managerWrapper.GetLog("unknownServer", "", "");
				Assert.Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				Assert.AreEqual("unknownServer", e.RequestedServer);
			}

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerSpecification[] { new ServerSpecification("myserver", "url")}, ServersSectionHandler.SectionName);
			try
			{
				managerWrapper.GetLatestBuildName("unknownServer", "");
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
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl")}, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndThrow("GetLatestBuildName", new NoSuchProjectException("myproject"), "myproject");

			try
			{
				managerWrapper.GetLatestBuildName("myserver", "myproject");
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
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyserverformyproject", "myproject");
			Assert.AreEqual("mylogformyserverformyproject", managerWrapper.GetLatestBuildName("myserver", "myproject"));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyserverformyotherproject", "myotherproject");
			Assert.AreEqual("mylogformyserverformyotherproject", managerWrapper.GetLatestBuildName("myserver", "myotherproject"));
			
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyotherserverformyproject", "myproject");
			Assert.AreEqual("mylogformyotherserverformyproject", managerWrapper.GetLatestBuildName("myotherserver", "myproject"));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyotherserverformyotherproject", "myotherproject");
			Assert.AreEqual("mylogformyotherserverformyotherproject", managerWrapper.GetLatestBuildName("myotherserver", "myotherproject"));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectLogFromCorrectProjectOnCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLog", "log\r\ncontents", "myproject", "mybuild");
			Assert.AreEqual("log\r\ncontents", managerWrapper.GetLog("myserver", "myproject", "mybuild"));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectLogNamesFromCorrectProjectOnCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", new string[] {"log1", "log2"}, "myproject");
			Assert.AreEqual("log1", managerWrapper.GetBuildNames("myserver", "myproject")[0]);
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectBuildNamesFromCorrectProjectOnCorrectServerWhenNumberOfBuildsSpecified()
		{
			// Setup
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetMostRecentBuildNames", new string[] {"log1", "log2"}, "myproject", 99);

			// Execute
			string[] returnedBuildNames = managerWrapper.GetMostRecentBuildNames("myserver", "myproject", 99);

			// Verify
			Assert.AreEqual("log1", returnedBuildNames[0]);
			Assert.AreEqual(2, returnedBuildNames.Length);
			
			VerifyAll();
		}

		[Test]
		public void DeletesProjectOnCorrectServer()
		{
			// Setup
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.Expect("DeleteProject", "myproject");

			// Execute
			managerWrapper.DeleteProject("myserver", "myproject");

			// Verify
			VerifyAll();
		}

		[Test]
		public void GetsProjectFromCorrectServer()
		{
			// Setup
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};
			string serializedProject = "a serialized project";

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetProject", serializedProject, "myproject");

			// Execute
			string returnedProject = managerWrapper.GetProject("myserver", "myproject");

			// Verify
			VerifyAll();
			Assert.AreEqual(serializedProject, returnedProject);
		}

		[Test]
		public void ReturnsServerLogFromCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetServerLog", "a server log");
			Assert.AreEqual("a server log", managerWrapper.GetServerLog("myserver"));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsServerNames()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			string[] names = managerWrapper.GetServerNames();
			Assert.AreEqual(2, names.Length);
			Assert.AreEqual("myserver", names[0]);
			Assert.AreEqual("myotherserver", names[1]);
			
			VerifyAll();
		}

		[Test]
		public void SavesProjectToCorrectServer()
		{
			/// Setup
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};
			string serializedProject = "myproject---";

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.Expect("AddProject", serializedProject);
			
			/// Execute
			managerWrapper.AddProject("myserver", serializedProject);
			
			/// Verify
			VerifyAll();
		}
	}
}
