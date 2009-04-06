using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
    public class EMailRegexConverterTest
    {
        [Test]
        public void ShouldPopulateFromSimpleXml()
        {
            EmailRegexConverter regexConverter = (EmailRegexConverter) NetReflector.Read(
                @"<regexConverter find=""$"" replace=""@Example.com""/>"
                );
            Assert.AreEqual("$", regexConverter.Find);
            Assert.AreEqual("@Example.com", regexConverter.Replace);
        }

        [Test]
        public void ShouldPopulateFromComplexXml()
        {
            EmailRegexConverter regexConverter = (EmailRegexConverter)NetReflector.Read(
@"<regexConverter>
    <find>$</find>
    <replace>@Example.com</replace>
</regexConverter>"
                );
            Assert.AreEqual("$", regexConverter.Find);
            Assert.AreEqual("@Example.com", regexConverter.Replace);
        }

        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (find) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailRegexConverter.Find).\r\n" +
            "Xml: <regexConverter />")]
		public void ShouldFailToReadEmptyConverter()
		{
            NetReflector.Read(@"<regexConverter/>");
		}

        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (find) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailRegexConverter.Find).\r\n" +
            "Xml: <regexConverter replace=\"asdf\" />")]
        public void ShouldFailToReadOmittedFindAttribute()
		{
            NetReflector.Read(@"<regexConverter replace=""asdf"" />");
		}

        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (replace) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailRegexConverter.Replace).\r\n" +
            "Xml: <regexConverter find=\"asdf\" />")]
		public void ShouldFailToReadOmittedReplaceAttribute()
		{
            NetReflector.Read(@"<regexConverter find=""asdf""/>");
		}

	}
}