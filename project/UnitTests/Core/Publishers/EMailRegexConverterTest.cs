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

        [Test]
        public void ShouldFailToReadEmptyConverter()
        {
            Assert.That(delegate { NetReflector.Read(@"<regexConverter/>"); },
                        Throws.TypeOf<NetReflectorException>());
        }

        [Test]
        public void ShouldFailToReadOmittedFindAttribute()
        {
            Assert.That(delegate { NetReflector.Read(@"<regexConverter replace=""asdf"" />"); },
                        Throws.TypeOf<NetReflectorException>());
        }

        [Test]
        public void ShouldFailToReadOmittedReplaceAttribute()
        {
            Assert.That(delegate { NetReflector.Read(@"<regexConverter find=""asdf""/>"); },
                        Throws.TypeOf<NetReflectorException>());
        }
	}
}