using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

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

			string[] configFiles = new string[]
				{
					"ccnet.config", "CVSAndNAntAndEmailPublisherCCNet.config", "VSSAndDevenvAndNUnitCCNet.config", "P4AndDevenv.config"
				};
			foreach (string f in configFiles)
			{
				filename = f;
				XmlDocument xml = LoadConfigXml();
				Assert.IsNotNull(reader.Read(xml, null));
			}
		}
		
		[Test]
		public void InvalidTaskXmlShouldThrowNetReflectorException()
		{
			NetReflectorConfigurationReader reader = new NetReflectorConfigurationReader();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(@"
<cruisecontrol>
       <project name=""WebTrunkTest"" artifactDirectory=""..\WebTrunkTest"" >

               <tasks>
                       <build type=""nant"">
							<executable>d:\build\bin\nant.exe</executable>
                       </build>
               </tasks>
       </project>
</cruisecontrol>
");
		    Assert.That(delegate { reader.Read(xml, null); },
                        Throws.TypeOf<ConfigurationException>());
		}

		private XmlDocument LoadConfigXml()
		{
			Stream stream = ResourceUtil.LoadResource(GetType(), filename);
			XmlDocument xml = new XmlDocument();
			xml.Load(stream);
			return xml;
		}
	}
}