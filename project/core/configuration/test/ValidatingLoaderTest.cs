using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

using tw.ccnet.core.util;
using tw.ccnet.core.configuration;

namespace tw.ccnet.core.configuration.test
{
	[TestFixture]
	public class ValidatingLoaderTest : CustomAssertion
	{
		[Test]
		public void FailedLoad() 
		{
			ValidationEventHandler hd = new ValidationEventHandler(handler);
			XmlSchema schema = loadSchema();
			XmlTextReader xr = new XmlTextReader(new StringReader(@"<cruisecontrol><projectx></projectx></cruisecontrol>"));
			XmlValidatingLoader loader = new XmlValidatingLoader(xr);
			loader.ValidationEventHandler += hd;
			loader.Schemas.Add(schema);
			XmlDocument doc = loader.Load();
			AssertNull(doc);
		}

		[Test]
		public void SucceededLoad() 
		{
			ValidationEventHandler hd = new ValidationEventHandler(handler);
			XmlSchema schema = loadSchema();
			XmlTextReader xr = new XmlTextReader(new StringReader(ConfigurationFixture.GenerateConfigXml()));
			XmlValidatingLoader loader = new XmlValidatingLoader(xr);
			loader.ValidationEventHandler += hd;
			loader.Schemas.Add(schema);
			XmlDocument doc = loader.Load();
			AssertNotNull(doc);
		}

		private XmlSchema loadSchema() 
		{
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
			Stream s = ass.GetManifestResourceStream("tw.ccnet.core.configuration.ccnet.xsd");
			return XmlSchema.Read(s, new ValidationEventHandler(handler));
		}

		private void handler(object sender, ValidationEventArgs args) 
		{

		}
	}


}
