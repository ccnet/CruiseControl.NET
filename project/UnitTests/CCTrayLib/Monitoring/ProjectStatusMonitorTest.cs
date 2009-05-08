using System;
using Rhino.Mocks;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ProjectStatusMonitorTest
	{
        private MockRepository repository = new MockRepository();
        private CruiseServerClient mockCruiseManager;
		private ICruiseProjectManager manager;
		const string PROJECT_NAME = "projectName";

		[SetUp]
		public void SetUp()
		{
            mockCruiseManager = repository.StrictMock<CruiseServerClient>();

			manager = new RemotingCruiseProjectManager((CruiseServerClient) mockCruiseManager, PROJECT_NAME);
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
            Expect.Call(() => { mockCruiseManager.ForceBuild(null); })
                .IgnoreArguments();
            repository.ReplayAll();
			manager.ForceBuild(null, parameters);
            repository.VerifyAll();
		}
	}
}