using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Builder.Test;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Config.Test;
using ThoughtWorks.CruiseControl.Core.Publishers.Test;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class ConfigurationFileLoaderTest : CustomAssertion
	{
		private DefaultConfigurationFileLoader fileLoader;

		[SetUp]
		protected void SetUp()
		{
			fileLoader = new DefaultConfigurationFileLoader();
		}

		[TearDown]
		protected void TearDown() 
		{
			TempFileUtil.DeleteTempDir(this);
		}

		[Test]
		public void LoadConfigurationFile()
		{
			string xml = "<cruisecontrol></cruisecontrol>";
			FileInfo configFile = new FileInfo(TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", xml));
			XmlDocument config = fileLoader.LoadConfiguration(configFile);
			Assert.IsNotNull(config);
			Assert.AreEqual(xml, config.OuterXml);
		}

		[Test, ExpectedException(typeof(ConfigurationFileMissingException))]
		public void LoadConfigurationFile_MissingFile()
		{
			FileInfo configFile = new FileInfo(@"c:\unknown\config.file.xml");
			fileLoader.LoadConfiguration(configFile);
		}

		[Test, ExpectedException(typeof(ConfigurationFileMissingException))]
		public void LoadConfigurationFile_FileOnlyNoPath()
		{
			FileInfo configFile = new FileInfo(@"ccnet_unknown.config");
			fileLoader.LoadConfiguration(configFile);
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void LoadConfiguration_BadXml()
		{
			FileInfo configFile = new FileInfo(TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config"
				, "<test><a><b/></test>"));
			fileLoader.LoadConfiguration(configFile);
		}

		[Test]
		public void PopulateProjectsFromXml()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			IConfiguration configuration = fileLoader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml));
			ValidateProject(configuration, "test");
		}

		[Test]
		public void PopulateProjectsFromXml_TwoProjects()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			string project2Xml = ConfigurationFixture.GenerateProjectXml("test2");
			IConfiguration configuration = fileLoader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml + project2Xml));
			ValidateProject(configuration, "test");
			ValidateProject(configuration, "test2");
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void Populate_MissingProjectProperties()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml(null, null, null, null, null, null);
			fileLoader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml));
		}

		private void ValidateProject(IConfiguration configuration, string projectName)
		{
			Project project = configuration.Projects[projectName] as Project;
			Assert.AreEqual(projectName, project.Name);
			Assert.IsTrue(project.Builder is MockBuilder);
			Assert.IsTrue(project.SourceControl is NullSourceControl);
			Assert.AreEqual(1, project.Publishers.Length);
			Assert.IsTrue(project.Publishers[0] is MockPublisher);

		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateProjectsFromXml_EmptyDocument()
		{
			fileLoader.PopulateProjectsFromXml(new XmlDocument());
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateProjectsFromXml_InvalidRootElement()
		{
			fileLoader.PopulateProjectsFromXml(XmlUtil.CreateDocument("<loader/>"));
		}
        
		[Test]
		// [CCNET-63] XML comments before project tag was causing NetReflectorException
		public void PopulateProjectsFromXml_WithComments()
		{
			string projectXml = @"<!-- A Comment -->" + ConfigurationFixture.GenerateProjectXml("test");
			IConfiguration configuration = fileLoader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml));
			ValidateProject(configuration, "test");
		}

		[Test]
		public void PopulateCustomProjectFromXml()
		{
			string xml = @"<customtestproject name=""foo"" />";
			IConfiguration configuration = fileLoader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(xml));
			Assert.IsNotNull(configuration.Projects["foo"]);
			Assert.IsTrue(configuration.Projects["foo"] is CustomTestProject);
			Assert.AreEqual("foo", ((CustomTestProject) configuration.Projects["foo"]).Name);
		}

		[ReflectorType("customtestproject")]
			class CustomTestProject : ProjectBase, IProject
		{
			public ProjectActivity CurrentActivity { get { return ProjectActivity.Building; } }
			public IIntegrationResult RunIntegration(BuildCondition buildCondition) { return null; }
			public IntegrationStatus LatestBuildStatus { get { return IntegrationStatus.Success; } }
			public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment) { }
			public string WebURL { get {return ""; } }
		}
	}
}
