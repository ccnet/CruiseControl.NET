namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using System.Collections.Specialized;
    using System;
    using System.Net;
    using ThoughtWorks.CruiseControl.Remote.Messages;
    using System.Text;

    [TestFixture]
    public class HttpConnectionTests
    {
        #region Tests
        #region IsBusy tests
        [Test]
        public void IsBusyReturnsFalseWhenNothingIsHappening()
        {
            var connection = new HttpConnection("http://somewhere");
            Assert.IsFalse(connection.IsBusy);
        }
        #endregion

        #region Address tests
        [Test]
        public void AddressReturnsTheUrl()
        {
            var url = "http://somewhere/";
            var connection = new HttpConnection(url);
            Assert.AreEqual(url, connection.Address);
        }
        #endregion

        #region SendMessage() tests
        [Test]
        public void SendMessageSendsAndReceivesAMessage()
        {
            var action = "DoSomething";
            var request = new ServerRequest
            {
                ServerName = "TestServer"
            };
            var url = "http://somewhere/";
            var factory = new TestClientFactory((u, a, d) =>
            {
                Assert.AreEqual(url + "/server/TestServer/RawXmlMessage.aspx", u.AbsoluteUri);
                Assert.AreEqual("POST", a);
                Assert.AreEqual(action, d["action"]);
                Assert.AreEqual(request.ToString(), d["message"]);
                var theResponse = new Response
                {
                    RequestIdentifier = request.Identifier
                };
                return Encoding.UTF8.GetBytes(theResponse.ToString());
            });
            var connection = new HttpConnection(new Uri(url), factory);
            var response = connection.SendMessage(action, request);
            Assert.IsInstanceOf<Response>(response);
            Assert.AreEqual(request.Identifier, response.RequestIdentifier);
        }
        #endregion
        #endregion

        #region Private classes
        #region TestClientFactory
        private class TestClientFactory
            : IWebClientFactory
        {
            private Func<Uri, string, NameValueCollection, byte[]> action;

            public TestClientFactory(Func<Uri, string, NameValueCollection, byte[]> action)
            {
                this.action = action;
            }

            public IWebClient Generate()
            {
                return new TestClient(this.action);
            }
        }
        #endregion

        #region TestClient
        private class TestClient
            : IWebClient
        {
            private Func<Uri, string, NameValueCollection, byte[]> action;

            public TestClient(Func<Uri, string, NameValueCollection, byte[]> action)
            {
                this.action = action;
            }

            public byte[] UploadValues(Uri address, string method, NameValueCollection data)
            {
                return action(address, method, data);
            }

            public void UploadValuesAsync(Uri address, string method, NameValueCollection data)
            {
                throw new NotImplementedException();
            }

            public void CancelAsync()
            {
                throw new NotImplementedException();
            }

            public event UploadValuesCompletedEventHandler UploadValuesCompleted;
        }
        #endregion
        #endregion
    }
}
