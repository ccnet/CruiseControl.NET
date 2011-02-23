namespace CruiseControl.Core.Tests.Channels
{
    using System;
    using CruiseControl.Common;
    using CruiseControl.Core.Channels;
    using CruiseControl.Core.Interfaces;
    using Moq;
    using NUnit.Framework;

    [TestFixture(Description = "These are general tests to check that the communications API works")]
    public class CommunicationsTests
    {
        #region Tests
        [Test]
        public void RetrieveServerNameOverWcf()
        {
            var serverName = "urn:ccnet:server";
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.RetrieveServerName()).Returns(serverName);
            var result = this.RunTest(invokerMock.Object, c => c.RetrieveServerName());
            Assert.AreEqual(serverName, result);
        }
        #endregion

        #region Helper methods
        private TResult RunTest<TResult>(IActionInvoker invoker, Func<ServerConnection, TResult> test)
        {
            var channel = new Wcf();
            var address = "net.tcp://localhost/client";
            channel.Endpoints.Add(new NetTcp { Address = address });
            var opened = false;
            try
            {
                opened = channel.Initialise(invoker);
                Assert.IsTrue(opened);
                var connection = new ServerConnection(address);
                return test(connection);
            }
            finally
            {
                if (opened)
                {
                    channel.CleanUp();
                }
            }
        }
        #endregion
    }
}
