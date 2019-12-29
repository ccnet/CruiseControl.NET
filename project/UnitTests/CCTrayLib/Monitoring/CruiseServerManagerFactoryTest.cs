using System;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CruiseServerManagerFactoryTest
	{
        private MockRepository mocks = new MockRepository(MockBehavior.Default);

		[Test]
		public void WhenRequestingACruiseServerManagerWithATCPUrlAsksTheCruiseManagerFactory()
		{
            var serverAddress = @"tcp://1.2.3.4";
            var server = new BuildServer(serverAddress);
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);
            var client = mocks.Create<CruiseServerClientBase>().Object;
            Mock.Get(mockCruiseManagerFactory).Setup(clientFactory => clientFactory.GenerateRemotingClient(serverAddress, It.IsAny<ClientStartUpSettings>()))
                .Returns(client);

            var manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.Configuration.Url);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

            mocks.VerifyAll();
		}

		[Test]
		public void WhenRequestingACruiseServerManagerWithAnHttpUrlConstructsANewHttpServerManagerDecoratedWithACachingServerManager()
		{
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var serverAddress = "http://somethingOrOther";
            var server = new BuildServer(serverAddress);
            var client = mocks.Create<CruiseServerClientBase>().Object;
            Mock.Get(mockCruiseManagerFactory).Setup(clientFactory => clientFactory.GenerateHttpClient(serverAddress, It.IsAny<ClientStartUpSettings>()))
                .Returns(client);

            var manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.Configuration.Url);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

            mocks.VerifyAll();
		}

        [Test(Description = "Unable to find extension 'Extension.Unknown,Extension'")]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolCannotFindExtension()
        {
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension,
                                         "Extension.Unknown,Extension", string.Empty);

            Assert.That(delegate { factory.Create(server); }, Throws.TypeOf<CCTrayLibException>());
        }

        [Test]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolEmptyExtension()
        {
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "New Extension", string.Empty);
            server.ExtensionName = null;

            Assert.That(delegate { factory.Create(server); },
                        Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("name"));
        }

        [Test(Description = "Extension 'CruiseServerManagerFactoryTest'does not implement ITransportExtension")]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolCannotInvalidExtension()
        {
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension,
                                         "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.CruiseServerManagerFactoryTest,ThoughtWorks.CruiseControl.UnitTests",
                                         string.Empty);

            Assert.That(delegate { factory.Create(server); }, Throws.TypeOf<CCTrayLibException>());
        }

	    [Test]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolValidExtension()
        {
            var mockCruiseManagerFactory = mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests", "Some settings");

            var manager = factory.Create(server);
            Assert.AreEqual(server.Url, manager.Configuration.Url);
            Assert.AreEqual(server.ExtensionName, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests");
            Assert.AreEqual(server.ExtensionSettings, "Some settings");

            mocks.VerifyAll();
        }
	}
}
