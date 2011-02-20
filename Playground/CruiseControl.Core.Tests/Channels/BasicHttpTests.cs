namespace CruiseControl.Core.Tests.Channels
{
    using System.ServiceModel;
    using CruiseControl.Core.Channels;
    using NUnit.Framework;

    [TestFixture]
    public class BasicHttpTests
    {
        #region Tests
        [Test]
        public void BindingReturnsCorrectType()
        {
            var endpoint = new BasicHttp();
            Assert.IsInstanceOf<BasicHttpBinding>(endpoint.Binding);
        }
        #endregion
    }
}
