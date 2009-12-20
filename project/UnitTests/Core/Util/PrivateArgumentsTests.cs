namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Util;

    [TestFixture]
    public class PrivateArgumentsTests
    {
        [Test]
        public void ConstructorWithNoArgumentsInitialises()
        {
            var args = new PrivateArguments();
            Assert.AreEqual(0, args.Count);
        }

        [Test]
        public void ConstructorWithOneArgumentInitialises()
        {
            var args = new PrivateArguments("test");
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("test", args.ToString());
        }

        [Test]
        public void ConstructorWithTwoArgumentsInitialises()
        {
            var args = new PrivateArguments("first", "second");
            Assert.AreEqual(2, args.Count);
            Assert.AreEqual("first second", args.ToString());
        }

        [Test]
        public void ToStringGeneratesPublicString()
        {
            PrivateString hidden = "private";
            var args = new PrivateArguments("public", hidden);
            Assert.AreEqual("public " + hidden.PublicValue, args.ToString());
        }

        [Test]
        public void ToStringPublicGeneratesPublicString()
        {
            PrivateString hidden = "private";
            var args = new PrivateArguments("public", hidden);
            Assert.AreEqual("public ********", args.ToString(SecureDataMode.Public));
        }

        [Test]
        public void ToStringPrivateGeneratesPublicString()
        {
            PrivateString hidden = "private";
            var args = new PrivateArguments("public", hidden);
            Assert.AreEqual("public private", args.ToString(SecureDataMode.Private));
        }

        [Test]
        public void AddWithValueAdds()
        {
            var args = new PrivateArguments();
            args.Add("testValue");
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("testValue", args.ToString());
        }

        [Test]
        public void AddWithPrefixedValueAdds()
        {
            var args = new PrivateArguments();
            args.Add("pre=", "test Value");
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("pre=test Value", args.ToString());
        }

        [Test]
        public void AddQuoteWithValueAdds()
        {
            var args = new PrivateArguments();
            args.AddQuote("testValue");
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("\"testValue\"", args.ToString());
        }

        [Test]
        public void AddQuoteWithPrefixedValueAdds()
        {
            var args = new PrivateArguments();
            args.AddQuote("pre=", "test Value");
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("pre=\"test Value\"", args.ToString());
        }

        [Test]
        public void AddWithAutoQuoteValueAdds()
        {
            var args = new PrivateArguments();
            args.Add("pre=", "test Value", true);
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("pre=\"test Value\"", args.ToString());
        }

        [Test]
        public void AddIfWithValueAddsOnTrue()
        {
            var args = new PrivateArguments();
            args.AddIf(true, "testValue");
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("testValue", args.ToString());
        }

        [Test]
        public void AddIfWithPrefixedValueAddsOnTrue()
        {
            var args = new PrivateArguments();
            args.AddIf(true, "pre=", "test Value");
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("pre=test Value", args.ToString());
        }

        [Test]
        public void AddIfWithAutoQuoteValueAddsOnTrue()
        {
            var args = new PrivateArguments();
            args.AddIf(true, "pre=", "test Value", true);
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("pre=\"test Value\"", args.ToString());
        }

        [Test]
        public void AddIfWithValueDoesNotAddOnFalse()
        {
            var args = new PrivateArguments();
            args.AddIf(false, "testValue");
            Assert.AreEqual(0, args.Count);
            Assert.AreEqual(string.Empty, args.ToString());
        }

        [Test]
        public void AddIfWithPrefixedValueDoesNotAddOnFalse()
        {
            var args = new PrivateArguments();
            args.AddIf(false, "pre=", "test Value");
            Assert.AreEqual(0, args.Count);
            Assert.AreEqual(string.Empty, args.ToString());
        }

        [Test]
        public void AddIfWithAutoQuoteValueDoesNotAddOnFalse()
        {
            var args = new PrivateArguments();
            args.AddIf(false, "pre=", "test Value", true);
            Assert.AreEqual(0, args.Count);
            Assert.AreEqual(string.Empty, args.ToString());
        }

        [Test]
        public void ImplicitOperatorGeneratesInstance()
        {
            PrivateArguments args = "test args";
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("test args", args.ToString());
        }

        [Test]
        public void PlusOperatorAddsPublicValue()
        {
            PrivateArguments args = "test args";
            args += "value";
            Assert.AreEqual(2, args.Count);
            Assert.AreEqual("test args value", args.ToString());
        }

        [Test]
        public void PlusOperatorAddsPrivateValue()
        {
            PrivateArguments args = "test args";
            args += new PrivateString
            {
                PrivateValue = "value"
            };
            Assert.AreEqual(2, args.Count);
            Assert.AreEqual("test args ********", args.ToString());
        }
    }
}
