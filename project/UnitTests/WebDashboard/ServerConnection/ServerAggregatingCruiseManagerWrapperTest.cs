using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.config;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.ServerConnection
{
	[TestFixture]
	public class ServerAggregatingCruiseManagerWrapperTest : Assertion
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
				Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				AssertEquals("unknownServer", e.RequestedServer);
			}

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerSpecification[] { new ServerSpecification("myserver", "url")}, ServersSectionHandler.SectionName);
			try
			{
				managerWrapper.GetLatestBuildName("unknownServer", "");
				Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				AssertEquals("unknownServer", e.RequestedServer);
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
				Fail("Should throw exception");
			}
			catch (NoSuchProjectException e)
			{
				AssertEquals("myproject", e.RequestedProject);
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
			AssertEquals("mylogformyserverformyproject", managerWrapper.GetLatestBuildName("myserver", "myproject"));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyserverformyotherproject", "myotherproject");
			AssertEquals("mylogformyserverformyotherproject", managerWrapper.GetLatestBuildName("myserver", "myotherproject"));
			
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyotherserverformyproject", "myproject");
			AssertEquals("mylogformyotherserverformyproject", managerWrapper.GetLatestBuildName("myotherserver", "myproject"));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", "mylogformyotherserverformyotherproject", "myotherproject");
			AssertEquals("mylogformyotherserverformyotherproject", managerWrapper.GetLatestBuildName("myotherserver", "myotherproject"));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectLogFromCorrectProjectOnCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLog", "log\r\ncontents", "myproject", "mybuild");
			AssertEquals("log\r\ncontents", managerWrapper.GetLog("myserver", "myproject", "mybuild"));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectLogNamesFromCorrectProjectOnCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", new string[] {"log1", "log2"}, "myproject");
			AssertEquals("log1", managerWrapper.GetBuildNames("myserver", "myproject")[0]);
			
			VerifyAll();
		}

		[Test]
		public void ReturnsServerLogFromCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetServerLog", "a server log");
			AssertEquals("a server log", managerWrapper.GetServerLog("myserver"));
			
			VerifyAll();
		}

		[Test]
		public void ReturnsServerNames()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			string[] names = managerWrapper.GetServerNames();
			AssertEquals(2, names.Length);
			AssertEquals("myserver", names[0]);
			AssertEquals("myotherserver", names[1]);
			
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
