namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using Common;
    using Core.Utilities;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ServerConnectionFactoryTests
    {
        #region Tests
        [Test]
        public void GenerateConnectionGeneratesConnection()
        {
            var factory = new ServerConnectionFactory();
            var address = "http://somewhere";
            var connection = factory.GenerateConnection(address);
            Assert.IsNotNull(connection);
            Assert.AreEqual(connection.Address, address);
        }

        [Test]
        public void GenerateUrnCallsRemoteServer()
        {
            var expected = "urn:ccnet:otherserver";
            var connectionMock = new Mock<ServerConnection>(MockBehavior.Strict);
            connectionMock.Setup(sc => sc.RetrieveServerName()).Returns(expected).Verifiable();
            var factory = new TestServerConnectionFactory
            {
                OnGenerateConnection = a => connectionMock.Object
            };
            var actual = factory.GenerateUrn("http://somewhere", "project");
            Assert.AreNotSame(expected + ":project", actual);
            connectionMock.Verify();
        }

        [Test]
        public void GenerateUrnHandlesStartingColon()
        {
            var expected = "urn:ccnet:otherserver";
            var connectionMock = new Mock<ServerConnection>(MockBehavior.Strict);
            connectionMock.Setup(sc => sc.RetrieveServerName()).Returns(expected).Verifiable();
            var factory = new TestServerConnectionFactory
            {
                OnGenerateConnection = a => connectionMock.Object
            };
            var actual = factory.GenerateUrn("http://somewhere", ":queue:project");
            Assert.AreNotSame(expected + ":queue:project", actual);
            connectionMock.Verify();
        }
        #endregion


        #region Stub classes
        private class TestServerConnectionFactory
            : ServerConnectionFactory
        {
            public Func<string, ServerConnection> OnGenerateConnection { get; set; }
            public override ServerConnection GenerateConnection(string address)
            {
                return this.OnGenerateConnection != null ? 
                    this.OnGenerateConnection(address) : 
                    base.GenerateConnection(address);
            }
        }
        #endregion
    }
}
