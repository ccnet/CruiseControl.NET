using System.Xml;

using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class CruiseControlConfigSectionHandlerTest
	{
		[Test]
		public void LoadConfiguration()
		{
			CruiseControlConfigSectionHandler handler = new CruiseControlConfigSectionHandler();

			string xml = "<cruisecontrol/>";
			XmlNode xmlNode = XmlUtil.CreateDocumentElement(xml);

			IConfiguration config = handler.Create(null, null, xmlNode) as IConfiguration;
		}
	}
}
