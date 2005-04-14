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
	}
}
