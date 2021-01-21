using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class ValidatingLoaderTest : CustomAssertion
	{
		private string tempfile;

		[Test]
		public void FailedLoad() 
		{
			XmlValidatingLoader loader = XmlValidatingLoader(@"<cruisecontrol><projectx></projectx></cruisecontrol>");
			Assert.IsNull(loader.Load());
		}

		[Test]
		public void SuccessfulLoad() 
		{
			XmlValidatingLoader loader = XmlValidatingLoader(ConfigurationFixture.GenerateConfigXml());
			Assert.IsNotNull(loader.Load());
		}

		[Test]
		public void ShouldBeAbleToLoadXmlWithDTD()
		{
			tempfile = TempFileUtil.CreateTempFile("config", "project1.xml", @"<project name=""p1"" />");
			string xml = 
@"<!DOCTYPE cruisecontrol [ 
	<!ENTITY project1 SYSTEM ""file://" + tempfile + @""">
]> 
<cruisecontrol>&project1;</cruisecontrol>";
			XmlTextReader xr = new XmlTextReader(new StringReader(xml));
			XmlValidatingLoader loader = new XmlValidatingLoader(xr);
			XmlDocument doc = loader.Load();
			Assert.IsNotNull(doc, "Unable to load document because it is not valid according to reader");
			IConfiguration config = new NetReflectorConfigurationReader().Read(doc, null);
			Assert.IsNotNull(config.Projects["p1"], "p1 should have been found");
		}

		[TearDown]
		protected void DeleteTempFile()
		{
			if (tempfile != null) TempFileUtil.DeleteTempFile(tempfile);
		}

		private static XmlValidatingLoader XmlValidatingLoader(string xml)
		{
			XmlTextReader xr = new XmlTextReader(new StringReader(xml));
			XmlValidatingLoader loader = new XmlValidatingLoader(xr);
			loader.ValidationEventHandler += new ValidationEventHandler(Handler);
			loader.AddSchema(LoadSchema());
			return loader;
		}

		private static XmlSchema LoadSchema() 
		{
			Assembly ass = typeof(DefaultConfigurationFileLoader).Assembly;
			Stream s = ass.GetManifestResourceStream(DefaultConfigurationFileLoader.XsdSchemaResourceName);
			return XmlSchema.Read(s, new ValidationEventHandler(Handler));
		}

		private static void Handler(object sender, ValidationEventArgs args) 
		{
//			throw args.Exception;
		}
	}
}
