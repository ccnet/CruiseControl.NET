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
				managerWrapper.GetLatestLogName("unknownServer", "");
				Fail("Should throw exception");
			}
			catch (UnknownServerException e)
			{
				AssertEquals("unknownServer", e.RequestedServer);
			}

			configurationGetterMock.Verify();
			cruiseManagerFactoryMock.Verify();
			cruiseManagerMock.Verify();
		}

		[Test]
		public void ThrowsCorrectExceptionIfProjectNotRunningOnServer()
		{
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl")}, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndThrow("GetLatestLogName", new NoSuchProjectException("myproject"), "myproject");

			try
			{
				managerWrapper.GetLatestLogName("myserver", "myproject");
				Fail("Should throw exception");
			}
			catch (NoSuchProjectException e)
			{
				AssertEquals("myproject", e.RequestedProject);
			}

			configurationGetterMock.Verify();
			cruiseManagerFactoryMock.Verify();
			cruiseManagerMock.Verify();
		}

		[Test]
		public void ReturnsLatestLogNameFromCorrectProjectOnCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestLogName", "mylogformyserverformyproject", "myproject");
			AssertEquals("mylogformyserverformyproject", managerWrapper.GetLatestLogName("myserver", "myproject"));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestLogName", "mylogformyserverformyotherproject", "myotherproject");
			AssertEquals("mylogformyserverformyotherproject", managerWrapper.GetLatestLogName("myserver", "myotherproject"));
			
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestLogName", "mylogformyotherserverformyproject", "myproject");
			AssertEquals("mylogformyotherserverformyproject", managerWrapper.GetLatestLogName("myotherserver", "myproject"));

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myotherurl");
			cruiseManagerMock.ExpectAndReturn("GetLatestLogName", "mylogformyotherserverformyotherproject", "myotherproject");
			AssertEquals("mylogformyotherserverformyotherproject", managerWrapper.GetLatestLogName("myotherserver", "myotherproject"));
			
			configurationGetterMock.Verify();
			cruiseManagerFactoryMock.Verify();
			cruiseManagerMock.Verify();
		}

		[Test]
		public void ReturnsCorrectLogFromCorrectProjectOnCorrectServer()
		{
			ServerSpecification[] servers = new ServerSpecification[] { new ServerSpecification("myserver", "http://myurl"), new ServerSpecification("myotherserver", "http://myotherurl")};

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", servers, ServersSectionHandler.SectionName);
			cruiseManagerFactoryMock.ExpectAndReturn("GetCruiseManager", (ICruiseManager) cruiseManagerMock.MockInstance, "http://myurl");
			cruiseManagerMock.ExpectAndReturn("GetLog", "log\r\ncontents", "myproject", "mybuild");
			AssertEquals("log\r\ncontents", managerWrapper.GetLog("myserver", "myproject", "mybuild"));
			
			configurationGetterMock.Verify();
			cruiseManagerFactoryMock.Verify();
			cruiseManagerMock.Verify();
		}
	}
}
