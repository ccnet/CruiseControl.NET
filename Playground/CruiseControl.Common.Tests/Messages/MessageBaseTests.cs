namespace CruiseControl.Common.Tests.Messages
{
    using System;
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class MessageBaseTests
    {
        #region Tests
        [Test]
        public void BasePropertiesAreSerialised()
        {
            var original = new TestMessage
                               {
                                   Identifier = Guid.NewGuid(),
                                   TimeStamp = DateTime.Now
                               };
            var result = original.DoRoundTripTest();
            Assert.AreEqual(original.Identifier, result.Identifier);
            Assert.AreEqual(original.TimeStamp, result.TimeStamp);
        }
        #endregion

        #region Classes
        public class TestMessage
            : BaseMessage
        {
        }
        #endregion
    }
}
