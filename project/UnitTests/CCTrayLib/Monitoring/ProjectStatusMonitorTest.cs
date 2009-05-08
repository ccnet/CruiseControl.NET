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
        private CruiseServerClientBase mockCruiseManager;
		private ICruiseProjectManager manager;
		const string PROJECT_NAME = "projectName";

		[SetUp]
		public void SetUp()
		{
            mockCruiseManager = repository.StrictMock<CruiseServerClientBase>();

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
            Expect.Call(() => { mockCruiseManager.ForceBuild(null, null); })
                .IgnoreArguments();
            Expect.Call(mockCruiseManager.SessionToken).PropertyBehavior();
            repository.ReplayAll();
			manager.ForceBuild(null, parameters);
            repository.VerifyAll();
		}
	}
}