using System.Configuration;
using System.Collections.Specialized;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Web.Test
{
	[TestFixture]
	public class ConfigSectionTest
	{
		[Test]
		public void LoadSection() 
		{
			NameValueCollection col = (NameValueCollection)ConfigurationSettings.GetConfig("xslFiles");
			Assert.AreEqual(2, col.Count);
			Assert.IsNotNull(col["one"]);
			Assert.AreEqual("foo.xsl", col["one"]);
			Assert.IsNotNull(col["two"]);
			Assert.AreEqual("bar.xsl", col["two"]);
		}
	}
}
