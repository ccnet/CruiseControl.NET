namespace CruiseControl.Common.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ServerConnectionTests
    {
        #region Tests
        [Test]
        public void PingReturnsFalseOnFailure()
        {
            var result = ServerConnection.Ping("net.tcp://nowhere");
            Assert.IsFalse(result);
        }
        #endregion
    }
}
