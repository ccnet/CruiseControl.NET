using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
    [TestFixture]
    public class HgWebUrlBuilderTest
    {
        [Test]
        public void ShouldBuildValidUrls()
        {
            HgWebUrlBuilder hgweb = new HgWebUrlBuilder();
            hgweb.Url = "http://selenic.com/hg/index.cgi/";
            Modification[] modifications = new Modification[] {new Modification()};
            modifications[0].Version = "4a064e1977f8";
            hgweb.SetupModification(modifications);

            Assert.IsNotNull(modifications[0].Url);
            Assert.AreEqual("http://selenic.com/hg/index.cgi/rev/4a064e1977f8", modifications[0].Url);
        }
    }
}
