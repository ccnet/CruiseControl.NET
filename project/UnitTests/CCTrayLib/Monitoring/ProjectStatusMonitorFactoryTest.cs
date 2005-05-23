using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectStatusMonitorFactoryTest
	{
		[Test]
		public void CanCreate()
		{
			DynamicMock mockCruiseManagerFactory = new DynamicMock(typeof(ICruiseManagerFactory));
			CruiseProjectManagerFactory factory = new CruiseProjectManagerFactory((ICruiseManagerFactory) mockCruiseManagerFactory.MockInstance);
			
			mockCruiseManagerFactory.ExpectAndReturn("GetCruiseManager", null, "a");
			ICruiseProjectManager result = factory.Create("a", "b");
			Assert.AreEqual("b", result.ProjectName);

		}
	}
}
