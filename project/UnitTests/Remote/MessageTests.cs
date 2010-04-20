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

        [Test]
        public void GetHashCodeReturnsStringHashCode()
        {
            var msg = new Message("A message");
            var hashCode = msg.GetHashCode();
            Assert.AreEqual(msg.ToString().GetHashCode(), msg.GetHashCode());
        }

        [Test]
        public void EqualsReturnsTrueWhenBothMessageAndKindAreSame()
        {
            var msg1 = new Message("The message", Message.MessageKind.NotDefined);
            var msg2 = new Message("The message", Message.MessageKind.NotDefined);
            Assert.IsTrue(msg1.Equals(msg2));
        }

        [Test]
        public void EqualsReturnsFalseIfMessageIsDifferent()
        {
            var msg1 = new Message("The message1", Message.MessageKind.NotDefined);
            var msg2 = new Message("The message2", Message.MessageKind.NotDefined);
            Assert.IsFalse(msg1.Equals(msg2));
        }

        [Test]
        public void EqualsReturnsFalseIfTypeIsDifferent()
        {
            var msg1 = new Message("The message", Message.MessageKind.NotDefined);
            var msg2 = new Message("The message", Message.MessageKind.Fixer);
            Assert.IsFalse(msg1.Equals(msg2));
        }

        [Test]
        public void EqualsReturnsFalseIfArgumentIsNotAMessage()
        {
            var msg1 = new Message("The message", Message.MessageKind.NotDefined);
            var msg2 = "A message";
            Assert.IsFalse(msg1.Equals(msg2));
        }
        #endregion
    }
}
