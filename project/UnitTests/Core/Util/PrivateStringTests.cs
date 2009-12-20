namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Util;

    [TestFixture]
    public class PrivateStringTests
    {
        [Test]
        public void ImplicitConvertsFromString()
        {
            PrivateString theString = "testing";
            Assert.AreEqual("testing", theString.PrivateValue);
        }

        [Test]
        public void PublicIsHiddenPrivateIsSeen()
        {
            var testValue = "testValue";
            var theString = new PrivateString
            {
                PrivateValue = testValue
            };
            Assert.AreEqual(testValue, theString.PrivateValue);
            Assert.AreNotEqual(testValue, theString.PublicValue);
        }

        [Test]
        public void ToStringReturnsPublicValue()
        {
            var testString = new PrivateString
            {
                PrivateValue = "hidden"
            };
            Assert.AreEqual(testString.PublicValue, testString.ToString());
        }

        [Test]
        public void ToStringWithPublicReturnsPublicValue()
        {
            var testString = new PrivateString
            {
                PrivateValue = "hidden"
            };
            Assert.AreEqual(testString.PublicValue, testString.ToString(SecureDataMode.Public));
        }

        [Test]
        public void ToStringWithPrivateReturnsPrivateValue()
        {
            var testString = new PrivateString
            {
                PrivateValue = "hidden"
            };
            Assert.AreEqual(testString.PrivateValue, testString.ToString(SecureDataMode.Private));
        }
    }
}
