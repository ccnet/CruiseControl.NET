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
</Configuration>";

		DynamicMock mockServerConfigFactory;

		[Test]
		public void CanLoadConfigurationFromFile()
		{
			CCTrayMultiConfiguration configuration = CreateTestConfiguration();

			Assert.AreEqual( 2, configuration.Projects.Length );
			Assert.AreEqual( "tcp://blah1", configuration.Projects[ 0 ].ServerUrl );
			Assert.AreEqual( "ProjectOne", configuration.Projects[ 0 ].ProjectName );
			Assert.AreEqual( "tcp://blah2", configuration.Projects[ 1 ].ServerUrl );
			Assert.AreEqual( "Project Two", configuration.Projects[ 1 ].ProjectName );

		}

		private CCTrayMultiConfiguration CreateTestConfiguration()
		{
			const string configFileName = "test_config.xml";
			using (TextWriter configFile = File.CreateText( configFileName ))
				configFile.Write( ConfigXml );

			mockServerConfigFactory = new DynamicMock(typeof(ICruiseProjectManagerFactory));
			mockServerConfigFactory.Strict = true;
			return new CCTrayMultiConfiguration( 
				(ICruiseProjectManagerFactory) mockServerConfigFactory.MockInstance,  
				configFileName );
		}


		[Test]
		public void CanProvideASetOfProjectStatusMonitors()
		{
			CCTrayMultiConfiguration provider = CreateTestConfiguration();

			mockServerConfigFactory.ExpectAndReturn("Create", null, "tcp://blah1", "ProjectOne");
			mockServerConfigFactory.ExpectAndReturn("Create", null, "tcp://blah2", "Project Two");

			IProjectMonitor[] monitorList = provider.GetProjectStatusMonitors();
			Assert.AreEqual(2, monitorList.Length);

			mockServerConfigFactory.Verify();
		}
	}

}