namespace CruiseControl.Common.Tests.Messages
{
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class BuildMessageTests
    {
        #region Tests
        [Test]
        public void BasePropertiesAreSerialised()
        {
            var original = new BuildMessage
                               {
                                   BuildName = "local"
                               };
            var result = original.DoRoundTripTest();
            Assert.AreEqual(original.BuildName, result.BuildName);
        }
        #endregion
    }
}
