using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildTest : Assertion
	{
		[Test]
		public void SuccessfulBuildIsMarkedAsSuccessful()
		{
			Build build = new Build("log20040721095851Lbuild.1.xml", "", "", "");
			AssertEquals(true, build.IsSuccessful);
		}

		[Test]
		public void FailedlBuildIsMarkedAsFailed()
		{
			Build build = new Build("log20020916143556.xml", "", "", "");
			AssertEquals(false, build.IsSuccessful);
		}
	}
}
