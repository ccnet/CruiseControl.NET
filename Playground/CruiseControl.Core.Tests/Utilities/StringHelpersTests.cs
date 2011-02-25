namespace CruiseControl.Core.Tests.Utilities
{
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class StringHelpersTests
    {
        #region Tests
        [Test]
        public void StripQuotesHandlesNull()
        {
            Assert.IsNull(
                StringHelpers.StripQuotes(null));
        }

        [Test]
        public void StripQuotesHandlesNoQuotes()
        {
            var value = "some data";
            var actual = value.StripQuotes();
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void StripQuotesHandlesStartingQuote()
        {
            var expected = "some data";
            var value = "\"" + expected;
            var actual = value.StripQuotes();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StripQuotesHandlesEndingQuote()
        {
            var expected = "some data";
            var value = expected + "\"";
            var actual = value.StripQuotes();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StripQuotesHandlesStartingAndEndingQuotes()
        {
            var expected = "some data";
            var value = "\"" + expected + "\"";
            var actual = value.StripQuotes();
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
