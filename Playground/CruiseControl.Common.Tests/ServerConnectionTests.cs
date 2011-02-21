namespace CruiseControl.Common.Tests
{
    using System;
    using System.ServiceModel;
    using NUnit.Framework;

    [TestFixture]
    public class ServerConnectionTests
    {
        #region Tests
        [Test]
        public void PingReturnsFalseOnFailure()
        {
            var connection = new ServerConnection("net.tcp://nowhere");
            var result = connection.Ping();
            Assert.IsFalse(result);
        }

        [Test]
        public void GenerateBindingFromProtocolFailsWhenAddressIsMissing()
        {
            var connection = new ServerConnection();
            Assert.Throws<InvalidOperationException>(connection.GenerateBindingFromProtocol);
        }

        [Test]
        public void GenerateBindingFromProtocolFailsWhenProtocolIsMissing()
        {
            var connection = new ServerConnection
                                 {
                                     Address = "somewhere.com"
                                 };
            Assert.Throws<InvalidOperationException>(connection.GenerateBindingFromProtocol);
        }

        [Test]
        public void GenerateBindingFromProtocolFailsWithUnknownProtocol()
        {
            var connection = new ServerConnection
                                 {
                                     Address = "unknown://somewhere.com"
                                 };
            Assert.Throws<InvalidOperationException>(connection.GenerateBindingFromProtocol);
        }

        [Test]
        public void ConstructorSetsProperties()
        {
            var binding = new NetTcpBinding();
            var address = "net.tcp://somewhere";
            var connection = new ServerConnection(address, binding);
            Assert.AreEqual(connection.Address, address);
            Assert.AreSame(binding, connection.Binding);
        }

        [Test]
        public void GenerateBindingFromProtocolGeneratesBasicHttpForHttpProtocol()
        {
            var connection = new ServerConnection
                                 {
                                     Address = "http://somewhere"
                                 };
            connection.GenerateBindingFromProtocol();
            Assert.IsInstanceOf<BasicHttpBinding>(connection.Binding);
        }

        [Test]
        public void GenerateBindingFromProtocolGeneratesBasicHttpForNetTcpProtocol()
        {
            var connection = new ServerConnection
                                 {
                                     Address = "net.tcp://somewhere"
                                 };
            connection.GenerateBindingFromProtocol();
            Assert.IsInstanceOf<NetTcpBinding>(connection.Binding);
        }
        #endregion
    }
}
