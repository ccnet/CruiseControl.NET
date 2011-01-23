namespace CruiseControl.Common.Tests.Messages
{
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class ServerMessageTests
    {
        #region Tests
        [Test]
        public void BasePropertiesAreSerialised()
        {
            var original = new ServerMessage
                               {
                                   ServerName = "local"
                               };
            var result = original.DoRoundTripTest();
            Assert.AreEqual(original.ServerName, result.ServerName);
        }
        #endregion
    }
}
