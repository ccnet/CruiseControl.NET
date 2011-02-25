namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class PrivateStringTests
    {
        #region Tests
        [Test]
        public void ConstructorStoresValue()
        {
            var value = "testData";
            var privateString = new PrivateString(value);
            Assert.AreEqual(value, privateString.PrivateValue);
        }

        [Test]
        public void PublicValueReturnsMask()
        {
            var value = "testData";
            var privateString = new PrivateString
                            {
                                PrivateValue = value
                            };
            Assert.AreNotEqual(value, privateString);
        }

        [Test]
        public void ToStringReturnsPublicData()
        {
            var value = "testData";
            var privateString = new PrivateString
                                    {
                                        PrivateValue = value
                                    };
            var actual = privateString.ToString();
            Assert.AreNotEqual(value, actual);
        }

        [Test]
        public void ToStringReturnsPrivateDataInPrivateMode()
        {
            var value = "testData";
            var privateString = new PrivateString
                                    {
                                        PrivateValue = value
                                    };
            var actual = privateString.ToString(SecureDataMode.Private);
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void StringIsConvertedToPrivateStringImplicity()
        {
            Func<PrivateString, PrivateString> doNothing = ps => ps;
            var value = "Testing";
            var actual = doNothing(value);
            Assert.IsInstanceOf<PrivateString>(actual);
            Assert.AreEqual(value, actual.PrivateValue);
        }
        #endregion
    }
}
