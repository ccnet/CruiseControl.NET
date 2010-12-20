namespace CruiseControl.Core.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class VersionTypeConverterTests
    {
        #region Tests
        [Test]
        public void CanConvertFromReturnsTrueForStrings()
        {
            var converter = new VersionTypeConverter();
            var result = converter.CanConvertFrom(null, typeof(string));
            Assert.IsTrue(result);
        }

        [Test]
        public void CanConvertFromReturnsFalseForNonStrings()
        {
            var converter = new VersionTypeConverter();
            var result = converter.CanConvertFrom(null, typeof(int));
            Assert.IsFalse(result);
        }

        [Test]
        public void ConvertFromReturnsVersionFromVersionString()
        {
            var converter = new VersionTypeConverter();
            var result = converter.ConvertFrom("1.0");
            Assert.IsInstanceOf<Version>(result);
            Assert.AreEqual(new Version("1.0"), result);
        }

        [Test]
        public void ConvertFromNonStringThrowsException()
        {
            var converter = new VersionTypeConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(1.0));
        }
        #endregion
    }
}
