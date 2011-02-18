namespace CruiseControl.Core.Tests.Channels
{
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
        [Ignore("Need to implement tests for initialising the channel first")]
        public void CleanUpCloseChannel()
        {
            var channel = new Wcf();
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
