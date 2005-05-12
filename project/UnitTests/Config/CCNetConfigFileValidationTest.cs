using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.UnitTests.Config
{
	[TestFixture]
	public class CCNetConfigFileValidationTest
	{
		private string filename;

		[Test]
		public void ExampleConfigFilesShouldNotContainAnyInvalidElements()
		{
			NetReflectorConfigurationReader reader = new NetReflectorConfigurationReader();
			reader.InvalidNodeEventHandler += new InvalidNodeEventHandler(reader_InvalidNodeEventHandler);

			foreach (string f in new string[] {"ccnet.config", "CVSAndNAntAndEmailPublisherCCNet.config", "VSSAndDevenvAndNUnitCCNet.config"})
			{
				filename = f;
				XmlDocument xml = LoadConfigXml(filename);
				Assert.IsNotNull(reader.Read(xml));
			}
		}

		private void reader_InvalidNodeEventHandler(InvalidNodeEventArgs args)
		{
			throw new Exception(string.Format("configuration file {0} contains invalid xml: {1}", filename, args.Message));
		}

		private XmlDocument LoadConfigXml(string filename)
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType().Namespace + "." + filename);
			XmlDocument xml = new XmlDocument();
			xml.Load(stream);
			return xml;
		}
	}
}