using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectStatusMonitorTest
	{
        private MockRepository repository = new MockRepository(MockBehavior.Default);
        private CruiseServerClientBase mockCruiseManager;
		private ICruiseProjectManager manager;
		const string PROJECT_NAME = "projectName";

		[SetUp]
		public void SetUp()
		{
            mockCruiseManager = repository.Create<CruiseServerClientBase>(MockBehavior.Strict).Object;

			manager = new RemotingCruiseProjectManager(mockCruiseManager, PROJECT_NAME);
		}

		[Test]
		public void CanRetriveProjectName()
		{
			Assert.AreEqual(PROJECT_NAME, manager.ProjectName);
		}

		[Test]
		public void CanForceABuild()
		{
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Mock.Get(mockCruiseManager).Setup(_mockCruiseManager => _mockCruiseManager.ForceBuild(It.IsAny<string>(), It.IsAny<List<NameValuePair>>()));
            Mock.Get(mockCruiseManager).SetupSet(_mockCruiseManager => _mockCruiseManager.SessionToken = It.IsAny<string>());
			manager.ForceBuild(null, parameters, null);
            repository.VerifyAll();
		}
	}
}