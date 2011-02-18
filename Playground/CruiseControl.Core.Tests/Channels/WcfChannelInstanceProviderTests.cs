namespace CruiseControl.Core.Tests.Channels
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using CruiseControl.Core.Channels;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class WcfChannelInstanceProviderTests
    {
        #region Tests
        [Test]
        public void ReleaseInstanceDoesNothing()
        {
            var provider = new WcfChannelInstanceProvider(null);
            provider.ReleaseInstance(null, null);
        }

        [Test]
        public void GetInstanceGeneratesNewInstanceWithServer()
        {
            var invoker = new ActionInvoker();
            var provider = new WcfChannelInstanceProvider(invoker);
            var instance = provider.GetInstance(null, null);
            Assert.IsInstanceOf<WcfChannel>(instance);
            var channel = instance as WcfChannel;
            Assert.AreSame(invoker, channel.Invoker);
        }

        [Test]
        public void ValidateDoesNothing()
        {
            var provider = new WcfChannelInstanceProvider(null);
            provider.Validate(null, null);
        }

        [Test]
        public void AddBindingParametersDoesNothing()
        {
            var provider = new WcfChannelInstanceProvider(null);
            provider.AddBindingParameters(null, null, null, null);
        }

        [Test]
        public void ApplyDispatchBehaviorAddsTheProvider()
        {
            // TODO: Figure out how to mock up the service host with endpoint dispatchers
            var listenerMock = new Mock<IChannelListener>();
            var channelDispatcher = new ChannelDispatcher(listenerMock.Object);
            var hostMock = new Mock<ServiceHostBase>();
            hostMock.Object.ChannelDispatchers.Add(channelDispatcher);
            var provider = new WcfChannelInstanceProvider(null);
            provider.ApplyDispatchBehavior(null, hostMock.Object);
        }
        #endregion
    }
}
