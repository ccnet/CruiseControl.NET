using System;
using System.Configuration;
using System.Collections;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class XslFilesSectionHandler : CustomAssertion
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
