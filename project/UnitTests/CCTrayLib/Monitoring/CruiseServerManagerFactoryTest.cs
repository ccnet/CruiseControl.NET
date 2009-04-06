using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
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
			mockCruiseManagerFactory.ExpectAndReturn("GetCruiseManager", null, server.Url);

			ICruiseServerManager manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.ServerUrl);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

			mockCruiseManagerFactory.Verify();
		}

		[Test]
		public void WhenRequestingACruiseProjectManagerWithAnHttpUrlConstructsANewHttpServerManagerDecoratedWithACachingServerManager()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof (ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseServerManagerFactory factory = new CruiseServerManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);

			BuildServer server = new BuildServer("http://somethingOrOther");

			ICruiseServerManager manager = factory.Create(server);
			Assert.AreEqual(server.Url, manager.ServerUrl);
			Assert.AreEqual(typeof (CachingCruiseServerManager), manager.GetType());

			mockCruiseManagerFactory.Verify();
		}
	}
}
