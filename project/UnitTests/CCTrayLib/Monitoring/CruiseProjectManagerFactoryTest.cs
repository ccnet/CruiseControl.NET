using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CruiseProjectManagerFactoryTest
	{
		private const string ProjectName = "projectName";

		[Test]
		public void WhenRequestingACruiseProjectManagerWithATCPUrlAsksTheCruiseManagerFactory()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);

			const string serverUrl = "tcp://somethingOrOther";
			mockCruiseManagerFactory.ExpectAndReturn("GetCruiseManager", null, serverUrl);

			ICruiseProjectManager manager = factory.Create(serverUrl, ProjectName);
			Assert.AreEqual(ProjectName, manager.ProjectName);
			
			mockCruiseManagerFactory.Verify();
		}
		
		[Test]
		public void WhenRequestingACruiseProjectManagerWithAnHttpUrlConstructsANewDashboardCruiseProjectManager()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
			mockCruiseManagerFactory.Strict = true;
			CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);

			const string serverUrl = "http://somethingOrOther";

			ICruiseProjectManager manager = factory.Create(serverUrl, ProjectName);
			Assert.AreEqual(ProjectName, manager.ProjectName);
			Assert.AreEqual(typeof(DashboardCruiseProjectManager), manager.GetType());
			
			mockCruiseManagerFactory.Verify();
		}

	}
}
