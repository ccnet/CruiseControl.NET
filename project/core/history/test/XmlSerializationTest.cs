using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using tw.ccnet.core.test;

namespace tw.ccnet.core.history.test
{
	[TestFixture]
	public class XmlSerializationTest
	{
		public void TestSerialization()
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			XmlSerializer serializer = new XmlSerializer(typeof(IntegrationResult));
			StringBuilder buffer = new StringBuilder();
			serializer.Serialize(new StringWriter(buffer), result);

			IntegrationResult actual = (IntegrationResult)serializer.Deserialize(new StringReader(buffer.ToString()));
			Assertion.AssertEquals(result, actual);
		}
	}
}
