using System;
using System.Xml;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class XmlUtilTest : CustomAssertion
	{
		private static string TWO_SUCH_ELEMENTS = "two";
		private static string ONE_SUCH_ELEMENT = "one";
		private XmlDocument _doc;
		private XmlElement _elementOne;
		private XmlElement _elementTwo;
		private XmlElement _elementTwoAgain;
		
		[SetUp]
		public void SetUp()
		{
			InitTestDocument();
		}

		public void TestGetFirstElement()
		{			
			XmlElement actual = XmlUtil.GetFirstElement(_doc, TWO_SUCH_ELEMENTS);
			AssertEquals(_elementTwo, actual);
		}

		public void TestGetSingleElement()
		{
			XmlUtil.GetSingleElement(_doc, ONE_SUCH_ELEMENT);

			try
			{
				XmlUtil.GetSingleElement(_doc, TWO_SUCH_ELEMENTS);
				Fail("expected death at get single on multiple");
			}
			catch(CruiseControlException){}
		}

		protected void InitTestDocument()
		{
			_doc = new XmlDocument();
			_doc.AppendChild(_doc.CreateElement("root"));

			_elementOne = _doc.CreateElement(ONE_SUCH_ELEMENT);
			_elementTwo = _doc.CreateElement(TWO_SUCH_ELEMENTS);
			_elementTwoAgain = _doc.CreateElement(TWO_SUCH_ELEMENTS);

			_doc.DocumentElement.AppendChild(_elementOne);
			_doc.DocumentElement.AppendChild(_elementTwo);
			_doc.DocumentElement.AppendChild(_elementTwoAgain);
		}

		public void TestSelectValue()
		{
			XmlDocument document = XmlUtil.CreateDocument("<configuration><monkeys>bananas</monkeys></configuration>");
			string value = XmlUtil.SelectValue(document, "/configuration/monkeys", "orangutan");
			AssertEquals("bananas", value);			
		}

		public void TestSelectValue_missingValue()
		{
			XmlDocument document = XmlUtil.CreateDocument("<configuration><monkeys></monkeys></configuration>");
			string value = XmlUtil.SelectValue(document, "/configuration/monkeys", "orangutan");
			AssertEquals("orangutan", value);			
		}

		public void TestSelectValue_missingElement()
		{
			XmlDocument document = XmlUtil.CreateDocument("<configuration><monkeys></monkeys></configuration>");
			string value = XmlUtil.SelectValue(document, "/configuration/apes", "orangutan");
			AssertEquals("orangutan", value);			
		}

		public void TestSelectValue_attribute()
		{
			XmlDocument document = XmlUtil.CreateDocument("<configuration><monkeys part=\"brains\">booyah</monkeys></configuration>");
			string value = XmlUtil.SelectValue(document, "/configuration/monkeys/@part", "orangutan");
			AssertEquals("brains", value);			
		}

		public void TestSelectRequiredValue()
		{
			XmlDocument document = XmlUtil.CreateDocument("<configuration><martin>andersen</martin></configuration>");
			string value = XmlUtil.SelectRequiredValue(document, "/configuration/martin");
			AssertEquals("andersen", value);			
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestSelectRequiredValue_missingValue()
		{
			XmlDocument document = XmlUtil.CreateDocument("<configuration><martin></martin></configuration>");
			string value = XmlUtil.SelectRequiredValue(document, "/configuration/martin");
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestSelectRequiredValue_missingElement()
		{
			XmlDocument document = XmlUtil.CreateDocument("<configuration><martin></martin></configuration>");
			string value = XmlUtil.SelectRequiredValue(document, "/configuration/larry");
		}

		[Test]
		public void VerifyCDATAEncode()
		{
			string test = "a b <f>]]></a>";
			AssertEquals("a b <f>] ]></a>", XmlUtil.EncodeCDATA(test));
		}
	}
}