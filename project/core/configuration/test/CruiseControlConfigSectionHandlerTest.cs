using NUnit.Framework;
using System;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Configuration.test
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
