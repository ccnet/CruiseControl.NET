namespace CruiseControl.Core.Tests.Channels
{
    using CruiseControl.Common;
    using CruiseControl.Core.Channels;
    using CruiseControl.Core.Tests.Stubs;
    using NUnit.Framework;

    [TestFixture]
    public class WcfTests
    {
        #region Tests
        [Test]
        public void ValidateReturnsWarningIfNoName()
        {
            var warningAdded = false;
            var channel = new Wcf();
            var validator = new ValidationLogStub
                                {
                                    OnAddWarningMessage = (m, a) =>
                                                              {
                                                                  Assert.AreEqual("Channel does not have a name", m);
                                                                  CollectionAssert.IsEmpty(a);
                                                                  warningAdded = true;
                                                              }
                                };
            channel.Validate(validator);
            Assert.IsTrue(warningAdded);
        }

        [Test]
        public void InitialiseOpensTheWcfHost()
        {
            var channel = new Wcf();
            var address = "net.tcp://localhost/client";
            channel.Endpoints.Add(new NetTcp { Address = address });
            var opened = false;
            try
            {
                opened = channel.Initialise(null);
                Assert.IsTrue(opened);
                var canPing = ServerConnection.Ping(address);
                Assert.IsTrue(canPing);
            }
            finally
            {
                if (opened)
                {
                    channel.CleanUp();
                }
            }
        }

        [Test]
        public void CleanUpCloseChannel()
        {
            var channel = new Wcf();
            channel.Endpoints.Add(new NetTcp { Address = "net.tcp://localhost/client" });
            channel.Initialise(null);
            channel.CleanUp();
        }

        [Test]
        public void CleanUpDoesNothingIfNotOpen()
        {
            var channel = new Wcf();
            channel.CleanUp();
        }
        #endregion
    }
}
