namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class SecureArgumentsTests
    {
        #region Tests
        [Test]
        public void ConstructorAddsArguments()
        {
            var args = new SecureArguments("Test", 2);
            Assert.AreEqual(2, args.Count);
        }

        [Test]
        public void ToStringGeneratesPublicString()
        {
            var args = new SecureArguments(
                new PrivateString("privateData"),
                "publicData");
            var actual = args.ToString();
            var expected = "privateData publicData";
            Assert.AreNotEqual(expected, actual);
        }

        [Test]
        public void ToStringGeneratesPrivateStringInPrivateMode()
        {
            var args = new SecureArguments(
                new PrivateString("privateData"),
                "publicData");
            var actual = args.ToString(SecureDataMode.Private);
            var expected = "privateData publicData";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddAddsArgument()
        {
            var args = new SecureArguments("initial");
            args.Add("value1");
            var expected = "initial value1";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddAddsArgumentWithPrefix()
        {
            var args = new SecureArguments("initial");
            args.Add("value1", "p=");
            var expected = "initial p=value1";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddAddsArgumentWithPrefixAndDoubleQuotes()
        {
            var args = new SecureArguments("initial");
            args.Add("value1", "p=", true);
            var expected = "initial p=\"value1\"";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PlusAddsArgument()
        {
            var args = new SecureArguments("initial");
            args += "value1";
            var expected = "initial value1";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddIfAddsIfConditionMatches()
        {
            var args = new SecureArguments("initial");
            args.AddIf(true, "value1");
            var expected = "initial value1";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddIfDoesNotAddWithConditionMismatch()
        {
            var args = new SecureArguments("initial");
            args.AddIf(false, "value1");
            var expected = "initial";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddIfHandlesPrefix()
        {
            var args = new SecureArguments("initial");
            args.AddIf(true, "value1", "p=");
            var expected = "initial p=value1";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddIfHandlesPrefixAndQuotes()
        {
            var args = new SecureArguments("initial");
            args.AddIf(true, "value1", "p=", true);
            var expected = "initial p=\"value1\"";
            var actual = args.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StringisImplicitlyConverted()
        {
            var value = "some data to test";
            Func<SecureArguments, SecureArguments> doNothing = sa => sa;
            var args = doNothing(value);
            Assert.AreEqual(value, args.ToString());
        }
        #endregion
    }
}
