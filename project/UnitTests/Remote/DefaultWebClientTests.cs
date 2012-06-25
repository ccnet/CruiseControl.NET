namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using NUnit.Framework;
    using CruiseControl.Remote;
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using Rhino.Mocks;

    [TestFixture]
    public class DefaultWebClientTests
    {
        private readonly MockRepository _mocks = new MockRepository();
        private WebClient _mockClient;
        private DefaultWebClient _client;
        private Uri _uri;

        [SetUp]
        public void SetUp()
        {
            _mockClient = _mocks.DynamicMock<WebClient>();
            SetupResult.For(_mockClient.Credentials).PropertyBehavior();
            _client = new DefaultWebClient(_mockClient);
            _uri = new Uri("http://test1:test2@test3/");
        }

        [Test]
        public void UploadValuesSetsCredentialsOnWebClientWhenUserInfoInUrl()
        {
            _client.UploadValues(_uri, "", new NameValueCollection());

            AssertCredentialsMatch();
        }

        [Test]
        public void UploadValuesAsyncSetsCredentialsOnWebClientWhenUserInfoInUrl()
        {
            _client.UploadValuesAsync(_uri, "", new NameValueCollection());

            AssertCredentialsMatch();
        }

        private void AssertCredentialsMatch()
        {
            var cred = _mockClient.Credentials.GetCredential(_uri, "Basic");
            Assert.AreEqual("test1", cred.UserName);
            Assert.AreEqual("test2", cred.Password);
        }
    }
}
