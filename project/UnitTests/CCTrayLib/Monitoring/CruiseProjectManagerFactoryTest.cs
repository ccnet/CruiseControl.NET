using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CruiseProjectManagerFactoryTest
	{
		private const string ProjectName = "projectName";

		[Test]
		public void WhenRequestingACruiseProjectManagerWithATCPUrlAsksTheCruiseManagerFactory()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof (ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance, new WebRetriever());

			BuildServer server= new BuildServer("tcp://somethingOrOther");
			mockCruiseManagerFactory.ExpectAndReturn("GetCruiseManager", null, server.Url);

			ICruiseProjectManager manager = factory.Create(new CCTrayProject(server, ProjectName));
			Assert.AreEqual(ProjectName, manager.ProjectName);

			mockCruiseManagerFactory.Verify();
		}

		[Test]
		public void WhenRequestingACruiseProjectManagerWithAnHttpUrlConstructsANewDashboardCruiseProjectManager()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof (ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance, new WebRetriever());

			BuildServer server = new BuildServer("http://somethingOrOther");

			ICruiseProjectManager manager = factory.Create(new CCTrayProject(server, ProjectName));
			Assert.AreEqual(ProjectName, manager.ProjectName);
			Assert.AreEqual(typeof (HttpCruiseProjectManager), manager.GetType());

			mockCruiseManagerFactory.Verify();
		}
	}
}