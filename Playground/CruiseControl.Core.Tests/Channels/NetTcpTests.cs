namespace CruiseControl.Core.Tests.Channels
{
    using System.ServiceModel;
    using CruiseControl.Core.Channels;
    using NUnit.Framework;

    [TestFixture]
    public class NetTcpTests
    {
        #region Tests
        [Test]
        public void BindingReturnsCorrectType()
        {
            var endpoint = new NetTcp();
            Assert.IsInstanceOf<NetTcpBinding>(endpoint.Binding);
        }
        #endregion
    }
}
