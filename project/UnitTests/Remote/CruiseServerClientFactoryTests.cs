using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class CruiseServerClientFactoryTests
    {
        #region Test methods
        #region GenerateClient()
        [Test]
        public void GenerateClientDetectsHttpClient()
        {
            CruiseServerClient client = new CruiseServerClientFactory().GenerateClient("http://somewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        public void GenerateClientDetectsRemotingClient()
        {
            CruiseServerClient client = new CruiseServerClientFactory().GenerateClient("tcp://somewhere") as CruiseServerClient;
            Assert.AreEqual(".NET Remoting", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        public void GenerateClientThrowsExceptionOnUnknown()
        {
            Assert.That(delegate { new CruiseServerClientFactory().GenerateClient("ftp://somewhere"); },
                        Throws.TypeOf<ApplicationException>());
        }

        [Test]
        public void GenerateClientSetsTargetServer()
        {
            CruiseServerClient client = new CruiseServerClientFactory().GenerateClient("http://somewhere", "elsewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
            Assert.AreEqual("elsewhere", client.TargetServer);
        }
        #endregion

        #region GenerateHttpClient()
        [Test]
        public void GenerateHttpClientGeneratesClient()
        {
            CruiseServerClient client = new CruiseServerClientFactory().GenerateHttpClient("http://somewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        public void GenerateHttpClientSetsTargetServer()
        {
            CruiseServerClient client = new CruiseServerClientFactory().GenerateHttpClient("http://somewhere", "elsewhere") as CruiseServerClient;
            Assert.AreEqual("HTTP", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
            Assert.AreEqual("elsewhere", client.TargetServer);
        }
        #endregion

        #region GenerateRemotingClient()
        [Test]
        public void GenerateRemotingClientGeneratesClient()
        {
            CruiseServerClient client = new CruiseServerClientFactory().GenerateRemotingClient("http://somewhere") as CruiseServerClient;
            Assert.AreEqual(".NET Remoting", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
        }

        [Test]
        public void GenerateRemotingClientSetsTargetServer()
        {
            CruiseServerClient client = new CruiseServerClientFactory().GenerateRemotingClient("http://somewhere", "elsewhere") as CruiseServerClient;
            Assert.AreEqual(".NET Remoting", client.Connection.Type);
            Assert.AreEqual("somewhere", client.Connection.ServerName);
            Assert.AreEqual("elsewhere", client.TargetServer);
        }
        #endregion
        #endregion
    }
}
