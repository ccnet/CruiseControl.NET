using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultServerUrlGeneratorTest : Assertion
	{
		[Test]
		public void CombinesBaseUrlAndBuildToCreateCorrectUrl()
		{
			string returnedUrl = new DefaultServerUrlGenerator().GenerateUrl("foo.aspx", "myserver");
			AssertEquals("foo.aspx?server=myserver", returnedUrl);
		}

	}
}
