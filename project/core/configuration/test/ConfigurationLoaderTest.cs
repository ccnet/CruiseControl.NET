using System;
using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using tw.ccnet.core.util;
using tw.ccnet.core.builder.test;
using tw.ccnet.core.sourcecontrol;
using tw.ccnet.core.publishers.test;

namespace tw.ccnet.core.configuration.test
{
	[TestFixture]
	public class ConfigurationLoaderTest
	{
		private ConfigurationLoader loader;
		private int changed;
		
		[SetUp]
		protected void SetUp()
		{
			loader = new ConfigurationLoader();
			changed = 0;
		}

		[TearDown]
		protected void TearDown() 
		{
			//TempFileUtil.DeleteTempDir(this);
		}

		[Test]
		public void LoadConfigurationFile()
		{
			string xml = "<cruisecontrol></cruisecontrol>";
			loader.ConfigFile = TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", xml);
			XmlDocument config = loader.LoadConfiguration();
			Assertion.AssertNotNull("config file should not be null", config);
			Assertion.AssertEquals(xml, config.OuterXml);
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void LoadConfigurationFile_MissingFile()
		{
			loader.ConfigFile = @"c:\unknown\config.file.xml";
			loader.LoadConfiguration();
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void LoadConfigurationFile_FileOnlyNoPath()
		{
			loader.ConfigFile = @"ccnet_unknown.config";
			loader.LoadConfiguration();
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void LoadConfiguration_BadXml()
		{
			loader.ConfigFile = TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config"
				, "<test><a><b/></test>");
			XmlDocument config = loader.LoadConfiguration();
		}

		[Test]
		public void PopulateProjectsFromXml()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			IDictionary projects = loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml));
			ValidateProject(projects, "test");
		}

		[Test]
		public void PopulateProjectsFromXml_TwoProjects()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			string project2Xml = ConfigurationFixture.GenerateProjectXml("test2");
			IDictionary projects = loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml + project2Xml));
			ValidateProject(projects, "test");
			ValidateProject(projects, "test2");
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void Populate_MissingProjectProperties()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml(null, null, null, null, null, null);
			loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml));
		}

		private void ValidateProject(IDictionary projects, string projectName)
		{
			Project project = (Project)projects[projectName];
			Assertion.AssertEquals(projectName, project.Name);
			Assertion.AssertNotNull("missing builder", project.Builder);
			Assertion.AssertEquals(typeof(MockBuilder), project.Builder.GetType());

			Assertion.AssertNotNull("missing sourcecontrol", project.SourceControl);
			Assertion.AssertEquals(typeof(DefaultSourceControl), project.SourceControl.GetType());

			Assertion.AssertEquals("missing publisher", 1, project.Publishers.Count);
			Assertion.AssertEquals(typeof(MockPublisher), project.Publishers[0].GetType());

		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateProjectsFromXml_EmptyDocument()
		{
			loader.PopulateProjectsFromXml(new XmlDocument());
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateProjectsFromXml_InvalidRootElement()
		{
			loader.PopulateProjectsFromXml(XmlUtil.CreateDocument("<loader/>"));
		}

		[Test]
		public void PopulateCustomProjectFromXml()
		{
			string xml = @"<customtestproject name=""foo"" />";
			IDictionary projects = loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(xml));
			Assertion.Assert(projects["foo"] is CustomTestProject);
			Assertion.AssertEquals("foo", ((CustomTestProject) projects["foo"]).Name);
	}

	[ReflectorType("customtestproject")]
		class CustomTestProject // properly should implement IProject
		{
			[ReflectorProperty("name")]
			public string Name;
		}

		[Test]
		public void ConfigurationChanged()
		{
			string configFile = TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", ConfigurationFixture.GenerateConfigXml());
			loader.ConfigFile = configFile;
			ConfigurationChangedHandler handler = new ConfigurationChangedHandler(OnConfigurationChanged);
			loader.AddConfigurationChangedHandler(handler);
			Assertion.AssertEquals("configuration should not have changed yet!", 0, changed);
			TempFileUtil.UpdateTempFile(configFile, "<hello/>");
			// filesystemwatcher runs in separate thread so must wait for wake up
			System.Threading.Thread.Sleep(1000);
			lock(loader) 
			{
				Assertion.AssertEquals("configuration event should only be called once!", 1, changed);
			}
		}

		private void OnConfigurationChanged()
		{
			changed++;
		}
	}
}
