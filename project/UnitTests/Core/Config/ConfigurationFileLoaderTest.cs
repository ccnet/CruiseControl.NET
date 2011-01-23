using System.IO;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;

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
			string xml = "<cruisecontrol />";
			FileInfo configFile = new FileInfo(TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", xml));
			XmlDocument config = fileLoader.LoadConfiguration(configFile);
			Assert.IsNotNull(config);
			Assert.AreEqual(xml, config.OuterXml);
		}

		[Test]
		public void LoadConfigurationFile_MissingFile()
		{
			FileInfo configFile = new FileInfo(@"c:\unknown\config.file.xml");
			Assert.That(delegate { fileLoader.LoadConfiguration(configFile); },
                        Throws.TypeOf<ConfigurationFileMissingException>());
		}

		[Test]
		public void LoadConfigurationFile_FileOnlyNoPath()
		{
			FileInfo configFile = new FileInfo(@"ccnet_unknown.config");
			Assert.That(delegate { fileLoader.LoadConfiguration(configFile); },
                        Throws.TypeOf<ConfigurationFileMissingException>());
		}

		[Test]
		public void LoadConfiguration_BadXml()
		{
			FileInfo configFile = new FileInfo(TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config"
				, "<test><a><b/></test>"));
            Assert.That(delegate { fileLoader.LoadConfiguration(configFile); },
                        Throws.TypeOf<ConfigurationException>());
		}
	}
}
