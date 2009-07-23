using Rhino.Mocks;
using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using Rhino.Mocks.Constraints;
using ThoughtWorks.CruiseControl.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CruiseServerManagerFactoryTest
	{
        private MockRepository mocks = new MockRepository();

		[Test]
		public void WhenRequestingACruiseServerManagerWithATCPUrlAsksTheCruiseManagerFactory()
		{
            var serverAddress = @"tcp://1.2.3.4";
            var server = new BuildServer(serverAddress);
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            Expect.Call(mockCruiseManagerFactory.GenerateRemotingClient(serverAddress, new ClientStartUpSettings()))
                .Constraints(new Equal(serverAddress), new Anything())
                .Return(client);

            mocks.ReplayAll();
            var manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.Configuration.Url);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

            mocks.VerifyAll();
		}

		[Test]
		public void WhenRequestingACruiseServerManagerWithAnHttpUrlConstructsANewHttpServerManagerDecoratedWithACachingServerManager()
		{
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var serverAddress = "http://somethingOrOther";
            var server = new BuildServer(serverAddress);
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            Expect.Call(mockCruiseManagerFactory.GenerateHttpClient(serverAddress, new ClientStartUpSettings()))
                .Constraints(new Equal(serverAddress), new Anything())
                .Return(client);

            mocks.ReplayAll();
            var manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.Configuration.Url);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

            mocks.VerifyAll();
		}

        [Test(Description = "Unable to find extension 'Extension.Unknown,Extension'")]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolCannotFindExtension()
        {
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension,
                                         "Extension.Unknown,Extension", string.Empty);

            mocks.ReplayAll();
            Assert.That(delegate { factory.Create(server); }, Throws.TypeOf<CCTrayLibException>());
        }

        [Test]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolEmptyExtension()
        {
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "New Extension", string.Empty);
            server.ExtensionName = null;

            mocks.ReplayAll();
            Assert.That(delegate { factory.Create(server); },
                        Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("name"));
        }

        [Test(Description = "Extension 'CruiseServerManagerFactoryTest'does not implement ITransportExtension")]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolCannotInvalidExtension()
        {
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension,
                                         "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.CruiseServerManagerFactoryTest,ThoughtWorks.CruiseControl.UnitTests",
                                         string.Empty);

            mocks.ReplayAll();
            Assert.That(delegate { factory.Create(server); }, Throws.TypeOf<CCTrayLibException>());
        }

	    [Test]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolValidExtension()
        {
            var mockCruiseManagerFactory = mocks.StrictMock<ICruiseServerClientFactory>();
            var factory = new CruiseServerManagerFactory(mockCruiseManagerFactory);

            var server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests", "Some settings");

            mocks.ReplayAll();
            var manager = factory.Create(server);
            Assert.AreEqual(server.Url, manager.Configuration.Url);
            Assert.AreEqual(server.ExtensionName, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests");
            Assert.AreEqual(server.ExtensionSettings, "Some settings");

            mocks.VerifyAll();
        }
	}
}
