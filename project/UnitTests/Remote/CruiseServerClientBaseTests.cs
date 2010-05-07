namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using NUnit.Framework;

    public class CruiseServerClientBaseTests
    {
        #region Tests
        [Test]
        public void LoginConvertsObjectToCredentials()
        {
            var client = new TestClient();
            var credentials = new
            {
                UserName = "JohnDoe",
                Password = (string)null
            };
            client.Login(credentials);
            Assert.AreEqual(2, client.Credentials.Count);
            Assert.AreEqual("UserName", client.Credentials[0].Name);
            Assert.AreEqual(credentials.UserName, client.Credentials[0].Value);
            Assert.AreEqual("Password", client.Credentials[1].Name);
            Assert.AreEqual(credentials.Password, client.Credentials[1].Value);
        }

        [Test]
        public void DisposeLogsOutWhenLoggedIn()
        {
            var client = new TestClient();
            Assert.IsFalse(client.LoggedOut);
            client.Login(new { });
            client.Dispose();
            Assert.IsTrue(client.LoggedOut);
        }

        [Test]
        public void DisposeDoesNothingWhenNotLoggedIn()
        {
            var client = new TestClient();
            Assert.IsFalse(client.LoggedOut);
            client.Dispose();
            Assert.IsFalse(client.LoggedOut);
        }
        #endregion

        #region Classes
        private class TestClient
            : CruiseServerClientBase
        {
            public List<NameValuePair> Credentials { get; set; }
            public bool LoggedOut { get; set; }

            public override bool Login(List<NameValuePair> Credentials)
            {
                this.Credentials = Credentials;
                this.SessionToken = "1234";
                return true;
            }

            public override void Logout()
            {
                this.LoggedOut = true;
            }

            public override string TargetServer
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override bool IsBusy
            {
                get { throw new NotImplementedException(); }
            }

            public override string Address
            {
                get { throw new NotImplementedException(); }
            }
        }
        #endregion
    }
}
