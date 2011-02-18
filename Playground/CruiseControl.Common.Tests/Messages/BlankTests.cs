namespace CruiseControl.Common.Tests.Messages
{
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class BlankTests
    {
        #region Tests
        [Test]
        public void BasePropertiesAreSerialised()
        {
            var original = new Blank();
            original.DoRoundTripTest();
        }
        #endregion
    }
}
