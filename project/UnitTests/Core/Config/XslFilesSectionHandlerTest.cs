using System.Collections;
using System.Configuration;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
    [TestFixture]
    public class XslFilesSectionHandlerTest : CustomAssertion
    {
        [Test]
        public void GetConfig()
        {
            var section = ConfigurationManager.GetSection("xslFiles") as XslFilesSectionHandler;
            Assert.IsNotNull(section, "section should not be null");
            IList list = section.FileNames;
            Assert.IsNotNull(list, "file names list should not be null");
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(@"xsl\header.xsl", list[0]);
            Assert.AreEqual(@"xsl\compile.xsl", list[1]);
            Assert.AreEqual(@"xsl\unittests.xsl", list[2]);
            Assert.AreEqual(@"xsl\fit.xsl", list[3]);
            Assert.AreEqual(@"xsl\modifications.xsl", list[4]);
        }
    }
}