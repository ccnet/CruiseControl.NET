using NMock;
using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CruiseServerManagerFactoryTest
	{
		[Test]
		public void WhenRequestingACruiseServerManagerWithATCPUrlAsksTheCruiseManagerFactory()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof (ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseServerManagerFactory factory = new CruiseServerManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);

			BuildServer server = new BuildServer(@"tcp://1.2.3.4");

			ICruiseServerManager manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.Configuration.Url);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

			mockCruiseManagerFactory.Verify();
		}

		[Test]
		public void WhenRequestingACruiseServerManagerWithAnHttpUrlConstructsANewHttpServerManagerDecoratedWithACachingServerManager()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof (ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseServerManagerFactory factory = new CruiseServerManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);

			BuildServer server = new BuildServer("http://somethingOrOther");

			ICruiseServerManager manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.Configuration.Url);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

			mockCruiseManagerFactory.Verify();
		}

        [Test]
        [ExpectedException(typeof(CruiseControlException), "Unable to find extension 'Extension.Unknown,Extension'")]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolCannotFindExtension()
        {
            DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
            mockCruiseManagerFactory.Strict = true;
            CruiseServerManagerFactory factory = new CruiseServerManagerFactory((ICruiseManagerFactory)mockCruiseManagerFactory.MockInstance);

            BuildServer server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "Extension.Unknown,Extension", string.Empty);

            ICruiseServerManager manager = factory.Create(server);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Extension name cannot be empty or null\r\nParameter name: name")]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolEmptyExtension()
        {
            DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
            mockCruiseManagerFactory.Strict = true;
            CruiseServerManagerFactory factory = new CruiseServerManagerFactory((ICruiseManagerFactory)mockCruiseManagerFactory.MockInstance);

            BuildServer server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "New Extension", string.Empty);
            server.ExtensionName = null;

            ICruiseServerManager manager = factory.Create(server);
        }

        [Test]
        [ExpectedException(typeof(CruiseControlException), "Extension 'CruiseServerManagerFactoryTest'does not implement ITransportExtension")]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolCannotInvalidExtension()
        {
            DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
            mockCruiseManagerFactory.Strict = true;
            CruiseServerManagerFactory factory = new CruiseServerManagerFactory((ICruiseManagerFactory)mockCruiseManagerFactory.MockInstance);

            BuildServer server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.CruiseServerManagerFactoryTest,ThoughtWorks.CruiseControl.UnitTests", string.Empty);

            ICruiseServerManager manager = factory.Create(server);
        }

        [Test]
        public void WhenRequestingACruiseServerManagerWithAnExtensionProtocolValidExtension()
        {
            DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
            mockCruiseManagerFactory.Strict = true;
            CruiseServerManagerFactory factory = new CruiseServerManagerFactory((ICruiseManagerFactory)mockCruiseManagerFactory.MockInstance);

            BuildServer server = new BuildServer("http://somethingOrOther", BuildServerTransport.Extension, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests", "Some settings");

            ICruiseServerManager manager = factory.Create(server);
            Assert.AreEqual(server.Url, manager.Configuration.Url);
            Assert.AreEqual(server.ExtensionName, "ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring.ExtensionProtocolStub,ThoughtWorks.CruiseControl.UnitTests");
            Assert.AreEqual(server.ExtensionSettings, "Some settings");

            mockCruiseManagerFactory.Verify();
        }
	}
}
