using System;
using System.Xml;
using System.Reflection;
using NUnit.Framework;
using Exortech.NetReflector;
using tw.ccnet.core.util;

namespace tw.ccnet.core.schedule.test
{
	[TestFixture]
	public class ScheduleTest
	{
		[Test]
		public void PopulateFromReflector()
		{
			string xml = String.Format(@"<schedule timeout=""1"" iterations=""1""/>");
			XmlPopulator populator = new XmlPopulator();
			populator.Reflector.AddReflectorTypes(Assembly.GetExecutingAssembly());

			Schedule schedule = (Schedule)populator.Populate(XmlUtil.CreateDocumentElement(xml));
			Assertion.AssertEquals(1, schedule.TimeOut);
			Assertion.AssertEquals(1, schedule.TotalIterations);
		}

	}
}
