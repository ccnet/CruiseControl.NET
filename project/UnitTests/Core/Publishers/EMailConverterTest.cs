using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
    public class EMailConverterTest
    {
        [Test]
        public void ShouldPopulateFromSimpleXml()
        {
            EmailConverter converter = (EmailConverter) NetReflector.Read(
                @"<converter find=""$"" replace=""@Example.com""/>"
                );
            Assert.AreEqual("$", converter.Find);
            Assert.AreEqual("@Example.com", converter.Replace);
        }

        [Test]
        public void ShouldPopulateFromComplexXml()
        {
            EmailConverter converter = (EmailConverter)NetReflector.Read(
@"<converter>
    <find>$</find>
    <replace>@Example.com</replace>
</converter>"
                );
            Assert.AreEqual("$", converter.Find);
            Assert.AreEqual("@Example.com", converter.Replace);
        }

        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (find) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailConverter.Find).")]
		public void ShouldFailToReadEmptyConverter()
		{
			NetReflector.Read(@"<converter/>");
		}

        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (find) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailConverter.Find).")]
        public void ShouldFailToReadOmittedFindAttribute()
		{
			NetReflector.Read(@"<converter replace=""asdf""/>");
		}

        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (replace) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailConverter.Replace).")]
		public void ShouldFailToReadOmittedReplaceAttribute()
		{
			NetReflector.Read(@"<converter find=""asdf""/>");
		}

	}
}