namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using CruiseControl.Remote;
    using Moq;
    using Moq.Protected;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultWebClientTests
    {
        private readonly MockRepository _mocks = new MockRepository(MockBehavior.Default);
        private WebClient _mockClient;
        private DefaultWebClient _client;
        private Uri _uri;

        [SetUp]
        public void SetUp()
        {
            _uri = new Uri("http://test1:test2@test3/");
            var webRequest = _mocks.Create<WebRequest>().Object;
            Mock.Get(webRequest).SetupGet(_webRequest => _webRequest.RequestUri).Returns(_uri);
            Mock.Get(webRequest).Setup(_webRequest => _webRequest.GetRequestStream()).Returns(new MemoryStream());
            var webResponse = _mocks.Create<WebResponse>().Object;
            _mockClient = _mocks.Create<WebClient>().Object;
            Mock.Get(_mockClient).Protected().Setup<WebRequest>("GetWebRequest", ItExpr.IsAny<Uri>()).Returns(webRequest);
            Mock.Get(_mockClient).Protected().Setup<WebResponse>("GetWebResponse", ItExpr.IsAny<WebRequest>()).Returns(webResponse);
            _client = new DefaultWebClient(_mockClient);
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
