using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config.Test
{
	[TestFixture]
	public class ValidatingLoaderTest : CustomAssertion
	{
		[Test]
		public void FailedLoad() 
		{
			ValidationEventHandler hd = new ValidationEventHandler(Handler);
			XmlSchema schema = LoadSchema();
			XmlTextReader xr = new XmlTextReader(new StringReader(@"<cruisecontrol><projectx></projectx></cruisecontrol>"));
			XmlValidatingLoader loader = new XmlValidatingLoader(xr);
			loader.ValidationEventHandler += hd;
			loader.Schemas.Add(schema);
			XmlDocument doc = loader.Load();
			Assert.IsNull(doc);
		}

		[Test]
		public void SucceededLoad() 
		{
			ValidationEventHandler hd = new ValidationEventHandler(Handler);
			XmlSchema schema = LoadSchema();
			XmlTextReader xr = new XmlTextReader(new StringReader(ConfigurationFixture.GenerateConfigXml()));
			XmlValidatingLoader loader = new XmlValidatingLoader(xr);
			loader.ValidationEventHandler += hd;
			loader.Schemas.Add(schema);
			XmlDocument doc = loader.Load();
			Assert.IsNotNull(doc);
		}

		private XmlSchema LoadSchema() 
		{
			Assembly ass = Assembly.GetExecutingAssembly();
			Stream s = ass.GetManifestResourceStream(DefaultConfigurationFileLoader.XsdSchemaResourceName);
			return XmlSchema.Read(s, new ValidationEventHandler(Handler));
		}

		private void Handler(object sender, ValidationEventArgs args) 
		{
			// this handler is required, and not used
		}
	}
}
