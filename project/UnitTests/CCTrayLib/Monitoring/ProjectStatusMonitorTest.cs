using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectStatusMonitorTest
	{
		private DynamicMock mockCruiseManager;
		private ICruiseProjectManager manager;
		const string PROJECT_NAME = "projectName";

		[SetUp]
		public void SetUp()
		{
			mockCruiseManager = new DynamicMock( typeof (ICruiseManager) );
			mockCruiseManager.Strict = true;

			manager = new RemotingCruiseProjectManager((ICruiseManager) mockCruiseManager.MockInstance, PROJECT_NAME);
		}

		[TearDown]
		public void TearDown()
		{
			mockCruiseManager.Verify();
		}

		[Test]
		public void CanRetriveProjectName()
		{
			Assert.AreEqual(PROJECT_NAME, manager.ProjectName);
		}

		[Test]
		public void CanForceABuild()
		{
			mockCruiseManager.Expect("Request", null, PROJECT_NAME, new IntegrationRequest(BuildCondition.ForceBuild, Environment.UserName));
			manager.ForceBuild(null);
		}
	}
}