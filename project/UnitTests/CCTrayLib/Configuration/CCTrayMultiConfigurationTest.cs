using System;
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
		public const string ConfigXml = @"
<Configuration>
	<Projects>
		<Project serverUrl='tcp://blah1' projectName='ProjectOne' />
		<Project serverUrl='tcp://blah2' projectName='Project Two' />
	</Projects>
	<BuildTransitionNotification showBalloon='true'>
	</BuildTransitionNotification>

</Configuration>";

		DynamicMock mockServerConfigFactory;

		[Test]
		public void CanLoadConfigurationFromFile()
		{
			CCTrayMultiConfiguration configuration = CreateTestConfiguration(ConfigXml);

			Assert.AreEqual( 2, configuration.Projects.Length );
			Assert.AreEqual( "tcp://blah1", configuration.Projects[ 0 ].ServerUrl );
			Assert.AreEqual( "ProjectOne", configuration.Projects[ 0 ].ProjectName );
			Assert.AreEqual( "tcp://blah2", configuration.Projects[ 1 ].ServerUrl );
			Assert.AreEqual( "Project Two", configuration.Projects[ 1 ].ProjectName );

			Assert.IsTrue(configuration.ShouldShowBalloonOnBuildTransition);
		}

		[Test]
		public void WhenTheConfigurationDoesNotContainDirectivesRelatingToShowingBalloonsItDefaultsToTrue()
		{
			const string ConfigWithoutBalloonStuff= @"
<Configuration>
	<Projects />
</Configuration>";

			CCTrayMultiConfiguration configuration = CreateTestConfiguration(ConfigWithoutBalloonStuff);
			Assert.IsTrue(configuration.ShouldShowBalloonOnBuildTransition);
		}

		const string configFileName = "test_config.xml";

		private CCTrayMultiConfiguration CreateTestConfiguration(string configFileContents)
		{
			using (TextWriter configFile = File.CreateText( configFileName ))
				configFile.Write( configFileContents );

			mockServerConfigFactory = new DynamicMock(typeof(ICruiseProjectManagerFactory));
			mockServerConfigFactory.Strict = true;
			return new CCTrayMultiConfiguration( 
				(ICruiseProjectManagerFactory) mockServerConfigFactory.MockInstance,  
				configFileName );
		}


		[Test]
		public void CanProvideASetOfProjectStatusMonitors()
		{
			CCTrayMultiConfiguration provider = CreateTestConfiguration(ConfigXml);

			mockServerConfigFactory.ExpectAndReturn("Create", null, "tcp://blah1", "ProjectOne");
			mockServerConfigFactory.ExpectAndReturn("Create", null, "tcp://blah2", "Project Two");

			IProjectMonitor[] monitorList = provider.GetProjectStatusMonitors();
			Assert.AreEqual(2, monitorList.Length);

			mockServerConfigFactory.Verify();
		}

		[Test]
		public void CanPersist()
		{
			const string SimpleConfig= @"
<Configuration>
	<Projects />
</Configuration>";

			CCTrayMultiConfiguration configuration = CreateTestConfiguration(SimpleConfig);
			configuration.Projects = new Project[1] { new Project("url", "projName") };

			configuration.Persist();

			using (TextReader configFile = File.OpenText( configFileName ))
			{
				string content = configFile.ReadToEnd();

				const string expectedContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Configuration xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Projects>
    <Project serverUrl=""url"" projectName=""projName"" />
  </Projects>
</Configuration>";

				Assert.AreEqual(expectedContent, content);
			}
		}
	}

}