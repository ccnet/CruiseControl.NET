using System.Collections;
using System.Configuration;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class XslFilesSectionHandler : CustomAssertion
	{
		[Test]
		public void GetConfig() 
		{
			IList list = (IList)ConfigurationSettings.GetConfig("xslFiles");
			Assert.IsNotNull(list);
			Assert.AreEqual(5, list.Count);
			Assert.AreEqual(@"xsl\header.xsl", (string)list[0]);
			Assert.AreEqual(@"xsl\compile.xsl", (string)list[1]);
			Assert.AreEqual(@"xsl\unittests.xsl", (string)list[2]);
			Assert.AreEqual(@"xsl\fit.xsl", (string)list[3]);
			Assert.AreEqual(@"xsl\modifications.xsl", (string)list[4]);
		}
	}
}
