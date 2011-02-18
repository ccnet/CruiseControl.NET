namespace CruiseControl.Common.Tests.Messages
{
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class BuildRequestTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsDefaults()
        {
            var message = new BuildRequest();
            Assert.IsNotNull(message.Values);
        }
        #endregion
    }
}
