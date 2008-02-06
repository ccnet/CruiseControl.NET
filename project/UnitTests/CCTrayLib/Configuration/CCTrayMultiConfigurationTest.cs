using System;
using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Configuration
{
	[TestFixture]
	public class CCTrayMultiConfigurationTest
	{
		public const string ConfigXml =
			@"
<Configuration>
	<Projects>
		<Project serverUrl='tcp://blah1' projectName='ProjectOne' />
		<Project serverUrl='tcp://blah2' projectName='Project Two' />
	</Projects>
	<BuildTransitionNotification showBalloon='true'>
	</BuildTransitionNotification>

</Configuration>";

		private DynamicMock mockServerConfigFactory;
		private DynamicMock mockProjectConfigFactory;

		[Test]
		public void CanLoadConfigurationFromFile()
		{
			CCTrayMultiConfiguration configuration = CreateTestConfiguration(ConfigXml);

			Assert.AreEqual(2, configuration.Projects.Length);
			Assert.AreEqual("tcp://blah1", configuration.Projects[0].ServerUrl);
			Assert.AreEqual("ProjectOne", configuration.Projects[0].ProjectName);
			Assert.AreEqual("tcp://blah2", configuration.Projects[1].ServerUrl);
			Assert.AreEqual("Project Two", configuration.Projects[1].ProjectName);

			Assert.IsTrue(configuration.ShouldShowBalloonOnBuildTransition);
		}

		[Test]
		public void WhenTheConfigurationDoesNotContainDirectivesRelatingToShowingBalloonsItDefaultsToTrue()
		{
			const string ConfigWithoutBalloonStuff = @"
<Configuration>
	<Projects />
</Configuration>";

			CCTrayMultiConfiguration configuration = CreateTestConfiguration(ConfigWithoutBalloonStuff);
			Assert.IsTrue(configuration.ShouldShowBalloonOnBuildTransition);
		}

		private const string configFileName = "test_config.xml";

		private CCTrayMultiConfiguration CreateTestConfiguration(string configFileContents)
		{
			using (TextWriter configFile = File.CreateText(configFileName))
				configFile.Write(configFileContents);

			mockServerConfigFactory = new DynamicMock(typeof (ICruiseServerManagerFactory));
			mockServerConfigFactory.Strict = true;

			mockProjectConfigFactory = new DynamicMock(typeof (ICruiseProjectManagerFactory));
			mockProjectConfigFactory.Strict = true;
			return new CCTrayMultiConfiguration(
				(ICruiseServerManagerFactory) mockServerConfigFactory.MockInstance,
				(ICruiseProjectManagerFactory) mockProjectConfigFactory.MockInstance,
				configFileName);
		}


		[Test]
		public void CanProvideASetOfProjectStatusMonitors()
		{
			CCTrayMultiConfiguration provider = CreateTestConfiguration(ConfigXml);
            DynamicMock mockCruiseServerManager = new DynamicMock(typeof(ICruiseServerManager));
		    mockCruiseServerManager.Strict = true;
            mockCruiseServerManager.ExpectAndReturn("ServerUrl", "tcp://blah1");
            mockCruiseServerManager.ExpectAndReturn("ServerUrl", "tcp://blah2");
		    ICruiseServerManager cruiseServerManagerInstance = (ICruiseServerManager) mockCruiseServerManager.MockInstance;

            mockServerConfigFactory.ExpectAndReturn("Create", cruiseServerManagerInstance, provider.GetUniqueBuildServerList()[0]);
            mockServerConfigFactory.ExpectAndReturn("Create", cruiseServerManagerInstance, provider.GetUniqueBuildServerList()[1]);
            ISingleServerMonitor[] serverMonitorList = provider.GetServerMonitors();

			mockProjectConfigFactory.ExpectAndReturn("Create", null, provider.Projects[0]);
			mockProjectConfigFactory.ExpectAndReturn("Create", null, provider.Projects[1]);

            IProjectMonitor[] monitorList = provider.GetProjectStatusMonitors(serverMonitorList);
			Assert.AreEqual(2, monitorList.Length);

			mockProjectConfigFactory.Verify();
            mockServerConfigFactory.Verify();
            mockCruiseServerManager.Verify();
		}

		[Test]
		public void CanPersist()
		{
			const string SimpleConfig = @"
<Configuration>
	<Projects />
</Configuration>";

			CCTrayMultiConfiguration configuration = CreateTestConfiguration(SimpleConfig);
			configuration.Projects = new CCTrayProject[1] {new CCTrayProject("url", "projName")};

			configuration.Persist();

			configuration.Reload();
			Assert.AreEqual(1, configuration.Projects.Length);
			Assert.AreEqual("projName", configuration.Projects[0].ProjectName);
		}

		[Test]
		public void CreatesAnEmptySettingsFileIfTheConfigFileIsNotFound()
		{
			mockProjectConfigFactory = new DynamicMock(typeof (ICruiseProjectManagerFactory));
			mockProjectConfigFactory.Strict = true;
			CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration(
				(ICruiseServerManagerFactory) mockServerConfigFactory.MockInstance,
				(ICruiseProjectManagerFactory) mockProjectConfigFactory.MockInstance,
				"config_file_that_isnt_present.xml");

			Assert.IsNotNull(configuration);
			Assert.AreEqual(0, configuration.Projects.Length);
			Assert.IsTrue(configuration.ShouldShowBalloonOnBuildTransition);
			Assert.IsNull(configuration.Audio.BrokenBuildSound);
			Assert.IsNull(configuration.Audio.FixedBuildSound);
			Assert.IsNull(configuration.Audio.StillFailingBuildSound);
			Assert.IsNull(configuration.Audio.StillSuccessfulBuildSound);
			Assert.IsFalse(configuration.X10.Enabled);
		}

		[Test]
		public void CanBuildUniqueServerListWithTwoUniqueServerProjects()
		{
			CCTrayMultiConfiguration configuration = CreateTestConfiguration(ConfigXml);

			BuildServer[] buildServers = configuration.GetUniqueBuildServerList();
			Assert.AreEqual(2, buildServers.Length);
			Assert.AreEqual("tcp://blah1", buildServers[0].Url);
			Assert.AreEqual("tcp://blah2", buildServers[1].Url);
		}

		[Test]
		public void CanBuildUniqueServerListWithTwoSameServerProjects()
		{
			const string SameServerProjectConfigXml = @"
<Configuration>
	<Projects>
		<Project serverUrl='tcp://blah1' projectName='ProjectOne' />
		<Project serverUrl='tcp://blah1' projectName='ProjectTwo' />
	</Projects>
</Configuration>";
			CCTrayMultiConfiguration configuration = CreateTestConfiguration(SameServerProjectConfigXml);

			BuildServer[] buildServers = configuration.GetUniqueBuildServerList();
			Assert.AreEqual(1, buildServers.Length);
			Assert.AreEqual("tcp://blah1", buildServers[0].Url);
		}

		[Test]
		public void CanProvideASetOfServerMonitors()
		{
			CCTrayMultiConfiguration configuration = CreateTestConfiguration(ConfigXml);

			mockServerConfigFactory.ExpectAndReturn("Create", null, configuration.GetUniqueBuildServerList()[0]);
			mockServerConfigFactory.ExpectAndReturn("Create", null, configuration.GetUniqueBuildServerList()[1]);

			IServerMonitor[] monitorList = configuration.GetServerMonitors();
			Assert.AreEqual(2, monitorList.Length);

			mockServerConfigFactory.Verify();
		}

		[Test]
		public void CanPersistAndReloadX10Configuration()
		{
			CCTrayMultiConfiguration configuration = CreateTestConfiguration(ConfigXml);

			configuration.Persist();

			configuration.Reload();
		}

	}
}
