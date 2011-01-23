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
        #endregion
    }
}
