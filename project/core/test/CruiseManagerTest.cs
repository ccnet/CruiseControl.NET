using System;
using System.Diagnostics;
using System.Threading;

using NMock;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class CruiseManagerTest : CustomAssertion
	{
		private ConfigurationStub configStub;
		private CruiseManager manager;

		[SetUp]
		protected void SetUp()
		{
			configStub = new ConfigurationStub(2);
			manager = new CruiseManager(configStub);
		}

		[Test]
		public void ForceBuildForProject()
		{
			configStub.GetIntegratorMock(1).Expect("ForceBuild");

			manager.ForceBuild("project2");

			configStub.Verify();
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void AttemptToForceBuildOnProjectThatDoesNotExist()
		{
			manager.ForceBuild("foo");
		}

		[Test]
		public void WaitForExit()
		{
			configStub.GetIntegratorMock(0).Expect("WaitForExit");

			manager.WaitForExit("project1");

			configStub.Verify();
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void AttemptToWaitForExitOnProjectThatDoesNotExist()
		{
			manager.WaitForExit("foo");
		}

//		[Test]
//		public void GetProjectStatus() 
//		{
//			// setup
//			Configuration configuration = new Configuration();
//			
//			Project project1 = new Project();
//			project1.Name = "blue cheese";
//			configuration.AddProject(project1);
//
//			Project project2 = new Project();
//			project2.Name = "ranch";
//			configuration.AddProject(project2);
//
//			Mock mockCC = new DynamicMock(typeof(ICruiseControl));
//			mockCC.ExpectAndReturn("Configuration", configuration);
//			mockCC.ExpectAndReturn("Status", CruiseControlStatus.Running);
//			mockCC.ExpectAndReturn("Status", CruiseControlStatus.Running);
//
//			// test
//			CruiseManager manager = new CruiseManager((ICruiseControl)mockCC.MockInstance);
//			ProjectStatus [] status = manager.GetProjectStatus();
//
//			// check
//			AssertEquals(2, status.Length);
//			AssertEquals("blue cheese", status[0].Name);
//		}
	}
}
