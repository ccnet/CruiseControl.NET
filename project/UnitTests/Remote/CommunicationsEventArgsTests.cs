namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class CommunicationsEventArgsTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsProperties()
        {
            var message = new Response();
            var args = new CommunicationsEventArgs("action", message);
            Assert.AreEqual("action", args.Action);
            Assert.AreSame(message, args.Message);
        }
        #endregion
    }
}
