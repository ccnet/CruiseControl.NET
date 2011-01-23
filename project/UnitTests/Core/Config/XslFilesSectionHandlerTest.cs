using System.Collections;
using System.Configuration;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
    [TestFixture]
    public class XslFilesSectionHandler : CustomAssertion
    {
        [Test]
        public void GetConfig()
        {
            IList list = (IList) ConfigurationManager.GetSection("xslFiles");
            Assert.IsNotNull(list);
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(@"xsl\header.xsl", list[0]);
            Assert.AreEqual(@"xsl\compile.xsl", list[1]);
            Assert.AreEqual(@"xsl\unittests.xsl", list[2]);
            Assert.AreEqual(@"xsl\fit.xsl", list[3]);
            Assert.AreEqual(@"xsl\modifications.xsl", list[4]);
        }
    }
}