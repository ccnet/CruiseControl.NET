using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultBuildUrlGeneratorTest : Assertion
	{
		[Test]
		public void CombinesBaseUrlAndBuildToCreateCorrectUrl()
		{
			string returnedUrl = new DefaultBuildUrlGenerator().GenerateUrl("foo.aspx", "myserver", "myproject", "mybuild");
			AssertEquals("foo.aspx?server=myserver&amp;project=myproject&amp;build=mybuild", returnedUrl);
		}
	}
}
