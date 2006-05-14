using System.IO;
using NMock;
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

		private DynamicMock mockServerConfigFactory;

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

			mockServerConfigFactory = new DynamicMock(typeof (ICruiseProjectManagerFactory));
			mockServerConfigFactory.Strict = true;
			return new CCTrayMultiConfiguration(
				(ICruiseProjectManagerFactory) mockServerConfigFactory.MockInstance,
				configFileName);
		}


		[Test]
		public void CanProvideASetOfProjectStatusMonitors()
		{
			CCTrayMultiConfiguration provider = CreateTestConfiguration(ConfigXml);

			mockServerConfigFactory.ExpectAndReturn("Create", null, provider.Projects[0]);
			mockServerConfigFactory.ExpectAndReturn("Create", null, provider.Projects[1]);

			IProjectMonitor[] monitorList = provider.GetProjectStatusMonitors();
			Assert.AreEqual(2, monitorList.Length);

			mockServerConfigFactory.Verify();
		}

		[Test]
		public void CanPersist()
		{
			const string SimpleConfig = @"
<Configuration>
	<Projects />
</Configuration>";

			CCTrayMultiConfiguration configuration = CreateTestConfiguration(SimpleConfig);
			configuration.Projects = new Project[1] {new Project("url", "projName")};

			configuration.Persist();

			configuration.Reload();
			Assert.AreEqual(1, configuration.Projects.Length);
			Assert.AreEqual("projName", configuration.Projects[0].ProjectName);
		}

		[Test]
		public void CreatesAnEmptySettingsFileIfTheConfigFileIsNotFound()
		{
			mockServerConfigFactory = new DynamicMock(typeof (ICruiseProjectManagerFactory));
			mockServerConfigFactory.Strict = true;
			CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration(
				(ICruiseProjectManagerFactory) mockServerConfigFactory.MockInstance,
				"config_file_that_isnt_present.xml");

			Assert.IsNotNull(configuration);
			Assert.AreEqual(0, configuration.Projects.Length);
			Assert.IsTrue(configuration.ShouldShowBalloonOnBuildTransition);
			Assert.IsNull(configuration.Audio.BrokenBuildSound);
			Assert.IsNull(configuration.Audio.FixedBuildSound);
			Assert.IsNull(configuration.Audio.StillFailingBuildSound);
			Assert.IsNull(configuration.Audio.StillSuccessfulBuildSound);
		}
	}
}