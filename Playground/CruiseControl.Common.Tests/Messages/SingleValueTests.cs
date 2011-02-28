namespace CruiseControl.Common.Tests.Messages
{
    using Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class SingleValueTests
    {
        #region Tests
        [Test]
        public void ValueIsPassedCorrectly()
        {
            var expected = "some test value";
            var original = new SingleValue
                               {
                                   Value = expected
                               };
            var result = original.DoRoundTripTest();
            Assert.AreEqual(expected, result.Value);
        }
        #endregion
    }
}
