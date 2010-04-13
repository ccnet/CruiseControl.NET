namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class CommunicationsMessageTests
    {
        #region Tests
        [Test]
        public void ChannelCanBeSetAndRetrieved()
        {
            var channelInfo = new object();
            var message = new ServerRequest();
            message.ChannelInformation = channelInfo;
            Assert.AreSame(channelInfo, message.ChannelInformation);
        }
        #endregion
    }
}
