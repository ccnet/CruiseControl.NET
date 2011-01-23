namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class NameValuePairSerialiserFactoryTests
    {
        #region Tests
        [Test]
        public void CreateGeneratesNewSerialiser()
        {
            var factory = new NameValuePairSerialiserFactory();
            var serialiser = factory.Create(null, null);
            Assert.IsInstanceOf<NameValuePairSerialiser>(serialiser);
            var actualSerialiser = serialiser as NameValuePairSerialiser;
            Assert.IsFalse(actualSerialiser.IsList);
        }
        #endregion
    }
}
