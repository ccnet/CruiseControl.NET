namespace CruiseControl.Core.Tests.Channels
{
    using CruiseControl.Common;
    using CruiseControl.Core.Channels;
    using CruiseControl.Core.Interfaces;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class WcfChannelTests
    {
        #region Tests
        [Test]
        public void InvokePassesOnCall()
        {
            var urn = string.Empty;
            var args = new InvokeArguments();
            var result = new InvokeResult();
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(i => i.Invoke(urn, args)).Returns(result).Verifiable();

            var channel = new WcfChannel(invokerMock.Object);
            var actual = channel.Invoke(urn, args);
            Assert.AreSame(result, actual);
            invokerMock.Verify();
        }

        [Test]
        public void QueryPassesOnCall()
        {
            var urn = string.Empty;
            var args = new QueryArguments();
            var result = new QueryResult();
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(i => i.Query(urn, args)).Returns(result).Verifiable();

            var channel = new WcfChannel(invokerMock.Object);
            var actual = channel.Query(urn, args);
            Assert.AreSame(result, actual);
            invokerMock.Verify();
        }
        #endregion
    }
}
