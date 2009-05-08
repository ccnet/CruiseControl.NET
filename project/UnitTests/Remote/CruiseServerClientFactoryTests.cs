using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class CruiseServerClientFactoryTests
    {
        #region Private fields
        private MockRepository mocks = new MockRepository();
        #endregion

        #region Test methods
        #region GenerateClient()
        [Test]
        public void GenerateClientDetectsHttpClient()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateClient("http://somewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        public void GenerateClientDetectsRemotingClient()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateClient("tcp://somewhere") as CruiseServerClient;
            Assert.AreEqual(".NET Remoting", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void GenerateClientThrowsExceptionOnUnknown()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateClient("ftp://somewhere") as CruiseServerClient;
        }

        [Test]
        public void GenerateClientSetsTargetServer()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateClient("http://somewhere", "elsewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
            Assert.AreEqual("elsewhere", client.TargetServer);
        }
        #endregion

        #region GenerateHttpClient()
        [Test]
        public void GenerateHttpClientGeneratesClient()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateHttpClient("http://somewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        public void GenerateHttpClientSetsTargetServer()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateHttpClient("http://somewhere", "elsewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
            Assert.AreEqual("elsewhere", client.TargetServer);
        }
        #endregion

        #region GenerateRemotingClient()
        [Test]
        public void GenerateRemotingClientGeneratesClient()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateRemotingClient("http://somewhere") as CruiseServerClient;
            Assert.AreEqual(".NET Remoting", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        public void GenerateRemotingClientSetsTargetServer()
        {
            CruiseServerClient client = CruiseServerClientFactory.GenerateRemotingClient("http://somewhere", "elsewhere") as CruiseServerClient;
            Assert.AreEqual(".NET Remoting", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
            Assert.AreEqual("elsewhere", client.TargetServer);
        }
        #endregion
        #endregion
    }
}
