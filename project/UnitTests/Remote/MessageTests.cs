using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class MessageTests
    {
        #region Test methods
        [Test]
        public void StartANewBlankMessage()
        {
            Message value = new Message();
            Assert.IsNull(value.Text);
        }

        [Test]
        public void StartANewMessageWithText()
        {
            string expected = "Testing";
            Message value = new Message(expected);
            Assert.AreEqual(expected, value.Text);
        }

        [Test]
        public void TextPropertyCanBeSet()
        {
            string expected = "Testing";
            Message value = new Message();
            Assert.IsNull(value.Text);
            value.Text = expected;
            Assert.AreEqual(expected, value.Text);
        }

        [Test]
        public void ToStringReturnsMessage()
        {
            string expected = "Testing";
            Message value = new Message(expected);
            Assert.AreEqual(expected, value.ToString());
        }
        #endregion
    }
}
