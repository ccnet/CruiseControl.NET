using System;
using System.Diagnostics;
using System.Threading;

using NMock;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;
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
	}
}
