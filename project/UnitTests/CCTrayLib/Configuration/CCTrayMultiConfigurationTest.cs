using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

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

		private Mock<ICruiseServerManagerFactory> mockServerConfigFactory;
		private Mock<ICruiseProjectManagerFactory> mockProjectConfigFactory;

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

			mockServerConfigFactory = new Mock<ICruiseServerManagerFactory>(MockBehavior.Strict);

			mockProjectConfigFactory = new Mock<ICruiseProjectManagerFactory>(MockBehavior.Strict);
			return new CCTrayMultiConfiguration(
				(ICruiseServerManagerFactory) mockServerConfigFactory.Object,
				(ICruiseProjectManagerFactory) mockProjectConfigFactory.Object,
				configFileName);
		}


		[Test]
		public void CanProvideASetOfProjectStatusMonitors()
		{
			CCTrayMultiConfiguration provider = CreateTestConfiguration(ConfigXml);
            var mockCruiseServerManager = new Mock<ICruiseServerManager>(MockBehavior.Strict);
            MockSequence sequence = new MockSequence();
            mockCruiseServerManager.InSequence(sequence).SetupGet(manager => manager.Configuration)
                .Returns(new BuildServer("tcp://blah1")).Verifiable();
            mockCruiseServerManager.InSequence(sequence).SetupGet(manager => manager.Configuration)
                .Returns(new BuildServer("tcp://blah2")).Verifiable();
		    ICruiseServerManager cruiseServerManagerInstance = (ICruiseServerManager) mockCruiseServerManager.Object;

            mockServerConfigFactory.Setup(factory => factory.Create(provider.GetUniqueBuildServerList()[0]))
                .Returns(() => cruiseServerManagerInstance).Verifiable();
            mockServerConfigFactory.Setup(factory => factory.Create(provider.GetUniqueBuildServerList()[1]))
                .Returns(() => cruiseServerManagerInstance).Verifiable();
            ISingleServerMonitor[] serverMonitorList = provider.GetServerMonitors();

			mockProjectConfigFactory.Setup(factory => factory.Create(provider.Projects[0], It.IsAny<IDictionary<BuildServer, ICruiseServerManager>>()))
				.Returns(() => null).Verifiable();
			mockProjectConfigFactory.Setup(factory => factory.Create(provider.Projects[1], It.IsAny<IDictionary<BuildServer, ICruiseServerManager>>()))
				.Returns(() => null).Verifiable();

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
			configuration.Projects = new CCTrayProject[1] {new CCTrayProject("tcp://url", "projName")};

			configuration.Persist();

			configuration.Reload();
			Assert.AreEqual(1, configuration.Projects.Length);
			Assert.AreEqual("projName", configuration.Projects[0].ProjectName);
		}

		[Test]
		public void CreatesAnEmptySettingsFileIfTheConfigFileIsNotFound()
		{
			mockProjectConfigFactory = new Mock<ICruiseProjectManagerFactory>(MockBehavior.Strict);
			CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration(
				(ICruiseServerManagerFactory) mockServerConfigFactory.Object,
				(ICruiseProjectManagerFactory) mockProjectConfigFactory.Object,
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

			mockServerConfigFactory.Setup(factory => factory.Create(configuration.GetUniqueBuildServerList()[0]))
				.Returns(() => null).Verifiable();
			mockServerConfigFactory.Setup(factory => factory.Create(configuration.GetUniqueBuildServerList()[1]))
				.Returns(() => null).Verifiable();

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

        [Test]
        public void SetExtensionSettings()
        {
            CCTrayProject newValue = new CCTrayProject();
            newValue.ExtensionSettings = "Some settings";
            Assert.AreEqual("Some settings", newValue.ExtensionSettings);
        }

        [Test]
        public void SetExtensionNameNonBlank()
        {
            CCTrayProject newValue = new CCTrayProject();
            newValue.ExtensionName = "An extension";
            Assert.AreEqual("An extension", newValue.ExtensionName);
            Assert.AreEqual(BuildServerTransport.Extension, newValue.BuildServer.Transport);
        }

        [Test]
        public void SetExtensionNameBlank()
        {
            CCTrayProject newValue = new CCTrayProject();
            newValue.ExtensionName = string.Empty;
            Assert.AreEqual(string.Empty, newValue.ExtensionName);
            Assert.AreEqual(BuildServerTransport.HTTP, newValue.BuildServer.Transport);
        }
	}
}
