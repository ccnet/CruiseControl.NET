using System;
using System.Configuration;
using System.Collections;
using NUnit.Framework;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class ConfigSectionHandler
	{
		[Test]
		public void GetConfig() 
		{
			IList list = (IList)ConfigurationSettings.GetConfig("xslFiles");
			Assertion.AssertNotNull(list);
			Assertion.AssertEquals(5, list.Count);
			Assertion.AssertEquals(@"xsl\header.xsl", (string)list[0]);
			Assertion.AssertEquals(@"xsl\compile.xsl", (string)list[1]);
			Assertion.AssertEquals(@"xsl\unittests.xsl", (string)list[2]);
			Assertion.AssertEquals(@"xsl\fit.xsl", (string)list[3]);
			Assertion.AssertEquals(@"xsl\modifications.xsl", (string)list[4]);
		}
	}
}
