using System;
using System.Configuration;
using System.Collections.Specialized;
using NUnit.Framework;

namespace tw.ccnet.web.test
{
	[TestFixture]
	public class ConfigSectionTest
	{
		[Test]
		public void LoadSection() 
		{
			NameValueCollection col = (NameValueCollection)ConfigurationSettings.GetConfig("xslFiles");
			Assertion.AssertEquals(2, col.Count);
			Assertion.AssertNotNull("one should not be null", col["one"]);
			Assertion.AssertEquals("foo.xsl", col["one"]);
			Assertion.AssertNotNull("two should not be null", col["two"]);
			Assertion.AssertEquals("bar.xsl", col["two"]);
			foreach (string key in col) 
			{
				Console.WriteLine(col[key]);
			}
		}
	}
}
