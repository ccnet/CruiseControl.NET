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
		[Test]
		public void ForceBuild()
		{
			string testProjectName = "TestProjectName";
			MockProject mockProject = new MockProject(testProjectName, null);

			Mock mockCC = new DynamicMock(typeof(ICruiseControl));
			mockCC.Expect("ForceBuild", testProjectName);

			CruiseManager manager = new CruiseManager((ICruiseControl)mockCC.MockInstance);
			manager.ForceBuild(testProjectName);

			mockCC.Verify();
		}

		[Test]
		public void GetProjectStatus() 
		{
			// setup
			Configuration configuration = new Configuration();
			
			Project project1 = new Project();
			project1.Name = "blue cheese";
			configuration.AddProject(project1);

			Project project2 = new Project();
			project2.Name = "ranch";
			configuration.AddProject(project2);

			Mock mockCC = new DynamicMock(typeof(ICruiseControl));
			mockCC.ExpectAndReturn("Configuration", configuration);
			mockCC.ExpectAndReturn("Status", CruiseControlStatus.Running);
			mockCC.ExpectAndReturn("Status", CruiseControlStatus.Running);

			// test
			CruiseManager manager = new CruiseManager((ICruiseControl)mockCC.MockInstance);
			ProjectStatus [] status = manager.GetProjectStatus();

			// check
			AssertEquals(2, status.Length);
			AssertEquals("blue cheese", status[0].Name);
		}
	}
}
