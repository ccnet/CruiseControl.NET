using System;
using System.Configuration;
using System.Collections;
using NUnit.Framework;
using tw.ccnet.core.util;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class ConfigSectionHandlerTest : CustomAssertion
	{
		[Test]
		public void GetConfig() 
		{
			IList list = (IList)ConfigurationSettings.GetConfig("xslFiles");
			AssertNotNull(list);
			AssertEquals(5, list.Count);
			AssertEquals(@"xsl\header.xsl", (string)list[0]);
			AssertEquals(@"xsl\compile.xsl", (string)list[1]);
			AssertEquals(@"xsl\unittests.xsl", (string)list[2]);
			AssertEquals(@"xsl\fit.xsl", (string)list[3]);
			AssertEquals(@"xsl\modifications.xsl", (string)list[4]);
		}
	}
}
