using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultBuildNameFormatterTest : Assertion
	{
		[Test]
		public void ShouldFormatPassedBuildCorrectly()
		{
			string formattedBuildName = new DefaultBuildNameFormatter().GetPrettyBuildName("log20020830164057Lbuild.6.xml");
			AssertEquals("30 Aug 2002 16:40 (6)", formattedBuildName);
		}

		[Test]
		public void ShouldFormatFailedBuildCorrectly()
		{
			string formattedBuildName = new DefaultBuildNameFormatter().GetPrettyBuildName("log20020507042535.xml");
			AssertEquals("07 May 2002 04:25 (Failed)", formattedBuildName);
		}
	}
}
