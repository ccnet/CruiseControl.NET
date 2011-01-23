namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class WebClientFactoryTests
    {
        [Test]
        public void GeneratesStartsNewClient()
        {
            var factory = new WebClientFactory<DefaultWebClient>();
            var client = factory.Generate();
            Assert.IsInstanceOf<DefaultWebClient>(client);
        }
    }
}
