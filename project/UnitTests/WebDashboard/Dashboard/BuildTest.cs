using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildTest
	{
		[Test]
		public void SuccessfulBuildIsMarkedAsSuccessful()
		{
			Build build = new Build("log20040721095851Lbuild.1.xml", "", "", "", "");
			Assert.AreEqual(true, build.IsSuccessful);
		}

		[Test]
		public void FailedlBuildIsMarkedAsFailed()
		{
			Build build = new Build("log20020916143556.xml", "", "", "", "");
			Assert.AreEqual(false, build.IsSuccessful);
		}
	}
}
