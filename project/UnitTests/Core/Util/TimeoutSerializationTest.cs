using System.IO;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class TimeoutSerializationTest
	{
		private TimeoutSerializer serializer;
		private XmlDocument doc;

		[SetUp]
		public void SetUp()
		{
			serializer = new TimeoutSerializer(null, null);
			doc = new XmlDocument();
		}

		[Test]
		public void HandleSerializingNull()
		{
			serializer.Write(new XmlTextWriter(new StringWriter()), null);	
		}
		
		[Test]
		public void MillisAreSerializedWithNoPeriodAttribute()
		{
			Timeout period = new Timeout(100);
			StringWriter stringWriter = new StringWriter();
			serializer.Write(new XmlTextWriter(stringWriter), period);
			Assert.AreEqual("<timeout>100</timeout>", stringWriter.ToString());
		}

		[Test]
		public void MinutesAreSerializedWithAPeriodAttribute()
		{
			Timeout period = new Timeout(100, TimeUnits.MINUTES);
			StringWriter stringWriter = new StringWriter();
			serializer.Write(new XmlTextWriter(stringWriter), period);
			Assert.AreEqual("<timeout units=\"minutes\">100</timeout>", stringWriter.ToString());
		}

		[Test]
		public void CanDeserializeAttributes()
		{
			XmlAttribute a = doc.CreateAttribute("timeout");
			a.Value = "100";
			Assert.AreEqual(new Timeout(100, TimeUnits.MILLIS), serializer.Read(a, null));
		}

		[Test]
		public void CanDeserializeAsElements()
		{
			XmlElement e = doc.CreateElement("timeout");
			e.InnerText = "100";
			Assert.AreEqual(new Timeout(100, TimeUnits.MILLIS), serializer.Read(e, null));
		}

		[Test]
		public void CanDeserializeAsElementsWithUnits()
		{
			XmlElement e = doc.CreateElement("timeout");
			XmlAttribute units = doc.CreateAttribute("units");
			units.Value = "Seconds";
			e.Attributes.Append(units);
			e.InnerText = "2";
			Assert.AreEqual(new Timeout(2000, TimeUnits.MILLIS), serializer.Read(e, null));
		}
	}
}