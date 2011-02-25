namespace CruiseControl.Core.Tests.Utilities
{
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    public class PrivateStringTypeConverterTests
    {
        #region Tests
        [Test]
        public void CanConvertFromReturnsTrueForStrings()
        {
            var converter = new PrivateStringTypeConverter();
            var actual = converter.CanConvertFrom(typeof(string));
            Assert.IsTrue(actual);
        }

        [Test]
        public void CanConvertFromReturnsFalseForNonStrings()
        {
            var converter = new PrivateStringTypeConverter();
            var actual = converter.CanConvertFrom(typeof(int));
            Assert.IsFalse(actual);
        }

        [Test]
        public void ConvertFromHandlesNull()
        {
            var converter = new PrivateStringTypeConverter();
            var actual = converter.ConvertFrom(null);
            Assert.IsNull(actual);
        }

        [Test]
        public void ConvertFromGeneratesPrivateString()
        {
            var converter = new PrivateStringTypeConverter();
            var value = "testData";
            var actual = converter.ConvertFrom(value);
            Assert.IsInstanceOf<PrivateString>(actual);
            var privateString = actual as PrivateString;
            Assert.AreEqual(value, privateString.PrivateValue);
        }
        #endregion
    }
}
