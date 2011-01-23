using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class NameValuePairTests
    {
        #region Test methods
        #region Properties
        [Test]
        public void NameGetSetTest()
        {
            NameValuePair pair = new NameValuePair();
            pair.Name = "pairName";
            Assert.AreEqual("pairName", pair.Name);
        }

        [Test]
        public void ValueGetSetTest()
        {
            NameValuePair pair = new NameValuePair();
            pair.Value = "pairValue";
            Assert.AreEqual("pairValue", pair.Value);
        }
        #endregion

        #region ToDictionary()
        [Test]
        public void ToDictionaryHandlesNull()
        {
            Dictionary<string, string> dictionary = NameValuePair.ToDictionary(null);
            Assert.AreEqual(0, dictionary.Count);
        }

        [Test]
        public void ToDictionaryConvertsValues()
        {
            List<NameValuePair> pairs = new List<NameValuePair>();
            pairs.Add(new NameValuePair("name", "value"));
            Dictionary<string, string> dictionary = NameValuePair.ToDictionary(pairs);
            Assert.AreEqual(1, dictionary.Count);
            Assert.IsTrue(dictionary.ContainsKey("name"));
            Assert.AreEqual("value", dictionary["name"]);
        }
        #endregion

        #region FromDictionary()
        [Test]
        public void FromDictionaryHandlesNull()
        {
            List<NameValuePair> list = NameValuePair.FromDictionary(null);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void FromDictionaryConvertsValues()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("name", "value");
            List<NameValuePair> list = NameValuePair.FromDictionary(dictionary);
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("name", list[0].Name);
            Assert.AreEqual("value", list[0].Value);
        }
        #endregion

        #region Equals()
        [Test]
        public void EqualsReturnsTrueIfTheNameIsTheSame()
        {
            var value1 = new NameValuePair("value", "key");
            var value2 = new NameValuePair("value", "key");
            Assert.IsTrue(value1.Equals(value2));
        }

        [Test]
        public void EqualsReturnsFalseIfTheNameIsDifferent()
        {
            var value1 = new NameValuePair("value1", "key");
            var value2 = new NameValuePair("value2", "key");
            Assert.IsFalse(value1.Equals(value2));
        }

        [Test]
        public void EqualsReturnsFalseIfTheOtherIsNull()
        {
            var value = new NameValuePair("value1", "key");
            Assert.IsFalse(value.Equals(null));
        }

        [Test]
        public void EqualsReturnsFalseIfTheOtherIsNotNamedValue()
        {
            var value = new NameValuePair("value1", "key");
            Assert.IsFalse(value.Equals("test"));
        }
        #endregion
        #endregion
    }
}
